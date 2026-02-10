using System.Text.Json;
using System.Linq;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Services.Content;
using Microsoft.EntityFrameworkCore;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryTranslationService
{
    Task<TranslateStoryResult> TranslateStoryAsync(
        string storyId,
        string referenceLanguage,
        IReadOnlyList<string> targetLanguages,
        string apiKey,
        IReadOnlyList<string>? selectedTileIds = null,
        CancellationToken ct = default);
}

public record TranslateStoryResult(
    IReadOnlyList<string> UpdatedLanguages,
    int FieldsTranslated,
    int FieldsSkipped);

public class StoryTranslationService : IStoryTranslationService
{
    private readonly XooDbContext _db;
    private readonly IGoogleTextService _googleTextService;
    private readonly IStoryTranslationManager _translationManager;

    public StoryTranslationService(
        XooDbContext db,
        IGoogleTextService googleTextService,
        IStoryTranslationManager translationManager)
    {
        _db = db;
        _googleTextService = googleTextService;
        _translationManager = translationManager;
    }

    public async Task<TranslateStoryResult> TranslateStoryAsync(
        string storyId,
        string referenceLanguage,
        IReadOnlyList<string> targetLanguages,
        string apiKey,
        IReadOnlyList<string>? selectedTileIds = null,
        CancellationToken ct = default)
    {
        var refLang = (referenceLanguage ?? string.Empty).Trim().ToLowerInvariant();
        var targets = targetLanguages
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l.Trim().ToLowerInvariant())
            .Where(l => l != refLang)
            .Distinct()
            .ToList();

        if (string.IsNullOrWhiteSpace(refLang))
            throw new ArgumentException("Reference language is required.", nameof(referenceLanguage));

        if (targets.Count == 0)
            throw new ArgumentException("At least one target language is required.", nameof(targetLanguages));

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("ApiKey is required.", nameof(apiKey));

        var craft = await _db.StoryCrafts
            .Include(c => c.Translations)
            .Include(c => c.Tiles)
                .ThenInclude(t => t.Translations)
            .Include(c => c.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Translations)
            .Include(c => c.Tiles)
                .ThenInclude(t => t.DialogTile!)
                    .ThenInclude(dt => dt.Nodes)
                        .ThenInclude(n => n.Translations)
            .Include(c => c.Tiles)
                .ThenInclude(t => t.DialogTile!)
                    .ThenInclude(dt => dt.Nodes)
                        .ThenInclude(n => n.OutgoingEdges)
                            .ThenInclude(e => e.Translations)
            .FirstOrDefaultAsync(c => c.StoryId == storyId, ct);

        if (craft == null)
            throw new InvalidOperationException("Story craft not found.");

        var refTranslation = craft.Translations.FirstOrDefault(t => t.LanguageCode == refLang);
        if (refTranslation == null)
            throw new InvalidOperationException("Reference language does not exist on story craft.");

        foreach (var lang in targets)
        {
            await _translationManager.EnsureTranslationAsync(craft.OwnerUserId, storyId, lang, ct: ct);
        }

        var selectedTileIdSet = ParseSelectedTileIds(selectedTileIds, craft);

        // Ensure answer translations exist for targets (only for tiles we will translate)
        foreach (var tile in craft.Tiles)
        {
            if (selectedTileIdSet != null && !selectedTileIdSet.Contains(tile.Id))
                continue;
            foreach (var answer in tile.Answers)
            {
                foreach (var lang in targets)
                {
                    if (answer.Translations.All(t => t.LanguageCode != lang))
                    {
                        _db.StoryCraftAnswerTranslations.Add(new StoryCraftAnswerTranslation
                        {
                            Id = Guid.NewGuid(),
                            StoryCraftAnswerId = answer.Id,
                            LanguageCode = lang,
                            Text = string.Empty
                        });
                    }
                }
            }
        }

        var sourceMap = BuildSourceMap(craft, refLang, selectedTileIdSet);
        var updatedLanguages = new List<string>();
        var fieldsTranslated = 0;
        var fieldsSkipped = sourceMap.Count(kv => string.IsNullOrWhiteSpace(kv.Value));

        foreach (var targetLang in targets)
        {
            var translatedMap = await TranslateMapAsync(sourceMap, refLang, targetLang, apiKey, ct);
            fieldsTranslated += ApplyTranslations(craft, targetLang, sourceMap, translatedMap);
            updatedLanguages.Add(targetLang);
        }

