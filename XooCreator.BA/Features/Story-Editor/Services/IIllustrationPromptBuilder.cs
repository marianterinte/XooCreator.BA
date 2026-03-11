using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Builds illustration prompts for image generation from Story Bible and scenes.
/// This is a local service (no AI calls) that constructs consistent prompts.
/// </summary>
public interface IIllustrationPromptBuilder
{
    /// <summary>
    /// Build an illustration prompt for a single scene.
    /// </summary>
    /// <param name="bible">The Story Bible with character and style information</param>
    /// <param name="scene">The scene to illustrate</param>
    /// <returns>A complete illustration prompt ready for image generation</returns>
    IllustrationPrompt Build(StoryBible bible, SceneDefinition scene);

    /// <summary>
    /// Build illustration prompts for all scenes.
    /// </summary>
    /// <param name="bible">The Story Bible</param>
    /// <param name="scenes">All scenes to illustrate</param>
    /// <returns>List of illustration prompts</returns>
    List<IllustrationPrompt> BuildAll(StoryBible bible, List<SceneDefinition> scenes);

    /// <summary>
    /// Get the raw prompt text for direct use in image generation.
    /// </summary>
    string GetPromptText(IllustrationPrompt prompt);
}
