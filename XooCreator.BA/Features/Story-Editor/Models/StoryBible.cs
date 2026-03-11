namespace XooCreator.BA.Features.StoryEditor.Models;

/// <summary>
/// Story Bible is the central source of truth for semantic and visual consistency.
/// Generated before the story text, it anchors all subsequent generation steps.
/// </summary>
public sealed class StoryBible
{
    public required string Title { get; init; }
    public required string Language { get; init; }
    public required string AgeRange { get; init; }
    public required string Tone { get; init; }
    public required string VisualStyle { get; init; }
    public required StorySetting Setting { get; init; }
    public required List<CharacterProfile> Characters { get; init; }
    public required PlotOutline Plot { get; init; }
    public required List<string> SceneSkeleton { get; init; }
    public string? Version { get; init; }
}

/// <summary>
/// Setting/environment for the story.
/// </summary>
public sealed class StorySetting
{
    public required string Place { get; init; }
    public required string Time { get; init; }
}

/// <summary>
/// Character profile with visual and personality traits.
/// </summary>
public sealed class CharacterProfile
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Role { get; init; }
    public required string Species { get; init; }
    public required VisualProfile Visual { get; init; }
    public List<string> Personality { get; init; } = [];
}

/// <summary>
/// Visual appearance details for a character.
/// </summary>
public sealed class VisualProfile
{
    public required string PrimaryColor { get; init; }
    public string? SecondaryColor { get; init; }
    public required string Size { get; init; }
    public List<string> Features { get; init; } = [];
    public List<string> Accessories { get; init; } = [];
}

/// <summary>
/// Plot structure with conflict and resolution.
/// </summary>
public sealed class PlotOutline
{
    public required string Problem { get; init; }
    public string? Escalation { get; init; }
    public required string Resolution { get; init; }
    public required string Moral { get; init; }
}
