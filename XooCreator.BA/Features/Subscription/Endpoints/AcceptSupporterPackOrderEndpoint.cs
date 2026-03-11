using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.Subscription.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Email;

namespace XooCreator.BA.Features.Subscription.Endpoints;

[Endpoint]
public class AcceptSupporterPackOrderEndpoint
{
    /// <summary>Fallback when plan not in config.</summary>
    private static readonly IReadOnlyDictionary<string, int> FallbackPlanGenerativeCredits = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        { "Bronze", 5 },
        { "Silver", 10 },
        { "Gold", 30 },
        { "Platinum", 30 }
    };

    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly ISupporterPackPlanConfigService _planConfig;
    private readonly IResendEmailService _resend;

    public AcceptSupporterPackOrderEndpoint(XooDbContext db, IAuth0UserService auth0, ISupporterPackPlanConfigService planConfig, IResendEmailService resend)
    {
        _db = db;
        _auth0 = auth0;
        _planConfig = planConfig;
        _resend = resend;
    }

    [Route("/api/admin/supporter-packs/orders/{orderId:guid}/accept")]
    [Authorize]
    public static async Task<Results<Ok<object>, NotFound, Conflict<string>, ForbidHttpResult>> HandlePost(
        Guid orderId,
        [FromServices] AcceptSupporterPackOrderEndpoint ep,
        CancellationToken ct)
    {
        var admin = await ep._auth0.GetCurrentUserAsync(ct);
        if (admin == null || !admin.Roles.Contains(UserRole.Admin))
            return TypedResults.Forbid();

        var order = await ep._db.SupporterPackOrders
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);
        if (order == null)
            return TypedResults.NotFound();

        if (order.Status == SupporterPackOrderStatus.Fulfilled)
            return TypedResults.Conflict("Order already fulfilled.");

        if (order.Status != SupporterPackOrderStatus.PendingPayment && order.Status != SupporterPackOrderStatus.PaymentConfirmed)
            return TypedResults.Conflict("Order cannot be accepted in current status.");

        var grant = new UserPackGrant
        {
            Id = Guid.NewGuid(),
            UserId = order.UserId,
            PlanId = order.PlanId,
            GrantedAtUtc = DateTime.UtcNow,
            GrantedByUserId = admin.Id,
            EmailUsed = null,
            OrderId = order.Id
        };
        ep._db.UserPackGrants.Add(grant);

        var plan = await ep._planConfig.GetPlanAsync(order.PlanId, ct);
        var creditsToAdd = plan != null ? plan.GenerativeCredits : (FallbackPlanGenerativeCredits.TryGetValue(order.PlanId, out var c) ? c : 0);
        var fullStoryToAdd = plan?.FullStoryGenerationCount ?? 0;
        if (creditsToAdd > 0 || fullStoryToAdd > 0)
        {
            var wallet = await ep._db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == order.UserId, ct);
            if (wallet == null)
            {
                wallet = new CreditWallet
                {
                    UserId = order.UserId,
                    Balance = 0,
                    DiscoveryBalance = 0,
                    GenerativeBalance = creditsToAdd,
                    FullStoryGenerationBalance = fullStoryToAdd,
                    UpdatedAt = DateTime.UtcNow
                };
                ep._db.CreditWallets.Add(wallet);
            }
            else
            {
                if (creditsToAdd > 0) wallet.GenerativeBalance += creditsToAdd;
                if (fullStoryToAdd > 0) wallet.FullStoryGenerationBalance += fullStoryToAdd;
                wallet.UpdatedAt = DateTime.UtcNow;
            }
        }

        order.Status = SupporterPackOrderStatus.Fulfilled;
        order.ProcessedAtUtc = DateTime.UtcNow;
        order.ProcessedByUserId = admin.Id;

        await ep._db.SaveChangesAsync(ct);

        // Notify user that pack is active
        var user = await ep._db.AlchimaliaUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == order.UserId, ct);
        if (user != null && !string.IsNullOrWhiteSpace(user.Email))
        {
            var subject = "Pack-ul tău Supporter este activ – Alchimalia";
            var body = $@"
<p>Bună ziua,</p>
<p>Am confirmat plata pentru comanda ta <strong>{order.PlanId}</strong>. Pack-ul tău Supporter este acum <strong>activ</strong>.</p>
<p>Poți folosi toate beneficiile: credite generative LOI, export PDF, acces la conținut exclusiv (în funcție de plan).</p>
<p>Mulțumim!</p>
<p>Echipa Alchimalia</p>";
            await ep._resend.SendAsync(user.Email, subject, body, ct);
        }

        return TypedResults.Ok((object)new { OrderId = order.Id, GrantId = grant.Id, UserId = order.UserId, PlanId = order.PlanId });
    }
}
