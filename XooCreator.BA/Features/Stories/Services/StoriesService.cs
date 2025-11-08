using XooCreator.BA.Features.Stories.DTOs;
using XooCreator.BA.Features.Stories.Repositories;
using XooCreator.BA.Features.StoryEditor.Repositories;
using System.Text.Json;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.Stories.Services;

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
    private readonly IStoryCraftsRepository _crafts;

    public StoriesService(IStoriesRepository repository, IStoryCraftsRepository crafts)
    {
        _repository = repository;
        _crafts = crafts;
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
        await _repository.SeedIndependentStoriesAsync();
    }

    public async Task<EditableStoryDto?> GetStoryForEditAsync(string storyId, string locale)
    {
        // 1) Try to load StoryCraft (editor working copy) first
        var lang = ToLanguageCode(locale);
        var craft = await _crafts.GetAsync(storyId, lang);
        if (craft != null && !string.IsNullOrWhiteSpace(craft.Json))
        {
            try
            {
                var dto = JsonSerializer.Deserialize<EditableStoryDto>(craft.Json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (dto != null)
                {
                    // Ensure id/title defaults exist
                    if (string.IsNullOrWhiteSpace(dto.Id)) dto.Id = storyId;
                    dto.Title ??= string.Empty;
                    dto.CoverImageUrl ??= string.Empty;
                    dto.Summary ??= string.Empty;
                    dto.Tiles ??= new();
                    return dto;
                }
            }
            catch
            {
                // If parsing fails, fall back to StoryDefinition mapping
            }
        }

        // 2) Fallback to StoryDefinition for initial bootstrapping
        var story = await _repository.GetStoryDefinitionByIdAsync(storyId);
        if (story == null) return null;

        // Get translation for the requested locale
        var translation = story.Translations.FirstOrDefault(t => t.LanguageCode == locale) 
            ?? story.Translations.FirstOrDefault(t => t.LanguageCode == "ro-ro")
            ?? story.Translations.FirstOrDefault();

        return new EditableStoryDto
        {
            Id = story.StoryId,
            Title = translation?.Title ?? story.Title,
            CoverImageUrl = story.CoverImageUrl ?? string.Empty,
            Summary = story.Summary ?? string.Empty,
            StoryType = (int)story.StoryType,
            Tiles = story.Tiles.OrderBy(t => t.SortOrder).Select(t =>
            {
                var tileTranslation = t.Translations.FirstOrDefault(tr => tr.LanguageCode == locale)
                    ?? t.Translations.FirstOrDefault(tr => tr.LanguageCode == "ro-ro")
                    ?? t.Translations.FirstOrDefault();

                return new EditableTileDto
                {
                    Type = t.Type,
                    Id = t.TileId,
                    Caption = tileTranslation?.Caption ?? t.Caption ?? string.Empty,
                    Text = tileTranslation?.Text ?? t.Text ?? string.Empty,
                    ImageUrl = t.ImageUrl ?? string.Empty,
                    AudioUrl = t.AudioUrl ?? string.Empty,
                    Question = tileTranslation?.Question ?? t.Question ?? string.Empty,
                    Answers = (t.Answers ?? new()).OrderBy(a => a.SortOrder).Select(a =>
                    {
                        var answerTranslation = a.Translations.FirstOrDefault(at => at.LanguageCode == locale)
                            ?? a.Translations.FirstOrDefault(at => at.LanguageCode == "ro-ro")
                            ?? a.Translations.FirstOrDefault();

                        return new EditableAnswerDto
                        {
                            Id = a.AnswerId,
                            Text = answerTranslation?.Text ?? a.Text ?? string.Empty,
                            Tokens = (a.Tokens ?? new()).Select(tok => new EditableTokenDto
                            {
                                Type = tok.Type ?? string.Empty, 
                                Value = tok.Value ?? string.Empty,
                                Quantity = tok.Quantity
                            }).ToList()
                        };
                    }).ToList()
                };
            }).ToList()
        };
    }

    private static LanguageCode ToLanguageCode(string tag)
    {
        var t = (tag ?? "ro-ro").ToLowerInvariant();
        return t switch
        {
            "en-us" => LanguageCode.EnUs,
            "hu-hu" => LanguageCode.HuHu,
            _ => LanguageCode.RoRo
        };
    }
}

public class EditableStoryDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public int StoryType { get; set; } = 0; // 0 = AlchimaliaEpic (Tree Of Light), 1 = Indie (Independent)
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
    public string Type { get; set; } = string.Empty; // TokenFamily as string (e.g., "Personality", "Discovery")
    public string Value { get; set; } = string.Empty;
    public int Quantity { get; set; }
}


