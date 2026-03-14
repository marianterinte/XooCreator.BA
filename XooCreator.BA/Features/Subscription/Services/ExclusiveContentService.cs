using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.Subscription.Services;

public interface IExclusiveContentService
{
    Task<bool> IsStoryExclusiveAsync(string storyId, CancellationToken ct = default);
    Task<bool> IsEpicExclusiveAsync(string epicId, CancellationToken ct = default);
    Task<bool> HasAccessToStoryAsync(Guid userId, string storyId, CancellationToken ct = default);
    Task<bool> HasAccessToEpicAsync(Guid userId, string epicId, CancellationToken ct = default);
    Task<(IReadOnlyList<string> StoryIds, IReadOnlyList<string> EpicIds)> GetUserExclusiveContentAsync(Guid userId, CancellationToken ct = default);
    /// <summary>Returns the union of all story/epic IDs that are in any plan's exclusive bundle (for listing badges).</summary>
    Task<(IReadOnlySet<string> StoryIds, IReadOnlySet<string> EpicIds)> GetAllExclusiveIdsAsync(CancellationToken ct = default);
    /// <summary>Returns plan IDs that include this story in their exclusive bundle (e.g. ["Gold","Platinum"]).</summary>
    Task<IReadOnlyList<string>> GetPlanIdsForExclusiveStoryAsync(string storyId, CancellationToken ct = default);
    /// <summary>Returns plan IDs that include this epic in their exclusive bundle.</summary>
    Task<IReadOnlyList<string>> GetPlanIdsForExclusiveEpicAsync(string epicId, CancellationToken ct = default);
    /// <summary>Minimum tier for story: lowest plan in order Bronze&lt;Silver&lt;Gold&lt;Platinum that contains the story, or null if not exclusive.</summary>
    Task<string?> GetMinimumTierForStoryAsync(string storyId, CancellationToken ct = default);
    /// <summary>Minimum tier for epic: same as story.</summary>
    Task<string?> GetMinimumTierForEpicAsync(string epicId, CancellationToken ct = default);
    /// <summary>Returns minimum tier for every exclusive story (storyId -> tier). Used to enrich list without N+1.</summary>
    Task<IReadOnlyDictionary<string, string>> GetMinimumTiersForExclusiveStoriesAsync(CancellationToken ct = default);
    /// <summary>Returns minimum tier for every exclusive epic (epicId -> tier).</summary>
    Task<IReadOnlyDictionary<string, string>> GetMinimumTiersForExclusiveEpicsAsync(CancellationToken ct = default);
}

public class ExclusiveContentService : IExclusiveContentService
{
    private readonly XooDbContext _db;

    public ExclusiveContentService(XooDbContext db)
    {
        _db = db;
    }

    public async Task<bool> IsStoryExclusiveAsync(string storyId, CancellationToken ct = default)
    {
        var all = await GetAllExclusiveStoryIdsAsync(ct);
        return all.Contains(NormalizeId(storyId));
    }

    public async Task<bool> IsEpicExclusiveAsync(string epicId, CancellationToken ct = default)
    {
        var all = await GetAllExclusiveEpicIdsAsync(ct);
        return all.Contains(NormalizeId(epicId));
    }

    public async Task<bool> HasAccessToStoryAsync(Guid userId, string storyId, CancellationToken ct = default)
    {
        var userStoryIds = (await GetUserExclusiveContentAsync(userId, ct)).StoryIds;
        return userStoryIds.Contains(NormalizeId(storyId));
    }

    public async Task<bool> HasAccessToEpicAsync(Guid userId, string epicId, CancellationToken ct = default)
    {
        var userEpicIds = (await GetUserExclusiveContentAsync(userId, ct)).EpicIds;
        return userEpicIds.Contains(NormalizeId(epicId));
    }

