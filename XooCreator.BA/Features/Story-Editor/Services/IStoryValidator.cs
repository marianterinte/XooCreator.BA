using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Validates story text against the Story Bible for consistency.
/// </summary>
public interface IStoryValidator
{
    /// <summary>
    /// Validate the generated story text against the Story Bible.
    /// </summary>
    /// <param name="bible">The Story Bible (source of truth)</param>
    /// <param name="storyText">The generated story text</param>
    /// <param name="apiKey">API key for AI-based validation</param>
    /// <param name="model">Optional model override</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Validation result with any issues found</returns>
    Task<StoryValidationResult> ValidateAsync(
        StoryBible bible,
        string storyText,
        string apiKey,
        string? model = null,
        CancellationToken ct = default);
}
