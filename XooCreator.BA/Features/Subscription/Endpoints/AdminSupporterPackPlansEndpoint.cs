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
public class AdminSupporterPackPlansEndpoint
{
    private static readonly HashSet<string> ValidPlanIds = new(StringComparer.OrdinalIgnoreCase)
        { "Bronze", "Silver", "Gold", "Platinum" };

    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;

    public AdminSupporterPackPlansEndpoint(XooDbContext db, IAuth0UserService auth0)
    {
        _db = db;
        _auth0 = auth0;
    }

    public record PlanAdminDto(
        string PlanId,
        decimal PriceRon,
        int GenerativeCredits,
        int PrintQuota,
        int ExclusiveStoryAccessCount,
        int ExclusiveEpicAccessCount,
        int FullStoryGenerationCount,
        bool IsActive,
        DateTime UpdatedAtUtc);

    public record UpdatePlanRequest(
        decimal? PriceRon,
        int? GenerativeCredits,
        int? PrintQuota,
        int? ExclusiveStoryAccessCount,
        int? ExclusiveEpicAccessCount,
        int? FullStoryGenerationCount,
        bool? IsActive);

    [Route("/api/admin/supporter-packs/plans")]
    [Authorize]
    public static async Task<Results<Ok<IReadOnlyList<PlanAdminDto>>, ForbidHttpResult>> HandleGet(
        [FromServices] AdminSupporterPackPlansEndpoint ep,
        CancellationToken ct)
    {
        var admin = await ep._auth0.GetCurrentUserAsync(ct);
        if (admin == null || !admin.Roles.Contains(UserRole.Admin))
            return TypedResults.Forbid();

        var plans = await ep._db.SupporterPackPlans
            .AsNoTracking()
            .OrderBy(p => p.PlanId)
            .ToListAsync(ct);

        var dtos = plans.Select(p => new PlanAdminDto(
            p.PlanId,
            p.PriceRon,
            p.GenerativeCredits,
            p.PrintQuota,
            p.ExclusiveStoryAccessCount,
            p.ExclusiveEpicAccessCount,
            p.FullStoryGenerationCount,
            p.IsActive,
            p.UpdatedAtUtc)).ToList();

        return TypedResults.Ok<IReadOnlyList<PlanAdminDto>>(dtos);
    }

    [Route("/api/admin/supporter-packs/plans/{planId}")]
    [Authorize]
    public static async Task<Results<Ok<PlanAdminDto>, NotFound, ForbidHttpResult, BadRequest<string>>> HandlePut(
        string planId,
        [FromBody] UpdatePlanRequest? request,
        [FromServices] AdminSupporterPackPlansEndpoint ep,
        CancellationToken ct)
    {
        var admin = await ep._auth0.GetCurrentUserAsync(ct);
        if (admin == null || !admin.Roles.Contains(UserRole.Admin))
            return TypedResults.Forbid();

        var plan = (planId ?? "").Trim();
        if (string.IsNullOrEmpty(plan) || !ValidPlanIds.Contains(plan))
            return TypedResults.BadRequest("PlanId must be one of: Bronze, Silver, Gold, Platinum.");

        var entity = await ep._db.SupporterPackPlans.FirstOrDefaultAsync(p => p.PlanId == plan, ct);
        if (entity == null)
            return TypedResults.NotFound();

        if (request != null)
        {
            if (request.PriceRon.HasValue)
            {
                if (request.PriceRon.Value < 0) return TypedResults.BadRequest("PriceRon must be >= 0.");
                entity.PriceRon = request.PriceRon.Value;
            }
            if (request.GenerativeCredits.HasValue)
            {
                if (request.GenerativeCredits.Value < 0) return TypedResults.BadRequest("GenerativeCredits must be >= 0.");
                entity.GenerativeCredits = request.GenerativeCredits.Value;
            }
            if (request.PrintQuota.HasValue)
            {
                if (request.PrintQuota.Value < 0) return TypedResults.BadRequest("PrintQuota must be >= 0.");
                entity.PrintQuota = request.PrintQuota.Value;
            }
            if (request.ExclusiveStoryAccessCount.HasValue)
            {
                if (request.ExclusiveStoryAccessCount.Value < 0) return TypedResults.BadRequest("ExclusiveStoryAccessCount must be >= 0.");
                entity.ExclusiveStoryAccessCount = request.ExclusiveStoryAccessCount.Value;
            }
            if (request.ExclusiveEpicAccessCount.HasValue)
            {
                if (request.ExclusiveEpicAccessCount.Value < 0) return TypedResults.BadRequest("ExclusiveEpicAccessCount must be >= 0.");
                entity.ExclusiveEpicAccessCount = request.ExclusiveEpicAccessCount.Value;
            }
            if (request.FullStoryGenerationCount.HasValue)
            {
                if (request.FullStoryGenerationCount.Value < 0) return TypedResults.BadRequest("FullStoryGenerationCount must be >= 0.");
                entity.FullStoryGenerationCount = request.FullStoryGenerationCount.Value;
            }
            if (request.IsActive.HasValue)
                entity.IsActive = request.IsActive.Value;
        }

        entity.UpdatedAtUtc = DateTime.UtcNow;
        await ep._db.SaveChangesAsync(ct);

        return TypedResults.Ok(new PlanAdminDto(
            entity.PlanId,
            entity.PriceRon,
            entity.GenerativeCredits,
            entity.PrintQuota,
            entity.ExclusiveStoryAccessCount,
            entity.ExclusiveEpicAccessCount,
            entity.FullStoryGenerationCount,
            entity.IsActive,
            entity.UpdatedAtUtc));
    }
}
