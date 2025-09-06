using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.Stories;

public interface IStoriesRepository
{
    Task<List<StoryContentDto>> GetAllStoriesAsync();
    Task<StoryContentDto?> GetStoryByIdAsync(string storyId);
    Task<List<UserStoryProgressDto>> GetUserStoryProgressAsync(Guid userId, string storyId);
    Task<bool> MarkTileAsReadAsync(Guid userId, string storyId, string tileId);
    Task SeedStoriesAsync();
}

public class StoriesRepository : IStoriesRepository
{
    private readonly XooDbContext _context;

    public StoriesRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<List<StoryContentDto>> GetAllStoriesAsync()
    {
        var stories = await _context.StoryDefinitions
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Answers)
            .Where(s => s.IsActive)
            .OrderBy(s => s.SortOrder)
            .ToListAsync();

        return stories.Select(MapToDto).ToList();
    }

    public async Task<StoryContentDto?> GetStoryByIdAsync(string storyId)
    {
    storyId = NormalizeStoryId(storyId);
    var story = await _context.StoryDefinitions
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Answers)
            .FirstOrDefaultAsync(s => s.StoryId == storyId && s.IsActive);

        return story == null ? null : MapToDto(story);
    }

    public async Task<List<UserStoryProgressDto>> GetUserStoryProgressAsync(Guid userId, string storyId)
    {
    storyId = NormalizeStoryId(storyId);
        var progress = await _context.UserStoryReadProgress
            .Where(p => p.UserId == userId && p.StoryId == storyId)
            .ToListAsync();

        return progress.Select(p => new UserStoryProgressDto
        {
            StoryId = p.StoryId,
            TileId = p.TileId,
            ReadAt = p.ReadAt
        }).ToList();
    }

    public async Task<bool> MarkTileAsReadAsync(Guid userId, string storyId, string tileId)
    {
        try
        {
            storyId = NormalizeStoryId(storyId);
            var existing = await _context.UserStoryReadProgress
                .FirstOrDefaultAsync(p => p.UserId == userId && p.StoryId == storyId && p.TileId == tileId);

            if (existing != null)
            {
                return true; // Already marked as read
            }

            var readProgress = new UserStoryReadProgress
            {
                UserId = userId,
                StoryId = storyId,
                TileId = tileId
            };

            _context.UserStoryReadProgress.Add(readProgress);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task SeedStoriesAsync()
    {
        // Check if stories already exist
        var existingCount = await _context.StoryDefinitions.CountAsync();
        if (existingCount > 0) return;

        var stories = await LoadStoriesFromJsonAsync();
        
        foreach (var story in stories)
        {
            _context.StoryDefinitions.Add(story);
        }

        await _context.SaveChangesAsync();
    }

    private static async Task<List<StoryDefinition>> LoadStoriesFromJsonAsync()
    {
        var jsonPath = Path.Combine("Data", "SeedData", "stories-seed.json");
        
        if (!File.Exists(jsonPath))
        {
            throw new FileNotFoundException($"Stories seed file not found at: {jsonPath}");
        }

        var jsonContent = await File.ReadAllTextAsync(jsonPath);
        var seedData = JsonSerializer.Deserialize<StoriesSeedData>(jsonContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        });

        if (seedData?.Stories == null)
        {
            throw new InvalidOperationException("Invalid stories seed data format");
        }

        return seedData.Stories.Select(MapFromSeedData).ToList();
    }

    private static StoryDefinition MapFromSeedData(StorySeedData seedData)
    {
        var story = new StoryDefinition
        {
            StoryId = seedData.StoryId,
            Title = seedData.Title,
            CoverImageUrl = seedData.CoverImageUrl,
            Category = seedData.Category,
            SortOrder = seedData.SortOrder
        };

        if (seedData.Tiles != null)
        {
            foreach (var tileSeed in seedData.Tiles)
            {
                var tile = new StoryTile
                {
                    TileId = tileSeed.TileId,
                    Type = tileSeed.Type,
                    SortOrder = tileSeed.SortOrder,
                    Caption = tileSeed.Caption,
                    Text = tileSeed.Text,
                    ImageUrl = tileSeed.ImageUrl,
                    AudioUrl = tileSeed.AudioUrl,
                    Question = tileSeed.Question
                };

                if (tileSeed.Answers != null)
                {
                    foreach (var answerSeed in tileSeed.Answers)
                    {
                        var answer = new StoryAnswer
                        {
                            AnswerId = answerSeed.AnswerId,
                            Text = answerSeed.Text,
                            Reward = answerSeed.Reward,
                            SortOrder = answerSeed.SortOrder
                        };
                        tile.Answers.Add(answer);
                    }
                }

                story.Tiles.Add(tile);
            }
        }

        return story;
    }

    private static StoryContentDto MapToDto(StoryDefinition story)
    {
        return new StoryContentDto
        {
            Id = story.StoryId,
            Title = story.Title,
            CoverImageUrl = story.CoverImageUrl,
            Tiles = story.Tiles
                .OrderBy(t => t.SortOrder)
                .Select(t => new StoryTileDto
                {
                    Type = t.Type,
                    Id = t.TileId,
                    Caption = t.Caption,
                    Text = t.Text,
                    ImageUrl = t.ImageUrl,
                    AudioUrl = t.AudioUrl,
                    Question = t.Question,
                    Answers = t.Answers
                        .OrderBy(a => a.SortOrder)
                        .Select(a => new StoryAnswerDto
                        {
                            Id = a.AnswerId,
                            Text = a.Text,
                            Reward = a.Reward
                        }).ToList()
                }).ToList()
        };
    }

    private static string NormalizeStoryId(string storyId)
    {
        if (string.Equals(storyId, "intro-puf-puf", StringComparison.OrdinalIgnoreCase))
            return "intro-pufpuf";
        return storyId;
    }
}

// JSON deserialization models
public class StoriesSeedData
{
    public List<StorySeedData> Stories { get; set; } = new();
}

public class StorySeedData
{
    public string StoryId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public List<TileSeedData>? Tiles { get; set; }
}

public class TileSeedData
{
    public string TileId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string? Caption { get; set; }
    public string? Text { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public string? Question { get; set; }
    public List<AnswerSeedData>? Answers { get; set; }
}

public class AnswerSeedData
{
    public string AnswerId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Reward { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
