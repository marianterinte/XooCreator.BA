using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Data.SeedData.DTOs;
using XooCreator.BA.Features.Stories.DTOs;
using XooCreator.BA.Features.TreeOfLight.DTOs;

namespace XooCreator.BA.Features.Stories.Mappers;

/// <summary>
/// Mapper class for converting StorySeedData to StoryDefinition entities.
/// Separates mapping logic from repository concerns.
/// </summary>
public static class StoryDefinitionMapper
{
    /// <summary>
    /// Marian Teacher (Marian T) GUID constant for Indie stories.
    /// </summary>
    private static readonly Guid MarianTeacherUserId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    /// <summary>
    /// Maps seed data to StoryDefinition for Indie stories.
    /// Sets CreatedBy and UpdatedBy to Marian Teacher (Marian T) GUID.
    /// </summary>
    public static StoryDefinition MapFromSeedDataForIndie(StorySeedData seedData)
    {
        var story = new StoryDefinition
        {
            StoryId = seedData.StoryId,
            Title = seedData.Title,
            CoverImageUrl = seedData.CoverImageUrl,
            StoryTopic = seedData.StoryTopic ?? seedData.Category,
            Summary = seedData.Summary ?? string.Empty,
            SortOrder = seedData.SortOrder,
            StoryType = StoryType.Indie,
            Status = StoryStatus.Published,
            IsActive = true,
            CreatedBy = MarianTeacherUserId,
            UpdatedBy = MarianTeacherUserId
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

    /// <summary>
    /// Maps seed data to StoryDefinition for AlchimaliaEpic stories (original logic).
    /// Preserves original behavior without modifications for Indie stories.
    /// </summary>
    public static StoryDefinition MapFromSeedData(StorySeedData seedData)
    {
        var storyType = seedData.StoryType.HasValue 
            ? (StoryType)seedData.StoryType.Value 
            : StoryType.AlchimaliaEpic; // Default to AlchimaliaEpic if not specified
        
        var story = new StoryDefinition
        {
            StoryId = seedData.StoryId,
            Title = seedData.Title,
            CoverImageUrl = seedData.CoverImageUrl,
            StoryTopic = seedData.StoryTopic ?? seedData.Category,
            Summary = seedData.Summary ?? string.Empty,
            SortOrder = seedData.SortOrder,
            StoryType = storyType,
            Status = StoryStatus.Published,
            IsActive = true,
            CreatedBy = null,
            UpdatedBy = null
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

    /// <summary>
    /// Maps StoryDefinition entity to StoryContentDto with locale support.
    /// </summary>
    public static StoryContentDto MapToDtoWithLocale(StoryDefinition story, string locale)
    {
        var lc = (locale ?? "ro-ro").ToLowerInvariant();
        var defTitle = TryGetTitle(story, lc) ?? story.Title;
        return new StoryContentDto
        {
            Id = story.StoryId,
            Title = defTitle,
            CoverImageUrl = story.CoverImageUrl,
            Summary = story.Summary,
            StoryTopic = story.StoryTopic,
            UnlockedStoryHeroes = GetUnlockedHeroesFromSeed(story.StoryId),
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

    /// <summary>
    /// Maps token type string to TokenFamily enum.
    /// </summary>
    public static TokenFamily MapFamily(string type)
    {
        if (string.IsNullOrWhiteSpace(type)) return TokenFamily.Personality;
        return type.Trim() switch
        {
            "TreeOfHeroes" => TokenFamily.Personality,
            "Personality" => TokenFamily.Personality,
            "Alchemy" => TokenFamily.Alchemy,
            "Discovery" => TokenFamily.Discovery,
            "Generative" => TokenFamily.Generative,
            "Learning" => TokenFamily.Discovery, // Learning tokens map to Discovery family
            _ => TokenFamily.Personality
        };
    }

    #region Helper Methods for Translations

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

    #endregion

    #region Helper Methods for Unlocked Heroes

    private static List<string> GetUnlockedHeroesFromSeed(string storyId)
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var candidates = new[]
            {
                Path.Combine(baseDir, "Data", "SeedData", "Stories", "i18n", "en-us", $"{storyId}.json"),
                Path.Combine(baseDir, "Data", "SeedData", "Stories", "i18n", "ro-ro", $"{storyId}.json"),
                Path.Combine(baseDir, "Data", "SeedData", "Stories", "i18n", "hu-hu", $"{storyId}.json"),
                Path.Combine(baseDir, "Data", "SeedData", "Stories", "independent", "i18n", "en-us", $"{storyId}.json"),
                Path.Combine(baseDir, "Data", "SeedData", "Stories", "independent", "i18n", "ro-ro", $"{storyId}.json"),
                Path.Combine(baseDir, "Data", "SeedData", "Stories", "independent", "i18n", "hu-hu", $"{storyId}.json")
            };

            foreach (var file in candidates)
            {
                if (!File.Exists(file)) continue;
                var json = File.ReadAllText(file);
                var data =JsonSerializer.Deserialize<StoryJsonProbe>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });
                if (data?.UnlockedStoryHeroes != null && data.UnlockedStoryHeroes.Count > 0)
                {
                    return data.UnlockedStoryHeroes;
                }
            }
        }
        catch { }
        return new List<string>();
    }

    private sealed class StoryJsonProbe
    {
        public List<string> UnlockedStoryHeroes { get; set; } = new();
    }

    #endregion
}

