using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public class EpicsMarketplaceRepository
{
    private readonly XooDbContext _context;
    private readonly ILogger<EpicsMarketplaceRepository>? _logger;

    public EpicsMarketplaceRepository(
        XooDbContext context,
        ILogger<EpicsMarketplaceRepository>? logger = null)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(List<EpicMarketplaceItemDto> Epics, int TotalCount, bool HasMore)> GetMarketplaceEpicsWithPaginationAsync(
        Guid userId,
        string locale,
        SearchEpicsRequest request)
    {
        try
        {
            // Query only published epics
            var query = _context.StoryEpics
                .Include(e => e.Owner)
                .Include(e => e.StoryNodes)
                .Where(e => e.Status == "published" && e.PublishedAtUtc != null);

            // Apply search filter - search in Name and Description (case-insensitive)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim();
                query = query.Where(e =>
                    (e.Name != null && EF.Functions.ILike(e.Name, $"%{searchTerm}%")) ||
                    (e.Description != null && EF.Functions.ILike(e.Description, $"%{searchTerm}%")));
            }

            // Apply sorting
            query = request.SortBy.ToLowerInvariant() switch
            {
                "name" => request.SortOrder.ToLowerInvariant() == "asc"
                    ? query.OrderBy(e => e.Name)
                    : query.OrderByDescending(e => e.Name),
                "readers" => request.SortOrder.ToLowerInvariant() == "asc"
                    ? query.OrderBy(e => e.StoryNodes.Count) // Simplified: use story count as proxy
                    : query.OrderByDescending(e => e.StoryNodes.Count),
                "rating" => request.SortOrder.ToLowerInvariant() == "asc"
                    ? query.OrderBy(e => e.CreatedAt) // Placeholder: would need to join with reviews
                    : query.OrderByDescending(e => e.CreatedAt),
                _ => request.SortOrder.ToLowerInvariant() == "asc"
                    ? query.OrderBy(e => e.PublishedAtUtc)
                    : query.OrderByDescending(e => e.PublishedAtUtc)
            };

            // Calculate total count BEFORE pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var epics = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            // Get story IDs for all epics to calculate readers and ratings
            var epicStoryIds = epics
                .SelectMany(e => e.StoryNodes.Select(sn => sn.StoryId))
                .Distinct()
                .ToList();

            // Get readers count for stories in these epics
            var readersCounts = await _context.StoryReaders
                .Where(sr => epicStoryIds.Contains(sr.StoryId))
                .GroupBy(sr => sr.StoryId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            // Get review statistics for stories in these epics
            var reviewStatsRaw = await _context.StoryReviews
                .Where(r => epicStoryIds.Contains(r.StoryId))
                .GroupBy(r => r.StoryId)
                .Select(g => new
                {
                    StoryId = g.Key,
                    AverageRating = g.Average(r => (double)r.Rating),
                    TotalReviews = g.Count()
                })
                .ToListAsync();

            var reviewStats = reviewStatsRaw.ToDictionary(
                x => x.StoryId,
                x => new { x.AverageRating, x.TotalReviews });

            // Map to DTOs
            var dtoList = epics.Select(epic =>
            {
                var epicStoryIds = epic.StoryNodes.Select(sn => sn.StoryId).ToList();
                
                // Calculate totals from dictionaries
                var totalReaders = epicStoryIds
                    .Where(id => readersCounts.ContainsKey(id))
                    .Sum(id => readersCounts[id]);
                
                var reviewData = epicStoryIds
                    .Where(id => reviewStats.ContainsKey(id))
                    .Select(id => reviewStats[id])
                    .ToList();
                
                var totalReviews = reviewData.Sum(r => r.TotalReviews);
                var avgRating = reviewData.Any() && totalReviews > 0
                    ? reviewData.Sum(r => r.AverageRating * r.TotalReviews) / totalReviews
                    : 0.0;

                return new EpicMarketplaceItemDto
                {
                    Id = epic.Id,
                    Name = epic.Name,
                    Description = epic.Description,
                    CoverImageUrl = epic.CoverImageUrl,
                    CreatedBy = epic.OwnerUserId,
                    CreatedByName = epic.Owner?.Email ?? epic.Owner?.Name ?? "Unknown",
                    CreatedAt = epic.CreatedAt,
                    PublishedAtUtc = epic.PublishedAtUtc,
                    StoryCount = epic.StoryNodes.Count,
                    RegionCount = epic.Regions.Count,
                    ReadersCount = totalReaders,
                    AverageRating = avgRating
                };
            }).ToList();

            // Calculate hasMore
            var hasMore = (request.Page * request.PageSize) < totalCount;

            return (dtoList, totalCount, hasMore);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting marketplace epics");
            throw;
        }
    }

    public async Task<EpicDetailsDto?> GetEpicDetailsAsync(string epicId, Guid userId, string locale)
    {
        try
        {
            var epic = await _context.StoryEpics
                .Include(e => e.Owner)
                .Include(e => e.StoryNodes)
                .Include(e => e.Regions)
                .FirstOrDefaultAsync(e => e.Id == epicId && e.Status == "published");

            if (epic == null)
                return null;

            // Get story IDs for this epic
            var storyIds = epic.StoryNodes.Select(sn => sn.StoryId).ToList();

            // Get readers count for stories in this epic
            var readersCounts = await _context.StoryReaders
                .Where(sr => storyIds.Contains(sr.StoryId))
                .GroupBy(sr => sr.StoryId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            // Get review statistics for stories in this epic
            var reviewStatsRaw = await _context.StoryReviews
                .Where(r => storyIds.Contains(r.StoryId))
                .GroupBy(r => r.StoryId)
                .Select(g => new
                {
                    StoryId = g.Key,
                    AverageRating = g.Average(r => (double)r.Rating),
                    TotalReviews = g.Count()
                })
                .ToListAsync();

            var reviewStats = reviewStatsRaw.ToDictionary(
                x => x.StoryId,
                x => new { x.AverageRating, x.TotalReviews });

            // Calculate totals
            var totalReaders = storyIds
                .Where(id => readersCounts.ContainsKey(id))
                .Sum(id => readersCounts[id]);

            var reviewData = storyIds
                .Where(id => reviewStats.ContainsKey(id))
                .Select(id => reviewStats[id])
                .ToList();

            var totalReviews = reviewData.Sum(r => r.TotalReviews);
            var avgRating = reviewData.Any() && totalReviews > 0
                ? reviewData.Sum(r => r.AverageRating * r.TotalReviews) / totalReviews
                : 0.0;

            return new EpicDetailsDto
            {
                Id = epic.Id,
                Name = epic.Name,
                Description = epic.Description,
                CoverImageUrl = epic.CoverImageUrl,
                CreatedBy = epic.OwnerUserId,
                CreatedByName = epic.Owner?.Email ?? epic.Owner?.Name ?? "Unknown",
                CreatedAt = epic.CreatedAt,
                PublishedAtUtc = epic.PublishedAtUtc,
                StoryCount = epic.StoryNodes.Count,
                RegionCount = epic.Regions.Count,
                ReadersCount = totalReaders,
                AverageRating = avgRating,
                TotalReviews = totalReviews
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting epic details for epicId: {EpicId}", epicId);
            throw;
        }
    }

    /// <summary>
    /// Gets list of story IDs that are assigned to published epics
    /// </summary>
    public async Task<List<string>> GetStoryIdsInPublishedEpicsAsync()
    {
        try
        {
            return await _context.StoryEpicStoryNodes
                .Where(sn => sn.Epic.Status == "published" && sn.Epic.PublishedAtUtc != null)
                .Select(sn => sn.StoryId)
                .Distinct()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting story IDs in published epics");
            throw;
        }
    }
}

