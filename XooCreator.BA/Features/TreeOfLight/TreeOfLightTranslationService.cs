
using System.Collections.Concurrent;
using System.Text.Json;

namespace XooCreator.BA.Features.TreeOfLight;

public class TreeOfLightTranslationService : ITreeOfLightTranslationService
{
    private static readonly ConcurrentDictionary<string, Dictionary<string, string>> _cache = new();
    private readonly string _resourcesPath;

    public TreeOfLightTranslationService()
    {
        _resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "TreeOfLight");
    }

    public async Task<Dictionary<string, string>> GetTranslationsAsync(string locale)
    {
        // First, try to get the combined (locale + fallback) translations from cache.
        if (_cache.TryGetValue(locale, out var cachedTranslations))
        {
            return cachedTranslations;
        }

        // Load fallback (en-us) translations.
        var fallbackTranslations = await LoadTranslationFileAsync("en-us");

        // Load specific locale translations if different from fallback.
        var localeTranslations = new Dictionary<string, string>();
        if (locale.ToLower() != "en-us")
        {
            localeTranslations = await LoadTranslationFileAsync(locale);
        }

        // Combine the dictionaries. Locale-specific keys will overwrite fallback keys.
        var combinedTranslations = new Dictionary<string, string>(fallbackTranslations);
        foreach (var entry in localeTranslations)
        {
            combinedTranslations[entry.Key] = entry.Value;
        }
        
        // Cache the combined result.
        _cache.TryAdd(locale, combinedTranslations);

        return combinedTranslations;
    }

    private async Task<Dictionary<string, string>> LoadTranslationFileAsync(string locale)
    {
        var filePath = Path.Combine(_resourcesPath, $"{locale}.json");
        if (!File.Exists(filePath))
        {
            // For the fallback, if en-us.json is missing, it's a critical error.
            if (locale.ToLower() == "en-us")
            {
                 throw new FileNotFoundException($"Default translation file not found: {filePath}");
            }
            // For other languages, it's acceptable to be missing, so we return an empty dictionary.
            return new Dictionary<string, string>();
        }

        var json = await File.ReadAllTextAsync(filePath);
        var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        return translations ?? new Dictionary<string, string>();
    }
}
