using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Uses Google text generation to add proper diacritics and minor spelling fixes to text.
/// Best-effort: on any error, returns the original text.
/// </summary>
public sealed class DiacriticsNormalizer : IDiacriticsNormalizer
{
    private static readonly HashSet<string> SupportedLanguages = new(StringComparer.OrdinalIgnoreCase)
    {
        "ro-ro",
        "hu-hu",
        "de-de",
        "fr-fr",
        "es-es",
        "it-it",
        "en-us"
    };

    private readonly IGoogleTextService _googleTextService;
    private readonly ILogger<DiacriticsNormalizer> _logger;

    public DiacriticsNormalizer(
        IGoogleTextService googleTextService,
        ILogger<DiacriticsNormalizer> logger)
    {
        _googleTextService = googleTextService;
        _logger = logger;
    }

    public async Task<string> NormalizeAsync(
        string text,
        string languageCode,
        string apiKey,
        string? model,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        // Only normalize for languages where diacritics are common.
        var lang = (languageCode ?? string.Empty).Trim().ToLowerInvariant();
        if (!SupportedLanguages.Contains(lang))
            return text;

        // Avoid extra calls for very short texts.
        if (text.Length < 20)
            return text;

        try
        {
            var systemInstruction = BuildSystemInstruction(lang);
            var userContent = BuildUserContent(text, lang);

            var normalized = await _googleTextService.GenerateContentAsync(
                systemInstruction,
                userContent,
                apiKey,
                model,
                responseMimeType: "text/plain",
                ct);

            if (string.IsNullOrWhiteSpace(normalized))
            {
                _logger.LogWarning(
                    "DiacriticsNormalizer returned empty text. Falling back to original. Lang={LanguageCode}, Length={Length}",
                    lang,
                    text.Length);
                return text;
            }

            return normalized.Trim();
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning(
                ex,
                "DiacriticsNormalizer failed. Falling back to original text. Lang={LanguageCode}, Length={Length}",
                languageCode,
                text.Length);
            return text;
        }
    }

    private static string BuildSystemInstruction(string languageCode)
    {
        return
            "You are a text normalizer for children's stories.\n" +
            "Task: add proper diacritics and fix minor spelling/typo issues, while keeping the meaning and sentence structure.\n" +
            "Do NOT translate the text. Do NOT change wording more than necessary.\n" +
            "Return ONLY the corrected text, in the same language: " + languageCode + ".";
    }

    private static string BuildUserContent(string text, string languageCode)
    {
        return
            "LANGUAGE: " + languageCode + "\n\n" +
            "ORIGINAL TEXT:\n" +
            text +
            "\n\n" +
            "Correct the text with proper diacritics and minor spelling fixes.";
    }
}

