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
        bool alreadyDiscovered = discovered.Count > 0;
        int variant;
        if (alreadyDiscovered)
        {
            // Reuse the last discovered variant (or 1 as fallback)
            variant = discovered.Max();
        }
        else
        {
            // Spend one discovery credit only when this combination is first discovered
            var wallet = await ep._db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId.Value, ct);
            if (wallet == null)
                return TypedResults.BadRequest(new DiscoverResponseDto(false, false, null, "Wallet not found", null));
            if (wallet.DiscoveryBalance <= 0)
                return TypedResults.BadRequest(new DiscoverResponseDto(false, false, null, "Insufficient discovery credits", wallet.DiscoveryBalance));

            wallet.DiscoveryBalance -= 1;
            wallet.UpdatedAt = DateTime.UtcNow;

            // First time discovery: assign variant 1
            variant = 1;

            ep._db.UserDiscoveries.Add(new UserDiscovery
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
                DiscoveryItemId = item.Id,
                VariantIndex = variant,
                DiscoveredAt = DateTime.UtcNow
            });
            await ep._db.SaveChangesAsync(ct);
        }

        // Build response item
        // Build image path from combination keys (Name or parts) with .jpg
        string normalize(string s) => s == "—" ? "None" : s;
        string imageUrl = $"{normalize(item.ArmsKey)}{normalize(item.BodyKey)}{normalize(item.HeadKey)}.jpg";
        var walletNow = await ep._db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId.Value, ct);

        var res = new DiscoverResponseDto(
            Success: true,
            AlreadyDiscovered: alreadyDiscovered,
            Item: new DiscoverItemDto(item.Id, item.Name, imageUrl, item.Story, variant),
            ErrorMessage: null,
            DiscoveryCredits: walletNow?.DiscoveryBalance
        );
        return TypedResults.Ok(res);
    }
}
