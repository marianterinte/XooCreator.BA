using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.Services.Content;

public interface IStoryDraftManager
{
    Task EnsureDraftAsync(Guid ownerUserId, string storyId, StoryType? storyType = null, CancellationToken ct = default);
}

public class StoryDraftManager : IStoryDraftManager
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly XooDbContext _context;

    public StoryDraftManager(IStoryCraftsRepository crafts, XooDbContext context)
    {
        _crafts = crafts;
        _context = context;
    }

    public async Task EnsureDraftAsync(Guid ownerUserId, string storyId, StoryType? storyType = null, CancellationToken ct = default)
    {
        var existing = await _crafts.GetAsync(storyId, ct);
        if (existing != null) return;
        
        var craft = await _crafts.CreateAsync(ownerUserId, storyId, StoryStatus.Draft.ToDb(), ct);
        
        // If storyType is provided, update it (default is already set to Indie in CreateAsync)
        if (storyType.HasValue && craft.StoryType != storyType.Value)
        {
            craft.StoryType = storyType.Value;
            await _context.SaveChangesAsync(ct);
        }
    }
}
