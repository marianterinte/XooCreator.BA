using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.TreeOfHeroes.DTOs;
using XooCreator.BA.Features.TreeOfHeroes.Repositories;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class ResetPersonalityTokensEndpoint
{
    private readonly ITreeOfHeroesRepository _repository;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ResetPersonalityTokensEndpoint> _logger;

    public ResetPersonalityTokensEndpoint(
        ITreeOfHeroesRepository repository,
        IAuth0UserService auth0,
        ILogger<ResetPersonalityTokensEndpoint> logger)
    {
        _repository = repository;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/tokens/personality/reset")]
    [Authorize]
    public static async Task<Results<Ok<ResetPersonalityTokensResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromServices] ResetPersonalityTokensEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var result = await ep._repository.ResetPersonalityTokensAsync(user.Id);

        if (!result.Success)
        {
            ep._logger.LogWarning("ResetPersonalityTokens: Failed to reset tokens userId={UserId}", user.Id);
        }
        else
        {
            ep._logger.LogInformation("ResetPersonalityTokens: Successfully reset tokens userId={UserId}, tokensReturned={TokensReturned}", 
                user.Id, result.TokensReturned);
        }

        var response = new ResetPersonalityTokensResponse
        {
            Success = result.Success,
            ErrorMessage = result.ErrorMessage,
            TokensReturned = result.TokensReturned
        };

        return TypedResults.Ok(response);
    }
}

public record ResetPersonalityTokensResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int TokensReturned { get; init; }
}

