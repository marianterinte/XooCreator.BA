namespace XooCreator.BA.Features.StoryEditor.Models;

/// <summary>
/// A single illustratable scene derived from the story.
/// </summary>
public sealed record SceneDefinition
{
    public required string SceneId { get; init; }
    public required int OrderIndex { get; init; }
    public required string Title { get; init; }
    public required string Summary { get; init; }
    public required List<string> CharactersPresent { get; init; }
    public required string Environment { get; init; }
    public required string Emotion { get; init; }
    public required string VisualFocus { get; init; }
    public string SourceText { get; init; } = string.Empty;
}

/// <summary>
/// Scene plan containing all scenes for a story.
/// </summary>
public sealed record ScenePlan
{
    public required string StoryId { get; init; }
    public required List<SceneDefinition> Scenes { get; init; }
    public string? Version { get; init; }
}
