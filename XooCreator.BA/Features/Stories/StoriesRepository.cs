using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Features.TreeOfLight;

namespace XooCreator.BA.Features.Stories;

public interface IStoriesRepository
{
    Task<List<StoryContentDto>> GetAllStoriesAsync(string locale);
    Task<StoryContentDto?> GetStoryByIdAsync(string storyId, string locale);
    Task<List<UserStoryProgressDto>> GetUserStoryProgressAsync(Guid userId, string storyId);
    Task<bool> MarkTileAsReadAsync(Guid userId, string storyId, string tileId);
    Task SeedStoriesAsync();
}

public class StoriesRepository : IStoriesRepository
{
    private readonly XooDbContext _context;

    public StoriesRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<List<StoryContentDto>> GetAllStoriesAsync(string locale)
    {
        var stories = await _context.StoryDefinitions
            .Include(s => s.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Tokens)
            .Where(s => s.IsActive)
            .OrderBy(s => s.SortOrder)
            .ToListAsync();

        return stories.Select(s => MapToDtoWithLocale(s, locale)).ToList();
    }

    public async Task<StoryContentDto?> GetStoryByIdAsync(string storyId, string locale)
    {
    storyId = NormalizeStoryId(storyId);
    var story = await _context.StoryDefinitions
            .Include(s => s.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Tokens)
            .FirstOrDefaultAsync(s => s.StoryId == storyId && s.IsActive);

        return story == null ? null : MapToDtoWithLocale(story, locale);
    }

    public async Task<List<UserStoryProgressDto>> GetUserStoryProgressAsync(Guid userId, string storyId)
    {
    storyId = NormalizeStoryId(storyId);
        var progress = await _context.UserStoryReadProgress
            .Where(p => p.UserId == userId && p.StoryId == storyId)
            .ToListAsync();

        return progress.Select(p => new UserStoryProgressDto
        {
            StoryId = p.StoryId,
            TileId = p.TileId,
            ReadAt = p.ReadAt
        }).ToList();
    }