    public async Task<(IReadOnlyList<string> StoryIds, IReadOnlyList<string> EpicIds)> GetUserExclusiveContentAsync(Guid userId, CancellationToken ct = default)
    {
        var planIds = await _db.UserPackGrants
            .AsNoTracking()
            .Where(g => g.UserId == userId)
            .Select(g => g.PlanId)
            .Distinct()
            .ToListAsync(ct);

        if (planIds.Count == 0)
            return (new List<string>(), new List<string>());

        var bundles = await _db.SupporterPackPlanExclusives
            .AsNoTracking()
            .Where(b => planIds.Contains(b.PlanId))
            .ToListAsync(ct);

        var storyIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var epicIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var b in bundles)
        {
            foreach (var id in ParseJsonIds(b.ExclusiveStoryIdsJson))
                storyIds.Add(id);
            foreach (var id in ParseJsonIds(b.ExclusiveEpicIdsJson))
                epicIds.Add(id);
        }
        return (storyIds.ToList(), epicIds.ToList());
    }

    public async Task<(IReadOnlySet<string> StoryIds, IReadOnlySet<string> EpicIds)> GetAllExclusiveIdsAsync(CancellationToken ct = default)
    {
        var storyIds = await GetAllExclusiveStoryIdsAsync(ct);
        var epicIds = await GetAllExclusiveEpicIdsAsync(ct);
        return (storyIds, epicIds);
    }

    /// <summary>Plan tier order for minimum tier: Bronze=0, Silver=1, Gold=2, Platinum=3.</summary>
    private static readonly string[] PlanTierOrder = { "Bronze", "Silver", "Gold", "Platinum" };

    public async Task<IReadOnlyList<string>> GetPlanIdsForExclusiveStoryAsync(string storyId, CancellationToken ct = default)
    {
        var normalized = NormalizeId(storyId);
        if (string.IsNullOrEmpty(normalized)) return Array.Empty<string>();
        var bundles = await _db.SupporterPackPlanExclusives.AsNoTracking().ToListAsync(ct);
        var planIds = new List<string>();
        foreach (var b in bundles)
        {
            foreach (var id in ParseJsonIds(b.ExclusiveStoryIdsJson))
            {
                if (string.Equals(id, normalized, StringComparison.OrdinalIgnoreCase))
                {
                    planIds.Add(b.PlanId.Trim());
                    break;
                }
            }
        }
        return planIds;
    }

    public async Task<IReadOnlyList<string>> GetPlanIdsForExclusiveEpicAsync(string epicId, CancellationToken ct = default)
    {
        var normalized = NormalizeId(epicId);
        if (string.IsNullOrEmpty(normalized)) return Array.Empty<string>();
        var bundles = await _db.SupporterPackPlanExclusives.AsNoTracking().ToListAsync(ct);
        var planIds = new List<string>();
        foreach (var b in bundles)
        {
            foreach (var id in ParseJsonIds(b.ExclusiveEpicIdsJson))
            {
                if (string.Equals(id, normalized, StringComparison.OrdinalIgnoreCase))
                {
                    planIds.Add(b.PlanId.Trim());
                    break;
                }
            }
        }
        return planIds;
    }

    public async Task<string?> GetMinimumTierForStoryAsync(string storyId, CancellationToken ct = default)
    {
        var planIds = await GetPlanIdsForExclusiveStoryAsync(storyId, ct);
        return GetMinimumTierFromPlanIds(planIds);
    }

    public async Task<string?> GetMinimumTierForEpicAsync(string epicId, CancellationToken ct = default)
    {
        var planIds = await GetPlanIdsForExclusiveEpicAsync(epicId, ct);
        return GetMinimumTierFromPlanIds(planIds);
    }

    private static string? GetMinimumTierFromPlanIds(IReadOnlyList<string> planIds)
    {
        if (planIds == null || planIds.Count == 0) return null;
        var tierIndex = int.MaxValue;
        for (var i = 0; i < PlanTierOrder.Length; i++)
        {
            if (planIds.Any(p => string.Equals(p, PlanTierOrder[i], StringComparison.OrdinalIgnoreCase)))
            {
                tierIndex = i;
                break;
            }
        }
        return tierIndex < PlanTierOrder.Length ? PlanTierOrder[tierIndex] : null;
    }

    public async Task<IReadOnlyDictionary<string, string>> GetMinimumTiersForExclusiveStoriesAsync(CancellationToken ct = default)
    {
        var bundles = await _db.SupporterPackPlanExclusives.AsNoTracking().ToListAsync(ct);
        var storyIdToPlans = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var b in bundles)
        {
            foreach (var id in ParseJsonIds(b.ExclusiveStoryIdsJson))
            {
                if (string.IsNullOrWhiteSpace(id)) continue;
                if (!storyIdToPlans.TryGetValue(id, out var set))
                {
                    set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    storyIdToPlans[id] = set;
                }
                set.Add(b.PlanId.Trim());
            }
        }
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in storyIdToPlans)
        {
            var tier = GetMinimumTierFromPlanIds(kv.Value.ToList());
            if (tier != null)
                result[kv.Key] = tier;
        }
        return result;
    }

    public async Task<IReadOnlyDictionary<string, string>> GetMinimumTiersForExclusiveEpicsAsync(CancellationToken ct = default)
    {
        var bundles = await _db.SupporterPackPlanExclusives.AsNoTracking().ToListAsync(ct);
        var epicIdToPlans = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var b in bundles)
        {
            foreach (var id in ParseJsonIds(b.ExclusiveEpicIdsJson))
            {
                if (string.IsNullOrWhiteSpace(id)) continue;
                if (!epicIdToPlans.TryGetValue(id, out var set))
                {
                    set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    epicIdToPlans[id] = set;
                }
                set.Add(b.PlanId.Trim());
            }
        }
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in epicIdToPlans)
        {
            var tier = GetMinimumTierFromPlanIds(kv.Value.ToList());
            if (tier != null)
                result[kv.Key] = tier;
        }
        return result;
    }

    private async Task<HashSet<string>> GetAllExclusiveStoryIdsAsync(CancellationToken ct)
    {
        var bundles = await _db.SupporterPackPlanExclusives.AsNoTracking().ToListAsync(ct);
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var b in bundles)
            foreach (var id in ParseJsonIds(b.ExclusiveStoryIdsJson))
                set.Add(id);
        return set;
    }

    private async Task<HashSet<string>> GetAllExclusiveEpicIdsAsync(CancellationToken ct)
    {
        var bundles = await _db.SupporterPackPlanExclusives.AsNoTracking().ToListAsync(ct);
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var b in bundles)
            foreach (var id in ParseJsonIds(b.ExclusiveEpicIdsJson))
                set.Add(id);
        return set;
    }

    private static List<string> ParseJsonIds(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<string>();
        try
        {
            var list = JsonSerializer.Deserialize<List<string>>(json);
            return list?.Select(s => (s ?? "").Trim()).Where(s => s.Length > 0).ToList() ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private static string NormalizeId(string id) => (id ?? "").Trim();
}
