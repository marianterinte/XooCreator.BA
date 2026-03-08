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
public class GrantSupporterPackEndpoint
{
    private static readonly HashSet<string> ValidPlanIds = new(StringComparer.OrdinalIgnoreCase)
        { "Bronze", "Silver", "Gold", "Platinum" };

    /// <summary>Generative LOI credits per plan (03, 05).</summary>
    private static readonly IReadOnlyDictionary<string, int> PlanGenerativeCredits = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        { "Bronze", 5 },
        { "Silver", 10 },
        { "Gold", 30 },
        { "Platinum", 30 }
    };

    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;

    public GrantSupporterPackEndpoint(XooDbContext db, IAuth0UserService auth0)
    {
        _db = db;
        _auth0 = auth0;
    }

    public record GrantSupporterPackRequest(string? UserId, string? Email, string PlanId, string? EmailUsed);

    [Route("/api/admin/supporter-packs/grant")]
    [Authorize]
    public static async Task<Results<Ok<object>, NotFound, ForbidHttpResult, BadRequest<string>>> HandlePost(
        [FromBody] GrantSupporterPackRequest? request,
        [FromServices] GrantSupporterPackEndpoint ep,
        CancellationToken ct)
    {
        var admin = await ep._auth0.GetCurrentUserAsync(ct);
        if (admin == null || !admin.Roles.Contains(UserRole.Admin))
            return TypedResults.Forbid();

        if (request == null || string.IsNullOrWhiteSpace(request.PlanId))
            return TypedResults.BadRequest("PlanId is required.");
        if (!ValidPlanIds.Contains(request.PlanId.Trim()))
            return TypedResults.BadRequest("PlanId must be one of: Bronze, Silver, Gold, Platinum.");
        if (string.IsNullOrWhiteSpace(request.UserId) && string.IsNullOrWhiteSpace(request.Email))
            return TypedResults.BadRequest("UserId or Email is required.");

        Guid? userId = null;
        if (!string.IsNullOrWhiteSpace(request.UserId) && Guid.TryParse(request.UserId.Trim(), out var parsedId))
            userId = parsedId;
        if (userId == null && !string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim();
            var found = await ep._db.AlchimaliaUsers
                .AsNoTracking()
                .Where(u => u.Email.ToLower() == email.ToLower())
                .Select(u => u.Id)
                .FirstOrDefaultAsync(ct);
            if (found == default)
                return TypedResults.NotFound();
            userId = found;
        }
        if (userId == null)
            return TypedResults.NotFound();

        var userExists = await ep._db.AlchimaliaUsers.AsNoTracking().AnyAsync(u => u.Id == userId.Value, ct);
        if (!userExists)
            return TypedResults.NotFound();

        var grant = new UserPackGrant
        {
            Id = Guid.NewGuid(),
            UserId = userId.Value,
            PlanId = request.PlanId.Trim(),
            GrantedAtUtc = DateTime.UtcNow,
            GrantedByUserId = admin.Id,
            EmailUsed = string.IsNullOrWhiteSpace(request.EmailUsed) ? null : request.EmailUsed.Trim(),
            OrderId = null
        };
        ep._db.UserPackGrants.Add(grant);

        var creditsToAdd = PlanGenerativeCredits.TryGetValue(grant.PlanId, out var c) ? c : 0;
        if (creditsToAdd > 0)
        {
            var wallet = await ep._db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId.Value, ct);
            if (wallet == null)
            {
                wallet = new CreditWallet
                {
                    UserId = userId.Value,
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

        await ep._db.SaveChangesAsync(ct);

        return TypedResults.Ok((object)new { GrantId = grant.Id, UserId = grant.UserId, PlanId = grant.PlanId });
    }
}
