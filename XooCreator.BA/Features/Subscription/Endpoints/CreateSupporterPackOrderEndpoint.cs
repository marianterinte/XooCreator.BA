using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.Subscription.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Email;

namespace XooCreator.BA.Features.Subscription.Endpoints;

[Endpoint]
public class CreateSupporterPackOrderEndpoint
{
    private static readonly HashSet<string> ValidPlanIds = new(StringComparer.OrdinalIgnoreCase)
        { "Bronze", "Silver", "Gold", "Platinum" };

    /// <summary>Fallback amount RON when plan not in config.</summary>
    private static readonly IReadOnlyDictionary<string, decimal> FallbackPlanAmountRon = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
    {
        { "Bronze", 30 },
        { "Silver", 50 },
        { "Gold", 100 },
        { "Platinum", 250 }
    };

    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly ISupporterPackPlanConfigService _planConfig;
    private readonly IResendEmailService _resend;

    public CreateSupporterPackOrderEndpoint(XooDbContext db, IAuth0UserService auth0, ISupporterPackPlanConfigService planConfig, IResendEmailService resend)
    {
        _db = db;
        _auth0 = auth0;
        _planConfig = planConfig;
        _resend = resend;
    }

    public record CreateOrderRequest(string PlanId);

    public record CreateOrderResponse(Guid OrderId, string PlanId, decimal AmountRon);

    [Route("/api/supporter-packs/order")]
    [Authorize]
    public static async Task<Results<Ok<CreateOrderResponse>, UnauthorizedHttpResult, BadRequest<string>>> HandlePost(
        [FromBody] CreateOrderRequest? request,
        [FromServices] CreateSupporterPackOrderEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        if (request == null || string.IsNullOrWhiteSpace(request.PlanId))
            return TypedResults.BadRequest("PlanId is required.");
        var planId = request.PlanId.Trim();
        if (!ValidPlanIds.Contains(planId))
            return TypedResults.BadRequest("PlanId must be one of: Bronze, Silver, Gold, Platinum.");

        var plan = await ep._planConfig.GetPlanAsync(planId, ct);
        var amount = plan != null ? plan.PriceRon : (FallbackPlanAmountRon.TryGetValue(planId, out var a) ? a : 0);

        var order = new SupporterPackOrder
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            PlanId = planId,
            Amount = amount,
            Status = SupporterPackOrderStatus.PendingPayment,
            CreatedAtUtc = DateTime.UtcNow
        };
        ep._db.SupporterPackOrders.Add(order);
        await ep._db.SaveChangesAsync(ct);

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            try
            {
                var subject = $"Ai creat comanda #{order.Id:N} – Supporter Pack {order.PlanId}";
                var body = new StringBuilder();
                body.AppendLine("<p>Ai creat o comandă Supporter Pack.</p>");
                body.AppendLine("<ul>");
                body.AppendLine($"<li><strong>ID comandă:</strong> {order.Id:N}</li>");
                body.AppendLine($"<li><strong>Plan:</strong> {WebUtility.HtmlEncode(order.PlanId)}</li>");
                body.AppendLine($"<li><strong>Sumă:</strong> {order.Amount} RON</li>");
                body.AppendLine("</ul>");
                body.AppendLine("<p>Următorul pas: completează plata prin virament bancar conform instrucțiunilor de pe site. După ce confirmăm plata, pack-ul tău va fi activat.</p>");
                body.AppendLine("<p>Mulțumim!</p>");
                body.AppendLine("<p>Echipa Alchimalia</p>");
                await ep._resend.SendAsync(user.Email, subject, body.ToString(), ct);
            }
            catch (Exception)
            {
                // Log is handled inside ResendEmailService; do not fail the request
            }
        }

        return TypedResults.Ok(new CreateOrderResponse(order.Id, order.PlanId, order.Amount));
    }
}
