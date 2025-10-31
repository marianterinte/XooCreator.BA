using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public class AcquireFreeStoryEndpoint
{
    private readonly XooDbContext _context;
    private readonly IUserContextService _userContext;

    public AcquireFreeStoryEndpoint(XooDbContext context, IUserContextService userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public record GetFreeStoryResponse(bool Success, string? ErrorMessage);
    public record AcquireFreeStoryRequest(string StoryId);

    [Route("/api/{locale}/tales-of-alchimalia/market/acquire-free-story")]
    [Authorize]
    public static async Task<Results<Ok<GetFreeStoryResponse>, BadRequest<GetFreeStoryResponse>, UnauthorizedHttpResult, NotFound>> HandlePost(
        [FromRoute] string locale,
        [FromBody] AcquireFreeStoryRequest? body,
        [FromServices] AcquireFreeStoryEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var finalStoryId = body?.StoryId;
        if (string.IsNullOrWhiteSpace(finalStoryId))
            return TypedResults.BadRequest(new GetFreeStoryResponse(false, "Missing storyId"));

        var info = await ep._context.StoryMarketplaceInfos
            .Include(smi => smi.Story)
            .FirstOrDefaultAsync(smi => smi.StoryId == finalStoryId && smi.Story.IsActive);
        if (info == null) return TypedResults.NotFound();

        if (info.PriceInCredits > 0)
        {
            return TypedResults.BadRequest(new GetFreeStoryResponse(false, "Story is not free"));
        }

        var alreadyOwned = await ep._context.UserOwnedStories
            .AnyAsync(uos => uos.UserId == userId && uos.StoryDefinitionId == info.Story.Id);
        if (alreadyOwned)
        {
            return TypedResults.Ok(new GetFreeStoryResponse(true, null));
        }

        ep._context.UserOwnedStories.Add(new UserOwnedStories
        {
            Id = Guid.NewGuid(),
            UserId = userId.Value,
            StoryDefinitionId = info.Story.Id,
            PurchasedAt = DateTime.UtcNow,
            PurchasePrice = 0,
            PurchaseReference = "FREE_GET"
        });
        await ep._context.SaveChangesAsync();

        return TypedResults.Ok(new GetFreeStoryResponse(true, null));
    }
}


