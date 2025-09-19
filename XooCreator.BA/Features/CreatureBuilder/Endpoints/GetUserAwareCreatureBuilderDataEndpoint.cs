using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Services;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.CreatureBuilder.Endpoints;

[Endpoint]
public class GetUserAwareCreatureBuilderDataEndpoint
{
    private readonly ICreatureBuilderService _service;
    private readonly IUserContextService _userContext;
    private readonly XooDbContext _db;

    public GetUserAwareCreatureBuilderDataEndpoint(ICreatureBuilderService service, IUserContextService userContext, XooDbContext db)
    {
        _service = service;
        _userContext = userContext;
        _db = db;
    }

    [Route("/api/creature-builder/user-data")] // GET /api/creature-builder/user-data
    public static async Task<Results<Ok<UserAwareCreatureBuilderDataDto>, UnauthorizedHttpResult>> HandleGet(
        [FromServices] GetUserAwareCreatureBuilderDataEndpoint ep, 
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) 
            return TypedResults.Unauthorized();

        var data = await ep._service.GetUserAwareDataAsync(userId.Value, ct);
        return TypedResults.Ok(data);
    }

    [Route("/api/creature-builder/discover")] // POST
    public static async Task<Results<Ok<DiscoverResponseDto>, UnauthorizedHttpResult, BadRequest<DiscoverResponseDto>>> HandlePost(
        [FromServices] GetUserAwareCreatureBuilderDataEndpoint ep,
        [FromBody] DiscoverRequestDto request,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        // Normalize input (trim, keep case as provided in seed)
        var arms = request.Combination.Arms?.Trim() ?? string.Empty;
        var body = request.Combination.Body?.Trim() ?? string.Empty;
        var head = request.Combination.Head?.Trim() ?? string.Empty;

        // 1) Find matching discovery item
        var item = await ep._db.DiscoveryItems.FirstOrDefaultAsync(d =>
            d.ArmsKey == arms && d.BodyKey == body && d.HeadKey == head, ct);

        if (item == null)
        {
            return TypedResults.BadRequest(new DiscoverResponseDto(false, false, null, "Combination not found", null));
        }

        // 2) Which variants user already discovered for this item
        var discovered = await ep._db.UserDiscoveries
            .Where(ud => ud.UserId == userId.Value && ud.DiscoveryItemId == item.Id)
            .Select(ud => ud.VariantIndex)
            .ToListAsync(ct);

        // 3) Pick the lowest missing variant from 1..3
        int variant = new[] { 1, 2, 3 }.FirstOrDefault(v => !discovered.Contains(v));
        if (variant == 0)
        {
            // All discovered for this combination
            var walletLeft = await ep._db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId.Value, ct);
            return TypedResults.Ok(new DiscoverResponseDto(
                Success: true,
                AllDiscoveredForCombination: true,
                Item: null,
                ErrorMessage: null,
                DiscoveryCredits: walletLeft?.DiscoveryBalance
            ));
        }

        // 4) Record discovery
        ep._db.UserDiscoveries.Add(new UserDiscovery
        {
            Id = Guid.NewGuid(),
            UserId = userId.Value,
            DiscoveryItemId = item.Id,
            VariantIndex = variant,
            DiscoveredAt = DateTime.UtcNow
        });
        await ep._db.SaveChangesAsync(ct);

        // 5) Build response item
        string? imageUrl = variant switch
        {
            1 => item.ImageV1,
            2 => item.ImageV2,
            3 => item.ImageV3,
            _ => null
        };
        var wallet = await ep._db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId.Value, ct);

        var res = new DiscoverResponseDto(
            Success: true,
            AllDiscoveredForCombination: false,
            Item: new DiscoverItemDto(item.Id, item.Name, imageUrl, variant),
            ErrorMessage: null,
            DiscoveryCredits: wallet?.DiscoveryBalance
        );
        return TypedResults.Ok(res);
    }
}
