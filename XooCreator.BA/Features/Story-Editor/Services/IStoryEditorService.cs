using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryEditorService
{
    Task EnsureDraftAsync(Guid ownerUserId, string storyId, LanguageCode lang, CancellationToken ct = default);
    Task SaveDraftJsonAsync(Guid ownerUserId, string storyId, LanguageCode lang, string json, CancellationToken ct = default);
}

