using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Endpoints;

[Endpoint]
public class DeleteHeroDefinitionCraftEndpoint
{
    private readonly IHeroDefinitionCraftService _heroService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<DeleteHeroDefinitionCraftEndpoint> _logger;

    public DeleteHeroDefinitionCraftEndpoint(
        IHeroDefinitionCraftService heroService,
        IAuth0UserService auth0,
        ILogger<DeleteHeroDefinitionCraftEndpoint> logger)
    {
        _heroService = heroService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/alchimalia-universe/hero-crafts/{heroId}")]
    [Authorize]
    public static async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandleDelete(
        [FromRoute] string heroId,
        [FromServices] DeleteHeroDefinitionCraftEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            return TypedResults.Forbid();
        }

        try
        {
            await ep._heroService.DeleteAsync(user.Id, heroId, ct);
            ep._logger.LogInformation("DeleteHeroDefinitionCraft: userId={UserId} heroId={HeroId}", user.Id, heroId);
            return TypedResults.Ok();
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    }
}
