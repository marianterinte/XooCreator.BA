using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Caching;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public sealed class AdminMarketplaceCacheStatusEndpoint
{
    private readonly IAuth0UserService _auth0;
    private readonly IMarketplaceCacheControl _cache;

    public AdminMarketplaceCacheStatusEndpoint(IAuth0UserService auth0, IMarketplaceCacheControl cache)
    {
        _auth0 = auth0;
        _cache = cache;
    }

    [Route("/api/admin/cache/marketplace/status")]
    [Authorize]
    public static async Task<Results<Ok<MarketplaceCacheStatus>, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromServices] AdminMarketplaceCacheStatusEndpoint ep,
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

        return TypedResults.Ok(ep._cache.GetStatus());
    }
}


