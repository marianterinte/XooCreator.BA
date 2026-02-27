using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Validates a loaded StoryCraft before publish to avoid DB constraint violations and produce copyable diagnostics.
/// </summary>
public interface IStoryPublishCraftValidator
{
    /// <summary>
    /// Validates the craft for publish. Returns a result with any failures; does not throw.
    /// </summary>
    PublishCraftValidationResult Validate(StoryCraft craft);
}
