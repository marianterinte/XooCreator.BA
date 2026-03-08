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
public class GetSupporterPackOrdersEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;

    public GetSupporterPackOrdersEndpoint(XooDbContext db, IAuth0UserService auth0)
    {
        _db = db;
        _auth0 = auth0;
    }

    public record OrderDto(
        Guid Id,
        Guid UserId,
        string? UserEmail,
        string PlanId,
        decimal AmountRon,
        int Status,
        DateTime CreatedAtUtc,
        string? OrderReference,
        DateTime? ProcessedAtUtc,
        Guid? ProcessedByUserId);

    [Route("/api/admin/supporter-packs/orders")]
    [Authorize]
    public static async Task<Results<Ok<IReadOnlyList<OrderDto>>, ForbidHttpResult>> HandleGet(
        [FromQuery] string? status,
        [FromServices] GetSupporterPackOrdersEndpoint ep,
        CancellationToken ct)
    {
        var admin = await ep._auth0.GetCurrentUserAsync(ct);
        if (admin == null || !admin.Roles.Contains(UserRole.Admin))
            return TypedResults.Forbid();

        var query = ep._db.SupporterPackOrders
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(status) && int.TryParse(status, out var statusVal))
            query = query.Where(o => o.Status == (SupporterPackOrderStatus)statusVal);

        var orders = await query
            .OrderByDescending(o => o.CreatedAtUtc)
            .Join(
                ep._db.AlchimaliaUsers.AsNoTracking(),
                o => o.UserId,
                u => u.Id,
                (o, u) => new { Order = o, UserEmail = u.Email })
            .Select(x => new OrderDto(
                x.Order.Id,
                x.Order.UserId,
                x.UserEmail,
                x.Order.PlanId,
                x.Order.Amount,
                (int)x.Order.Status,
                x.Order.CreatedAtUtc,
                x.Order.OrderReference,
                x.Order.ProcessedAtUtc,
                x.Order.ProcessedByUserId))
            .ToListAsync(ct);

        return TypedResults.Ok<IReadOnlyList<OrderDto>>(orders);
    }
}
