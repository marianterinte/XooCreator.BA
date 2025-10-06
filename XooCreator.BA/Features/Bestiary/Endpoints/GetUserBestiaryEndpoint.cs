using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

    [Route("/api/{locale}/bestiary")] // GET
    [Authorize]
    public static async Task<Results<Ok<BestiaryResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromQuery] string? bestiaryType,
        [FromServices] GetUserBestiaryEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var query = ep._db.UserBestiary
            .Where(ub => ub.UserId == userId.Value);

        // Filter by bestiary type if provided
        if (!string.IsNullOrEmpty(bestiaryType))
        {
            query = query.Where(ub => ub.BestiaryType == bestiaryType);
        }

        var items = await query
            .Join(ep._db.BestiaryItems, ub => ub.BestiaryItemId, bi => bi.Id, (ub, bi) => new { ub, bi })
            .OrderByDescending(x => x.ub.DiscoveredAt)
            .Select(x => new BestiaryItemDto(
                x.bi.Id,
                x.bi.Name,
                x.ub.BestiaryType == "treeofheroes" 
                    ? x.bi.ArmsKey + ".jpg"  // For heroes, use ArmsKey (which contains HeroId) + .jpg
                    : (x.bi.ArmsKey == "—" ? "None" : x.bi.ArmsKey) + (x.bi.BodyKey == "—" ? "None" : x.bi.BodyKey) + (x.bi.HeadKey == "—" ? "None" : x.bi.HeadKey) + ".jpg",
                x.bi.Story,
                x.ub.DiscoveredAt,
                x.ub.BestiaryType
            ))
            .ToListAsync(ct);

        var res = new BestiaryResponse(items);
        return TypedResults.Ok(res);
    }
}


