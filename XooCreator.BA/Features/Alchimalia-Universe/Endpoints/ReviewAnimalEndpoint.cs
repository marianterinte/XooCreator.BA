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
public class ReviewAnimalEndpoint
{
    private readonly IAnimalService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ReviewAnimalEndpoint> _logger;

    public ReviewAnimalEndpoint(
        IAnimalService service,
        IAuth0UserService auth0,
        ILogger<ReviewAnimalEndpoint> logger)
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

    [Route("/api/{locale}/alchimalia-universe/animals/{animalId}/review")]
    [Authorize]
    public static async Task<Results<Ok<ReviewResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] Guid animalId,
        [FromServices] ReviewAnimalEndpoint ep,
        [FromBody] ReviewAnimalRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Reviewer))
        {
            ep._logger.LogWarning("ReviewAnimal forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            await ep._service.ReviewAsync(user.Id, animalId, req, ct);
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
