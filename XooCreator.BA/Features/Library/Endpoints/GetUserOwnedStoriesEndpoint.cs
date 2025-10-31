using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Data;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.Library.DTOs;

namespace XooCreator.BA.Features.Library.Endpoints;

[Endpoint]
public class GetUserOwnedStoriesEndpoint
{
    private readonly XooDbContext _context;
    private readonly IUserContextService _userContextService;
    
    public GetUserOwnedStoriesEndpoint(XooDbContext context, IUserContextService userContextService)
    {
        _context = context;
        _userContextService = userContextService;
    }

    [Route("/api/{locale}/stories/owned")]
    [Authorize]
    public static async Task<Ok<GetUserOwnedStoriesResponse>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetUserOwnedStoriesEndpoint ep)
    {
        var userId = ep._userContextService.GetCurrentUserId();
        
        var ownedStories = await ep._context.UserOwnedStories
            .Include(uos => uos.StoryDefinition)
            .ThenInclude(sd => sd.Translations.Where(t => t.LanguageCode == locale))
            .Where(uos => uos.UserId == userId)
            .Select(uos => new OwnedStoryDto
            {
                Id = uos.StoryDefinition.Id,
                StoryId = uos.StoryDefinition.StoryId,
                Title = uos.StoryDefinition.Translations
                    .Where(t => t.LanguageCode == locale)
                    .Select(t => t.Title)
                    .FirstOrDefault() ?? uos.StoryDefinition.Title,
                CoverImageUrl = uos.StoryDefinition.CoverImageUrl,
                Category = uos.StoryDefinition.Category,
                StoryCategory = uos.StoryDefinition.StoryCategory,
                Status = uos.StoryDefinition.Status,
                PurchasedAt = uos.PurchasedAt,
                PurchasePrice = uos.PurchasePrice
            })
            .ToListAsync();

        var response = new GetUserOwnedStoriesResponse
        {
            Stories = ownedStories,
            TotalCount = ownedStories.Count
        };

        return TypedResults.Ok(response);
    }
}
