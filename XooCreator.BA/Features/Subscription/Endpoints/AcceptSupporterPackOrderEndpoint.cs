using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.Subscription.Endpoints;

[Endpoint]
public class AcceptSupporterPackOrderEndpoint
{
    private static readonly IReadOnlyDictionary<string, int> PlanGenerativeCredits = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        { "Bronze", 5 },
        { "Silver", 10 },
        { "Gold", 30 },
        { "Platinum", 30 }
    };

    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;

    public AcceptSupporterPackOrderEndpoint(XooDbContext db, IAuth0UserService auth0)
    {
        _db = db;
        _auth0 = auth0;
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

        var creditsToAdd = PlanGenerativeCredits.TryGetValue(order.PlanId, out var c) ? c : 0;
        if (creditsToAdd > 0)
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
                    UpdatedAt = DateTime.UtcNow
                };
                ep._db.CreditWallets.Add(wallet);
            }
            else
            {
                wallet.GenerativeBalance += creditsToAdd;
                wallet.UpdatedAt = DateTime.UtcNow;
            }
        }

        order.Status = SupporterPackOrderStatus.Fulfilled;
        order.ProcessedAtUtc = DateTime.UtcNow;
        order.ProcessedByUserId = admin.Id;

        await ep._db.SaveChangesAsync(ct);

        return TypedResults.Ok((object)new { OrderId = order.Id, GrantId = grant.Id, UserId = order.UserId, PlanId = order.PlanId });
    }
}
