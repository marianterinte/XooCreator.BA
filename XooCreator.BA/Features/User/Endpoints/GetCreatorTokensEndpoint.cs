using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.User.DTOs;
using XooCreator.BA.Features.User.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.User.Endpoints;

[Endpoint]
public class GetCreatorTokensEndpoint
{
    private readonly ICreatorTokenService _tokenService;
    private readonly Auth0UserService _auth0UserService;

    public GetCreatorTokensEndpoint(ICreatorTokenService tokenService, Auth0UserService auth0UserService)
    {
        _tokenService = tokenService;
        _auth0UserService = auth0UserService;
    }

    [Route("/api/user/creator-tokens")]
    [Authorize]
    public static async Task<Ok<GetCreatorTokensResponse>> HandleGet(
        [FromServices] GetCreatorTokensEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0UserService.GetCurrentUserAsync(ct);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var response = await ep._tokenService.GetCreatorTokensAsync(user.Id, ct);
        return TypedResults.Ok(response);
    }
}
