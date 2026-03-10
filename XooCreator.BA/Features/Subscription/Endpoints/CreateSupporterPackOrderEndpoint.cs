using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.Subscription.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Email;
using System.Text.Json;

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
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public CreateSupporterPackOrderEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        ISupporterPackPlanConfigService planConfig,
        IResendEmailService resend,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _db = db;
        _auth0 = auth0;
        _planConfig = planConfig;
        _resend = resend;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public record CreateOrderRequest(string PlanId);

    public record CreateOrderResponse(Guid OrderId, string PlanId, decimal AmountRon);

    [Route("/api/supporter-packs/order")]
    [Authorize]
    public static async Task<IResult> HandlePost(
        [FromBody] CreateOrderRequest? request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        [FromHeader(Name = "X-Turnstile-Token")] string? turnstileToken,
        HttpContext httpContext,
        [FromServices] CreateSupporterPackOrderEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return Results.Unauthorized();

        if (request == null || string.IsNullOrWhiteSpace(request.PlanId))
            return Results.BadRequest("PlanId is required.");
        var planId = request.PlanId.Trim();
        if (!ValidPlanIds.Contains(planId))
            return Results.BadRequest("PlanId must be one of: Bronze, Silver, Gold, Platinum.");

        idempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey)
            ? null
            : idempotencyKey.Trim();

        if (idempotencyKey?.Length > 64)
            idempotencyKey = idempotencyKey[..64];

        // Idempotency: if a PendingPayment order already exists for this user + key, return it.
        if (!string.IsNullOrEmpty(idempotencyKey))
        {
            var existing = await ep._db.SupporterPackOrders
                .Where(o => o.UserId == user.Id && o.IdempotencyKey == idempotencyKey)
                .FirstOrDefaultAsync(ct);

                if (existing != null)
                {
                    return Results.Ok(new CreateOrderResponse(existing.Id, existing.PlanId, existing.Amount));
                }
        }

        // Rate limit: max 4 PendingPayment orders in last 30 minutes, unless CAPTCHA passed.
        var nowUtc = DateTime.UtcNow;
        var windowStart = nowUtc.AddMinutes(-30);

        var recentPendingCount = await ep._db.SupporterPackOrders
            .Where(o => o.UserId == user.Id
                        && o.Status == SupporterPackOrderStatus.PendingPayment
                        && o.CreatedAtUtc >= windowStart)
            .CountAsync(ct);

        var captchaRequired = recentPendingCount >= 4;

        if (captchaRequired)
        {
            var turnstileSecret = ep._configuration["Captcha:Turnstile:SecretKey"];
            var captchaOk = false;

            if (!string.IsNullOrWhiteSpace(turnstileSecret) && !string.IsNullOrWhiteSpace(turnstileToken))
            {
                try
                {
                    using var client = ep._httpClientFactory.CreateClient("captcha");
                    using var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("secret", turnstileSecret),
                        new KeyValuePair<string, string>("response", turnstileToken)
                    });

                    var resp = await client.PostAsync("https://challenges.cloudflare.com/turnstile/v0/siteverify", content, ct);
                    if (resp.IsSuccessStatusCode)
                    {
                        using var stream = await resp.Content.ReadAsStreamAsync(ct);
                        var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
                        captchaOk = doc.RootElement.TryGetProperty("success", out var successProp) &&
                                    successProp.ValueKind == JsonValueKind.True;
                    }
                }
                catch
                {
                    // On failure, fall through and treat as not ok – better safe than sorry.
                    captchaOk = false;
                }
            }

            if (!captchaOk)
            {
                var secondsLeft = (int)Math.Max(0, (windowStart.AddMinutes(30) - nowUtc).TotalSeconds);
                var payload = new
                {
                    message = "Ai trimis deja mai multe cereri de cumpărare într-un timp scurt. Te rugăm să completezi verificarea pentru a continua.",
                    captchaRequired = true,
                    retryAfterSeconds = secondsLeft
                };

                httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                httpContext.Response.Headers["Retry-After"] = secondsLeft.ToString();
                return Results.Json(payload);
            }
        }

        var plan = await ep._planConfig.GetPlanAsync(planId, ct);
        var amount = plan != null ? plan.PriceRon : (FallbackPlanAmountRon.TryGetValue(planId, out var a) ? a : 0);

        var order = new SupporterPackOrder
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            PlanId = planId,
            Amount = amount,
            Status = SupporterPackOrderStatus.PendingPayment,
            CreatedAtUtc = nowUtc,
            IdempotencyKey = idempotencyKey
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

        return Results.Ok(new CreateOrderResponse(order.Id, order.PlanId, order.Amount));
    }
}
