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
public class AdminGrantUnlimitedPrintEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;

    public AdminGrantUnlimitedPrintEndpoint(XooDbContext db, IAuth0UserService auth0)
    {
        _db = db;
        _auth0 = auth0;
    }

    [Route("/api/admin/users/{userId}/grant-unlimited-print")]
    [Authorize]
    public static async Task<Results<Ok, NotFound, ForbidHttpResult, BadRequest<string>>> HandlePost(
        [FromRoute] Guid userId,
        [FromServices] AdminGrantUnlimitedPrintEndpoint ep,
        CancellationToken ct)
    {
        var admin = await ep._auth0.GetCurrentUserAsync(ct);
        if (admin == null || !admin.Roles.Contains(UserRole.Admin))
            return TypedResults.Forbid();

        var userExists = await ep._db.AlchimaliaUsers.AsNoTracking().AnyAsync(u => u.Id == userId, ct);
        if (!userExists)
            return TypedResults.NotFound();

        var hasActive = await ep._db.UserSubscriptions
            .AsNoTracking()
            .AnyAsync(u => u.UserId == userId && (u.ExpiresAtUtc == null || u.ExpiresAtUtc > DateTime.UtcNow), ct);
        if (hasActive)
            return TypedResults.BadRequest("User already has unlimited print.");

        var sub = new UserSubscription
        {
            UserId = userId,
            PlanId = "donation-lifetime",
            StripeSessionId = null,
            StripeCustomerId = null,
            StripePaymentIntentId = null,
            PaidAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = null,
            CreatedAtUtc = DateTime.UtcNow
        };
        ep._db.UserSubscriptions.Add(sub);
        await ep._db.SaveChangesAsync(ct);

        return TypedResults.Ok();
    }
}
