using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

public record PublishStoryRegionResponse
{
    public bool Ok { get; init; } = true;
    public string Status { get; init; } = "Published";
    public DateTime? PublishedAtUtc { get; init; }
}

[Endpoint]
public class PublishStoryRegionEndpoint
{
    private readonly IStoryRegionService _regionService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<PublishStoryRegionEndpoint> _logger;

    public PublishStoryRegionEndpoint(
        IStoryRegionService regionService,
        IAuth0UserService auth0,
        ILogger<PublishStoryRegionEndpoint> logger)
    {
        _regionService = regionService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/regions/{regionId}/publish")]
    [Authorize]
    public static async Task<Results<Ok<PublishStoryRegionResponse>, NotFound, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string regionId,
        [FromServices] PublishStoryRegionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            return TypedResults.Forbid();
        }

        var email = user.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            return TypedResults.BadRequest("User email is required.");
        }

        try
        {
            await ep._regionService.PublishAsync(user.Id, regionId, email.Trim(), ct);
            var region = await ep._regionService.GetRegionAsync(regionId, ct);
            return TypedResults.Ok(new PublishStoryRegionResponse 
            { 
                PublishedAtUtc = region?.PublishedAtUtc 
            });
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Conflict(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    }
}

