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
        sb.AppendLine("3. Animals work best for children's stories.");
        sb.AppendLine("4. Scene skeleton should have exactly " + numberOfPages + " scenes.");
        sb.AppendLine("5. All text content must be in language: " + languageCode);
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
        return sb.ToString();
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
