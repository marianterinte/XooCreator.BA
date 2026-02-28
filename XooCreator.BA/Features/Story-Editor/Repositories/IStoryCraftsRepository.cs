using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Repositories;


public interface IStoryCraftsRepository
{
    // Get StoryCraft by storyId (no lang - one craft per story)
    Task<StoryCraft?> GetAsync(string storyId, CancellationToken ct = default);
    
    // Get StoryCraft with translations and tiles loaded for a specific language
    Task<StoryCraft?> GetWithLanguageAsync(string storyId, string languageCode, CancellationToken ct = default);
    
    // Create a new StoryCraft
    Task<StoryCraft> CreateAsync(Guid ownerUserId, string storyId, string status, CancellationToken ct = default);
    
    // Save StoryCraft (with all related entities)
    Task SaveAsync(StoryCraft craft, CancellationToken ct = default);
    
    // List crafts by owner
    Task<List<StoryCraft>> ListByOwnerAsync(Guid ownerUserId, CancellationToken ct = default);
    
    /// <summary>List crafts by owner with pagination. Returns (page items, total count).</summary>
    Task<(List<StoryCraft> Items, int TotalCount)> ListByOwnerPagedAsync(Guid ownerUserId, int skip, int take, CancellationToken ct = default);
    
    // List all crafts
    Task<List<StoryCraft>> ListAllAsync(CancellationToken ct = default);
    
    /// <summary>List all crafts with pagination. Returns (page items, total count).</summary>
    Task<(List<StoryCraft> Items, int TotalCount)> ListAllPagedAsync(int skip, int take, CancellationToken ct = default);
    
    // Count distinct story IDs by owner
    Task<int> CountDistinctStoryIdsByOwnerAsync(Guid ownerUserId, CancellationToken ct = default);
    
    // Delete StoryCraft (cascades to all translations, tiles, answers, etc.)
    Task DeleteAsync(string storyId, CancellationToken ct = default);
    
    // Get available languages for a story (from StoryCraftTranslation)
    Task<List<string>> GetAvailableLanguagesAsync(string storyId, CancellationToken ct = default);
}
