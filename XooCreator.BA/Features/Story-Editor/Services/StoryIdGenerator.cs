using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryIdGenerator
{
    Task<string> GenerateNextAsync(Guid ownerUserId, string ownerEmail, CancellationToken ct);
}

/// <summary>
/// Centralized logic for generating the next storyId per owner.
/// Keeps the legacy pattern {email}-s{n} while preventing accidental reuse
/// when drafts are deleted or stories are already published.
/// </summary>
public class StoryIdGenerator : IStoryIdGenerator
{
    private readonly XooDbContext _db;
    private readonly ILogger<StoryIdGenerator> _logger;

    public StoryIdGenerator(XooDbContext db, ILogger<StoryIdGenerator> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<string> GenerateNextAsync(Guid ownerUserId, string ownerEmail, CancellationToken ct)
    {
        if (ownerUserId == Guid.Empty)
        {
            throw new ArgumentException("Owner id is required", nameof(ownerUserId));
        }

        if (string.IsNullOrWhiteSpace(ownerEmail))
        {
            throw new ArgumentException("Owner email is required", nameof(ownerEmail));
        }

        var normalizedEmail = ownerEmail.Trim();
        var existingIds = await LoadExistingStoryIdsAsync(ownerUserId, ct);
        var nextIndex = CalculateNextIndex(existingIds, normalizedEmail);
        var nextId = $"{normalizedEmail}-s{nextIndex}";

        _logger.LogDebug("Generated storyId {StoryId} for owner {OwnerId}", nextId, ownerUserId);
        return nextId;
    }

    private async Task<List<string>> LoadExistingStoryIdsAsync(Guid ownerUserId, CancellationToken ct)
    {
        var craftIds = await _db.StoryCrafts
            .Where(c => c.OwnerUserId == ownerUserId)
            .Select(c => c.StoryId)
            .ToListAsync(ct);

        var publishedIds = await _db.StoryDefinitions
            .Where(d => d.CreatedBy == ownerUserId)
            .Select(d => d.StoryId)
            .ToListAsync(ct);

        if (publishedIds.Count == 0)
        {
            return craftIds;
        }

        craftIds.AddRange(publishedIds);
        return craftIds;
    }

    private static int CalculateNextIndex(IEnumerable<string> storyIds, string ownerEmail)
    {
        var prefix = $"{ownerEmail}-s";
        var maxIndex = storyIds
            .Select(id => ExtractIndex(id, prefix))
            .DefaultIfEmpty(0)
            .Max();

        return maxIndex + 1;
    }

    private static int ExtractIndex(string storyId, string expectedPrefix)
    {
        if (string.IsNullOrWhiteSpace(storyId))
        {
            return 0;
        }

        if (!storyId.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }

        var suffix = storyId[expectedPrefix.Length..];
        return int.TryParse(suffix, out var number) ? number : 0;
    }
}

