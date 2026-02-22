using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Caching;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public class EpicsMarketplaceRepository
{
    private readonly XooDbContext _context;
    private readonly IMarketplaceCatalogCache _catalogCache;
    private readonly ILogger<EpicsMarketplaceRepository>? _logger;

    public EpicsMarketplaceRepository(
        XooDbContext context,
        IMarketplaceCatalogCache catalogCache,
        ILogger<EpicsMarketplaceRepository>? logger = null)
    {
        _context = context;
        _catalogCache = catalogCache;
        _logger = logger;
    }

    public async Task<(List<EpicMarketplaceItemDto> Epics, int TotalCount, bool HasMore)> GetMarketplaceEpicsWithPaginationAsync(
        Guid userId,
        string locale,
        SearchEpicsRequest request)
    {
        try
        {
            var normalizedLocale = string.IsNullOrWhiteSpace(locale) ? "ro-ro" : locale.Trim().ToLowerInvariant();
            var baseItems = await _catalogCache.GetEpicsBaseAsync(normalizedLocale, CancellationToken.None);
            var stats = await _catalogCache.GetEpicStatsAsync(CancellationToken.None);

            IEnumerable<EpicMarketplaceBaseItem> q = baseItems;

            // Search by title/translation titles OR author (case-insensitive)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim();
                
                if (string.Equals(request.SearchType, "author", StringComparison.OrdinalIgnoreCase))
                {
                    // Search by Author Name
                    q = q.Where(e => e.SearchAuthors.Any(a => 
                        !string.IsNullOrWhiteSpace(a) &&
                        a.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
                }
                else
                {
                    // Default: Search by Title/Description
                    q = q.Where(e => e.SearchTexts.Any(t => !string.IsNullOrWhiteSpace(t) && t.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
                }
            }

            // Filter by topics (epic must have at least one of the selected topics)
            if (request.Topics != null && request.Topics.Count > 0)
            {
                var topicSet = request.Topics.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t!.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
                if (topicSet.Count > 0)
                {
                    q = q.Where(e => e.TopicIds.Any(t => topicSet.Contains(t)));
                }
            }

            // Filter by age groups (epic must have at least one of the selected age groups)
            if (request.AgeGroupIds != null && request.AgeGroupIds.Count > 0)
            {
                var ageGroupSet = request.AgeGroupIds.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a!.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
                if (ageGroupSet.Count > 0)
                {
                    q = q.Where(e => e.AgeGroupIds.Any(ag => ageGroupSet.Contains(ag)));
                }
            }

            var sortBy = (request.SortBy ?? "publishedAt").ToLowerInvariant();
            var sortDesc = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

            q = sortBy switch
            {
                "name" => sortDesc ? q.OrderByDescending(e => e.Name) : q.OrderBy(e => e.Name),
                "readers" => sortDesc
                    ? q.OrderByDescending(e => stats.TryGetValue(e.EpicId, out var st) ? st.ReadersCount : 0)
                    : q.OrderBy(e => stats.TryGetValue(e.EpicId, out var st) ? st.ReadersCount : 0),
                "rating" => sortDesc
                    ? q.OrderByDescending(e => stats.TryGetValue(e.EpicId, out var st) ? st.AverageRating : 0.0)
                    : q.OrderBy(e => stats.TryGetValue(e.EpicId, out var st) ? st.AverageRating : 0.0),
                _ => sortDesc ? q.OrderByDescending(e => e.PublishedAtUtc) : q.OrderBy(e => e.PublishedAtUtc)
            };

            var page = request.Page <= 0 ? 1 : request.Page;
            var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;
            var skip = (page - 1) * pageSize;
            var totalCount = q.Count();
            var pageItems = q.Skip(skip).Take(pageSize).ToList();

            var dtoList = pageItems.Select(epic =>
            {
                stats.TryGetValue(epic.EpicId, out var st);
                return new EpicMarketplaceItemDto
                {
                    Id = epic.EpicId,
                    Name = epic.Name,
                    Description = epic.Description,
                    CoverImageUrl = epic.CoverImageUrl,
                    CreatedBy = epic.CreatedBy,
                    CreatedByName = epic.CreatedByName ?? "Unknown",
                    CreatedAt = epic.CreatedAt,
                    PublishedAtUtc = epic.PublishedAtUtc,
                    StoryCount = epic.StoryCount,
                    RegionCount = epic.RegionCount,
                    AvailableLanguages = epic.AvailableLanguages,
                    AudioLanguages = epic.AudioLanguages,
                    ReadersCount = st.ReadersCount,
                    AverageRating = st.AverageRating
                };
            }).ToList();

            var hasMore = (page * pageSize) < totalCount;
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
                .Include(e => e.Translations)
                .Include(e => e.CoAuthors).ThenInclude(ca => ca.User)
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

            // Get translated name and description based on locale
            var translation = epic.Translations?.FirstOrDefault(t => 
                t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
            var name = translation?.Name ?? epic.Translations?.FirstOrDefault()?.Name ?? epic.Name;
            var description = translation?.Description ?? epic.Translations?.FirstOrDefault()?.Description ?? epic.Description;

            // Get available languages from translations
            var availableLanguages = epic.Translations?.Select(t => t.LanguageCode).OrderBy(l => l).ToList() ?? new List<string>();

            var coAuthors = (epic.CoAuthors ?? Array.Empty<StoryEpicDefinitionCoAuthor>())
                .OrderBy(ca => ca.SortOrder)
                .Select(ca => new EpicCoAuthorDto
                {
                    UserId = ca.UserId,
                    DisplayName = ca.UserId != null ? (ca.User?.Name ?? ca.User?.Email ?? ca.DisplayName ?? "") : (ca.DisplayName ?? "")
                }).ToList();

            return new EpicDetailsDto
            {
                Id = epic.Id,
                Name = name,
                Description = description,
                CoverImageUrl = epic.CoverImageUrl,
                CreatedBy = epic.OwnerUserId,
                CreatedByName = epic.Owner?.Name ?? epic.Owner?.Email ?? "Unknown",
                CreatedAt = epic.CreatedAt,
                PublishedAtUtc = epic.PublishedAtUtc,
                StoryCount = epic.StoryNodes.Count,
                RegionCount = epic.Regions.Count,
                ReadersCount = totalReaders,
                AverageRating = avgRating,
                TotalReviews = totalReviews,
                UserReview = userReview,
                AvailableLanguages = availableLanguages,
                AudioLanguages = epic.AudioLanguages ?? new List<string>(),
                CoAuthors = coAuthors
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

