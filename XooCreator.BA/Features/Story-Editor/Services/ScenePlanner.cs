using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Infrastructure.Logging;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Transforms story text into detailed, illustratable scenes using AI.
/// </summary>
public sealed class ScenePlanner : IScenePlanner
{
    private readonly IGoogleTextService _googleTextService;
    private readonly ILogger<ScenePlanner> _logger;

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

        _logger.LogInformation(
            "{ColoredStatus}",
            ColoredLogHelper.FormatScenePlan("START", $"pages={storyPages.Count}"));

        var response = await _googleTextService.GenerateContentAsync(
            systemPrompt,
            userContent,
            apiKey,
            model,
            responseMimeType: "application/json",
            ct);

        var scenes = ParseScenes(response, storyPages);

        _logger.LogInformation(
            "{ColoredStatus}",
            ColoredLogHelper.FormatScenePlan("OK", $"scenes={scenes.Count}"));

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
- Environment should match the story setting
- Keep all descriptions child-safe and gentle (no explicit violence, no sexual content, no nudity, no graphic details)
- If a scene implies conflict, describe it in age-appropriate, non-graphic terms";
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
        var cleanJson = CleanJsonResponse(jsonResponse);

        try
        {
            var scenes = ParseScenesLenient(cleanJson, storyPages);
            if (scenes.Count == 0)
            {
                _logger.LogWarning(
                    "{ColoredStatus}",
                    ColoredLogHelper.FormatScenePlan("FALLBACK", "no usable scenes, using deterministic fallback"));
                return CreateFallbackScenes(storyPages);
            }

            return scenes;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(
                ex,
                "{ColoredStatus}",
                ColoredLogHelper.FormatScenePlan("FALLBACK", "JSON parse error"));
            return CreateFallbackScenes(storyPages);
        }
    }

    private static List<SceneDefinition> ParseScenesLenient(string cleanJson, List<string> storyPages)
    {
        using var doc = JsonDocument.Parse(cleanJson);
        var root = doc.RootElement;

        JsonElement sceneArray;
        if (root.ValueKind == JsonValueKind.Array)
        {
            sceneArray = root;
        }
        else if (root.ValueKind == JsonValueKind.Object
                 && root.TryGetProperty("scenes", out var scenesProp)
                 && scenesProp.ValueKind == JsonValueKind.Array)
        {
            sceneArray = scenesProp;
        }
        else
        {
            return [];
        }

        var rawScenes = sceneArray.EnumerateArray().ToList();
        var result = new List<SceneDefinition>(storyPages.Count);

        for (var i = 0; i < storyPages.Count; i++)
        {
            var pageText = storyPages[i];
            if (i >= rawScenes.Count || rawScenes[i].ValueKind != JsonValueKind.Object)
            {
                result.Add(CreateFallbackScene(i, pageText));
                continue;
            }

            var sceneEl = rawScenes[i];
            var sceneId = GetOptionalString(sceneEl, "sceneId") ?? $"scene-{i + 1:D2}";
            var orderIndex = GetOptionalInt(sceneEl, "orderIndex") ?? i;
            var title = GetOptionalString(sceneEl, "title") ?? $"Scene {i + 1}";
            var summary = GetOptionalString(sceneEl, "summary");
            if (string.IsNullOrWhiteSpace(summary))
                summary = pageText.Length > 100 ? pageText[..100] + "..." : pageText;

            var charactersPresent = GetOptionalStringArray(sceneEl, "charactersPresent");
            var environment = GetOptionalString(sceneEl, "environment") ?? "story setting";
            var emotion = GetOptionalString(sceneEl, "emotion") ?? "neutral";
            var visualFocus = GetOptionalString(sceneEl, "visualFocus") ?? pageText;
            var sourceText = GetOptionalString(sceneEl, "sourceText") ?? pageText;

            result.Add(new SceneDefinition
            {
                SceneId = sceneId,
                OrderIndex = orderIndex,
                Title = title,
                Summary = summary,
                CharactersPresent = charactersPresent,
                Environment = environment,
                Emotion = emotion,
                VisualFocus = visualFocus,
                SourceText = sourceText
            });
        }

        return result;
    }

    private static SceneDefinition CreateFallbackScene(int index, string sourceText)
    {
        return new SceneDefinition
        {
            SceneId = $"scene-{index + 1:D2}",
            OrderIndex = index,
            Title = $"Scene {index + 1}",
            Summary = sourceText.Length > 100 ? sourceText[..100] + "..." : sourceText,
            CharactersPresent = [],
            Environment = "story setting",
            Emotion = "neutral",
            VisualFocus = sourceText,
            SourceText = sourceText
        };
    }

    private static string? GetOptionalString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value) || value.ValueKind != JsonValueKind.String)
            return null;

        var s = value.GetString();
        return string.IsNullOrWhiteSpace(s) ? null : s;
    }

    private static int? GetOptionalInt(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value))
            return null;

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var i))
            return i;

        return null;
    }

    private static List<string> GetOptionalStringArray(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value) || value.ValueKind != JsonValueKind.Array)
            return [];

        return value
            .EnumerateArray()
            .Where(v => v.ValueKind == JsonValueKind.String)
            .Select(v => v.GetString())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Cast<string>()
            .ToList();
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
