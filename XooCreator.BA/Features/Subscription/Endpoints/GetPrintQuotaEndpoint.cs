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

namespace XooCreator.BA.Features.Subscription.Endpoints;

[Endpoint]
public class GetPrintQuotaEndpoint
{
    /// <summary>Fallback print quota per plan when SupporterPackPlans table is empty.</summary>
    private static readonly IReadOnlyDictionary<string, int> FallbackPlanPrintQuota = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        { "Bronze", 10 },
        { "Silver", 20 },
        { "Gold", 50 },
        { "Platinum", 50 }
    };

    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IConfiguration _config;
    private readonly ISupporterPackPlanConfigService _planConfig;

    public GetPrintQuotaEndpoint(XooDbContext db, IAuth0UserService auth0, IConfiguration config, ISupporterPackPlanConfigService planConfig)
    {
        _db = db;
        _auth0 = auth0;
        _config = config;
        _planConfig = planConfig;
    }

    public record PrintQuotaResponse(int UsedCount, int Limit, int Remaining, bool CanPrint, bool IsSupporter);

    /// <summary>
    /// Returns the user's print quota. Limit = sum of quotas from UserPackGrants (or FreePrintLimit if no grants).
    /// UsedCount = total exports/prints (each job completed + each StoryPrintRecord), not distinct stories.
    /// </summary>
    [Route("/api/{locale}/user/print-quota")]
    [Authorize]
    public static async Task<Results<Ok<PrintQuotaResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromQuery] string? storyId,
        [FromServices] GetPrintQuotaEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        if (isAdmin)
            return TypedResults.Ok(new PrintQuotaResponse(UsedCount: 0, Limit: int.MaxValue, Remaining: int.MaxValue, CanPrint: true, IsSupporter: true));

        // Limit = sum of quotas from all UserPackGrants; if none, use FreePrintLimit
        var grantLimit = await ep._db.UserPackGrants
            .AsNoTracking()
            .Where(g => g.UserId == user.Id)
            .Select(g => g.PlanId)
            .ToListAsync(ct);
        var plans = await ep._planConfig.GetAllPlansAsync(ct);
        var quotaByPlan = plans.ToDictionary(p => p.PlanId, p => p.PrintQuota, StringComparer.OrdinalIgnoreCase);
        var limit = grantLimit.Sum(planId => quotaByPlan.TryGetValue(planId, out var q) ? q : FallbackPlanPrintQuota.TryGetValue(planId, out var f) ? f : 0);
        if (limit <= 0)
        {
            limit = ep._config.GetValue("Subscription:FreePrintLimit", 1);
            if (limit < 0) limit = 1;
        }

        var isSupporter = grantLimit.Count > 0;

        // Legacy: treat active UserSubscription as supporter (and unlimited was old behaviour; keep IsSupporter true)
        if (!isSupporter)
        {
            var nowUtc = DateTime.UtcNow;
            var hasActiveSub = await ep._db.UserSubscriptions
                .AsNoTracking()
                .AnyAsync(u => u.UserId == user.Id && (u.ExpiresAtUtc == null || u.ExpiresAtUtc > nowUtc), ct);
            if (hasActiveSub)
            {
                isSupporter = true;
                limit = Math.Max(limit, int.MaxValue); // legacy unlimited
            }
        }

        // UsedCount = total exports + total client prints (each counts 1)
        var exportCount = await ep._db.StoryDocumentExportJobs
            .AsNoTracking()
            .CountAsync(j => j.RequestedByUserId == user.Id && j.Status == StoryDocumentExportJobStatus.Completed, ct);
        var printRecordCount = await ep._db.StoryPrintRecords
            .AsNoTracking()
            .CountAsync(r => r.UserId == user.Id, ct);
        var usedCount = exportCount + printRecordCount;

        var remaining = Math.Max(0, limit - usedCount);
        var canPrint = remaining > 0;

        return TypedResults.Ok(new PrintQuotaResponse(
            UsedCount: usedCount,
            Limit: limit,
            Remaining: remaining,
            CanPrint: canPrint,
            IsSupporter: isSupporter));
    }
}
