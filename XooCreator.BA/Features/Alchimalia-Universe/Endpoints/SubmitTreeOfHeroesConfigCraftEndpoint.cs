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
public class SubmitTreeOfHeroesConfigCraftEndpoint
{
    private readonly ITreeOfHeroesConfigCraftService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<SubmitTreeOfHeroesConfigCraftEndpoint> _logger;

    public SubmitTreeOfHeroesConfigCraftEndpoint(
        ITreeOfHeroesConfigCraftService service,
        IAuth0UserService auth0,
        ILogger<SubmitTreeOfHeroesConfigCraftEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/alchimalia-universe/tree-configs/crafts/{id}/submit")]
    [Authorize]
    public static async Task<Results<Ok, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] Guid id,
        [FromServices] SubmitTreeOfHeroesConfigCraftEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("SubmitTreeOfHeroesConfigCraft forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            await ep._service.SubmitForReviewAsync(user.Id, id, ct);
            return TypedResults.Ok();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
