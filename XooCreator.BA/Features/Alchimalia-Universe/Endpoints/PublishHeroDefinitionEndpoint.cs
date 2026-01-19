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
public class PublishHeroDefinitionEndpoint
{
    private readonly IHeroDefinitionService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<PublishHeroDefinitionEndpoint> _logger;

    public PublishHeroDefinitionEndpoint(
        IHeroDefinitionService service,
        IAuth0UserService auth0,
        ILogger<PublishHeroDefinitionEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    public record PublishResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "Published";
    }

    [Route("/api/{locale}/alchimalia-universe/hero-definitions/{heroId}/publish")]
    [Authorize]
    public static async Task<Results<Ok<PublishResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string heroId,
        [FromServices] PublishHeroDefinitionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("PublishHeroDefinition forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            await ep._service.PublishAsync(user.Id, heroId, ct);
            return TypedResults.Ok(new PublishResponse());
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Conflict(ex.Message);
        }
    }
}
