using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.StoryEditor.Services.Content;

namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// Orchestrates create draft, generate text (with user API key), parse ###T/###S/###P, optionally generate images/audio, and save draft.
/// </summary>
public interface IGenerateFullStoryDraftHandler
{
    Task<GenerateFullStoryDraftResponse> ExecuteAsync(
        GenerateFullStoryDraftRequest request,
        Guid ownerUserId,
        string ownerFirstName,
        string ownerLastName,
        string ownerEmail,
        CancellationToken ct = default);
}
