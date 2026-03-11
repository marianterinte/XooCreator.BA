namespace XooCreator.BA.Features.StoryEditor.Models;

/// <summary>
/// Specialized prompt for image generation, built from Story Bible and Scene.
/// </summary>
public sealed class IllustrationPrompt
{
    public required string SceneId { get; init; }
    public required string PromptText { get; init; }
    public string? NegativePrompt { get; init; }
    public string? StyleNotes { get; init; }
    public List<string> CharacterAnchors { get; init; } = [];
}
