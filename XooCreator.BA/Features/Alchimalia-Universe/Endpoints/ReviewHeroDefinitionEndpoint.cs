using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;
using XooCreator.BA.Features.AlchimaliaUniverse.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Endpoints;

[Endpoint]
public class ReviewHeroDefinitionEndpoint
{
    private readonly IHeroDefinitionService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ReviewHeroDefinitionEndpoint> _logger;

    public ReviewHeroDefinitionEndpoint(
        IHeroDefinitionService service,
        IAuth0UserService auth0,
        ILogger<ReviewHeroDefinitionEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    public record ReviewResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "Approved";
    }

    [Route("/api/{locale}/alchimalia-universe/hero-definitions/{heroId}/review")]
    [Authorize]
    public static async Task<Results<Ok<ReviewResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string heroId,
        [FromServices] ReviewHeroDefinitionEndpoint ep,
        [FromBody] ReviewHeroDefinitionRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Reviewer))
        {
            ep._logger.LogWarning("ReviewHeroDefinition forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            await ep._service.ReviewAsync(user.Id, heroId, req, ct);
            return TypedResults.Ok(new ReviewResponse { Status = req.Approve ? "Approved" : "ChangesRequested" });
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
