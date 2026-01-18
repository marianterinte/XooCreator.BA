using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.Services.Content;

public interface IStoryOwnershipService
{
    void VerifyOwnership(StoryCraft craft, Guid userId, bool bypassCheck = false);
    Task<bool> IsOwnerAsync(string storyId, Guid userId, CancellationToken ct = default);
}

public class StoryOwnershipService : IStoryOwnershipService
{
    public void VerifyOwnership(StoryCraft craft, Guid userId, bool bypassCheck = false)
    {
        if (bypassCheck) return;
        if (craft.OwnerUserId != userId)
        {
            throw new UnauthorizedAccessException($"User {userId} is not the owner of story {craft.StoryId}");
        }
    }

    public async Task<bool> IsOwnerAsync(string storyId, Guid userId, CancellationToken ct = default)
    {
        // This would need IStoryCraftsRepository injected if needed
        // For now, keeping it simple
        return await Task.FromResult(true);
    }
}
