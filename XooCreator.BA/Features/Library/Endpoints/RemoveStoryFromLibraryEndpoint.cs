using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.Library.Endpoints;

[Endpoint]
public class RemoveStoryFromLibraryEndpoint
{
    private readonly XooDbContext _context;
    private readonly IUserContextService _userContextService;

    public RemoveStoryFromLibraryEndpoint(
        XooDbContext context,
        IUserContextService userContextService)
    {
        _context = context;
        _userContextService = userContextService;
    }

    [Route("/api/{locale}/library/remove/{storyId}")]
    [Authorize]
    public static async Task<Results<Ok<RemoveStoryFromLibraryResponse>, NotFound, UnauthorizedHttpResult>> HandleDelete(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] RemoveStoryFromLibraryEndpoint ep)
    {
        var userId = ep._userContextService.GetCurrentUserId();

        // Get the story definition to find the StoryDefinitionId
        var storyDefinition = await ep._context.StoryDefinitions
            .FirstOrDefaultAsync(s => s.StoryId == storyId);

        if (storyDefinition == null)
            return TypedResults.NotFound();

        // Find and remove the UserOwnedStories entry
        var ownedStory = await ep._context.UserOwnedStories
            .FirstOrDefaultAsync(uos => uos.UserId == userId && uos.StoryDefinitionId == storyDefinition.Id);

        if (ownedStory == null)
            return TypedResults.NotFound();

        ep._context.UserOwnedStories.Remove(ownedStory);
        await ep._context.SaveChangesAsync();

        return TypedResults.Ok(new RemoveStoryFromLibraryResponse
        {
            Success = true,
            Message = "Story removed from library successfully"
        });
    }
}

public record RemoveStoryFromLibraryResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

