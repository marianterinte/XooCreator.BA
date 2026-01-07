using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Caching;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public sealed class AdminResetMarketplaceCacheEndpoint
{
    private readonly IAuth0UserService _auth0;
    private readonly IMarketplaceCacheInvalidator _cache;

    public AdminResetMarketplaceCacheEndpoint(IAuth0UserService auth0, IMarketplaceCacheInvalidator cache)
    {
        _auth0 = auth0;
        _cache = cache;
    }

    [Route("/api/admin/cache/marketplace/reset")]
    [Authorize]
    public static async Task<Results<Ok<object>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromServices] AdminResetMarketplaceCacheEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        if (!ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        ep._cache.ResetAll();
        return TypedResults.Ok<object>(new { success = true });
    }
}


