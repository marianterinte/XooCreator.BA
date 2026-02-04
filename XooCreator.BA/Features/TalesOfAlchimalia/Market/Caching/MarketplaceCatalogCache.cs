using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Data.SeedData.DTOs;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Caching;

public sealed class MarketplaceCatalogCache : IMarketplaceCatalogCache, IMarketplaceCacheControl
{
    private readonly IMemoryCache _cache;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptionsMonitor<MarketplaceCacheOptions> _options;
    private readonly ILogger<MarketplaceCatalogCache> _logger;

    private readonly ConcurrentDictionary<string, byte> _knownStoryLocales = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, byte> _knownEpicLocales = new(StringComparer.OrdinalIgnoreCase);

    private const string StoryStatsKey = "marketplace:stories:stats";
    private const string EpicStatsKey = "marketplace:epics:stats";

    // Runtime override for emergency disable/enable from Admin Dashboard.
    // -1 => no override (use config MarketplaceCacheOptions.Enabled), 0 => disabled, 1 => enabled.
    private int _enabledOverrideState = -1;

    public MarketplaceCatalogCache(
        IMemoryCache cache,
        IServiceScopeFactory scopeFactory,
        IOptionsMonitor<MarketplaceCacheOptions> options,
        ILogger<MarketplaceCatalogCache> logger)
    {
        _cache = cache;
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    public void ResetAll()
    {
        foreach (var locale in _knownStoryLocales.Keys)
        {
            _cache.Remove(GetStoriesBaseKey(locale));
        }

        foreach (var locale in _knownEpicLocales.Keys)
        {
            _cache.Remove(GetEpicsBaseKey(locale));
        }

        _cache.Remove(StoryStatsKey);
        _cache.Remove(EpicStatsKey);

        _logger.LogInformation("Marketplace cache reset (all locales). KnownStoryLocales={StoryLocales} KnownEpicLocales={EpicLocales}",
            _knownStoryLocales.Count, _knownEpicLocales.Count);
    }

    public bool IsEnabled
    {
        get
        {
            var state = Volatile.Read(ref _enabledOverrideState);
            return state switch
            {
                0 => false,
                1 => true,
                _ => _options.CurrentValue.Enabled
            };
        }
    }

    public void SetEnabled(bool enabled)
    {
        Volatile.Write(ref _enabledOverrideState, enabled ? 1 : 0);

        // If disabling caching, drop existing entries to avoid any further stale reads.
        if (!enabled)
        {
            ResetAll();
        }

        _logger.LogWarning("Marketplace cache runtime toggle changed. Enabled={Enabled} (Override={Override})",
            enabled, Volatile.Read(ref _enabledOverrideState) != -1);
    }

    public MarketplaceCacheStatus GetStatus()
    {
        var cfg = _options.CurrentValue;
        return new MarketplaceCacheStatus(
            Enabled: IsEnabled,
            BaseTtlMinutes: cfg.BaseTtlMinutes,
            StatsTtlMinutes: cfg.StatsTtlMinutes,
            HasRuntimeOverride: Volatile.Read(ref _enabledOverrideState) != -1);
    }

    public Task<IReadOnlyList<StoryMarketplaceBaseItem>> GetStoriesBaseAsync(string locale, CancellationToken ct)
    {
        var normalizedLocale = NormalizeLocale(locale);
        _knownStoryLocales.TryAdd(normalizedLocale, 0);

        if (!IsEnabled)
        {
            return LoadStoriesBaseAsync(normalizedLocale, ct);
        }

        return _cache.GetOrCreateAsync(GetStoriesBaseKey(normalizedLocale), async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Math.Max(1, _options.CurrentValue.BaseTtlMinutes));
            return await LoadStoriesBaseAsync(normalizedLocale, ct);
        })!;
    }

    public Task<IReadOnlyDictionary<string, StoryMarketplaceStats>> GetStoryStatsAsync(CancellationToken ct)
    {
        if (!IsEnabled)
        {
            return LoadStoryStatsAsync(ct);
        }

        return _cache.GetOrCreateAsync(StoryStatsKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Math.Max(1, _options.CurrentValue.StatsTtlMinutes));
            return await LoadStoryStatsAsync(ct);
        })!;
    }

    public Task<IReadOnlyList<EpicMarketplaceBaseItem>> GetEpicsBaseAsync(string locale, CancellationToken ct)
    {
        var normalizedLocale = NormalizeLocale(locale);
        _knownEpicLocales.TryAdd(normalizedLocale, 0);

        if (!IsEnabled)
        {
            return LoadEpicsBaseAsync(normalizedLocale, ct);
        }

        return _cache.GetOrCreateAsync(GetEpicsBaseKey(normalizedLocale), async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Math.Max(1, _options.CurrentValue.BaseTtlMinutes));
            return await LoadEpicsBaseAsync(normalizedLocale, ct);
        })!;
    }

    public Task<IReadOnlyDictionary<string, EpicMarketplaceStats>> GetEpicStatsAsync(CancellationToken ct)
    {
        if (!IsEnabled)
        {
            return LoadEpicStatsAsync(ct);
        }

        return _cache.GetOrCreateAsync(EpicStatsKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Math.Max(1, _options.CurrentValue.StatsTtlMinutes));
            return await LoadEpicStatsAsync(ct);
        })!;
    }

    private async Task<IReadOnlyList<StoryMarketplaceBaseItem>> LoadStoriesBaseAsync(string locale, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();

        var defs = await db.StoryDefinitions
            .AsNoTracking()
            .Include(s => s.Translations)
            .Include(s => s.Topics).ThenInclude(t => t.StoryTopic)
            .Include(s => s.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .Include(s => s.ClassicAuthor)
            .Include(s => s.CoAuthors).ThenInclude(c => c.User)
            .Where(s => s.IsActive && s.Status == StoryStatus.Published && !s.IsPartOfEpic)
            .AsSplitQuery()
            .ToListAsync(ct);

        var authorIds = defs
            .Where(d => d.CreatedBy.HasValue)
            .Select(d => d.CreatedBy!.Value)
            .Distinct()
            .ToList();

        var authorMap = authorIds.Count == 0
            ? new Dictionary<Guid, string?>()
            : await db.Set<AlchimaliaUser>()
                .AsNoTracking()
                .Where(u => authorIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Name, u.Email })
                .ToDictionaryAsync(x => x.Id, x => (string?)(x.Name ?? x.Email), ct);

        var result = new List<StoryMarketplaceBaseItem>(defs.Count);
        foreach (var def in defs)
        {
            authorMap.TryGetValue(def.CreatedBy ?? Guid.Empty, out var authorName);

            var availableLanguages = def.Translations?
                .Select(t => t.LanguageCode)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(l => l)
                .ToList() ?? new List<string>();

            var translation = def.Translations?.FirstOrDefault(t => string.Equals(t.LanguageCode, locale, StringComparison.OrdinalIgnoreCase));
            var localizedTitle = translation?.Title ?? def.Title ?? def.StoryId;

            var searchTitles = new List<string>();
            if (!string.IsNullOrWhiteSpace(def.Title)) searchTitles.Add(def.Title!);
            if (def.Translations != null)
            {
                foreach (var t in def.Translations)
                {
                    if (!string.IsNullOrWhiteSpace(t.Title))
                    {
                        searchTitles.Add(t.Title!);
                    }
                }
            }

            var summary = GetSummaryFromJson(def.StoryId, locale) ?? def.Summary ?? string.Empty;

            var topicIds = def.Topics?
                .Select(t => t.StoryTopic?.TopicId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(id => id!)
                .ToList() ?? new List<string>();

            var ageGroupIds = def.AgeGroups?
                .Select(ag => ag.StoryAgeGroup?.AgeGroupId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(id => id!)
                .ToList() ?? new List<string>();

            var searchAuthors = new List<string>();
            if (!string.IsNullOrWhiteSpace(authorName)) searchAuthors.Add(authorName!);
            if (!string.IsNullOrWhiteSpace(def.AuthorName)) searchAuthors.Add(def.AuthorName!);
            if (!string.IsNullOrWhiteSpace(def.ClassicAuthor?.Name)) searchAuthors.Add(def.ClassicAuthor.Name!);
            if (def.CoAuthors != null)
            {
                foreach (var co in def.CoAuthors)
                {
                    var coName = co.UserId.HasValue && co.User != null ? co.User.Name : co.DisplayName;
                    if (!string.IsNullOrWhiteSpace(coName)) searchAuthors.Add(coName!);
                }
            }
            // Deduplicate
            searchAuthors = searchAuthors.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            result.Add(new StoryMarketplaceBaseItem
            {
                DefinitionId = def.Id,
                StoryId = def.StoryId,
                Title = localizedTitle,
                DefaultTitle = def.Title,
                CoverImageUrl = def.CoverImageUrl,
                CreatedBy = def.CreatedBy,
                CreatedByName = !string.IsNullOrWhiteSpace(def.AuthorName) ? def.AuthorName : 
                               (!string.IsNullOrWhiteSpace(def.ClassicAuthor?.Name) ? def.ClassicAuthor.Name : authorName),
                Summary = summary,
                PriceInCredits = def.PriceInCredits,
                SortOrder = def.SortOrder,
                CreatedAt = def.CreatedAt,
                StoryTopic = def.StoryTopic,
                StoryType = def.StoryType,
                Status = def.Status,
                IsEvaluative = def.IsEvaluative,
                AvailableLanguages = availableLanguages,
                TopicIds = topicIds,
                AgeGroupIds = ageGroupIds,
                SearchTitles = searchTitles,
                SearchAuthors = searchAuthors,
                AgeRating = DetermineAgeRating(def.StoryId),
                Characters = ExtractCharactersFromStoryId(def.StoryId)
            });
        }

        _logger.LogInformation("Marketplace base stories cache rebuilt. Locale={Locale} Count={Count}", locale, result.Count);
        return result;
    }

    private async Task<IReadOnlyDictionary<string, StoryMarketplaceStats>> LoadStoryStatsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();

        var readers = await db.StoryReaders
            .AsNoTracking()
            .GroupBy(sr => sr.StoryId)
            .Select(g => new { StoryId = g.Key, ReadersCount = g.Count() })
            .ToListAsync(ct);

        var reviews = await db.StoryReviews
            .AsNoTracking()
            .GroupBy(r => r.StoryId)
            .Select(g => new
            {
                StoryId = g.Key,
                AverageRating = g.Count() > 0 ? g.Average(x => (double)x.Rating) : 0.0,
                TotalReviews = g.Count()
            })
            .ToListAsync(ct);

        var map = new Dictionary<string, StoryMarketplaceStats>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in readers)
        {
            map[r.StoryId] = new StoryMarketplaceStats(r.ReadersCount, 0.0, 0);
        }

        foreach (var rev in reviews)
        {
            map.TryGetValue(rev.StoryId, out var existing);
            map[rev.StoryId] = new StoryMarketplaceStats(
                existing.ReadersCount,
                Math.Round(rev.AverageRating, 2),
                rev.TotalReviews);
        }

        _logger.LogInformation("Marketplace story stats cache rebuilt. Items={Count}", map.Count);
        return map;
    }

    private async Task<IReadOnlyList<EpicMarketplaceBaseItem>> LoadEpicsBaseAsync(string locale, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();

        var epics = await db.StoryEpicDefinitions
            .AsNoTracking()
            .Include(e => e.Owner)
            .Include(e => e.StoryNodes)
            .Include(e => e.Regions)
            .Include(e => e.Translations)
            .Include(e => e.Topics).ThenInclude(t => t.StoryTopic)
            .Include(e => e.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .Include(e => e.CoAuthors).ThenInclude(ca => ca.User)
            .Where(e => e.Status == "published" && e.PublishedAtUtc != null)
            .AsSplitQuery()
            .ToListAsync(ct);

        var result = new List<EpicMarketplaceBaseItem>(epics.Count);
        foreach (var epic in epics)
        {
            var translation = epic.Translations?.FirstOrDefault(t => string.Equals(t.LanguageCode, locale, StringComparison.OrdinalIgnoreCase));
            var name = translation?.Name ?? epic.Translations?.FirstOrDefault()?.Name ?? epic.Name ?? epic.Id;
            var description = translation?.Description ?? epic.Translations?.FirstOrDefault()?.Description ?? epic.Description;

            // Extract available languages from translations
            var availableLanguages = epic.Translations?
                .Select(t => t.LanguageCode)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(l => l)
                .ToList() ?? new List<string>();

            var searchTexts = new List<string>();
            if (!string.IsNullOrWhiteSpace(epic.Name)) searchTexts.Add(epic.Name!);
            if (!string.IsNullOrWhiteSpace(epic.Description)) searchTexts.Add(epic.Description!);
            if (epic.Translations != null)
            {
                foreach (var t in epic.Translations)
                {
                    if (!string.IsNullOrWhiteSpace(t.Name)) searchTexts.Add(t.Name!);
                    if (!string.IsNullOrWhiteSpace(t.Description)) searchTexts.Add(t.Description!);
                }
            }

            // Build search authors list from owner + co-authors (user name/email or free-text DisplayName)
            var searchAuthors = new List<string>();
            var authorName = epic.Owner?.Name ?? epic.Owner?.Email;
            if (!string.IsNullOrWhiteSpace(authorName)) searchAuthors.Add(authorName!);
            if (epic.CoAuthors != null)
            {
                foreach (var ca in epic.CoAuthors)
                {
                    var coAuthorName = ca.UserId != null ? (ca.User?.Name ?? ca.User?.Email ?? ca.DisplayName) : ca.DisplayName;
                    if (!string.IsNullOrWhiteSpace(coAuthorName)) searchAuthors.Add(coAuthorName!);
                }
            }
            searchAuthors = searchAuthors.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            var topicIds = (epic.Topics ?? Array.Empty<StoryEpicDefinitionTopic>())
                .Select(t => t.StoryTopic?.TopicId ?? t.StoryTopicId.ToString())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var ageGroupIds = (epic.AgeGroups ?? Array.Empty<StoryEpicDefinitionAgeGroup>())
                .Select(ag => ag.StoryAgeGroup?.AgeGroupId ?? ag.StoryAgeGroupId.ToString())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            result.Add(new EpicMarketplaceBaseItem
            {
                EpicId = epic.Id,
                Name = name,
                Description = description,
                CoverImageUrl = epic.CoverImageUrl,
                CreatedBy = epic.OwnerUserId,
                CreatedByName = epic.Owner?.Name ?? epic.Owner?.Email,
                CreatedAt = epic.CreatedAt,
                PublishedAtUtc = epic.PublishedAtUtc,
                StoryCount = epic.StoryNodes?.Count ?? 0,
                RegionCount = epic.Regions?.Count ?? 0,
                AvailableLanguages = availableLanguages,
                SearchTexts = searchTexts,
                SearchAuthors = searchAuthors,
                TopicIds = topicIds,
                AgeGroupIds = ageGroupIds
            });
        }

        _logger.LogInformation("Marketplace base epics cache rebuilt. Locale={Locale} Count={Count}", locale, result.Count);
        return result;
    }

    private async Task<IReadOnlyDictionary<string, EpicMarketplaceStats>> LoadEpicStatsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();

        var readers = await db.EpicReaders
            .AsNoTracking()
            .GroupBy(er => er.EpicId)
            .Select(g => new { EpicId = g.Key, ReadersCount = g.Count() })
            .ToListAsync(ct);

        var reviews = await db.EpicReviews
            .AsNoTracking()
            .Where(r => r.IsActive)
            .GroupBy(r => r.EpicId)
            .Select(g => new
            {
                EpicId = g.Key,
                AverageRating = g.Count() > 0 ? g.Average(x => (double)x.Rating) : 0.0,
                TotalReviews = g.Count()
            })
            .ToListAsync(ct);

        var map = new Dictionary<string, EpicMarketplaceStats>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in readers)
        {
            map[r.EpicId] = new EpicMarketplaceStats(r.ReadersCount, 0.0, 0);
        }

        foreach (var rev in reviews)
        {
            map.TryGetValue(rev.EpicId, out var existing);
            map[rev.EpicId] = new EpicMarketplaceStats(
                existing.ReadersCount,
                Math.Round(rev.AverageRating, 2),
                rev.TotalReviews);
        }

        _logger.LogInformation("Marketplace epic stats cache rebuilt. Items={Count}", map.Count);
        return map;
    }

    private static string GetStoriesBaseKey(string locale) => $"marketplace:stories:base:{locale}";
    private static string GetEpicsBaseKey(string locale) => $"marketplace:epics:base:v2:{locale}";

    private static string NormalizeLocale(string? locale) =>
        string.IsNullOrWhiteSpace(locale) ? "ro-ro" : locale.Trim().ToLowerInvariant();

    // --- helpers copied (kept local) to avoid pulling EF entities into cache shape ---
    private static string DetermineAgeRating(string storyId)
    {
        if (storyId.Contains("intro", StringComparison.OrdinalIgnoreCase) || storyId.Contains("loi", StringComparison.OrdinalIgnoreCase))
            return "5+";
        return "8+";
    }

    private static List<string> ExtractCharactersFromStoryId(string storyId)
    {
        var characters = new List<string>();

        if (storyId.Contains("puf", StringComparison.OrdinalIgnoreCase) || storyId.Contains("loi", StringComparison.OrdinalIgnoreCase))
        {
            characters.Add("Puf-Puf");
            characters.Add("Emperor Pufus");
        }

        if (storyId.Contains("linkaro", StringComparison.OrdinalIgnoreCase))
            characters.Add("Linkaro");

        if (storyId.Contains("grubot", StringComparison.OrdinalIgnoreCase))
            characters.Add("Grubot");

        return characters;
    }

    private static string? GetSummaryFromJson(string storyId, string locale)
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
        catch
        {
            // ignore
        }

        return null;
    }

    private sealed class StorySeedDataJsonProbe
    {
        public string? Summary { get; set; }
    }
}


