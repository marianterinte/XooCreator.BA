using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class GetEpicIdForStoryEndpoint
{
    private readonly XooDbContext _context;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetEpicIdForStoryEndpoint> _logger;

    public GetEpicIdForStoryEndpoint(
        XooDbContext context,
        IAuth0UserService auth0,
        ILogger<GetEpicIdForStoryEndpoint> logger)
    {
        _context = context;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-epic/story/{storyId}/epic-id")]
    [Authorize]
    public static async Task<Results<Ok<GetEpicIdForStoryResponse>, NotFound, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string storyId,
        [FromServices] GetEpicIdForStoryEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Find epic that contains this story
        // Check both published (StoryEpicDefinition) and draft (StoryEpicCraft) epics
        // First check published epics
        var definitionNode = await ep._context.StoryEpicDefinitionStoryNodes
            .Where(sn => sn.StoryId == storyId)
            .Select(sn => new { sn.EpicId, sn.Epic.Status })
            .FirstOrDefaultAsync(ct);

        if (definitionNode != null)
        {
            var response = new GetEpicIdForStoryResponse
            {
                EpicId = definitionNode.EpicId,
                Status = definitionNode.Status
            };
            return TypedResults.Ok(response);
        }

        // Check draft epics
        var craftNode = await ep._context.StoryEpicCraftStoryNodes
            .Where(sn => sn.StoryId == storyId)
            .Select(sn => new { sn.EpicId, sn.Epic.Status })
            .FirstOrDefaultAsync(ct);

        if (craftNode != null)
        {
            var response = new GetEpicIdForStoryResponse
            {
                EpicId = craftNode.EpicId,
                Status = craftNode.Status
            };
            return TypedResults.Ok(response);
        }

        ep._logger.LogDebug("GetEpicIdForStory: Story not found in any epic storyId={StoryId} userId={UserId}", storyId, user.Id);
        return TypedResults.NotFound();

        //return TypedResults.Ok(response);
    }
}

public record GetEpicIdForStoryResponse
{
    public required string EpicId { get; init; }
    public required string Status { get; init; }
}

