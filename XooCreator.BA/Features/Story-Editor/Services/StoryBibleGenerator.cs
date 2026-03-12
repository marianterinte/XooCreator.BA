using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Infrastructure.Logging;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Generates a Story Bible JSON from user prompt using Google Gemini.
/// </summary>
public sealed class StoryBibleGenerator : IStoryBibleGenerator
{
    private static readonly string[] DefaultMarkers =
    [
        "red collar",
        "blue scarf",
        "green bow",
        "golden bell",
        "striped bandana"
    ];

    private readonly IGoogleTextService _googleTextService;
    private readonly ILogger<StoryBibleGenerator> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public StoryBibleGenerator(
        IGoogleTextService googleTextService,
        ILogger<StoryBibleGenerator> logger)
    {
        _googleTextService = googleTextService;
        _logger = logger;
    }

    public async Task<StoryBible> GenerateAsync(
        string userPrompt,
        string? title,
        int numberOfPages,
        string languageCode,
        string apiKey,
        string? model = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userPrompt))
            throw new ArgumentException("User prompt cannot be empty", nameof(userPrompt));

        var systemInstruction = BuildSystemInstruction(numberOfPages, languageCode);
        var userContent = BuildUserContent(userPrompt, title, numberOfPages);

        _logger.LogInformation(
            "{ColoredStatus}",
            ColoredLogHelper.FormatStoryBible("START", $"pages={numberOfPages}, lang={languageCode}"));

        var jsonResponse = await _googleTextService.GenerateContentAsync(
            systemInstruction,
            userContent,
            apiKey,
            model,
            responseMimeType: "application/json",
            ct);

        var bible = ParseStoryBible(jsonResponse, languageCode, numberOfPages);

        bible = await EnsureCharacterAnchorQualityAsync(
            bible,
            userPrompt,
            languageCode,
            numberOfPages,
            apiKey,
            model,
            ct);
        
        _logger.LogInformation(
            "{ColoredStatus}",
            ColoredLogHelper.FormatStoryBible("OK", $"chars={bible.Characters.Count}, scenes={bible.SceneSkeleton.Count}"));

        return bible;
    }

    private static string BuildSystemInstruction(int numberOfPages, string languageCode)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are a children's story planner. Generate a Story Bible JSON that will guide story and illustration generation.");
        sb.AppendLine();
        sb.AppendLine("CRITICAL RULES:");
        sb.AppendLine("1. Keep characters simple: 1 main character, 1-2 supporting characters maximum.");
        sb.AppendLine("2. Visual descriptions must be SPECIFIC and CONSISTENT - always include exact colors, sizes, and distinctive features.");
        sb.AppendLine("3. Each recurring character MUST include a unique marker in visual.accessories (example: red collar, blue scarf).");
        sb.AppendLine("4. If user prompt is vague, invent missing visual details yourself and keep them stable.");
        sb.AppendLine("5. If user prompt explicitly mentions a trait (color/species/feature), preserve it and do NOT override.");
        sb.AppendLine("3. Animals work best for children's stories.");
        sb.AppendLine("6. Scene skeleton should have exactly " + numberOfPages + " scenes.");
        sb.AppendLine("7. All text content must be in language: " + languageCode);
        sb.AppendLine();
        sb.AppendLine("OUTPUT FORMAT (strict JSON):");
        sb.AppendLine(@"{
  ""title"": ""Story title"",
  ""language"": """ + languageCode + @""",
  ""ageRange"": ""3-6"",
  ""tone"": ""warm, gentle, adventurous"",
  ""visualStyle"": ""storybook, colorful, soft light"",
  ""setting"": {
    ""place"": ""specific location description"",
    ""time"": ""time of day/season""
  },
  ""characters"": [
    {
      ""id"": ""unique_id"",
      ""name"": ""Character Name"",
      ""role"": ""main"",
      ""species"": ""animal type"",
      ""visual"": {
        ""primaryColor"": ""bright yellow"",
        ""secondaryColor"": ""orange"",
        ""size"": ""small"",
        ""features"": [""fluffy feathers"", ""round eyes"", ""tiny beak""],
        ""accessories"": []
      },
      ""personality"": [""curious"", ""brave""]
    }
  ],
  ""plot"": {
    ""problem"": ""what challenge does the character face"",
    ""escalation"": ""how does it get harder"",
    ""resolution"": ""how is it resolved"",
    ""moral"": ""lesson learned""
  },
  ""sceneSkeleton"": [
    ""Scene 1 description"",
    ""Scene 2 description""
  ]
}");
        return sb.ToString();
    }

    private static string BuildUserContent(string userPrompt, string? title, int numberOfPages)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Create a Story Bible for this story idea:");
        sb.AppendLine(userPrompt);
        if (!string.IsNullOrWhiteSpace(title))
        {
            sb.AppendLine();
            sb.AppendLine($"Suggested title: {title}");
        }
        sb.AppendLine();
        sb.AppendLine($"Generate exactly {numberOfPages} scenes in the sceneSkeleton.");
        sb.AppendLine("Be very specific with character visual descriptions - include exact colors and distinctive features that can be consistently illustrated.");
        sb.AppendLine("If details are missing, invent them deterministically and keep all invented traits stable.");
        sb.AppendLine("Do not override user-explicit traits. Only fill missing parts.");
        return sb.ToString();
    }

    private async Task<StoryBible> EnsureCharacterAnchorQualityAsync(
        StoryBible bible,
        string userPrompt,
        string languageCode,
        int numberOfPages,
        string apiKey,
        string? model,
        CancellationToken ct)
    {
        if (HasStrongAnchorQuality(bible, numberOfPages))
            return bible;

        _logger.LogInformation("Story Bible has weak anchors; trying auto-completion.");
        var repaired = await TryAiAnchorRepairAsync(bible, userPrompt, languageCode, numberOfPages, apiKey, model, ct);
        if (repaired != null && HasStrongAnchorQuality(repaired, numberOfPages))
            return repaired;

        _logger.LogWarning("AI anchor repair failed or still weak; applying deterministic fallback completion.");
        return ApplyDeterministicCompletion(bible, languageCode, numberOfPages);
    }

    private static bool HasStrongAnchorQuality(StoryBible bible, int numberOfPages)
    {
        if (bible.Characters.Count == 0)
            return false;
        if (bible.SceneSkeleton.Count < numberOfPages)
            return false;

        foreach (var character in bible.Characters)
        {
            if (string.IsNullOrWhiteSpace(character.Visual.PrimaryColor) ||
                string.IsNullOrWhiteSpace(character.Visual.Size) ||
                character.Visual.Features.Count < 2 ||
                character.Visual.Accessories.Count < 1)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<StoryBible?> TryAiAnchorRepairAsync(
        StoryBible bible,
        string userPrompt,
        string languageCode,
        int numberOfPages,
        string apiKey,
        string? model,
        CancellationToken ct)
    {
        try
        {
            var systemPrompt =
                "You are a Story Bible repair assistant.\n" +
                "Task: complete missing visual anchors ONLY.\n" +
                "Do not change existing user-explicit traits from the user prompt.\n" +
                "For each recurring character ensure:\n" +
                "- visual.primaryColor present\n" +
                "- visual.size present\n" +
                "- at least 2 visual.features\n" +
                "- at least 1 visual.accessories unique marker\n" +
                $"Ensure sceneSkeleton has exactly {numberOfPages} scenes.\n" +
                $"Return strict JSON only, language={languageCode}.";

            var userContent =
                "USER PROMPT:\n" + userPrompt + "\n\n" +
                "CURRENT STORY BIBLE JSON:\n" + JsonSerializer.Serialize(bible, JsonOptions);

            var response = await _googleTextService.GenerateContentAsync(
                systemPrompt,
                userContent,
                apiKey,
                model,
                responseMimeType: "application/json",
                ct);

            var clean = CleanJsonResponse(response);
            var parsed = JsonSerializer.Deserialize<StoryBible>(clean, JsonOptions);
            return parsed;
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "AI anchor repair failed.");
            return null;
        }
    }

    private static StoryBible ApplyDeterministicCompletion(StoryBible bible, string languageCode, int numberOfPages)
    {
        var clampedPages = Math.Clamp(numberOfPages, 1, 10);

        var completedCharacters = bible.Characters
            .Select((c, i) =>
            {
                var primaryColor = string.IsNullOrWhiteSpace(c.Visual.PrimaryColor)
                    ? GetDefaultColor(c.Species, i)
                    : c.Visual.PrimaryColor;
                var size = string.IsNullOrWhiteSpace(c.Visual.Size) ? "small" : c.Visual.Size;

                var features = c.Visual.Features.Where(f => !string.IsNullOrWhiteSpace(f)).ToList();
                while (features.Count < 2)
                {
                    features.Add(features.Count == 0 ? "expressive eyes" : "distinctive silhouette");
                }

                var accessories = c.Visual.Accessories.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
                if (accessories.Count == 0)
                    accessories.Add(DefaultMarkers[i % DefaultMarkers.Length]);

                return new CharacterProfile
                {
                    Id = string.IsNullOrWhiteSpace(c.Id) ? $"char_{i + 1}" : c.Id,
                    Name = string.IsNullOrWhiteSpace(c.Name) ? $"Character {i + 1}" : c.Name,
                    Role = string.IsNullOrWhiteSpace(c.Role) ? (i == 0 ? "main" : "supporting") : c.Role,
                    Species = string.IsNullOrWhiteSpace(c.Species) ? "animal" : c.Species,
                    Visual = new VisualProfile
                    {
                        PrimaryColor = primaryColor,
                        SecondaryColor = c.Visual.SecondaryColor,
                        Size = size,
                        Features = features,
                        Accessories = accessories
                    },
                    Personality = c.Personality.Count == 0 ? ["curious"] : c.Personality
                };
            })
            .ToList();

        var skeleton = bible.SceneSkeleton.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        while (skeleton.Count < clampedPages)
        {
            skeleton.Add($"Scene {skeleton.Count + 1}");
        }
        if (skeleton.Count > clampedPages)
            skeleton = skeleton.Take(clampedPages).ToList();

        return new StoryBible
        {
            Title = string.IsNullOrWhiteSpace(bible.Title) ? "Story" : bible.Title,
            Language = string.IsNullOrWhiteSpace(bible.Language) ? languageCode : bible.Language,
            AgeRange = string.IsNullOrWhiteSpace(bible.AgeRange) ? "4-6" : bible.AgeRange,
            Tone = string.IsNullOrWhiteSpace(bible.Tone) ? "warm, gentle" : bible.Tone,
            VisualStyle = string.IsNullOrWhiteSpace(bible.VisualStyle) ? "storybook, colorful, soft light" : bible.VisualStyle,
            Setting = new StorySetting
            {
                Place = string.IsNullOrWhiteSpace(bible.Setting.Place) ? "meadow" : bible.Setting.Place,
                Time = string.IsNullOrWhiteSpace(bible.Setting.Time) ? "morning" : bible.Setting.Time
            },
            Characters = completedCharacters,
            Plot = new PlotOutline
            {
                Problem = string.IsNullOrWhiteSpace(bible.Plot.Problem) ? "character faces a challenge" : bible.Plot.Problem,
                Escalation = bible.Plot.Escalation,
                Resolution = string.IsNullOrWhiteSpace(bible.Plot.Resolution) ? "character resolves the challenge" : bible.Plot.Resolution,
                Moral = string.IsNullOrWhiteSpace(bible.Plot.Moral) ? "friendship and courage matter" : bible.Plot.Moral
            },
            SceneSkeleton = skeleton,
            Version = bible.Version
        };
    }

    private static string GetDefaultColor(string species, int idx)
    {
        if (species.Contains("cat", StringComparison.OrdinalIgnoreCase))
        {
            var catColors = new[] { "white and orange", "gray tabby", "black with white paws", "cream" };
            return catColors[idx % catColors.Length];
        }

        var generic = new[] { "yellow", "brown", "gray", "white" };
        return generic[idx % generic.Length];
    }

    private StoryBible ParseStoryBible(string jsonResponse, string languageCode, int numberOfPages)
    {
        var cleanJson = CleanJsonResponse(jsonResponse);

        try
        {
            var strictBible = JsonSerializer.Deserialize<StoryBible>(cleanJson, JsonOptions);
            if (strictBible == null)
                throw new JsonException("Strict Story Bible parse produced null object.");

            return NormalizeStoryBible(strictBible, languageCode, numberOfPages);
        }
        catch (JsonException strictEx)
        {
            _logger.LogWarning(
                strictEx,
                "Strict Story Bible parse failed; attempting tolerant parse. ResponsePreview={ResponsePreview}",
                TrimForLog(cleanJson, 500));

            if (TryParseStoryBibleTolerant(cleanJson, out var tolerantBible, out var tolerantError))
            {
                _logger.LogWarning(
                    "Story Bible recovered via tolerant parse after strict parse failure. Detail={Detail}",
                    tolerantError);
                return NormalizeStoryBible(tolerantBible, languageCode, numberOfPages);
            }

            _logger.LogError(
                strictEx,
                "Tolerant parse also failed; using deterministic fallback Story Bible. TolerantError={TolerantError}",
                tolerantError);

            return CreateFallbackBible(languageCode, numberOfPages);
        }
    }

    private static StoryBible NormalizeStoryBible(StoryBible bible, string languageCode, int numberOfPages)
    {
        var clampedPages = Math.Clamp(numberOfPages, 1, 10);

        var normalizedCharacters = (bible.Characters ?? [])
            .Select((c, i) =>
            {
                var visual = c.Visual ?? new VisualProfile
                {
                    PrimaryColor = string.Empty,
                    SecondaryColor = null,
                    Size = string.Empty,
                    Features = [],
                    Accessories = []
                };

                return new CharacterProfile
                {
                    Id = string.IsNullOrWhiteSpace(c.Id) ? $"char_{i + 1}" : c.Id.Trim(),
                    Name = string.IsNullOrWhiteSpace(c.Name) ? $"Character {i + 1}" : c.Name.Trim(),
                    Role = string.IsNullOrWhiteSpace(c.Role) ? (i == 0 ? "main" : "supporting") : c.Role.Trim(),
                    Species = string.IsNullOrWhiteSpace(c.Species) ? "animal" : c.Species.Trim(),
                    Visual = new VisualProfile
                    {
                        PrimaryColor = visual.PrimaryColor?.Trim() ?? string.Empty,
                        SecondaryColor = string.IsNullOrWhiteSpace(visual.SecondaryColor) ? null : visual.SecondaryColor.Trim(),
                        Size = visual.Size?.Trim() ?? string.Empty,
                        Features = (visual.Features ?? [])
                            .Where(f => !string.IsNullOrWhiteSpace(f))
                            .Select(f => f.Trim())
                            .ToList(),
                        Accessories = (visual.Accessories ?? [])
                            .Where(a => !string.IsNullOrWhiteSpace(a))
                            .Select(a => a.Trim())
                            .ToList()
                    },
                    Personality = (c.Personality ?? [])
                        .Where(p => !string.IsNullOrWhiteSpace(p))
                        .Select(p => p.Trim())
                        .ToList()
                };
            })
            .ToList();

        if (normalizedCharacters.Count == 0)
        {
            normalizedCharacters.Add(new CharacterProfile
            {
                Id = "char_1",
                Name = "Character 1",
                Role = "main",
                Species = "animal",
                Visual = new VisualProfile
                {
                    PrimaryColor = string.Empty,
                    SecondaryColor = null,
                    Size = string.Empty,
                    Features = [],
                    Accessories = []
                },
                Personality = ["curious"]
            });
        }

        var normalizedSkeleton = (bible.SceneSkeleton ?? [])
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .ToList();

        while (normalizedSkeleton.Count < clampedPages)
            normalizedSkeleton.Add($"Scene {normalizedSkeleton.Count + 1}");

        if (normalizedSkeleton.Count > clampedPages)
            normalizedSkeleton = normalizedSkeleton.Take(clampedPages).ToList();

        return new StoryBible
        {
            Title = string.IsNullOrWhiteSpace(bible.Title) ? "Story" : bible.Title.Trim(),
            Language = string.IsNullOrWhiteSpace(bible.Language) ? languageCode : bible.Language.Trim(),
            AgeRange = string.IsNullOrWhiteSpace(bible.AgeRange) ? "4-6" : bible.AgeRange.Trim(),
            Tone = string.IsNullOrWhiteSpace(bible.Tone) ? "warm, gentle" : bible.Tone.Trim(),
            VisualStyle = string.IsNullOrWhiteSpace(bible.VisualStyle) ? "storybook, colorful, soft light" : bible.VisualStyle.Trim(),
            Setting = new StorySetting
            {
                Place = string.IsNullOrWhiteSpace(bible.Setting?.Place) ? "meadow" : bible.Setting.Place.Trim(),
                Time = string.IsNullOrWhiteSpace(bible.Setting?.Time) ? "morning" : bible.Setting.Time.Trim()
            },
            Characters = normalizedCharacters,
            Plot = new PlotOutline
            {
                Problem = string.IsNullOrWhiteSpace(bible.Plot?.Problem) ? "character faces a challenge" : bible.Plot.Problem.Trim(),
                Escalation = string.IsNullOrWhiteSpace(bible.Plot?.Escalation) ? null : bible.Plot.Escalation.Trim(),
                Resolution = string.IsNullOrWhiteSpace(bible.Plot?.Resolution) ? "character resolves the challenge" : bible.Plot.Resolution.Trim(),
                Moral = string.IsNullOrWhiteSpace(bible.Plot?.Moral) ? "friendship and courage matter" : bible.Plot.Moral.Trim()
            },
            SceneSkeleton = normalizedSkeleton,
            Version = string.IsNullOrWhiteSpace(bible.Version) ? null : bible.Version.Trim()
        };
    }

    private static bool TryParseStoryBibleTolerant(string cleanJson, out StoryBible bible, out string detail)
    {
        try
        {
            using var doc = JsonDocument.Parse(cleanJson);
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                detail = "Root element is not a JSON object.";
                bible = CreateFallbackBible("en-US", 5);
                return false;
            }

            var root = doc.RootElement;

            var setting = GetObjectProperty(root, "setting");
            var plot = GetObjectProperty(root, "plot");

            var characters = GetArrayProperty(root, "characters")
                .Select((c, i) => ParseCharacterProfile(c, i))
                .ToList();

            var sceneSkeleton = GetArrayProperty(root, "sceneSkeleton")
                .Select(GetStringValue)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s!)
                .ToList();

            bible = new StoryBible
            {
                Title = GetStringProperty(root, "title") ?? string.Empty,
                Language = GetStringProperty(root, "language") ?? string.Empty,
                AgeRange = GetStringProperty(root, "ageRange") ?? string.Empty,
                Tone = GetStringProperty(root, "tone") ?? string.Empty,
                VisualStyle = GetStringProperty(root, "visualStyle") ?? string.Empty,
                Setting = new StorySetting
                {
                    Place = GetStringProperty(setting, "place") ?? string.Empty,
                    Time = GetStringProperty(setting, "time") ?? string.Empty
                },
                Characters = characters,
                Plot = new PlotOutline
                {
                    Problem = GetStringProperty(plot, "problem") ?? string.Empty,
                    Escalation = GetStringProperty(plot, "escalation"),
                    Resolution = GetStringProperty(plot, "resolution") ?? string.Empty,
                    Moral = GetStringProperty(plot, "moral") ?? string.Empty
                },
                SceneSkeleton = sceneSkeleton,
                Version = GetStringProperty(root, "version")
            };

            detail = "Tolerant JSON mapping succeeded.";
            return true;
        }
        catch (Exception ex)
        {
            detail = ex.Message;
            bible = CreateFallbackBible("en-US", 5);
            return false;
        }
    }

    private static CharacterProfile ParseCharacterProfile(JsonElement element, int index)
    {
        var visual = GetObjectProperty(element, "visual");

        var features = GetArrayProperty(visual, "features")
            .Select(GetStringValue)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .ToList();

        var accessories = GetArrayProperty(visual, "accessories")
            .Select(GetStringValue)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .ToList();

        var personality = GetArrayProperty(element, "personality")
            .Select(GetStringValue)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .ToList();

        return new CharacterProfile
        {
            Id = GetStringProperty(element, "id") ?? $"char_{index + 1}",
            Name = GetStringProperty(element, "name") ?? $"Character {index + 1}",
            Role = GetStringProperty(element, "role") ?? (index == 0 ? "main" : "supporting"),
            Species = GetStringProperty(element, "species") ?? "animal",
            Visual = new VisualProfile
            {
                PrimaryColor = GetStringProperty(visual, "primaryColor") ?? string.Empty,
                SecondaryColor = GetStringProperty(visual, "secondaryColor"),
                Size = GetStringProperty(visual, "size") ?? string.Empty,
                Features = features,
                Accessories = accessories
            },
            Personality = personality
        };
    }

    private static JsonElement GetObjectProperty(JsonElement element, string name)
    {
        if (element.ValueKind == JsonValueKind.Object &&
            TryGetPropertyIgnoreCase(element, name, out var child) &&
            child.ValueKind == JsonValueKind.Object)
        {
            return child;
        }

        return default;
    }

    private static List<JsonElement> GetArrayProperty(JsonElement element, string name)
    {
        if (element.ValueKind == JsonValueKind.Object &&
            TryGetPropertyIgnoreCase(element, name, out var child) &&
            child.ValueKind == JsonValueKind.Array)
        {
            return child.EnumerateArray().ToList();
        }

        return [];
    }

    private static string? GetStringProperty(JsonElement element, string name)
    {
        if (element.ValueKind != JsonValueKind.Object ||
            !TryGetPropertyIgnoreCase(element, name, out var child))
        {
            return null;
        }

        return GetStringValue(child);
    }

    private static string? GetStringValue(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.String)
            return null;

        var value = element.GetString();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static bool TryGetPropertyIgnoreCase(JsonElement element, string name, out JsonElement value)
    {
        foreach (var prop in element.EnumerateObject())
        {
            if (string.Equals(prop.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                value = prop.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private static string TrimForLog(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value.Length > maxLength ? value[..maxLength] : value;
    }

    private static string CleanJsonResponse(string response)
    {
        var json = response.Trim();
        
        if (json.StartsWith("```json"))
            json = json[7..];
        else if (json.StartsWith("```"))
            json = json[3..];
        
        if (json.EndsWith("```"))
            json = json[..^3];
        
        return json.Trim();
    }

    private static StoryBible CreateFallbackBible(string languageCode, int numberOfPages)
    {
        var clampedPages = Math.Clamp(numberOfPages, 1, 10);
        var skeleton = new List<string>(clampedPages);
        for (var i = 0; i < clampedPages; i++)
        {
            skeleton.Add($"Scene {i + 1}");
        }

        return new StoryBible
        {
            Title = "Story",
            Language = languageCode,
            AgeRange = "4-6",
            Tone = "warm, friendly",
            VisualStyle = "storybook, colorful",
            Setting = new StorySetting
            {
                Place = "meadow",
                Time = "morning"
            },
            Characters =
            [
                new CharacterProfile
                {
                    Id = "main",
                    Name = "Character",
                    Role = "main",
                    Species = "animal",
                    Visual = new VisualProfile
                    {
                        PrimaryColor = "yellow",
                        Size = "small",
                        Features = ["friendly eyes"]
                    },
                    Personality = ["curious", "brave"]
                }
            ],
            Plot = new PlotOutline
            {
                Problem = "gets lost",
                Resolution = "finds the way home",
                Moral = "courage matters"
            },
            SceneSkeleton = skeleton
        };
    }
}
