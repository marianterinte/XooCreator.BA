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
