using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.Repositories;


public interface IStoryCraftsRepository
{
    Task<StoryCraft?> GetAsync(string storyId, LanguageCode lang, CancellationToken ct = default);
    Task<StoryCraft> CreateAsync(Guid ownerUserId, string storyId, LanguageCode lang, string status, string json, CancellationToken ct = default);
    Task UpsertAsync(Guid ownerUserId, string storyId, LanguageCode lang, string status, string json, CancellationToken ct = default);
    Task<List<StoryCraft>> ListByOwnerAsync(Guid ownerUserId, CancellationToken ct = default);
}

