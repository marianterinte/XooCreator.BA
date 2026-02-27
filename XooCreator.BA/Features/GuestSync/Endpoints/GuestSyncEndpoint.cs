using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.GuestSync.DTOs;
using XooCreator.BA.Features.GuestSync.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.GuestSync.Endpoints;

[Endpoint]
public class GuestSyncEndpoint
{
    private readonly IGuestSyncService _syncService;
    private readonly IAuth0UserService _auth0;

    public GuestSyncEndpoint(
        IGuestSyncService syncService,
        IAuth0UserService auth0)
    {
        _syncService = syncService;
        _auth0 = auth0;
    }

    [Route("/api/guest/sync")]
    [Authorize]
    public static async Task<Results<Ok<GuestSyncResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromBody] GuestSyncRequest? request,
        [FromServices] GuestSyncEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var req = request ?? new GuestSyncRequest();
        var response = await ep._syncService.SyncAsync(user.Id, req, ct);
        return TypedResults.Ok(response);
    }
}
