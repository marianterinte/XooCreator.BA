using System.Text.Json;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Helper for language merge logic when publishing with VersionCopyLanguageMode=selected.
/// </summary>
public static class StoryPublishLanguageMergeHelper
{
    /// <summary>
    /// Parses VersionCopySelectedLanguagesJson. Returns null if json is null/empty or parse fails.
    /// </summary>
    public static HashSet<string>? GetSelectedLanguagesFromCraft(StoryCraft craft)
    {
        var json = craft.VersionCopySelectedLanguagesJson;
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return TryParseSelectedLanguagesJson(json);
    }

    /// <summary>
    /// Parses selected languages JSON. Returns null if invalid or empty. Public for unit tests.
    /// </summary>
    public static HashSet<string>? TryParseSelectedLanguagesJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var list = JsonSerializer.Deserialize<List<string>>(json);
            if (list == null || list.Count == 0)
            {
                return null;
            }

            return new HashSet<string>(
                list.Select(l => l.ToLowerInvariant()).Where(l => !string.IsNullOrWhiteSpace(l)),
                StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Returns true when full/delta publish should use language merge (update only selected languages).
    /// </summary>
    public static bool ShouldUseLanguageMerge(StoryCraft craft, bool isNewDefinition)
    {
        if (isNewDefinition)
        {
            return false;
        }

        if (!string.Equals(craft.VersionCopyLanguageMode, "selected", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var selected = GetSelectedLanguagesFromCraft(craft);
        return selected != null && selected.Count > 0;
    }
}
