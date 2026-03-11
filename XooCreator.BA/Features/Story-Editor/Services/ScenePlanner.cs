using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Transforms story text into detailed, illustratable scenes using AI.
/// </summary>
public sealed class ScenePlanner : IScenePlanner
{
    private readonly IGoogleTextService _googleTextService;
    private readonly ILogger<ScenePlanner> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ScenePlanner(
        IGoogleTextService googleTextService,
        ILogger<ScenePlanner> logger)
    {
        _googleTextService = googleTextService;
        _logger = logger;
    }

    public async Task<ScenePlan> PlanAsync(
        StoryBible bible,
        List<string> storyPages,
        string storyId,
        string apiKey,
        string? model = null,
        CancellationToken ct = default)
    {
        if (storyPages.Count == 0)
            throw new ArgumentException("Story pages cannot be empty", nameof(storyPages));

        var systemPrompt = BuildSystemPrompt(bible);
        var userContent = BuildUserContent(bible, storyPages);

        _logger.LogInformation("Planning {PageCount} scenes for story", storyPages.Count);

        var response = await _googleTextService.GenerateContentAsync(
            systemPrompt,
            userContent,
            apiKey,
            model,
            responseMimeType: "application/json",
            ct);

        var scenes = ParseScenes(response, storyPages);

        _logger.LogInformation("Scene plan generated with {SceneCount} scenes", scenes.Count);

        return new ScenePlan
        {
            StoryId = storyId,
            Scenes = scenes,
            Version = "1.0"
        };
    }

    private static string BuildSystemPrompt(StoryBible bible)
    {
        var characterSummary = string.Join("\n", bible.Characters.Select(c =>
            $"- {c.Name} ({c.Role}): {c.Visual.PrimaryColor} {c.Species}, {c.Visual.Size}, features: {string.Join(", ", c.Visual.Features)}"));

        return $@"You are a scene planner for children's book illustrations.
For each story page, create a detailed scene definition that an illustrator can use.

CHARACTER REFERENCE (maintain EXACT appearance):
{characterSummary}

VISUAL STYLE: {bible.VisualStyle}
SETTING: {bible.Setting.Place}, {bible.Setting.Time}

OUTPUT FORMAT (JSON array of scenes):
[
  {{
    ""sceneId"": ""scene-01"",
    ""orderIndex"": 0,
    ""title"": ""Short scene title"",
    ""summary"": ""What happens in this scene"",
    ""charactersPresent"": [""character_id1"", ""character_id2""],
    ""environment"": ""specific environment details"",
    ""emotion"": ""emotional tone of the scene"",
    ""visualFocus"": ""what should be the main visual focus - be specific about character positions and actions""
  }}
]

IMPORTANT:
- visualFocus should describe the exact visual composition including character colors and features
- Always reference character colors from the Bible
- Environment should match the story setting";
    }

    private static string BuildUserContent(StoryBible bible, List<string> storyPages)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Create scene definitions for each story page:");
        sb.AppendLine();
        
        for (var i = 0; i < storyPages.Count; i++)
        {
            sb.AppendLine($"PAGE {i + 1}:");
            sb.AppendLine(storyPages[i]);
            sb.AppendLine();
        }

        sb.AppendLine($"Generate exactly {storyPages.Count} scenes, one for each page.");
        sb.AppendLine("Include character visual details in the visualFocus field.");
        
        return sb.ToString();
    }

    private List<SceneDefinition> ParseScenes(string jsonResponse, List<string> storyPages)
    {
        try
        {
            var cleanJson = CleanJsonResponse(jsonResponse);
            var scenes = JsonSerializer.Deserialize<List<SceneDefinition>>(cleanJson, JsonOptions);
            
            if (scenes == null || scenes.Count == 0)
            {
                _logger.LogWarning("Failed to parse scenes, creating fallback");
                return CreateFallbackScenes(storyPages);
            }

            // Ensure sourceText is set
            for (var i = 0; i < scenes.Count && i < storyPages.Count; i++)
            {
                if (string.IsNullOrEmpty(scenes[i].SourceText))
                {
                    scenes[i] = scenes[i] with { SourceText = storyPages[i] };
                }
            }

            return scenes;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse scenes JSON");
            return CreateFallbackScenes(storyPages);
        }
    }

    private static List<SceneDefinition> CreateFallbackScenes(List<string> storyPages)
    {
        return storyPages.Select((text, i) => new SceneDefinition
        {
            SceneId = $"scene-{i + 1:D2}",
            OrderIndex = i,
            Title = $"Scene {i + 1}",
            Summary = text.Length > 100 ? text[..100] + "..." : text,
            CharactersPresent = [],
            Environment = "story setting",
            Emotion = "neutral",
            VisualFocus = text,
            SourceText = text
        }).ToList();
    }

    private static string CleanJsonResponse(string response)
    {
        var json = response.Trim();
        if (json.StartsWith("```json")) json = json[7..];
        else if (json.StartsWith("```")) json = json[3..];
        if (json.EndsWith("```")) json = json[..^3];
        return json.Trim();
    }
}
