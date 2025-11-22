using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryTopicsSeedService
{
    Task SeedTopicsAndAgeGroupsAsync(CancellationToken ct = default);
    Task SeedClassicAuthorsAsync(CancellationToken ct = default);
}

public class StoryTopicsSeedService : IStoryTopicsSeedService
{
    private readonly XooDbContext _context;
    private readonly ILogger<StoryTopicsSeedService> _logger;
    private readonly string _basePath;

    public StoryTopicsSeedService(XooDbContext context, ILogger<StoryTopicsSeedService> logger)
    {
        _context = context;
        _logger = logger;
        _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "Story-Editor", "Topic", "i18n");
    }

    public async Task SeedTopicsAndAgeGroupsAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("🌱 Starting to seed story topics and age groups...");

        try
        {
            await SeedTopicsAsync(ct);
            await SeedAgeGroupsAsync(ct);
            await SeedClassicAuthorsAsync(ct);
            _logger.LogInformation("✅ Story topics and age groups seeded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to seed story topics and age groups");
            throw;
        }
    }

    public async Task SeedClassicAuthorsAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("🌱 Starting to seed classic authors...");

        try
        {
            var authorsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "Story-Editor", "authors.json");
            
            if (!File.Exists(authorsPath))
            {
                _logger.LogWarning("Authors file not found: {Path}", authorsPath);
                return;
            }

            var json = await File.ReadAllTextAsync(authorsPath, ct);
            var authorsData = JsonSerializer.Deserialize<AuthorsSeedData>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (authorsData == null)
            {
                _logger.LogWarning("No authors data found in {Path}", authorsPath);
                return;
            }

            // Seed Romanian authors
            await SeedAuthorsForLanguageAsync("ro-ro", authorsData.RomanianAuthors ?? new List<string>(), ct);
            
            // Seed Hungarian authors
            await SeedAuthorsForLanguageAsync("hu-hu", authorsData.HungarianAuthors ?? new List<string>(), ct);
            
            // Seed English/American authors
            await SeedAuthorsForLanguageAsync("en-us", authorsData.EnglishAmericanAuthors ?? new List<string>(), ct);

            _logger.LogInformation("✅ Classic authors seeded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to seed classic authors");
            throw;
        }
    }

    private async Task SeedAuthorsForLanguageAsync(string languageCode, List<string> authorNames, CancellationToken ct)
    {
        int sortOrder = 0;
        foreach (var authorName in authorNames)
        {
            // Generate AuthorId from name (lowercase, replace spaces and special chars with hyphens)
            var authorId = GenerateAuthorId(authorName);

            var existingAuthor = await _context.ClassicAuthors
                .FirstOrDefaultAsync(a => a.AuthorId == authorId && a.LanguageCode == languageCode, ct);

            if (existingAuthor == null)
            {
                var author = new ClassicAuthor
                {
                    Id = Guid.NewGuid(),
                    AuthorId = authorId,
                    Name = authorName,
                    LanguageCode = languageCode,
                    SortOrder = sortOrder++,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.ClassicAuthors.Add(author);
                _logger.LogInformation("Created classic author: {AuthorId} ({Name}) for {Language}", 
                    authorId, authorName, languageCode);
            }
            else
            {
                // Update name and sort order if changed
                existingAuthor.Name = authorName;
                existingAuthor.SortOrder = sortOrder++;
                existingAuthor.UpdatedAt = DateTime.UtcNow;
                _logger.LogInformation("Updated classic author: {AuthorId} ({Name}) for {Language}", 
                    authorId, authorName, languageCode);
            }
        }

        await _context.SaveChangesAsync(ct);
    }

    private string GenerateAuthorId(string authorName)
    {
        // Convert to lowercase, replace spaces and special characters with hyphens
        var authorId = authorName.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("ă", "a")
            .Replace("â", "a")
            .Replace("î", "i")
            .Replace("ș", "s")
            .Replace("ț", "t")
            .Replace("á", "a")
            .Replace("é", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ö", "o")
            .Replace("ő", "o")
            .Replace("ú", "u")
            .Replace("ü", "u")
            .Replace("ű", "u")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace("(", "")
            .Replace(")", "");
        
        // Remove multiple consecutive hyphens
        while (authorId.Contains("--"))
        {
            authorId = authorId.Replace("--", "-");
        }
        
        // Remove leading/trailing hyphens
        authorId = authorId.Trim('-');
        
        return authorId;
    }

    private async Task SeedTopicsAsync(CancellationToken ct)
    {
        var languages = new[] { "ro-ro", "en-us", "hu-hu" };
        var topicMap = new Dictionary<string, StoryTopic>(); // topicId -> StoryTopic

        // First pass: create all topics from ro-ro (primary language)
        var roRoPath = Path.Combine(_basePath, "ro-ro", "topics.json");
        if (!File.Exists(roRoPath))
        {
            _logger.LogWarning("Topics file not found: {Path}", roRoPath);
            return;
        }

        var roRoJson = await File.ReadAllTextAsync(roRoPath, ct);
        var roRoData = JsonSerializer.Deserialize<TopicsSeedData>(roRoJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (roRoData?.StoryDimensions == null)
        {
            _logger.LogWarning("No story dimensions found in {Path}", roRoPath);
            return;
        }

        int sortOrder = 0;
        foreach (var dimension in roRoData.StoryDimensions)
        {
            if (dimension.AllowedValues == null) continue;

            foreach (var topicValue in dimension.AllowedValues)
            {
                var existingTopic = await _context.StoryTopics
                    .FirstOrDefaultAsync(t => t.TopicId == topicValue.Id, ct);

                if (existingTopic == null)
                {
                    var topic = new StoryTopic
                    {
                        Id = Guid.NewGuid(),
                        TopicId = topicValue.Id,
                        DimensionId = dimension.Id,
                        SortOrder = sortOrder++,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.StoryTopics.Add(topic);
                    topicMap[topicValue.Id] = topic;
                    _logger.LogInformation("Created topic: {TopicId} in dimension {DimensionId}", topicValue.Id, dimension.Id);
                }
                else
                {
                    topicMap[topicValue.Id] = existingTopic;
                }
            }
        }

        await _context.SaveChangesAsync(ct);

        // Second pass: add translations for all languages
        foreach (var lang in languages)
        {
            var langPath = Path.Combine(_basePath, lang, "topics.json");
            if (!File.Exists(langPath))
            {
                _logger.LogWarning("Topics file not found for language {Lang}: {Path}", lang, langPath);
                continue;
            }

            var langJson = await File.ReadAllTextAsync(langPath, ct);
            var langData = JsonSerializer.Deserialize<TopicsSeedData>(langJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (langData?.StoryDimensions == null) continue;

            foreach (var dimension in langData.StoryDimensions)
            {
                if (dimension.AllowedValues == null) continue;

                foreach (var topicValue in dimension.AllowedValues)
                {
                    if (!topicMap.TryGetValue(topicValue.Id, out var topic)) continue;

                    var existingTranslation = await _context.StoryTopicTranslations
                        .FirstOrDefaultAsync(t => t.StoryTopicId == topic.Id && t.LanguageCode == lang, ct);

                    if (existingTranslation == null)
                    {
                        var translation = new StoryTopicTranslation
                        {
                            Id = Guid.NewGuid(),
                            StoryTopicId = topic.Id,
                            LanguageCode = lang,
                            Label = topicValue.Label ?? string.Empty
                        };
                        _context.StoryTopicTranslations.Add(translation);
                        _logger.LogInformation("Added translation for topic {TopicId} in {Lang}: {Label}", 
                            topicValue.Id, lang, topicValue.Label);
                    }
                    else
                    {
                        existingTranslation.Label = topicValue.Label ?? string.Empty;
                    }
                }
            }
        }

        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("✅ Topics seeded: {Count} topics with translations", topicMap.Count);
    }

    private async Task SeedAgeGroupsAsync(CancellationToken ct)
    {
        var languages = new[] { "ro-ro", "en-us", "hu-hu" };
        var ageGroupMap = new Dictionary<string, StoryAgeGroup>(); // ageGroupId -> StoryAgeGroup

        // First pass: create all age groups from ro-ro (primary language)
        var roRoPath = Path.Combine(_basePath, "ro-ro", "age-groups.json");
        if (!File.Exists(roRoPath))
        {
            _logger.LogWarning("Age groups file not found: {Path}", roRoPath);
            return;
        }

        var roRoJson = await File.ReadAllTextAsync(roRoPath, ct);
        var roRoData = JsonSerializer.Deserialize<AgeGroupsSeedData>(roRoJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (roRoData?.AgeGroups == null)
        {
            _logger.LogWarning("No age groups found in {Path}", roRoPath);
            return;
        }

        int sortOrder = 0;
        foreach (var ageGroupData in roRoData.AgeGroups)
        {
            var existingAgeGroup = await _context.StoryAgeGroups
                .FirstOrDefaultAsync(ag => ag.AgeGroupId == ageGroupData.Id, ct);

            if (existingAgeGroup == null)
            {
                var ageGroup = new StoryAgeGroup
                {
                    Id = Guid.NewGuid(),
                    AgeGroupId = ageGroupData.Id,
                    MinAge = ageGroupData.MinAge,
                    MaxAge = ageGroupData.MaxAge,
                    SortOrder = sortOrder++,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.StoryAgeGroups.Add(ageGroup);
                ageGroupMap[ageGroupData.Id] = ageGroup;
                _logger.LogInformation("Created age group: {AgeGroupId} ({MinAge}-{MaxAge})", 
                    ageGroupData.Id, ageGroupData.MinAge, ageGroupData.MaxAge);
            }
            else
            {
                existingAgeGroup.MinAge = ageGroupData.MinAge;
                existingAgeGroup.MaxAge = ageGroupData.MaxAge;
                ageGroupMap[ageGroupData.Id] = existingAgeGroup;
            }
        }

        await _context.SaveChangesAsync(ct);

        // Second pass: add translations for all languages
        foreach (var lang in languages)
        {
            var langPath = Path.Combine(_basePath, lang, "age-groups.json");
            if (!File.Exists(langPath))
            {
                _logger.LogWarning("Age groups file not found for language {Lang}: {Path}", lang, langPath);
                continue;
            }

            var langJson = await File.ReadAllTextAsync(langPath, ct);
            var langData = JsonSerializer.Deserialize<AgeGroupsSeedData>(langJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (langData?.AgeGroups == null) continue;

            foreach (var ageGroupData in langData.AgeGroups)
            {
                if (!ageGroupMap.TryGetValue(ageGroupData.Id, out var ageGroup)) continue;

                var existingTranslation = await _context.StoryAgeGroupTranslations
                    .FirstOrDefaultAsync(t => t.StoryAgeGroupId == ageGroup.Id && t.LanguageCode == lang, ct);

                if (existingTranslation == null)
                {
                    var translation = new StoryAgeGroupTranslation
                    {
                        Id = Guid.NewGuid(),
                        StoryAgeGroupId = ageGroup.Id,
                        LanguageCode = lang,
                        Label = ageGroupData.Label ?? string.Empty,
                        Description = ageGroupData.Description
                    };
                    _context.StoryAgeGroupTranslations.Add(translation);
                    _logger.LogInformation("Added translation for age group {AgeGroupId} in {Lang}: {Label}", 
                        ageGroupData.Id, lang, ageGroupData.Label);
                }
                else
                {
                    existingTranslation.Label = ageGroupData.Label ?? string.Empty;
                    existingTranslation.Description = ageGroupData.Description;
                }
            }
        }

        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("✅ Age groups seeded: {Count} age groups with translations", ageGroupMap.Count);
    }

    // DTOs for JSON deserialization
    private class TopicsSeedData
    {
        [JsonPropertyName("story_dimensions")]
        public List<StoryDimensionSeedData>? StoryDimensions { get; set; }
    }

    private class StoryDimensionSeedData
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        [JsonPropertyName("allowed_values")]
        public List<TopicValueSeedData>? AllowedValues { get; set; }
    }

    private class TopicValueSeedData
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    private class AgeGroupsSeedData
    {
        [JsonPropertyName("age_groups")]
        public List<AgeGroupSeedData>? AgeGroups { get; set; }
    }

    private class AgeGroupSeedData
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        [JsonPropertyName("min_age")]
        public int MinAge { get; set; }
        [JsonPropertyName("max_age")]
        public int MaxAge { get; set; }
        public string? Description { get; set; }
    }

    private class AuthorsSeedData
    {
        [JsonPropertyName("romanian_authors")]
        public List<string>? RomanianAuthors { get; set; }
        
        [JsonPropertyName("hungarian_authors")]
        public List<string>? HungarianAuthors { get; set; }
        
        [JsonPropertyName("english_american_authors")]
        public List<string>? EnglishAmericanAuthors { get; set; }
    }
}

