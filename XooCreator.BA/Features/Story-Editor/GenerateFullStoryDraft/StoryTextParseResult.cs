using System.Text.RegularExpressions;

namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// Result of parsing AI-generated or imported text in ###T / ###S / ###P1..###PN format.
/// Reusable for sync and async full-story-draft flow.
/// </summary>
public sealed class StoryTextParseResult
{
    public string Title { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public List<StoryTextPagePart> Pages { get; init; } = new();
}

public sealed class StoryTextPagePart
{
    public string PageId { get; init; } = string.Empty; // e.g. "p1", "p2"
    public string Text { get; init; } = string.Empty;
}
