namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// Generates a full private story (your-story) from an idea: text + images + audio, persisted as StoryDefinition with IsPrivate.
/// Uses server Google API key; 10 pages hardcoded.
/// </summary>
public interface IGeneratePrivateStoryHandler
{
    Task<GeneratePrivateStoryResponse> ExecuteAsync(
        GeneratePrivateStoryRequest request,
        Guid ownerUserId,
        string ownerFirstName,
        string ownerLastName,
        string ownerEmail,
        CancellationToken ct = default);
}

public sealed class GeneratePrivateStoryResponse
{
    public string StoryId { get; init; } = string.Empty;
    public List<string> Warnings { get; init; } = [];
}
