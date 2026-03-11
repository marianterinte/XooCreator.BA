using System.Threading;
using System.Threading.Tasks;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Normalizes diacritics (and minor spelling) for story text before TTS.
/// Implementations must be safe: on any error, return the original text.
/// </summary>
public interface IDiacriticsNormalizer
{
    /// <summary>
    /// Normalize diacritics for the given text in the specified language.
    /// Returns either the normalized text or the original text on failure.
    /// </summary>
    /// <param name="text">Original text to normalize.</param>
    /// <param name="languageCode">Language code (e.g. "ro-RO", "en-US").</param>
    /// <param name="apiKey">API key used for the underlying AI call.</param>
    /// <param name="model">Optional model override.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<string> NormalizeAsync(
        string text,
        string languageCode,
        string apiKey,
        string? model,
        CancellationToken ct = default);
}

