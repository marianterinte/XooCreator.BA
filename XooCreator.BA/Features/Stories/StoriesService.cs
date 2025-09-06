namespace XooCreator.BA.Features.Stories;

public interface IStoriesService
{
    Task<GetStoriesResponse> GetAllStoriesAsync();
    Task<GetStoryByIdResponse> GetStoryByIdAsync(Guid userId, string storyId);
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

    public async Task<GetStoriesResponse> GetAllStoriesAsync()
    {
        var stories = await _repository.GetAllStoriesAsync();
        return new GetStoriesResponse
        {
            Stories = stories
        };
    }

    public async Task<GetStoryByIdResponse> GetStoryByIdAsync(Guid userId, string storyId)
    {
        var story = await _repository.GetStoryByIdAsync(storyId);
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
