using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.Subscription.Services;

/// <summary>
/// Reads Supporter Pack plan configuration from DB. Used by grant, order, print-quota and public plans endpoints.
/// </summary>
public interface ISupporterPackPlanConfigService
{
    Task<SupporterPackPlan?> GetPlanAsync(string planId, CancellationToken ct = default);
    Task<IReadOnlyList<SupporterPackPlan>> GetAllPlansAsync(CancellationToken ct = default);
}

public class SupporterPackPlanConfigService : ISupporterPackPlanConfigService
{
    private readonly XooDbContext _db;

    public SupporterPackPlanConfigService(XooDbContext db)
    {
        _db = db;
    }

    public async Task<SupporterPackPlan?> GetPlanAsync(string planId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(planId)) return null;
        return await _db.SupporterPackPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PlanId == planId.Trim(), ct);
    }

    public async Task<IReadOnlyList<SupporterPackPlan>> GetAllPlansAsync(CancellationToken ct = default)
    {
        return await _db.SupporterPackPlans
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.PlanId)
            .ToListAsync(ct);
    }
}
