using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GetStoryAvailableLanguagesEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetStoryAvailableLanguagesEndpoint> _logger;

    public GetStoryAvailableLanguagesEndpoint(
        IStoryCraftsRepository crafts,
        IAuth0UserService auth0,
        ILogger<GetStoryAvailableLanguagesEndpoint> logger)
    {
        _crafts = crafts;
        _auth0 = auth0;
        _logger = logger;
    }

    public record AvailableLanguagesResponse
    {
        public required List<string> Languages { get; init; }
    }

    [Route("/api/stories/{storyId}/available-languages")]
    [Authorize]
    public static async Task<Results<Ok<AvailableLanguagesResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string storyId,
        [FromServices] GetStoryAvailableLanguagesEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);

        if (!isAdmin && !isCreator)
        {
            return TypedResults.Forbid();
        }

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null)
        {
            return TypedResults.NotFound();
        }

        var isOwner = craft.OwnerUserId == user.Id;
        if (!isAdmin && !isOwner)
        {
            ep._logger.LogWarning("Get available languages forbidden: userId={UserId} storyId={StoryId} not owner", user.Id, storyId);
            return TypedResults.Forbid();
        }

        var languages = await ep._crafts.GetAvailableLanguagesAsync(storyId, ct);

        return TypedResults.Ok(new AvailableLanguagesResponse
        {
            Languages = languages
        });
    }
}
