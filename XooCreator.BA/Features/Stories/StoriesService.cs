namespace XooCreator.BA.Features.Stories;

public interface IStoriesService
{
    Task<GetStoriesResponse> GetAllStoriesAsync(string locale);
    Task<GetStoryByIdResponse> GetStoryByIdAsync(Guid userId, string storyId, string locale);
    Task<MarkTileAsReadResponse> MarkTileAsReadAsync(Guid userId, MarkTileAsReadRequest request);
    Task InitializeStoriesAsync();
    Task<EditableStoryDto?> GetStoryForEditAsync(string storyId, string locale);
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

    public async Task<EditableStoryDto?> GetStoryForEditAsync(string storyId, string locale)
    {
        var story = await _repository.GetStoryByIdAsync(storyId, locale);
        if (story == null) return null;
        return new EditableStoryDto
        {
            Id = story.Id,
            Title = story.Title,
            CoverImageUrl = story.CoverImageUrl ?? string.Empty,
            Tiles = story.Tiles.Select(t => new EditableTileDto
            {
                Type = t.Type,
                Id = t.Id,
                Caption = t.Caption ?? string.Empty,
                Text = t.Text,
                ImageUrl = t.ImageUrl,
                AudioUrl = t.AudioUrl,
                Question = t.Question,
                Answers = (t.Answers ?? new()).Select(a => new EditableAnswerDto
                {
                    Id = a.Id,
                    Text = a.Text ?? string.Empty,
                    Tokens = (a.Tokens ?? new()).Select(tok => new EditableTokenDto
                    {
                        Type = (int)tok.Type,
                        Value = tok.Value,
                        Quantity = tok.Quantity
                    }).ToList()
                }).ToList()
            }).ToList()
        };
    }

}

public class EditableStoryDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public List<EditableTileDto> Tiles { get; set; } = new();
}

public class EditableTileDto
{
    public string Type { get; set; } = "page";
    public string Id { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? Text { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public string? Question { get; set; }
    public List<EditableAnswerDto> Answers { get; set; } = new();
}

public class EditableAnswerDto
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public List<EditableTokenDto> Tokens { get; set; } = new();
}

public class EditableTokenDto
{
    public int Type { get; set; }
    public string Value { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

