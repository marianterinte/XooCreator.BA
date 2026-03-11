using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Ensures generated image prompts contain required character anchors and immutable consistency rules.
/// </summary>
public interface IStoryImagePromptConsistencyValidator
{
    /// <summary>
    /// Validate and patch prompt text to include required character anchors and rules.
    /// Returns patched prompt and number of inserted anchors/rules.
    /// </summary>
    (string PromptText, int PatchedCount) ValidateAndPatch(
        string promptText,
        StoryBible bible,
        SceneDefinition? scene,
        IReadOnlyList<string> characterAnchors);
}

