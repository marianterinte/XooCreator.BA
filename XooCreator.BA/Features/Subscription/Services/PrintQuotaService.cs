using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.Subscription.Services;

public interface IPrintQuotaService
{
    Task<PrintQuotaResult> GetAsync(Guid userId, bool isAdmin, CancellationToken ct = default);
}

public record PrintQuotaResult(int UsedCount, int Limit, int Remaining, bool CanPrint, bool IsSupporter);

/// <summary>
/// Single source of truth for print/export quota enforcement and UI display.
/// Limit = sum of quotas from UserPackGrants (or FreePrintLimit if no grants).
/// UsedCount = total exports/prints (each completed job + each StoryPrintRecord).
/// Legacy: active UserSubscription implies unlimited quota (kept for backward compatibility).
/// </summary>
public sealed class PrintQuotaService : IPrintQuotaService
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
    private readonly IConfiguration _config;
    private readonly ISupporterPackPlanConfigService _planConfig;

    public PrintQuotaService(XooDbContext db, IConfiguration config, ISupporterPackPlanConfigService planConfig)
    {
        _db = db;
        _config = config;
        _planConfig = planConfig;
    }

    public async Task<PrintQuotaResult> GetAsync(Guid userId, bool isAdmin, CancellationToken ct = default)
    {
        if (isAdmin)
            return new PrintQuotaResult(UsedCount: 0, Limit: int.MaxValue, Remaining: int.MaxValue, CanPrint: true, IsSupporter: true);

        // Limit = sum of quotas from all UserPackGrants; if none, use FreePrintLimit
        var planIds = await _db.UserPackGrants
            .AsNoTracking()
            .Where(g => g.UserId == userId)
            .Select(g => g.PlanId)
            .ToListAsync(ct);

        var plans = await _planConfig.GetAllPlansAsync(ct);
        var quotaByPlan = plans.ToDictionary(p => p.PlanId, p => p.PrintQuota, StringComparer.OrdinalIgnoreCase);

        var limit = planIds.Sum(planId =>
            quotaByPlan.TryGetValue(planId, out var q) ? q :
            FallbackPlanPrintQuota.TryGetValue(planId, out var f) ? f :
            0);

        if (limit <= 0)
        {
            limit = _config.GetValue("Subscription:FreePrintLimit", 1);
            if (limit < 0) limit = 1;
        }

        var isSupporter = planIds.Count > 0;

        // Legacy: treat active UserSubscription as supporter (and unlimited was old behaviour; keep IsSupporter true)
        if (!isSupporter)
        {
            var nowUtc = DateTime.UtcNow;
            var hasActiveSub = await _db.UserSubscriptions
                .AsNoTracking()
                .AnyAsync(u => u.UserId == userId && (u.ExpiresAtUtc == null || u.ExpiresAtUtc > nowUtc), ct);
            if (hasActiveSub)
            {
                isSupporter = true;
                limit = int.MaxValue;
            }
        }

        // UsedCount = total exports + total client prints (each counts 1)
        var exportCount = await _db.StoryDocumentExportJobs
            .AsNoTracking()
            .CountAsync(j => j.RequestedByUserId == userId && j.Status == StoryDocumentExportJobStatus.Completed, ct);

        var printRecordCount = await _db.StoryPrintRecords
            .AsNoTracking()
            .CountAsync(r => r.UserId == userId, ct);

        var usedCount = exportCount + printRecordCount;

        var remaining = Math.Max(0, limit - usedCount);
        var canPrint = remaining > 0;

        return new PrintQuotaResult(
            UsedCount: usedCount,
            Limit: limit,
            Remaining: remaining,
            CanPrint: canPrint,
            IsSupporter: isSupporter);
    }
}

