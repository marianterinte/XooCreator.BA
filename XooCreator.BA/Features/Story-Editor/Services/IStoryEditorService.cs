using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.Stories.Services;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryEditorService
{
    // Ensure a draft exists for a story (creates StoryCraft if needed)
    Task EnsureDraftAsync(Guid ownerUserId, string storyId, CancellationToken ct = default);
    
    // Ensure a translation exists for a specific language (creates StoryCraftTranslation if needed)
    Task EnsureTranslationAsync(Guid ownerUserId, string storyId, string languageCode, string? title = null, CancellationToken ct = default);
    
    // Save draft from EditableStoryDto (converts to StoryCraft structure)
    Task SaveDraftAsync(Guid ownerUserId, string storyId, string languageCode, EditableStoryDto dto, CancellationToken ct = default);
    
    // Delete a translation (removes StoryCraftTranslation and related data for that language)
    Task DeleteTranslationAsync(Guid ownerUserId, string storyId, string languageCode, CancellationToken ct = default);
    
    // Delete entire draft (removes StoryCraft and all translations)
    Task DeleteDraftAsync(Guid ownerUserId, string storyId, CancellationToken ct = default);
}
