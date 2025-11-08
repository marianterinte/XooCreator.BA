using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.Repositories;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryEditorService : IStoryEditorService
{
    private readonly IStoryCraftsRepository _crafts;

    public StoryEditorService(IStoryCraftsRepository crafts)
    {
        _crafts = crafts;
    }

    public async Task EnsureDraftAsync(Guid ownerUserId, string storyId, LanguageCode lang, CancellationToken ct = default)
    {
        var existing = await _crafts.GetAsync(storyId, lang, ct);
        if (existing != null) return;
        await _crafts.CreateAsync(ownerUserId, storyId, lang, "draft", "{}", ct);
    }

    public Task SaveDraftJsonAsync(Guid ownerUserId, string storyId, LanguageCode lang, string json, CancellationToken ct = default)
        => _crafts.UpsertAsync(ownerUserId, storyId, lang, "draft", json, ct);
}


