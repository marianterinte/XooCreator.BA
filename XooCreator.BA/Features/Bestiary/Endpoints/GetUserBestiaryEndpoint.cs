using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.Bestiary.Endpoints;

[Endpoint]
public sealed class GetUserBestiaryEndpoint
{
    private readonly XooDbContext _db;
    private readonly IUserContextService _userContext;

    public GetUserBestiaryEndpoint(XooDbContext db, IUserContextService userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public record BestiaryResponse(List<DiscoveryBestiaryItemDto> Discovery /*, List<...> TreeOfHeroes, List<...> Generated */);

    [Route("/api/{locale}/bestiary")] // GET
    public static async Task<Results<Ok<BestiaryResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetUserBestiaryEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var discovery = await ep._db.UserDiscoveries
            .Where(ud => ud.UserId == userId.Value)
            .Join(ep._db.DiscoveryItems, ud => ud.DiscoveryItemId, di => di.Id, (ud, di) => new { ud, di })
            .OrderByDescending(x => x.ud.DiscoveredAt)
            .Select(x => new DiscoveryBestiaryItemDto(
                x.di.Id,
                x.di.Name,
                (x.di.ArmsKey == "—" ? "None" : x.di.ArmsKey) + (x.di.BodyKey == "—" ? "None" : x.di.BodyKey) + (x.di.HeadKey == "—" ? "None" : x.di.HeadKey) + ".jpg",
                x.di.Story,
                x.ud.DiscoveredAt
            ))
            .ToListAsync(ct);

        var res = new BestiaryResponse(discovery);
        return TypedResults.Ok(res);
    }
}


