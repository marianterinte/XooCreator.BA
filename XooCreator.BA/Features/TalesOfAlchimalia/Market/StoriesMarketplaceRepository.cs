using System.Diagnostics;
using System.Text.Json;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Caching;
using XooCreator.BA.Data.SeedData.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Mappers;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public record StoryProgressInfo(string StoryId, int ProgressCount);
public record StoryReadersAggregate(string StoryId, int ReadersCount, DateTime LastAcquiredAt);
public record StoryReadersTrendPoint(DateOnly Date, int ReadersCount);
public record StoryReadersCorrelationItem(string StoryId, string Title, int ReadersCount, int ReviewsCount, double AverageRating);

public interface IStoriesMarketplaceRepository
{
    Task<(List<StoryMarketplaceItemDto> Stories, int TotalCount, bool HasMore)> GetMarketplaceStoriesWithPaginationAsync(Guid userId, string locale, SearchStoriesRequest request);
    Task<List<StoryMarketplaceItemDto>> GetMarketplaceStoriesAsync(Guid userId, string locale, SearchStoriesRequest request);
    Task<List<StoryMarketplaceItemDto>> GetFeaturedStoriesAsync(Guid userId, string locale);
    Task<List<string>> GetAvailableRegionsAsync();
    Task<List<string>> GetAvailableAgeRatingsAsync();
    Task<List<string>> GetAvailableCharactersAsync();
    Task<bool> PurchaseStoryAsync(Guid userId, string storyId, double creditsSpent);
    Task<bool> IsStoryPurchasedAsync(Guid userId, string storyId);
    Task<List<StoryMarketplaceItemDto>> GetUserPurchasedStoriesAsync(Guid userId, string locale);
    Task<StoryDetailsDto?> GetStoryDetailsAsync(string storyId, Guid userId, string locale);
    Task<double> GetComputedPriceAsync(string storyId);
    Task EnsureStoryReaderAsync(Guid userId, string storyId, StoryAcquisitionSource source);
    Task<int> GetStoryReadersCountAsync(string storyId);
    Task<List<StoryReadersAggregate>> GetTopStoriesByReadersAsync(int limit);
    Task<List<StoryReadersTrendPoint>> GetReadersTrendAsync(int days);
    Task<List<StoryReadersCorrelationItem>> GetReadersVsReviewsAsync(int limit);
    Task<int> GetTotalReadersAsync();
}

public class StoriesMarketplaceRepository : IStoriesMarketplaceRepository
{
    private readonly XooDbContext _context;
    private readonly StoryDetailsMapper _storyDetailsMapper;
    private readonly EpicsMarketplaceRepository? _epicsRepository;
    private readonly IMarketplaceCatalogCache _catalogCache;
    private readonly ILogger<StoriesMarketplaceRepository>? _logger;
    private readonly TelemetryClient? _telemetryClient;

