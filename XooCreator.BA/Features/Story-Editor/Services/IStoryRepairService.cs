using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Repairs story text to fix consistency issues with the Story Bible.
/// </summary>
public interface IStoryRepairService
{
    /// <summary>
    /// Repair the story text to fix consistency issues.
    /// </summary>
    /// <param name="bible">The Story Bible (source of truth)</param>
    /// <param name="storyText">The story text to repair</param>
    /// <param name="validationResult">The validation result with issues to fix</param>
    /// <param name="apiKey">API key for AI-based repair</param>
    /// <param name="model">Optional model override</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Repaired story text</returns>
    Task<string> RepairAsync(
        StoryBible bible,
        string storyText,
        StoryValidationResult validationResult,
        string apiKey,
        string? model = null,
        CancellationToken ct = default);
}