        craft.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return new TranslateStoryResult(updatedLanguages, fieldsTranslated, fieldsSkipped);
    }

    private static HashSet<Guid>? ParseSelectedTileIds(IReadOnlyList<string>? selectedTileIds, StoryCraft craft)
    {
        if (selectedTileIds == null || selectedTileIds.Count == 0)
            return null;

        var set = new HashSet<Guid>();
        var tileIdsByString = craft.Tiles.ToDictionary(t => t.TileId, t => t.Id, StringComparer.OrdinalIgnoreCase);

        foreach (var id in selectedTileIds)
        {
            if (string.IsNullOrWhiteSpace(id)) continue;
            var s = id.Trim();
            if (Guid.TryParse(s, out var guid) && craft.Tiles.Any(t => t.Id == guid))
            {
                set.Add(guid);
            }
            else if (tileIdsByString.TryGetValue(s, out var tileGuid))
            {
                set.Add(tileGuid);
            }
        }

        return set.Count == 0 ? null : set;
    }

    private Dictionary<string, string> BuildSourceMap(StoryCraft craft, string refLang, HashSet<Guid>? selectedTileIds)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var storyTranslation = craft.Translations.FirstOrDefault(t => t.LanguageCode == refLang);
        map["story.title"] = storyTranslation?.Title ?? string.Empty;
        map["story.summary"] = storyTranslation?.Summary ?? string.Empty;

        foreach (var tile in craft.Tiles)
        {
            if (selectedTileIds != null && !selectedTileIds.Contains(tile.Id))
                continue;
            var tileTranslation = tile.Translations.FirstOrDefault(t => t.LanguageCode == refLang);
            var tileId = tile.TileId;
            map[$"tile.{tileId}.caption"] = tileTranslation?.Caption ?? string.Empty;
            map[$"tile.{tileId}.text"] = tileTranslation?.Text ?? string.Empty;
            map[$"tile.{tileId}.question"] = tileTranslation?.Question ?? string.Empty;

            if (string.Equals(tile.Type, "dialog", StringComparison.OrdinalIgnoreCase) && tile.DialogTile != null)
            {
                foreach (var node in tile.DialogTile.Nodes)
                {
                    var nodeText = node.Translations.FirstOrDefault(t => t.LanguageCode == refLang)?.Text ?? string.Empty;
                    map[$"dialog.tile.{tileId}.node.{node.NodeId}.text"] = nodeText;

                    foreach (var edge in node.OutgoingEdges)
                    {
                        var optionText = edge.Translations.FirstOrDefault(t => t.LanguageCode == refLang)?.OptionText ?? string.Empty;
                        map[$"dialog.tile.{tileId}.edge.{edge.EdgeId}.optionText"] = optionText;
                    }
                }
            }

            foreach (var answer in tile.Answers)
            {
                var answerTranslation = answer.Translations.FirstOrDefault(t => t.LanguageCode == refLang);
                map[$"tile.{tileId}.answer.{answer.AnswerId}.text"] = answerTranslation?.Text ?? string.Empty;
            }
        }

        return map;
    }

    private async Task<Dictionary<string, string>> TranslateMapAsync(
        Dictionary<string, string> sourceMap,
        string referenceLanguage,
        string targetLanguage,
        string apiKey,
        CancellationToken ct)
    {
        var systemInstruction = $"""
            You are a translation engine.
            Translate all JSON values from {referenceLanguage} to {targetLanguage}.
            Do not change keys. Do not add or remove keys.
            Keep empty strings empty.
            Output JSON only, no markdown.
            """;

        var userContent = JsonSerializer.Serialize(sourceMap);
        // Use default model from config (gemini-2.5-flash) so the same endpoint works as for "Generate next page".
        // Passing modelOverride: null avoids 404 from gemini-1.5-flash when key or project only has access to 2.5-flash.
        var responseText = await _googleTextService.GenerateContentAsync(
            systemInstruction,
            userContent,
            apiKeyOverride: apiKey,
            modelOverride: null,
            ct: ct);

        var json = ExtractJson(responseText);
        var translated = JsonSerializer.Deserialize<Dictionary<string, string>>(json) 
            ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        return translated;
    }

    private int ApplyTranslations(
        StoryCraft craft,
        string targetLang,
        Dictionary<string, string> sourceMap,
        Dictionary<string, string> translatedMap)
    {
        var translatedCount = 0;

        var storyTranslation = craft.Translations.FirstOrDefault(t => t.LanguageCode == targetLang);
        if (storyTranslation == null)
        {
            storyTranslation = new StoryCraftTranslation
            {
                Id = Guid.NewGuid(),
                StoryCraftId = craft.Id,
                LanguageCode = targetLang,
                Title = string.Empty,
                Summary = null
            };
            craft.Translations.Add(storyTranslation);
        }

        if (ShouldTranslate(sourceMap, "story.title") && TryGetTranslated(translatedMap, "story.title", out var title))
        {
            storyTranslation.Title = title;
            translatedCount++;
        }
        if (ShouldTranslate(sourceMap, "story.summary") && TryGetTranslated(translatedMap, "story.summary", out var summary))
        {
            storyTranslation.Summary = summary;
            translatedCount++;
        }

        foreach (var tile in craft.Tiles)
        {
            // Only apply if this tile was in the source map (i.e. was selected for translation)
            var tileId = tile.TileId;
            if (!sourceMap.ContainsKey($"tile.{tileId}.caption") && !sourceMap.ContainsKey($"tile.{tileId}.text") && !sourceMap.ContainsKey($"tile.{tileId}.question"))
                continue;

            var tileTranslation = tile.Translations.FirstOrDefault(t => t.LanguageCode == targetLang);
            if (tileTranslation == null)
            {
                tileTranslation = new StoryCraftTileTranslation
                {
                    Id = Guid.NewGuid(),
                    StoryCraftTileId = tile.Id,
                    LanguageCode = targetLang
                };
                tile.Translations.Add(tileTranslation);
            }

            if (ShouldTranslate(sourceMap, $"tile.{tileId}.caption") &&
                TryGetTranslated(translatedMap, $"tile.{tileId}.caption", out var caption))
            {
                tileTranslation.Caption = caption;
                translatedCount++;
            }
            if (ShouldTranslate(sourceMap, $"tile.{tileId}.text") &&
                TryGetTranslated(translatedMap, $"tile.{tileId}.text", out var text))
            {
                tileTranslation.Text = text;
                translatedCount++;
            }
            if (ShouldTranslate(sourceMap, $"tile.{tileId}.question") &&
                TryGetTranslated(translatedMap, $"tile.{tileId}.question", out var question))
            {
                tileTranslation.Question = question;
                translatedCount++;
            }

            foreach (var answer in tile.Answers)
            {
                var answerTranslation = answer.Translations.FirstOrDefault(t => t.LanguageCode == targetLang);
                if (answerTranslation == null)
                {
                    answerTranslation = new StoryCraftAnswerTranslation
                    {
                        Id = Guid.NewGuid(),
                        StoryCraftAnswerId = answer.Id,
                        LanguageCode = targetLang,
                        Text = string.Empty
                    };
                    answer.Translations.Add(answerTranslation);
                }

                var answerKey = $"tile.{tileId}.answer.{answer.AnswerId}.text";
                if (ShouldTranslate(sourceMap, answerKey) && TryGetTranslated(translatedMap, answerKey, out var answerText))
                {
                    answerTranslation.Text = answerText;
                    translatedCount++;
                }
            }

            if (string.Equals(tile.Type, "dialog", StringComparison.OrdinalIgnoreCase) && tile.DialogTile != null)
            {
                foreach (var node in tile.DialogTile.Nodes)
                {
                    var nodeKey = $"dialog.tile.{tileId}.node.{node.NodeId}.text";
                    if (ShouldTranslate(sourceMap, nodeKey) && TryGetTranslated(translatedMap, nodeKey, out var nodeText))
                    {
                        var nodeTranslation = node.Translations.FirstOrDefault(t => t.LanguageCode == targetLang);
                        if (nodeTranslation == null)
                        {
                            nodeTranslation = new StoryCraftDialogNodeTranslation
                            {
                                Id = Guid.NewGuid(),
                                StoryCraftDialogNodeId = node.Id,
                                LanguageCode = targetLang,
                                Text = string.Empty
                            };
                            node.Translations.Add(nodeTranslation);
                        }
                        nodeTranslation.Text = nodeText;
                        translatedCount++;
                    }

                    foreach (var edge in node.OutgoingEdges)
                    {
                        var edgeKey = $"dialog.tile.{tileId}.edge.{edge.EdgeId}.optionText";
                        if (ShouldTranslate(sourceMap, edgeKey) && TryGetTranslated(translatedMap, edgeKey, out var optionText))
                        {
                            var edgeTranslation = edge.Translations.FirstOrDefault(t => t.LanguageCode == targetLang);
                            if (edgeTranslation == null)
                            {
                                edgeTranslation = new StoryCraftDialogEdgeTranslation
                                {
                                    Id = Guid.NewGuid(),
                                    StoryCraftDialogEdgeId = edge.Id,
                                    LanguageCode = targetLang,
                                    OptionText = string.Empty
                                };
                                edge.Translations.Add(edgeTranslation);
                            }
                            edgeTranslation.OptionText = optionText;
                            translatedCount++;
                        }
                    }
                }
            }
        }

        return translatedCount;
    }

    private static bool ShouldTranslate(Dictionary<string, string> sourceMap, string key)
        => sourceMap.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value);

    private static bool TryGetTranslated(Dictionary<string, string> translatedMap, string key, out string value)
    {
        if (translatedMap.TryGetValue(key, out var raw) && raw != null)
        {
            value = raw;
            return true;
        }
        value = string.Empty;
        return false;
    }

    private static string ExtractJson(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "{}";

        var trimmed = text.Trim();

        if (trimmed.StartsWith("```"))
        {
            var firstBrace = trimmed.IndexOf('{');
            var lastBrace = trimmed.LastIndexOf('}');
            if (firstBrace >= 0 && lastBrace > firstBrace)
            {
                return trimmed.Substring(firstBrace, lastBrace - firstBrace + 1);
            }
        }

        if (trimmed.StartsWith("{") && trimmed.EndsWith("}"))
            return trimmed;

        var start = trimmed.IndexOf('{');
        var end = trimmed.LastIndexOf('}');
        if (start >= 0 && end > start)
            return trimmed.Substring(start, end - start + 1);

        return "{}";
    }
}