    public StoriesMarketplaceRepository(
        XooDbContext context, 
        StoryDetailsMapper storyDetailsMapper,
        IMarketplaceCatalogCache catalogCache,
        EpicsMarketplaceRepository? epicsRepository = null,
        ILogger<StoriesMarketplaceRepository>? logger = null,
        TelemetryClient? telemetryClient = null)
    {
        _context = context;
        _storyDetailsMapper = storyDetailsMapper;
        _catalogCache = catalogCache;
        _epicsRepository = epicsRepository;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    public async Task<(List<StoryMarketplaceItemDto> Stories, int TotalCount, bool HasMore)> GetMarketplaceStoriesWithPaginationAsync(Guid userId, string locale, SearchStoriesRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var normalizedLocale = (locale ?? "ro-ro").ToLowerInvariant();
        
        try
        {
            // Base data + stats are cached (global, per-locale for text fields).
            var baseItems = await _catalogCache.GetStoriesBaseAsync(normalizedLocale, CancellationToken.None);
            var stats = await _catalogCache.GetStoryStatsAsync(CancellationToken.None);

            // User preferences for auto age-filter (kept exact behavior).
            var user = await _context.AlchimaliaUsers
                .Where(u => u.Id == userId)
                .Select(u => new { u.AutoFilterStoriesByAge, u.SelectedAgeGroupIds })
                .FirstOrDefaultAsync();

            IEnumerable<StoryMarketplaceBaseItem> q = baseItems;

            // Implicit filters: Published already enforced by cache source; apply Indie default if no categories.
            if (!(request.Categories?.Any() ?? false))
            {
                q = q.Where(s => s.StoryType == StoryType.Indie);
            }

            // Filter by topics
            if (request.Topics?.Any() == true)
            {
                var topics = new HashSet<string>(request.Topics, StringComparer.OrdinalIgnoreCase);
                q = q.Where(s => s.TopicIds.Any(t => topics.Contains(t)));
            }

            // Filter by age groups (explicit)
            if (request.AgeGroupIds?.Any() == true)
            {
                var ageGroupIds = new HashSet<string>(request.AgeGroupIds, StringComparer.OrdinalIgnoreCase);
                q = q.Where(s => s.AgeGroupIds.Any(id => ageGroupIds.Contains(id)));
            }

            // Filter by IsEvaluative
            if (request.IsEvaluative.HasValue)
            {
                q = q.Where(s => s.IsEvaluative == request.IsEvaluative.Value);
            }

            // Auto-filter by age groups if enabled in parent dashboard
            if (user != null && user.AutoFilterStoriesByAge && user.SelectedAgeGroupIds != null && user.SelectedAgeGroupIds.Count > 0)
            {
                var selected = new HashSet<string>(user.SelectedAgeGroupIds, StringComparer.OrdinalIgnoreCase);
                q = q.Where(s => s.AgeGroupIds.Any(id => selected.Contains(id)));
            }

            // Search by title/translation titles (case-insensitive)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim();
                q = q.Where(s => s.SearchTitles.Any(t =>
                    !string.IsNullOrWhiteSpace(t) &&
                    t.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            // Sorting (keep same keys)
            var sortBy = (request.SortBy ?? "sortOrder").ToLowerInvariant();
            var sortDesc = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

            q = sortBy switch
            {
                "title" => sortDesc ? q.OrderByDescending(s => s.Title) : q.OrderBy(s => s.Title),
                "date" => sortDesc ? q.OrderByDescending(s => s.CreatedAt) : q.OrderBy(s => s.CreatedAt),
                "price" => sortDesc ? q.OrderByDescending(s => s.PriceInCredits) : q.OrderBy(s => s.PriceInCredits),
                "readers" => sortDesc
                    ? q.OrderByDescending(s => stats.TryGetValue(s.StoryId, out var st) ? st.ReadersCount : 0)
                    : q.OrderBy(s => stats.TryGetValue(s.StoryId, out var st) ? st.ReadersCount : 0),
                _ => sortDesc ? q.OrderByDescending(s => s.SortOrder) : q.OrderBy(s => s.SortOrder)
            };

            var filtered = q.ToList();
            var totalCount = filtered.Count;

            var page = request.Page <= 0 ? 1 : request.Page;
            var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;
            var pageItems = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Per-user overlay (small, batched): purchased + owned for only the current page.
            var pageStoryIds = pageItems.Select(p => p.StoryId).ToList();
            var purchasedIds = await _context.StoryPurchases
                .AsNoTracking()
                .Where(sp => sp.UserId == userId && pageStoryIds.Contains(sp.StoryId))
                .Select(sp => sp.StoryId)
                .ToListAsync();
            var purchasedSet = purchasedIds.ToHashSet(StringComparer.OrdinalIgnoreCase);

            var defIds = pageItems.Select(p => p.DefinitionId).ToList();
            var ownedDefIds = await _context.UserOwnedStories
                .AsNoTracking()
                .Where(uos => uos.UserId == userId && defIds.Contains(uos.StoryDefinitionId))
                .Select(uos => uos.StoryDefinitionId)
                .ToListAsync();
            var ownedSet = ownedDefIds.ToHashSet();

            var dtoList = pageItems.Select(p =>
            {
                stats.TryGetValue(p.StoryId, out var st);
                var isPurchased = purchasedSet.Contains(p.StoryId);
                var isOwned = isPurchased || ownedSet.Contains(p.DefinitionId);

                return new StoryMarketplaceItemDto
                {
                    Id = p.StoryId,
                    Title = p.Title,
                    CoverImageUrl = p.CoverImageUrl,
                    CreatedBy = p.CreatedBy,
                    CreatedByName = p.CreatedByName,
                    Summary = p.Summary,
                    PriceInCredits = p.PriceInCredits,
                    AgeRating = p.AgeRating,
                    Characters = p.Characters,
                    Tags = p.TopicIds,
                    CreatedAt = p.CreatedAt,
                    StoryTopic = p.StoryTopic,
                    StoryType = p.StoryType.ToString(),
                    Status = p.Status.ToString(),
                    AvailableLanguages = p.AvailableLanguages,
                    IsPurchased = isPurchased,
                    IsOwned = isOwned,
                    ReadersCount = st.ReadersCount,
                    AverageRating = st.AverageRating,
                    TotalReviews = st.TotalReviews,
                    IsEvaluative = p.IsEvaluative
                };
            }).ToList();

            var hasMore = (page * pageSize) < totalCount;
            return (dtoList, totalCount, hasMore);
        }
        finally
        {
            stopwatch.Stop();
            var durationMs = stopwatch.ElapsedMilliseconds;
            
            // Log to ILogger
            _logger?.LogInformation(
                "GetMarketplaceStoriesWithPaginationAsync completed | Duration={DurationMs}ms | Page={Page} | PageSize={PageSize} | Locale={Locale} | UserId={UserId}",
                durationMs, request.Page, request.PageSize, normalizedLocale, userId);

            // Track as dependency in Application Insights
            if (_telemetryClient != null)
            {
                var dependencyTelemetry = new DependencyTelemetry
                {
                    Type = "Database",
                    Name = "GetMarketplaceStoriesWithPagination",
                    Data = $"Page={request.Page}, PageSize={request.PageSize}, Locale={normalizedLocale}",
                    Duration = TimeSpan.FromMilliseconds(durationMs),
                    Success = true,
                    Target = "PostgreSQL"
                };

                dependencyTelemetry.Properties["Page"] = request.Page.ToString();
                dependencyTelemetry.Properties["PageSize"] = request.PageSize.ToString();
                dependencyTelemetry.Properties["Locale"] = normalizedLocale;
                dependencyTelemetry.Properties["SortBy"] = request.SortBy ?? "sortOrder";
                dependencyTelemetry.Properties["SortOrder"] = request.SortOrder ?? "asc";
                dependencyTelemetry.Properties["UserId"] = userId.ToString();

                _telemetryClient.TrackDependency(dependencyTelemetry);
                
                // Also track as custom metric for easier querying
                _telemetryClient.TrackMetric("MarketplaceStoriesQueryDuration", durationMs, new Dictionary<string, string>
                {
                    ["Operation"] = "GetMarketplaceStoriesWithPagination",
                    ["Locale"] = normalizedLocale,
                    ["Page"] = request.Page.ToString(),
                    ["PageSize"] = request.PageSize.ToString()
                });
            }
        }
    }

    public async Task<List<StoryMarketplaceItemDto>> GetMarketplaceStoriesAsync(Guid userId, string locale, SearchStoriesRequest request)
    {
        var (stories, _, _) = await GetMarketplaceStoriesWithPaginationAsync(userId, locale, request);
        return stories;
    }

    private IQueryable<StoryDefinition> ApplySorting(IQueryable<StoryDefinition> query, SearchStoriesRequest request)
    {
        var sortBy = (request.SortBy ?? "sortOrder").ToLowerInvariant();
        var sortOrderDesc = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy switch
        {
            "title" => sortOrderDesc ? query.OrderByDescending(s => s.Title) : query.OrderBy(s => s.Title),
            "date" => sortOrderDesc ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
            "price" => sortOrderDesc ? query.OrderByDescending(s => s.PriceInCredits) : query.OrderBy(s => s.PriceInCredits),
            _ => sortOrderDesc ? query.OrderByDescending(s => s.SortOrder) : query.OrderBy(s => s.SortOrder)
        };
    }

    public async Task<List<StoryMarketplaceItemDto>> GetFeaturedStoriesAsync(Guid userId, string locale)
    {
        var normalizedLocale = (locale ?? "ro-ro").ToLowerInvariant();
        var baseItems = await _catalogCache.GetStoriesBaseAsync(normalizedLocale, CancellationToken.None);
        var stats = await _catalogCache.GetStoryStatsAsync(CancellationToken.None);

        var user = await _context.AlchimaliaUsers
            .Where(u => u.Id == userId)
            .Select(u => new { u.AutoFilterStoriesByAge, u.SelectedAgeGroupIds })
            .FirstOrDefaultAsync();

        IEnumerable<StoryMarketplaceBaseItem> q = baseItems;
        if (user != null && user.AutoFilterStoriesByAge && user.SelectedAgeGroupIds != null && user.SelectedAgeGroupIds.Count > 0)
        {
            var selected = new HashSet<string>(user.SelectedAgeGroupIds, StringComparer.OrdinalIgnoreCase);
            q = q.Where(s => s.AgeGroupIds.Any(id => selected.Contains(id)));
        }

        var featured = q
            .OrderBy(s => s.SortOrder)
            .Take(5)
            .ToList();

        var storyIds = featured.Select(s => s.StoryId).ToList();
        var purchased = await _context.StoryPurchases
            .AsNoTracking()
            .Where(sp => sp.UserId == userId && storyIds.Contains(sp.StoryId))
            .Select(sp => sp.StoryId)
            .ToListAsync();
        var purchasedSet = purchased.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var defIds = featured.Select(s => s.DefinitionId).ToList();
        var ownedDefIds = await _context.UserOwnedStories
            .AsNoTracking()
            .Where(uos => uos.UserId == userId && defIds.Contains(uos.StoryDefinitionId))
            .Select(uos => uos.StoryDefinitionId)
            .ToListAsync();
        var ownedSet = ownedDefIds.ToHashSet();

        return featured.Select(p =>
        {
            stats.TryGetValue(p.StoryId, out var st);
            var isPurchased = purchasedSet.Contains(p.StoryId);
            var isOwned = isPurchased || ownedSet.Contains(p.DefinitionId);

            return new StoryMarketplaceItemDto
            {
                Id = p.StoryId,
                Title = p.Title,
                CoverImageUrl = p.CoverImageUrl,
                CreatedBy = p.CreatedBy,
                CreatedByName = p.CreatedByName,
                Summary = p.Summary,
                PriceInCredits = p.PriceInCredits,
                AgeRating = p.AgeRating,
                Characters = p.Characters,
                Tags = p.TopicIds,
                CreatedAt = p.CreatedAt,
                StoryTopic = p.StoryTopic,
                StoryType = p.StoryType.ToString(),
                Status = p.Status.ToString(),
                AvailableLanguages = p.AvailableLanguages,
                IsPurchased = isPurchased,
                IsOwned = isOwned,
                ReadersCount = st.ReadersCount,
                AverageRating = st.AverageRating,
                TotalReviews = st.TotalReviews,
                IsEvaluative = p.IsEvaluative
            };
        }).ToList();
    }

    public async Task<List<string>> GetAvailableRegionsAsync()
    {
        var ids = await _context.StoryDefinitions
            .Where(s => s.IsActive && s.Status == StoryStatus.Published)
            .Select(s => s.StoryId)
            .ToListAsync();
        return ids.Select(ExtractRegionFromStoryId)
            .Distinct()
            .OrderBy(r => r)
            .ToList();
    }

    public async Task<List<string>> GetAvailableAgeRatingsAsync()
    {
        var ids = await _context.StoryDefinitions
            .Where(s => s.IsActive && s.Status == StoryStatus.Published)
            .Select(s => s.StoryId)
            .ToListAsync();
        return ids.Select(DetermineAgeRating)
            .Distinct()
            .OrderBy(r => r)
            .ToList();
    }

    public async Task<List<string>> GetAvailableCharactersAsync()
    {
        var ids = await _context.StoryDefinitions
            .Where(s => s.IsActive && s.Status == StoryStatus.Published)
            .Select(s => s.StoryId)
            .ToListAsync();
        return ids.SelectMany(ExtractCharactersFromStoryId)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }

    public async Task<bool> PurchaseStoryAsync(Guid userId, string storyId, double creditsSpent)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Check if already purchased
            var existingPurchase = await _context.StoryPurchases
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.StoryId == storyId);

            if (existingPurchase != null)
            {
                return false; // Already purchased
            }

            // Deduct credits from user's wallet
            var wallet = await _context.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null || wallet.DiscoveryBalance < creditsSpent)
            {
                return false; // Insufficient credits
            }

            wallet.DiscoveryBalance -= creditsSpent;
            wallet.UpdatedAt = DateTime.UtcNow;

            // Create purchase record
            var purchase = new StoryPurchase
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StoryId = storyId,
                CreditsSpent = creditsSpent,
                PurchasedAt = DateTime.UtcNow
            };
            _context.StoryPurchases.Add(purchase);

