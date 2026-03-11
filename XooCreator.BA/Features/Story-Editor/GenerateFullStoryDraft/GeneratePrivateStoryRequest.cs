namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// Request for generating a private story (your-story). No API key; backend uses server Google config.
/// Idea max 2000 chars, 10 pages hardcoded, images and audio generated.
/// </summary>
public sealed class GeneratePrivateStoryRequest
{
    /// <summary>User's story idea/summary (max 2000 characters).</summary>
    public string Idea { get; init; } = string.Empty;
    /// <summary>Language code (e.g. ro-ro, en-us).</summary>
    public string LanguageCode { get; init; } = "ro-ro";
    /// <summary>Optional title hint.</summary>
    public string? Title { get; init; }
}
