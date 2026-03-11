using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Transforms story text into detailed, illustratable scenes.
/// </summary>
public interface IScenePlanner
{
    /// <summary>
    /// Generate a scene plan from the Story Bible and story text.
    /// </summary>
    /// <param name="bible">The Story Bible</param>
    /// <param name="storyPages">List of story page texts</param>
    /// <param name="apiKey">API key for AI service</param>
    /// <param name="model">Optional model override</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Scene plan with detailed scene definitions</returns>
    Task<ScenePlan> PlanAsync(
        StoryBible bible,
        List<string> storyPages,
        string storyId,
        string apiKey,
        string? model = null,
        CancellationToken ct = default);
}
