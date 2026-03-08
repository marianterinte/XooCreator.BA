using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.Subscription.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

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

    public CreateSupporterPackOrderEndpoint(XooDbContext db, IAuth0UserService auth0, ISupporterPackPlanConfigService planConfig)
    {
        _db = db;
        _auth0 = auth0;
        _planConfig = planConfig;
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

        return TypedResults.Ok(new CreateOrderResponse(order.Id, order.PlanId, order.Amount));
    }
}
