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
public class RejectSupporterPackOrderEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;

    public RejectSupporterPackOrderEndpoint(XooDbContext db, IAuth0UserService auth0)
    {
        _db = db;
        _auth0 = auth0;
    }

    [Route("/api/admin/supporter-packs/orders/{orderId:guid}/reject")]
    [Authorize]
    public static async Task<Results<Ok<object>, NotFound, Conflict<string>, ForbidHttpResult>> HandlePost(
        Guid orderId,
        [FromServices] RejectSupporterPackOrderEndpoint ep,
        CancellationToken ct)
    {
        var admin = await ep._auth0.GetCurrentUserAsync(ct);
        if (admin == null || !admin.Roles.Contains(UserRole.Admin))
            return TypedResults.Forbid();

        var order = await ep._db.SupporterPackOrders
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);
        if (order == null)
            return TypedResults.NotFound();

        if (order.Status != SupporterPackOrderStatus.PendingPayment && order.Status != SupporterPackOrderStatus.PaymentConfirmed)
            return TypedResults.Conflict("Order can only be rejected when PendingPayment or PaymentConfirmed.");

        order.Status = SupporterPackOrderStatus.Rejected;
        order.ProcessedAtUtc = DateTime.UtcNow;
        order.ProcessedByUserId = admin.Id;

        await ep._db.SaveChangesAsync(ct);

        return TypedResults.Ok((object)new { OrderId = order.Id, Status = "Rejected" });
    }
}
