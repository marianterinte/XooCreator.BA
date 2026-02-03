using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Mappers;

public class StoryDetailsMapper
{
    private readonly XooDbContext _context;
    private readonly IStoryReviewsRepository? _reviewsRepository;
    private readonly IStoryLikesRepository? _likesRepository;

    public StoryDetailsMapper(XooDbContext context, IStoryReviewsRepository? reviewsRepository = null, IStoryLikesRepository? likesRepository = null)
    {
        _context = context;
        _reviewsRepository = reviewsRepository;
        _likesRepository = likesRepository;
    }

    public async Task<StoryDetailsDto> MapToStoryDetailsFromDefinitionAsync(
        StoryDefinition def,
        string locale,
        bool isPurchased,
        bool isOwned,
        bool isCompleted,
        int progressPercentage,
        int completedTiles,
        int totalTiles,
        string? lastReadTileId = null,
        DateTime? lastReadAt = null,
        Guid? userId = null,
        int readersCount = 0,
        int likesCount = 0,
        bool isLiked = false)
    {
        var translation = def.Translations?.FirstOrDefault(t => t.LanguageCode == locale);
        var title = translation?.Title ?? def.Title;
        
        string? authorName = null;
        if (def.CreatedBy.HasValue)
        {
            authorName = await _context.Set<AlchimaliaUser>()
                .Where(u => u.Id == def.CreatedBy.Value)
                .Select(u => u.Name)
                .FirstOrDefaultAsync();
        }

        var summary = GetSummaryFromJson(def.StoryId, locale) ?? def.Summary ?? string.Empty;

        // Get review statistics
        double averageRating = 0;
        int totalReviews = 0;
        StoryReviewDto? userReview = null;

        if (_reviewsRepository != null)
        {
            var stats = await _reviewsRepository.GetReviewStatisticsAsync(def.StoryId);
            averageRating = stats.AverageRating;
            totalReviews = stats.TotalCount;

            // Get user's review if userId is provided
            if (userId.HasValue)
            {
                userReview = await _reviewsRepository.GetUserReviewAsync(userId.Value, def.StoryId);
            }
        }

        // Map age groups
        var ageGroups = new List<DTOs.AgeGroupInfoDto>();
        if (def.AgeGroups != null && def.AgeGroups.Any())
        {
            foreach (var ageGroupRel in def.AgeGroups)
            {
                var ageGroup = ageGroupRel.StoryAgeGroup;
                if (ageGroup == null) continue;

                var translation2 = ageGroup.Translations?.FirstOrDefault(t => t.LanguageCode == locale)
                    ?? ageGroup.Translations?.FirstOrDefault();

                if (translation2 != null)
                {
                    ageGroups.Add(new DTOs.AgeGroupInfoDto
                    {
                        Id = ageGroup.AgeGroupId,
                        Label = translation2.Label,
                        MinAge = ageGroup.MinAge,
                        MaxAge = ageGroup.MaxAge
                    });
                }
            }
        }

        // Map co-authors
        var coAuthors = (def.CoAuthors ?? Enumerable.Empty<StoryDefinitionCoAuthor>())
            .OrderBy(c => c.SortOrder)
            .Select(c => new DTOs.StoryCoAuthorDto
            {
                UserId = c.UserId,
                DisplayName = c.UserId.HasValue && c.User != null ? c.User.Name : (c.DisplayName ?? string.Empty)
            })
            .ToList();

        return new StoryDetailsDto
        {
            Id = def.StoryId,
            Title = title,
            CoverImageUrl = def.CoverImageUrl,
            CreatedBy = def.CreatedBy,
            CreatedByName = authorName,
            Summary = summary,
            PriceInCredits = def.PriceInCredits,
            Region = ExtractRegionFromStoryId(def.StoryId),
            Characters = ExtractCharactersFromStoryId(def.StoryId),
            Tags = new List<string>(),
            IsNew = false,
            IsPurchased = isPurchased,
            IsOwned = isOwned,
            IsCompleted = isCompleted,
            ProgressPercentage = progressPercentage,
            CompletedTiles = completedTiles,
            TotalTiles = totalTiles,
            LastReadTileId = lastReadTileId,
            LastReadAt = lastReadAt,
            CreatedAt = def.CreatedAt,
            UnlockedStoryHeroes = new List<string>(),
            StoryTopic = def.StoryTopic,
            StoryType = def.StoryType.ToString(),
            Status = def.Status.ToString(),
            SortOrder = def.SortOrder,
            IsActive = def.IsActive,
            UpdatedAt = def.UpdatedAt,
            UpdatedBy = def.UpdatedBy,
            AvailableLanguages = def.Translations?.Select(t => t.LanguageCode).OrderBy(l => l).ToList() ?? new List<string>(),
            AgeGroups = ageGroups,
            AverageRating = averageRating,
            TotalReviews = totalReviews,
            UserReview = userReview,
            ReadersCount = readersCount,
            LikesCount = likesCount,
            IsLiked = isLiked,
            IsEvaluative = def.IsEvaluative,
            IsPartOfEpic = def.IsPartOfEpic,
            CoAuthors = coAuthors
        };
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
}

