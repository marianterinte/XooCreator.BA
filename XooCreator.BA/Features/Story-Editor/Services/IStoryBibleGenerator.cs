using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Generates a Story Bible from user prompt before story text generation.
/// The Bible serves as the source of truth for characters, setting, and visual style.
/// </summary>
public interface IStoryBibleGenerator
{
    /// <summary>
    /// Generate a complete Story Bible from the user's prompt.
    /// </summary>
    /// <param name="userPrompt">The user's story idea/description</param>
    /// <param name="title">Optional title hint</param>
    /// <param name="numberOfPages">Number of pages for the story</param>
    /// <param name="languageCode">Target language code (e.g., "en-us", "ro-ro")</param>
    /// <param name="apiKey">API key for the AI service</param>
    /// <param name="model">Optional model override</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A complete StoryBible with characters, setting, plot, and scene skeleton</returns>
    Task<StoryBible> GenerateAsync(
        string userPrompt,
        string? title,
        int numberOfPages,
        string languageCode,
        string apiKey,
        string? model = null,
        CancellationToken ct = default);
}
