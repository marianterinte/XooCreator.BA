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
public class SubmitAnimalEndpoint
{
    private readonly IAnimalService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<SubmitAnimalEndpoint> _logger;

    public SubmitAnimalEndpoint(
        IAnimalService service,
        IAuth0UserService auth0,
        ILogger<SubmitAnimalEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    public record SubmitResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "SentForApproval";
    }

    [Route("/api/{locale}/alchimalia-universe/animals/{animalId}/submit")]
    [Authorize]
    public static async Task<Results<Ok<SubmitResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] Guid animalId,
        [FromServices] SubmitAnimalEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("SubmitAnimal forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            await ep._service.SubmitForReviewAsync(user.Id, animalId, ct);
            return TypedResults.Ok(new SubmitResponse());
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Conflict(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return TypedResults.Conflict(ex.Message);
        }
    }
}
