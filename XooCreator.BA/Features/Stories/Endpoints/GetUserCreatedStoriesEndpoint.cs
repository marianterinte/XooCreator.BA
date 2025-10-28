using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Data;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.Stories.Endpoints;

[Endpoint]
public class GetUserCreatedStoriesEndpoint
{
    private readonly XooDbContext _context;
    private readonly IUserContextService _userContextService;
    
    public GetUserCreatedStoriesEndpoint(XooDbContext context, IUserContextService userContextService)
    {
        _context = context;
        _userContextService = userContextService;
    }

    [Route("/api/{locale}/stories/created")]
    [Authorize]
    public static async Task<Ok<GetUserCreatedStoriesResponse>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetUserCreatedStoriesEndpoint ep)
    {
        var userId = ep._userContextService.GetCurrentUserId();
        
        var createdStories = await ep._context.UserCreatedStories
            .Include(ucs => ucs.StoryDefinition)
            .ThenInclude(sd => sd.Translations.Where(t => t.LanguageCode == locale))
            .Where(ucs => ucs.UserId == userId)
            .Select(ucs => new CreatedStoryDto
            {
                Id = ucs.StoryDefinition.Id,
                StoryId = ucs.StoryDefinition.StoryId,
                Title = ucs.StoryDefinition.Translations
                    .Where(t => t.LanguageCode == locale)
                    .Select(t => t.Title)
                    .FirstOrDefault() ?? ucs.StoryDefinition.Title,
                CoverImageUrl = ucs.StoryDefinition.CoverImageUrl,
                Category = ucs.StoryDefinition.Category,
                StoryCategory = ucs.StoryDefinition.StoryCategory,
                Status = ucs.StoryDefinition.Status,
                CreatedAt = ucs.CreatedAt,
                PublishedAt = ucs.PublishedAt,
                IsPublished = ucs.IsPublished,
                CreationNotes = ucs.CreationNotes
            })
            .ToListAsync();

        var response = new GetUserCreatedStoriesResponse
        {
            Stories = createdStories,
            TotalCount = createdStories.Count
        };

        return TypedResults.Ok(response);
    }
}

public class GetUserCreatedStoriesResponse
{
    public List<CreatedStoryDto> Stories { get; set; } = new();
    public int TotalCount { get; set; }
}

public class CreatedStoryDto
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public StoryCategory StoryCategory { get; set; }
    public StoryStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; }
    public string? CreationNotes { get; set; }
}
