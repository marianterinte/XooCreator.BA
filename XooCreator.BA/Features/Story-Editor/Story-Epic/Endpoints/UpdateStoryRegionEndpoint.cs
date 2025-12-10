using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class UpdateStoryRegionEndpoint
{
    private readonly IStoryRegionService _regionService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<UpdateStoryRegionEndpoint> _logger;

    public UpdateStoryRegionEndpoint(
        IStoryRegionService regionService,
        IAuth0UserService auth0,
        ILogger<UpdateStoryRegionEndpoint> logger)
    {
        _regionService = regionService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/regions/{regionId}")]
    [Authorize]
    public static async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePut(
        [FromRoute] string regionId,
        [FromBody] StoryRegionDto dto,
        [FromServices] UpdateStoryRegionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            return TypedResults.Forbid();
        }

        // Validate that at least one translation with name exists
        if (dto.Translations == null || dto.Translations.Count == 0 || 
            dto.Translations.All(t => string.IsNullOrWhiteSpace(t.Name)))
        {
            return TypedResults.BadRequest("Region name is required in at least one language.");
        }

        try
        {
            await ep._regionService.SaveRegionAsync(user.Id, regionId, dto, ct);
            ep._logger.LogInformation("UpdateStoryRegion: userId={UserId} regionId={RegionId}", user.Id, regionId);
            return TypedResults.Ok();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return TypedResults.Forbid();
        }
    }
}

