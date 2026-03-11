using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Features.StoryEditor.Models;

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

        _logger.LogInformation("Generating Story Bible for prompt: {PromptPreview}...", 
            userPrompt.Length > 50 ? userPrompt[..50] : userPrompt);

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
        
        _logger.LogInformation("Story Bible generated: Title={Title}, Characters={CharCount}, Scenes={SceneCount}",
            bible.Title, bible.Characters.Count, bible.SceneSkeleton.Count);

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
        try
        {
            var cleanJson = CleanJsonResponse(jsonResponse);
            var bible = JsonSerializer.Deserialize<StoryBible>(cleanJson, JsonOptions);
            
            if (bible == null)
                throw new InvalidOperationException("Failed to parse Story Bible JSON");

            return bible;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse Story Bible JSON: {Response}", 
                jsonResponse.Length > 500 ? jsonResponse[..500] : jsonResponse);
            
            return CreateFallbackBible(languageCode, numberOfPages);
        }
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
