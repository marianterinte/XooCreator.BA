using System.Text;
using System.Text.Json;
using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface ICharacterPresenceResolver
{
    Task<IReadOnlyCollection<string>> ResolveAsync(
        StoryBible bible,
        SceneDefinition scene,
        string tileText,
        string apiKey,
        string? model,
        CancellationToken ct);
}

/// <summary>
/// Resolves characters present on a page using a focused AI call.
/// This avoids language-specific heuristics in backend code.
/// </summary>
public sealed class CharacterPresenceResolver : ICharacterPresenceResolver
{
    private readonly IGoogleTextService _googleTextService;
    private readonly ILogger<CharacterPresenceResolver> _logger;

    public CharacterPresenceResolver(
        IGoogleTextService googleTextService,
        ILogger<CharacterPresenceResolver> logger)
    {
        _googleTextService = googleTextService;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<string>> ResolveAsync(
        StoryBible bible,
        SceneDefinition scene,
        string tileText,
        string apiKey,
        string? model,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(tileText) || bible.Characters.Count == 0 || string.IsNullOrWhiteSpace(apiKey))
            return [];

        var systemInstruction = BuildSystemInstruction();
        var userContent = BuildUserContent(bible, scene, tileText);

        try
        {
            var response = await _googleTextService.GenerateContentAsync(
                systemInstruction,
                userContent,
                apiKeyOverride: apiKey,
                modelOverride: model,
                responseMimeType: "application/json",
                ct: ct);

            return ParseCharacterIds(response, bible);
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning(
                ex,
                "Character presence resolution failed for sceneId={SceneId}",
                scene.SceneId);
            return [];
        }
    }

    private static string BuildSystemInstruction()
    {
        return """
You are a strict resolver for story character presence on a single page.
Return JSON only, with this exact schema:
{
  "characterIds": ["id1", "id2"]
}

Rules:
- Use ONLY IDs from the provided roster.
- Include a character only if there is strong evidence from page text or scene data.
- If uncertain, prefer omission over guessing.
- Never return names, species, explanations, or extra fields.
""";
    }

    private static string BuildUserContent(StoryBible bible, SceneDefinition scene, string tileText)
    {
        var roster = string.Join(
            Environment.NewLine,
            bible.Characters.Select(c =>
            {
                var marker = c.Visual.Accessories.Count > 0 ? string.Join(", ", c.Visual.Accessories) : "none";
                return $"- id={c.Id}; name={c.Name}; species={c.Species}; color={c.Visual.PrimaryColor}; marker={marker}";
            }));

        var currentSceneList = scene.CharactersPresent.Count > 0
            ? string.Join(", ", scene.CharactersPresent)
            : "none";

        var sb = new StringBuilder();
        sb.AppendLine("Character roster:");
        sb.AppendLine(roster);
        sb.AppendLine();
        sb.AppendLine($"Current scene metadata: sceneId={scene.SceneId}; sceneCharacters={currentSceneList}; visualFocus={scene.VisualFocus}");
        sb.AppendLine("Page text:");
        sb.AppendLine(tileText);
        sb.AppendLine();
        sb.AppendLine("Return only JSON.");
        return sb.ToString();
    }

    private static IReadOnlyCollection<string> ParseCharacterIds(string rawJson, StoryBible bible)
    {
        var cleanJson = CleanJson(rawJson);
        if (string.IsNullOrWhiteSpace(cleanJson))
            return [];

        using var doc = JsonDocument.Parse(cleanJson);
        var root = doc.RootElement;
        var candidates = ExtractIdTokens(root);
        if (candidates.Count == 0)
            return [];

        var resolved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var token in candidates)
        {
            if (string.IsNullOrWhiteSpace(token))
                continue;

            var match = bible.Characters.FirstOrDefault(c =>
                c.Id.Equals(token, StringComparison.OrdinalIgnoreCase) ||
                c.Name.Equals(token, StringComparison.OrdinalIgnoreCase));
            if (match != null)
                resolved.Add(match.Id);
        }

        return resolved.ToList();
    }

    private static List<string> ExtractIdTokens(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Array)
        {
            return root
                .EnumerateArray()
                .Where(x => x.ValueKind == JsonValueKind.String)
                .Select(x => x.GetString())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Cast<string>()
                .ToList();
        }

        if (root.ValueKind != JsonValueKind.Object)
            return [];

        foreach (var propertyName in new[] { "characterIds", "characters", "presentCharacters", "ids" })
        {
            if (!root.TryGetProperty(propertyName, out var prop) || prop.ValueKind != JsonValueKind.Array)
                continue;

            return prop
                .EnumerateArray()
                .Where(x => x.ValueKind == JsonValueKind.String)
                .Select(x => x.GetString())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Cast<string>()
                .ToList();
        }

        return [];
    }

    private static string CleanJson(string input)
    {
        var json = input.Trim();
        if (json.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
            json = json[7..];
        else if (json.StartsWith("```", StringComparison.OrdinalIgnoreCase))
            json = json[3..];

        if (json.EndsWith("```", StringComparison.OrdinalIgnoreCase))
            json = json[..^3];

        return json.Trim();
    }
}
