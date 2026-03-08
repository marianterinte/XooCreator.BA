namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// Response for generate-full-story-draft (sync flow).
/// </summary>
public sealed class GenerateFullStoryDraftResponse
{
    public string StoryId { get; init; } = string.Empty;
}

/// <summary>
/// Response when RunInBackground is true (202 Accepted).
/// </summary>
public sealed class GenerateFullStoryDraftAsyncResponse
{
    public Guid JobId { get; init; }
}
