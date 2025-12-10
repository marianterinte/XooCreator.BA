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
public class UpdateEpicHeroEndpoint
{
    private readonly IEpicHeroService _heroService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<UpdateEpicHeroEndpoint> _logger;

    public UpdateEpicHeroEndpoint(
        IEpicHeroService heroService,
        IAuth0UserService auth0,
        ILogger<UpdateEpicHeroEndpoint> logger)
    {
        _heroService = heroService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/heroes/{heroId}")]
    [Authorize]
    public static async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePut(
        [FromRoute] string heroId,
        [FromBody] EpicHeroDto dto,
        [FromServices] UpdateEpicHeroEndpoint ep,
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
            return TypedResults.BadRequest("Hero name is required in at least one language.");
        }

        try
        {
            await ep._heroService.SaveHeroAsync(user.Id, heroId, dto, ct);
            ep._logger.LogInformation("UpdateEpicHero: userId={UserId} heroId={HeroId}", user.Id, heroId);
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

