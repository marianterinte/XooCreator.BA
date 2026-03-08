using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Features.CreatureBuilder;
using XooCreator.BA.Features.CreatureBuilder.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.CreatureBuilder.Endpoints;

[Endpoint]
public sealed class GenerateGenerativeAnimalEndpoint
{
    private readonly IGenerateLoiAnimalService _generateService;
    private readonly IUserContextService _userContext;

    public GenerateGenerativeAnimalEndpoint(IGenerateLoiAnimalService generateService, IUserContextService userContext)
    {
        _generateService = generateService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/creature-builder/generate-generative-animal")]
    [Authorize]
    public static async Task<Results<Ok<GenerateGenerativeAnimalResponseDto>, BadRequest<GenerateGenerativeAnimalResponseDto>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] GenerateGenerativeAnimalEndpoint ep,
        [FromBody] GenerateGenerativeAnimalRequestDto request,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var result = await ep._generateService.GenerateAsync(userId.Value, request, ct);

        if (!result.Success)
            return TypedResults.BadRequest(result);

        return TypedResults.Ok(result);
    }
}
