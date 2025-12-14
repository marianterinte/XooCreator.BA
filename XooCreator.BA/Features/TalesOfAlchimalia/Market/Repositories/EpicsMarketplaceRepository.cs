using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
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
            // Query only published epics (use StoryEpicDefinitions)
            var query = _context.StoryEpicDefinitions
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

            // Get epic IDs to calculate readers count and review statistics
            var epicIds = epics.Select(e => e.Id).ToList();

            // Get readers count for epics (direct count from EpicReaders table)
            var epicReadersCounts = await _context.EpicReaders
                .Where(er => epicIds.Contains(er.EpicId))
                .GroupBy(er => er.EpicId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            // Get review statistics from EpicReviews (not StoryReviews)
            var epicReviewStats = await _context.EpicReviews
                .Where(r => epicIds.Contains(r.EpicId) && r.IsActive)
                .GroupBy(r => r.EpicId)
                .Select(g => new
                {
                    EpicId = g.Key,
                    AverageRating = g.Average(r => (double)r.Rating),
                    TotalReviews = g.Count()
                })
                .ToListAsync();

            var reviewStats = epicReviewStats.ToDictionary(
                x => x.EpicId,
                x => new { x.AverageRating, x.TotalReviews });

            // Map to DTOs
            var dtoList = epics.Select(epic =>
            {
                // Get epic readers count (direct from EpicReaders)
                var totalReaders = epicReadersCounts.TryGetValue(epic.Id, out var count) ? count : 0;
                
                // Get review statistics from EpicReviews
                var reviewData = reviewStats.TryGetValue(epic.Id, out var stats) ? stats : null;
                var totalReviews = reviewData?.TotalReviews ?? 0;
                var avgRating = reviewData?.AverageRating ?? 0.0;

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
            var epic = await _context.StoryEpicDefinitions
                .Include(e => e.Owner)
                .Include(e => e.StoryNodes)
                .Include(e => e.Regions)
                .FirstOrDefaultAsync(e => e.Id == epicId && e.Status == "published");

            if (epic == null)
                return null;

            // Get story IDs for this epic
            var storyIds = epic.StoryNodes.Select(sn => sn.StoryId).ToList();

            // Get epic readers count (direct count from EpicReaders table)
            var totalReaders = await GetEpicReadersCountAsync(epicId);

            // Get review statistics from EpicReviews (not StoryReviews)
            var epicReviews = await _context.EpicReviews
                .Where(r => r.EpicId == epicId && r.IsActive)
                .ToListAsync();

            var totalReviews = epicReviews.Count;
            var avgRating = totalReviews > 0
                ? epicReviews.Average(r => r.Rating)
                : 0.0;

            // Get user's review if exists
            EpicReviewDto? userReview = null;
            var userEpicReview = await _context.EpicReviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.EpicId == epicId && r.UserId == userId && r.IsActive);

            if (userEpicReview != null && userEpicReview.User != null)
            {
                userReview = new EpicReviewDto
                {
                    Id = userEpicReview.Id,
                    UserId = userEpicReview.UserId,
                    UserName = userEpicReview.User.Name,
                    UserPicture = userEpicReview.User.Picture,
                    EpicId = userEpicReview.EpicId,
                    Rating = userEpicReview.Rating,
                    Comment = userEpicReview.Comment,
                    CreatedAt = userEpicReview.CreatedAt,
                    UpdatedAt = userEpicReview.UpdatedAt,
                    IsOwnReview = true
                };
            }

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
                TotalReviews = totalReviews,
                UserReview = userReview
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
            return await _context.StoryEpicDefinitionStoryNodes
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

    /// <summary>
    /// Ensures an epic reader record exists (adds if not exists)
    /// </summary>
    public async Task EnsureEpicReaderAsync(Guid userId, string epicId, EpicAcquisitionSource source)
    {
        var exists = await _context.EpicReaders
            .AsNoTracking()
            .AnyAsync(er => er.UserId == userId && er.EpicId == epicId);

        if (exists)
        {
            return;
        }

        var reader = new EpicReader
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EpicId = epicId,
            AcquiredAt = DateTime.UtcNow,
            AcquisitionSource = source
        };

        _context.EpicReaders.Add(reader);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets the count of unique readers for an epic
    /// </summary>
    public Task<int> GetEpicReadersCountAsync(string epicId)
    {
        return _context.EpicReaders
            .Where(er => er.EpicId == epicId)
            .CountAsync();
    }
}