            // Create transaction record
            var creditTransaction = new CreditTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = -creditsSpent,
                Type = CreditTransactionType.Spend,
                Reference = $"Story Purchase - {storyId}",
                CreatedAt = DateTime.UtcNow
            };
            _context.CreditTransactions.Add(creditTransaction);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> IsStoryPurchasedAsync(Guid userId, string storyId)
    {
        return await _context.StoryPurchases
            .AnyAsync(sp => sp.UserId == userId && sp.StoryId == storyId);
    }

    public async Task<List<StoryMarketplaceItemDto>> GetUserPurchasedStoriesAsync(Guid userId, string locale)
    {
        var purchasedStories = await _context.StoryPurchases
            .Include(sp => sp.Story)
                .ThenInclude(s => s.Translations)
            .Where(sp => sp.UserId == userId && sp.Story.IsActive)
            .OrderBy(sp => sp.PurchasedAt)
            .ToListAsync();

        var storyProgress = await _context.UserStoryReadProgress
            .Where(usp => usp.UserId == userId)
            .GroupBy(usp => usp.StoryId)
            .Select(g => new StoryProgressInfo(g.Key, g.Count()))
            .ToListAsync();

        var ids = purchasedStories.Select(sp => sp.StoryId).ToList();
        var defs = await _context.StoryDefinitions
            .Include(s => s.Translations)
            .Include(s => s.Topics)
                .ThenInclude(t => t.StoryTopic)
            .Where(s => ids.Contains(s.StoryId))
            .ToListAsync();
        var normalizedLocale = (locale ?? "ro-ro").ToLowerInvariant();
        return await MapToMarketplaceListAsync(defs, normalizedLocale, userId);
    }

    public async Task<StoryDetailsDto?> GetStoryDetailsAsync(string storyId, Guid userId, string locale)
    {
        var def = await _context.StoryDefinitions
            .Include(s => s.Translations)
            .Include(s => s.Tiles)
            .Include(s => s.AgeGroups)
                .ThenInclude(ag => ag.StoryAgeGroup)
                    .ThenInclude(ag => ag.Translations)
            .FirstOrDefaultAsync(s => s.StoryId == storyId && s.IsActive && s.Status == StoryStatus.Published);

        if (def == null)
            return null;

        // Check if user has purchased this story
        var isPurchased = await _context.StoryPurchases
            .AnyAsync(sp => sp.UserId == userId && sp.StoryId == storyId);

        // Check if user owns this story (UserOwnedStories)
        var ownedRow = await _context.UserOwnedStories
            .AnyAsync(uos => uos.UserId == userId && uos.StoryDefinitionId == def.Id);
        var isOwned = isPurchased || ownedRow;

        // Get user's story progress
        // Use case-insensitive comparison to ensure we get the correct progress
        var allProgress = await _context.UserStoryReadProgress
            .Where(usp => usp.UserId == userId)
            .OrderBy(usp => usp.ReadAt)
            .ToListAsync();
        
        var progressEntries = allProgress
            .Where(usp => string.Equals(usp.StoryId, storyId, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var completedTiles = progressEntries.Count;
        var totalTiles = def.Tiles.Count;
        var progressPercentage = totalTiles > 0
            ? (int)global::System.Math.Round((double)completedTiles / global::System.Math.Max(1, totalTiles) * 100)
            : 0;
        var lastRead = progressEntries.LastOrDefault();
        var isCompleted = totalTiles > 0 && completedTiles >= totalTiles;
        // Progress entries are kept for historical tracking and parent dashboard
        // Parent dashboard lists all stories with progress (started or completed) based on UserStoryReadProgress entries

        // Normalize locale to lowercase for mapping
        var normalizedLocale = (locale ?? "ro-ro").ToLowerInvariant();
        var readersCount = await GetStoryReadersCountAsync(storyId);
        return await _storyDetailsMapper.MapToStoryDetailsFromDefinitionAsync(
            def,
            normalizedLocale,
            isPurchased,
            isOwned,
            isCompleted,
            progressPercentage,
            completedTiles,
            totalTiles,
            lastRead?.TileId,
            lastRead?.ReadAt,
            userId,
            readersCount);
    }

    public async Task EnsureStoryReaderAsync(Guid userId, string storyId, StoryAcquisitionSource source)
    {
        var exists = await _context.StoryReaders
            .AsNoTracking()
            .AnyAsync(sr => sr.UserId == userId && sr.StoryId == storyId);

        if (exists)
        {
            return;
        }

        var reader = new StoryReader
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            StoryId = storyId,
            AcquiredAt = DateTime.UtcNow,
            AcquisitionSource = source
        };

        _context.StoryReaders.Add(reader);
        await _context.SaveChangesAsync();
    }

    public Task<int> GetStoryReadersCountAsync(string storyId)
    {
        return _context.StoryReaders
            .Where(sr => sr.StoryId == storyId)
            .CountAsync();
    }

    public async Task<List<StoryReadersAggregate>> GetTopStoriesByReadersAsync(int limit)
    {
        limit = limit <= 0 ? 10 : limit;

        var raw = await _context.StoryReaders
            .GroupBy(sr => sr.StoryId)
            .Select(g => new
            {
                StoryId = g.Key,
                ReadersCount = g.Count(),
                LastAcquiredAt = g.Max(x => (DateTime?)x.AcquiredAt) ?? DateTime.MinValue
            })
            .OrderByDescending(x => x.ReadersCount)
            .ThenByDescending(x => x.LastAcquiredAt)
            .Take(limit)
            .ToListAsync();

        return raw
            .Select(x => new StoryReadersAggregate(x.StoryId, x.ReadersCount, x.LastAcquiredAt))
            .ToList();
    }

    public async Task<List<StoryReadersTrendPoint>> GetReadersTrendAsync(int days)
    {
        days = days <= 0 ? 7 : days;
        var today = DateTime.UtcNow.Date;
        var startDate = today.AddDays(-(days - 1));

        var rawData = await _context.StoryReaders
            .Where(sr => sr.AcquiredAt >= startDate)
            .GroupBy(sr => sr.AcquiredAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var map = rawData.ToDictionary(
            item => DateOnly.FromDateTime(item.Date),
            item => item.Count);

        var trend = new List<StoryReadersTrendPoint>(days);
        for (var cursor = startDate; cursor <= today; cursor = cursor.AddDays(1))
        {
            var dateOnly = DateOnly.FromDateTime(cursor);
            map.TryGetValue(dateOnly, out var count);
            trend.Add(new StoryReadersTrendPoint(dateOnly, count));
        }

        return trend;
    }

    public async Task<List<StoryReadersCorrelationItem>> GetReadersVsReviewsAsync(int limit)
    {
        limit = limit <= 0 ? 10 : limit;

        // Materialize readers aggregation
        var readersData = await _context.StoryReaders
            .GroupBy(sr => sr.StoryId)
            .Select(g => new
            {
                StoryId = g.Key,
                ReadersCount = g.Count(),
                LastAcquiredAt = g.Max(x => (DateTime?)x.AcquiredAt) ?? DateTime.MinValue
            })
            .OrderByDescending(x => x.ReadersCount)
            .ThenByDescending(x => x.LastAcquiredAt)
            .Take(limit)
            .ToListAsync();

        if (readersData.Count == 0)
        {
            return new List<StoryReadersCorrelationItem>();
        }

        var storyIds = readersData.Select(r => r.StoryId).ToList();

        // Materialize reviews aggregation
        var reviewsData = await _context.StoryReviews
            .Where(r => storyIds.Contains(r.StoryId))
            .GroupBy(r => r.StoryId)
            .Select(g => new
            {
                StoryId = g.Key,
                ReviewsCount = g.Count(),
                AverageRating = g.Count() > 0 ? (double?)g.Average(r => (double)r.Rating) : null
            })
            .ToListAsync();

        // Get story titles
        var storyTitles = await _context.StoryDefinitions
            .Where(sd => storyIds.Contains(sd.StoryId))
            .Select(sd => new { sd.StoryId, sd.Title })
            .ToListAsync();

        // Build dictionaries for lookups
        var titleMap = storyTitles.ToDictionary(s => s.StoryId, s => s.Title ?? s.StoryId);
        var reviewMap = reviewsData.ToDictionary(r => r.StoryId, r => new { r.ReviewsCount, r.AverageRating });

        // Build result in memory
        return readersData
            .Select(reader =>
            {
                titleMap.TryGetValue(reader.StoryId, out var title);
                reviewMap.TryGetValue(reader.StoryId, out var review);

                return new StoryReadersCorrelationItem(
                    reader.StoryId,
                    title ?? reader.StoryId,
                    reader.ReadersCount,
                    review != null ? review.ReviewsCount : 0,
                    review != null && review.AverageRating.HasValue ? Math.Round(review.AverageRating.Value, 2) : 0.0);
            })
            .ToList();
    }

    public Task<int> GetTotalReadersAsync()
    {
        return _context.StoryReaders.CountAsync();
    }

    private string? GetSummaryFromJson(string storyId, string locale)
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var candidates = new[]
            {
                Path.Combine(baseDir, "Data", "SeedData", "Stories", "seed@alchimalia.com", "independent", "i18n", locale, $"{storyId}.json"),
                Path.Combine(baseDir, "Data", "SeedData", "Stories", "seed@alchimalia.com", "i18n", locale, $"{storyId}.json")
            };

            foreach (var filePath in candidates)
            {
                if (!File.Exists(filePath)) continue;
                
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<StorySeedDataJsonProbe>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });
                
                if (!string.IsNullOrWhiteSpace(data?.Summary))
                {
                    return data.Summary;
                }
            }
        }
        catch { }
        
        return null;
    }

    private sealed class StorySeedDataJsonProbe
    {
        public string? Summary { get; set; }
    }

    // Removed old MapToMarketplaceItemDto using StoryMarketplaceInfo

    private string ExtractRegionFromStoryId(string storyId)
    {
        var regionMap = new Dictionary<string, string>
        {
            { "lunaria", "Lunaria" },
            { "terra", "Terra" },
            { "aetherion", "Aetherion" },
            { "auroria", "Auroria" },
            { "crystalia", "Crystalia" },
            { "pyron", "Pyron" },
            { "zephyra", "Zephyra" },
            { "verdantia", "Verdantia" },
            { "sylvaria", "Sylvaria" },
            { "nocturna", "Nocturna" },
            { "neptunia", "Neptunia" },
            { "oceanica", "Oceanica" },
            { "mechanika", "Mechanika" }
        };

        foreach (var kvp in regionMap)
        {
            if (storyId.Contains(kvp.Key))
                return kvp.Value;
        }

        return "Unknown";
    }

    private string DetermineAgeRating(string storyId)
    {
        if (storyId.Contains("intro") || storyId.Contains("loi"))
            return "5+";
        return "8+";
    }

    private string DetermineDifficulty(string storyId)
    {
        if (storyId.Contains("intro") || storyId.Contains("loi"))
            return "beginner";
        return "intermediate";
    }

    private List<string> ExtractCharactersFromStoryId(string storyId)
    {
        var characters = new List<string>();
        
        if (storyId.Contains("puf") || storyId.Contains("loi"))
        {
            characters.Add("Puf-Puf");
            characters.Add("Emperor Pufus");
        }
        
        if (storyId.Contains("linkaro"))
            characters.Add("Linkaro");
            
        if (storyId.Contains("grubot"))
            characters.Add("Grubot");

        return characters;
    }

    // New helpers for mapping from StoryDefinition directly
    private async Task<List<StoryMarketplaceItemDto>> MapToMarketplaceListAsync(List<StoryDefinition> defs, string locale, Guid userId)
    {
        var result = new List<StoryMarketplaceItemDto>();
        var storyIds = defs.Select(d => d.StoryId).ToList();
        var readersCounts = await _context.StoryReaders
            .Where(sr => storyIds.Contains(sr.StoryId))
            .GroupBy(sr => sr.StoryId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

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

        foreach (var def in defs)
        {
            var readersCount = readersCounts.TryGetValue(def.StoryId, out var count) ? count : 0;
            var stats = reviewStats.TryGetValue(def.StoryId, out var value)
                ? value
                : new { AverageRating = 0d, TotalReviews = 0 };
            result.Add(await MapToMarketplaceItemFromDefinitionAsync(
                def,
                locale,
                userId,
                readersCount,
                stats.AverageRating,
                stats.TotalReviews));
        }
        return result;
    }

    private async Task<StoryMarketplaceItemDto> MapToMarketplaceItemFromDefinitionAsync(
        StoryDefinition def,
        string locale,
        Guid userId,
        int readersCount,
        double averageRating,
        int totalReviews)
    {
        var translation = def.Translations?.FirstOrDefault(t => t.LanguageCode == locale);
        var title = translation?.Title ?? def.Title;

        // Extract available languages from translations
        var availableLanguages = def.Translations?
            .Select(t => t.LanguageCode)
            .OrderBy(l => l)
            .ToList() ?? new List<string>();

        // Get author name from database
        string? authorName = null;
        if (def.CreatedBy.HasValue)
        {
            authorName = await _context.Set<AlchimaliaUser>()
                .Where(u => u.Id == def.CreatedBy.Value)
                .Select(u => u.Name)
                .FirstOrDefaultAsync();
        }

        // Get summary from JSON file for the current locale, or use StoryDefinition.Summary, or empty string
        var summary = GetSummaryFromJson(def.StoryId, locale) 
            ?? def.Summary 
            ?? string.Empty;

        // Check if user has purchased this story
        var isPurchased = await _context.StoryPurchases
            .AnyAsync(sp => sp.UserId == userId && sp.StoryId == def.StoryId);

        // Check if user owns this story (UserOwnedStories)
        var ownedRow = await _context.UserOwnedStories
            .AnyAsync(uos => uos.UserId == userId && uos.StoryDefinitionId == def.Id);
        var isOwned = isPurchased || ownedRow;

        // Extract topic IDs from Topics collection
        var topicIds = def.Topics?
            .Select(t => t.StoryTopic?.TopicId)
            .Where(topicId => !string.IsNullOrEmpty(topicId))
            .ToList() ?? new List<string?>();
        
        return new StoryMarketplaceItemDto
        {
            Id = def.StoryId,
            Title = title,
            CoverImageUrl = def.CoverImageUrl,
            CreatedBy = def.CreatedBy,
            CreatedByName = authorName,
            Summary = summary,
            PriceInCredits = def.PriceInCredits,
            AgeRating = DetermineAgeRating(def.StoryId),
            Characters = ExtractCharactersFromStoryId(def.StoryId),
            Tags = topicIds.Where(t => t != null).Select(t => t!).ToList(),
            CreatedAt = def.CreatedAt,
            StoryTopic = def.StoryTopic,
            StoryType = def.StoryType.ToString(),
            Status = def.Status.ToString(),
            AvailableLanguages = availableLanguages,
            IsPurchased = isPurchased,
            IsOwned = isOwned,
            ReadersCount = readersCount,
            AverageRating = Math.Round(averageRating, 2),
            TotalReviews = totalReviews,
            IsEvaluative = def.IsEvaluative
        };
    }


    public async Task<double> GetComputedPriceAsync(string storyId)
    {
        var def = await _context.StoryDefinitions
            .FirstOrDefaultAsync(s => s.StoryId == storyId);
        return def?.PriceInCredits ?? 0;
    }

}