    public async Task<bool> MarkTileAsReadAsync(Guid userId, string storyId, string tileId)
    {
        try
        {
            storyId = NormalizeStoryId(storyId);
            var existing = await _context.UserStoryReadProgress
                .FirstOrDefaultAsync(p => p.UserId == userId && p.StoryId == storyId && p.TileId == tileId);

            if (existing != null)
            {
                return true; // Already marked as read
            }

            var readProgress = new UserStoryReadProgress
            {
                UserId = userId,
                StoryId = storyId,
                TileId = tileId
            };

            _context.UserStoryReadProgress.Add(readProgress);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task SeedStoriesAsync()
    {
        try
        {
            // Check if stories already exist
            var existingCount = await _context.StoryDefinitions.CountAsync();
            if (existingCount > 0) return;

            var stories = await LoadStoriesFromJsonAsync();

            foreach (var story in stories)
            {
                _context.StoryDefinitions.Add(story);
            }

            await _context.SaveChangesAsync();

            // Seed translations (English) if present
            var enTranslations = await LoadStoryTranslationsFromJsonAsync("en-us");
            if (enTranslations.Count > 0)
            {
                foreach (var tr in enTranslations)
                {
                    var def = await _context.StoryDefinitions
                        .Include(s => s.Tiles)
                        .FirstOrDefaultAsync(s => s.StoryId == tr.StoryId);
                    if (def == null) continue;

                    // Title translation
                    if (!string.IsNullOrWhiteSpace(tr.Title))
                    {
                        _context.StoryDefinitionTranslations.Add(new StoryDefinitionTranslation
                        {
                            StoryDefinitionId = def.Id,
                            LanguageCode = tr.Locale,
                            Title = tr.Title!
                        });
                    }

                    // Tile translations
                    if (tr.Tiles != null)
                    {
                        foreach (var t in tr.Tiles)
                        {
                            var dbTile = def.Tiles.FirstOrDefault(x => x.TileId == t.TileId);
                            if (dbTile == null) continue;
                            _context.StoryTileTranslations.Add(new StoryTileTranslation
                            {
                                StoryTileId = dbTile.Id,
                                LanguageCode = tr.Locale,
                                Caption = t.Caption,
                                Text = t.Text,
                                Question = t.Question
                            });
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                // No EN seed files present -> mirror RO into EN-US as placeholder translations
                var allDefs = await _context.StoryDefinitions.Include(s => s.Tiles).ToListAsync();
                foreach (var def in allDefs)
                {
                    _context.StoryDefinitionTranslations.Add(new StoryDefinitionTranslation
                    {
                        StoryDefinitionId = def.Id,
                        LanguageCode = "en-us",
                        Title = def.Title
                    });

                    foreach (var t in def.Tiles)
                    {
                        _context.StoryTileTranslations.Add(new StoryTileTranslation
                        {
                            StoryTileId = t.Id,
                            LanguageCode = "en-us",
                            Caption = t.Caption,
                            Text = t.Text,
                            Question = t.Question
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    private static async Task<List<StoryDefinition>> LoadStoriesFromJsonAsync(string baseLocale = "ro-ro")
    {
        // Preferred per-locale sources (one story per file):
        //   Data/SeedData/<locale>/Stories/*.json
        //   Data/SeedData/Stories/<locale>/*.json
        // Backward compatibility:
        //   Data/SeedData/Stories/*.json
        //   Data/SeedData/stories-seed.json

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var localeLc = (baseLocale ?? "ro-ro").ToLowerInvariant();
        var candidates = new[]
        {
            Path.Combine(baseDir, "Data", "SeedData", localeLc, "Stories"),
            Path.Combine(baseDir, "Data", "SeedData", "Stories", localeLc),
            // Fallback non-locale folder
            Path.Combine(baseDir, "Data", "SeedData", "Stories")
        };
        var legacyPath = Path.Combine(baseDir, "Data", "SeedData", "stories-seed.json");

        var storyMap = new Dictionary<string, StoryDefinition>(StringComparer.OrdinalIgnoreCase);

        foreach (var dir in candidates)
        {
            if (!Directory.Exists(dir)) continue;
            var files = Directory
                .EnumerateFiles(dir, "*.json", SearchOption.TopDirectoryOnly)
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var file in files)
            {
                var json = await File.ReadAllTextAsync(file);
                var seed = JsonSerializer.Deserialize<StorySeedData>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                if (seed == null)
                {
                    throw new InvalidOperationException($"Invalid story seed data in '{file}'.");
                }

                var def = MapFromSeedData(seed);
                storyMap[def.StoryId] = def;
            }
            // Stop at first matching folder that contains files
            if (files.Count > 0) break;
        }

        if (File.Exists(legacyPath))
        {
            var jsonContent = await File.ReadAllTextAsync(legacyPath);
            var seedData = JsonSerializer.Deserialize<StoriesSeedData>(jsonContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            if (seedData?.Stories == null)
            {
                throw new InvalidOperationException("Invalid stories seed data format");
            }

            foreach (var s in seedData.Stories)
            {
                if (string.IsNullOrWhiteSpace(s.StoryId)) continue;
                if (!storyMap.ContainsKey(s.StoryId))
                {
                    storyMap[s.StoryId] = MapFromSeedData(s);
                }
            }
        }

        if (storyMap.Count == 0)
        {
            throw new FileNotFoundException(
                $"No story seeds found. Checked folders: {string.Join(", ", candidates)} and legacy file '{legacyPath}'.");
        }

        return storyMap.Values
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.StoryId, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static StoryDefinition MapFromSeedData(StorySeedData seedData)
    {
        var story = new StoryDefinition
        {
            StoryId = seedData.StoryId,
            Title = seedData.Title,
            CoverImageUrl = seedData.CoverImageUrl,
            Category = seedData.Category,
            SortOrder = seedData.SortOrder
        };

        if (seedData.Tiles != null)
        {
            foreach (var tileSeed in seedData.Tiles)
            {
                var tile = new StoryTile
                {
                    TileId = tileSeed.TileId,
                    Type = tileSeed.Type,
                    SortOrder = tileSeed.SortOrder,
                    Caption = tileSeed.Caption,
                    Text = tileSeed.Text,
                    ImageUrl = tileSeed.ImageUrl,
                    AudioUrl = tileSeed.AudioUrl,
                    Question = tileSeed.Question
                };

                if (tileSeed.Answers != null)
                {
                    foreach (var answerSeed in tileSeed.Answers)
                    {
                        var answer = new StoryAnswer
                        {
                            AnswerId = answerSeed.AnswerId,
                            Text = answerSeed.Text,
                            TokensJson = null,
                            SortOrder = answerSeed.SortOrder
                        };
                        if (answerSeed.Tokens != null)
                        {
                            foreach (var tk in answerSeed.Tokens)
                            {
                                answer.Tokens.Add(new StoryAnswerToken
                                {
                                    Type = MapFamily(tk.Type).ToString(),
                                    Value = tk.Value,
                                    Quantity = tk.Quantity
                                });
                            }
                        }
                        tile.Answers.Add(answer);
                    }
                }

                story.Tiles.Add(tile);
            }
        }

        return story;
    }

    private sealed class StoryTranslationSeed
    {
        public string Locale { get; set; } = "en-us";
        public string StoryId { get; set; } = string.Empty;
        public string? Title { get; set; }
        public List<TileTranslationSeed>? Tiles { get; set; }
    }

    private sealed class TileTranslationSeed
    {
        public string TileId { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public string? Text { get; set; }
        public string? Question { get; set; }
    }

    private static async Task<List<StoryTranslationSeed>> LoadStoryTranslationsFromJsonAsync(string locale)
    {
        // Supported patterns:
        // Data/SeedData/<locale>/Stories/*.json
        // Data/SeedData/Stories/<locale>/*.json
        var results = new List<StoryTranslationSeed>();
        var candidates = new[]
        {
            Path.Combine("Data", "SeedData", locale, "Stories"),
            Path.Combine("Data", "SeedData", "Stories", locale)
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        foreach (var dir in candidates)
        {
            if (!Directory.Exists(dir)) continue;
            var files = Directory
                .EnumerateFiles(dir, "*.json", SearchOption.TopDirectoryOnly)
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var file in files)
            {
                var json = await File.ReadAllTextAsync(file);
                var seed = JsonSerializer.Deserialize<StorySeedData>(json, jsonOptions);
                if (seed == null || string.IsNullOrWhiteSpace(seed.StoryId))
                {
                    continue;
                }

                var tr = new StoryTranslationSeed
                {
                    Locale = locale.ToLowerInvariant(),
                    StoryId = seed.StoryId,
                    Title = seed.Title,
                    Tiles = seed.Tiles?.Select(t => new TileTranslationSeed
                    {
                        TileId = t.TileId,
                        Caption = t.Caption,
                        Text = t.Text,
                        Question = t.Question
                    }).ToList()
                };
                results.Add(tr);
            }
        }

        return results;
    }

    private static StoryContentDto MapToDtoWithLocale(StoryDefinition story, string locale)
    {
        var lc = (locale ?? "ro-ro").ToLowerInvariant();
        var defTitle = TryGetTitle(story, lc) ?? story.Title;
        return new StoryContentDto
        {
            Id = story.StoryId,
            Title = defTitle,
            CoverImageUrl = story.CoverImageUrl,
            Tiles = story.Tiles
                .OrderBy(t => t.SortOrder)
                .Select(t => new StoryTileDto
                {
                    Type = t.Type,
                    Id = t.TileId,
                    Caption = TryGetCaption(t, lc) ?? t.Caption,
                    Text = TryGetText(t, lc) ?? t.Text,
                    ImageUrl = t.ImageUrl,
                    AudioUrl = t.AudioUrl,
                    Question = TryGetQuestion(t, lc) ?? t.Question,
                    Answers = t.Answers
                        .OrderBy(a => a.SortOrder)
                        .Select(a => new StoryAnswerDto
                        {
                            Id = a.AnswerId,
                            Text = a.Text,
                            Tokens = a.Tokens.Select(tok => new TokenReward
                            {
                                Type = Enum.TryParse<TokenFamily>(tok.Type, true, out var fam) ? fam : TokenFamily.Personality,
                                Value = tok.Value,
                                Quantity = tok.Quantity
                            }).ToList()
                        }).ToList()
                }).ToList()
        };
    }

    private static string? TryGetTitle(StoryDefinition def, string lc)
        => def.Translations?.FirstOrDefault(t => t.LanguageCode == lc)?.Title;
    private static string? TryGetCaption(StoryTile t, string lc)
        => t.Translations?.FirstOrDefault(tr => tr.LanguageCode == lc)?.Caption;
    private static string? TryGetText(StoryTile t, string lc)
        => t.Translations?.FirstOrDefault(tr => tr.LanguageCode == lc)?.Text;
    private static string? TryGetQuestion(StoryTile t, string lc)
        => t.Translations?.FirstOrDefault(tr => tr.LanguageCode == lc)?.Question;

    private static string NormalizeStoryId(string storyId)
    {
        if (string.Equals(storyId, "intro-puf-puf", StringComparison.OrdinalIgnoreCase))
            return "intro-pufpuf";
        return storyId;
    }

    private static TokenFamily MapFamily(string type)
    {
        if (string.IsNullOrWhiteSpace(type)) return TokenFamily.Personality;
        return type.Trim() switch
        {
            "TreeOfHeroes" => TokenFamily.Personality,
            "Personality" => TokenFamily.Personality,
            "Alchemy" => TokenFamily.Alchemy,
            "Discovery" => TokenFamily.Discovery,
            "Generative" => TokenFamily.Generative,
            _ => TokenFamily.Personality
        };
    }
}

// JSON deserialization models
public class StoriesSeedData
{
    public List<StorySeedData> Stories { get; set; } = new();
}

public class StorySeedData
{
    public string StoryId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public List<TileSeedData>? Tiles { get; set; }
}

public class TileSeedData
{
    public string TileId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string? Caption { get; set; }
    public string? Text { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public string? Question { get; set; }
    public List<AnswerSeedData>? Answers { get; set; }
}

public class AnswerSeedData
{
    public string AnswerId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public List<TokenSeedData>? Tokens { get; set; }
    public int SortOrder { get; set; }
}

public class TokenSeedData
{
    public string Type { get; set; } = string.Empty; // e.g. "TreeOfHeroes" | "Personality" | "Alchemy"
    public string Value { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
