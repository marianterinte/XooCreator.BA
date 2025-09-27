namespace XooCreator.BA.Features.Stories;

public interface IStoriesService
{
    Task<GetStoriesResponse> GetAllStoriesAsync(string locale);
    Task<GetStoryByIdResponse> GetStoryByIdAsync(Guid userId, string storyId, string locale);
    Task<MarkTileAsReadResponse> MarkTileAsReadAsync(Guid userId, MarkTileAsReadRequest request);
    Task InitializeStoriesAsync();
}

public class StoriesService : IStoriesService
{
    private readonly IStoriesRepository _repository;

    public StoriesService(IStoriesRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetStoriesResponse> GetAllStoriesAsync(string locale)
    {
        var stories = await _repository.GetAllStoriesAsync(locale);
        return new GetStoriesResponse
        {
            Stories = stories
        };
    }

    public async Task<GetStoryByIdResponse> GetStoryByIdAsync(Guid userId, string storyId, string locale)
    {
        // NOTE: Currently fetching user read-progress together with story content.
        // For clarity & performance we keep business rule minimal: if caller only wants content
        // we could add a lighter method that does not touch progress.
        // A dedicated endpoint now exists (/api/tree-of-light/user-progress) for completion status.
        var story = await _repository.GetStoryByIdAsync(storyId, locale);
        var userProgress = story != null
            ? await _repository.GetUserStoryProgressAsync(userId, storyId)
            : new List<UserStoryProgressDto>();

        return new GetStoryByIdResponse
        {
            Story = story,
            UserProgress = userProgress
        };
    }

    public async Task<MarkTileAsReadResponse> MarkTileAsReadAsync(Guid userId, MarkTileAsReadRequest request)
    {
        try
        {
            var success = await _repository.MarkTileAsReadAsync(userId, request.StoryId, request.TileId);
            
            return new MarkTileAsReadResponse
            {
                Success = success,
                ErrorMessage = success ? null : "Failed to mark tile as read"
            };
        }
        catch (Exception ex)
        {
            return new MarkTileAsReadResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task InitializeStoriesAsync()
    {
        await _repository.SeedStoriesAsync();
    }
}
