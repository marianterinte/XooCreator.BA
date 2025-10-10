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
    Task<bool> UpdateStoryAsync(string locale, EditableStoryDto story);
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
                    .ThenInclude(a => a.Translations)
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
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Translations)
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
            if (await _context.StoryDefinitions.AnyAsync()) return;

            var stories = await LoadStoriesFromJsonAsync(LanguageCode.RoRo.ToFolder());
            foreach (var story in stories)
            {
                _context.StoryDefinitions.Add(story);
            }
            await _context.SaveChangesAsync();

            var processedDefTranslations = new HashSet<(Guid, string)>();
            var processedTileTranslations = new HashSet<(Guid, string)>();
            var processedAnswerTranslations = new HashSet<(Guid, string)>();

            foreach (var lc in LanguageCodeExtensions.All().Where(x => x != LanguageCode.RoRo))
            {
                var translations = await LoadStoryTranslationsFromJsonAsync(lc.ToTag());
                if (translations.Count == 0) continue;

                foreach (var tr in translations)
                {
                    var def = await _context.StoryDefinitions
                        .Include(s => s.Tiles)
                            .ThenInclude(t => t.Answers)
                        .FirstOrDefaultAsync(s => s.StoryId == tr.StoryId);
                    if (def == null) continue;

                    if (!string.IsNullOrWhiteSpace(tr.Title))
                    {
                        var key = (def.Id, tr.Locale);
                        if (!processedDefTranslations.Contains(key))
                        {
                            _context.StoryDefinitionTranslations.Add(new StoryDefinitionTranslation
                            {
                                StoryDefinitionId = def.Id,
                                LanguageCode = tr.Locale,
                                Title = tr.Title!
                            });
                            processedDefTranslations.Add(key);
                        }
                    }

                    if (tr.Tiles == null) continue;

                    foreach (var t in tr.Tiles)
                    {
                        var dbTile = def.Tiles.FirstOrDefault(x => x.TileId == t.TileId);
                        if (dbTile == null) continue;

                        var tileKey = (dbTile.Id, tr.Locale);
                        if (!processedTileTranslations.Contains(tileKey))
                        {
                            _context.StoryTileTranslations.Add(new StoryTileTranslation
                            {
                                StoryTileId = dbTile.Id,
                                LanguageCode = tr.Locale,
                                Caption = t.Caption,
                                Text = t.Text,
                                Question = t.Question
                            });
                            processedTileTranslations.Add(tileKey);
                        }

                        if (t.Answers == null) continue;

                        foreach (var answer in t.Answers)
                        {
                            var dbAnswer = dbTile.Answers.FirstOrDefault(x => x.AnswerId == answer.AnswerId);
                            if (dbAnswer == null) continue;

                            var answerKey = (dbAnswer.Id, tr.Locale);
                            if (!processedAnswerTranslations.Contains(answerKey))
                            {
                                _context.StoryAnswerTranslations.Add(new StoryAnswerTranslation
                                {
                                    StoryAnswerId = dbAnswer.Id,
                                    LanguageCode = tr.Locale,
                                    Text = answer.Text
                                });
                                processedAnswerTranslations.Add(answerKey);
                            }
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during story seeding: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> UpdateStoryAsync(string locale, EditableStoryDto story)
    {
        // Minimal implementation: update translated fields and base image urls.
        // Assumes story id and tile ids exist.
        var db = await _context.StoryDefinitions
            .Include(s => s.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Tokens)
            .FirstOrDefaultAsync(s => s.StoryId == story.Id);

        if (db == null) return false;

        db.Title = string.IsNullOrWhiteSpace(story.Title) ? db.Title : story.Title;
        db.CoverImageUrl = story.CoverImageUrl ?? db.CoverImageUrl;

        var lc = (locale ?? "ro-ro").ToLowerInvariant();

        foreach (var tileDto in story.Tiles)
        {
            var dbTile = db.Tiles.FirstOrDefault(t => t.TileId == tileDto.Id);
            if (dbTile == null) continue;

            if (!string.IsNullOrWhiteSpace(tileDto.ImageUrl)) dbTile.ImageUrl = tileDto.ImageUrl;
            if (!string.IsNullOrWhiteSpace(tileDto.AudioUrl)) dbTile.AudioUrl = tileDto.AudioUrl;

            // Upsert translation for tile (caption/text/question)
            var tr = dbTile.Translations?.FirstOrDefault(x => x.LanguageCode == lc);
            if (tr == null)
            {
                tr = new StoryTileTranslation { StoryTileId = dbTile.Id, LanguageCode = lc };
                _context.StoryTileTranslations.Add(tr);
            }
            tr.Caption = tileDto.Caption;
            tr.Text = tileDto.Text;
            tr.Question = tileDto.Question;

            // Answers
            if (tileDto.Answers != null && tileDto.Answers.Count > 0)
            {
                foreach (var aDto in tileDto.Answers)
                {
                    var dbAnswer = dbTile.Answers.FirstOrDefault(a => a.AnswerId == aDto.Id);
                    if (dbAnswer == null)
                    {
                        dbAnswer = new StoryAnswer { AnswerId = aDto.Id, SortOrder = dbTile.Answers.Count + 1 };
                        dbTile.Answers.Add(dbAnswer);
                    }
                    var aTr = dbAnswer.Translations?.FirstOrDefault(x => x.LanguageCode == lc);
                    if (aTr == null)
                    {
                        aTr = new StoryAnswerTranslation { StoryAnswerId = dbAnswer.Id, LanguageCode = lc };
                        _context.StoryAnswerTranslations.Add(aTr);
                    }
                    aTr.Text = aDto.Text;

                    // Replace tokens list
                    dbAnswer.Tokens.Clear();
                    if (aDto.Tokens != null)
                    {
                        foreach (var tk in aDto.Tokens)
                        {
                            dbAnswer.Tokens.Add(new StoryAnswerToken
                            {
                                Type = TokenFamily.Personality.ToString(),
                                Value = tk.Value,
                                Quantity = tk.Quantity
                            });
                        }
                    }
                }
            }
        }

        await _context.SaveChangesAsync();
        return true;
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
        public List<AnswerTranslationSeed>? Answers { get; set; }
    }

    private sealed class AnswerTranslationSeed
    {
        public string AnswerId { get; set; } = string.Empty;
        public string? Text { get; set; }
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
                        Question = t.Question,
                        Answers = t.Answers?.Select(a => new AnswerTranslationSeed
                        {
                            AnswerId = a.AnswerId,
                            Text = a.Text
                        }).ToList()
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
                            Text = TryGetAnswerText(a, lc) ?? a.Text,
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

    private static string? TryGetAnswerText(StoryAnswer a, string lc)
        => a.Translations?.FirstOrDefault(tr => tr.LanguageCode == lc)?.Text;

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
