using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.CreatureBuilder.Endpoints;

[Endpoint]
public sealed class DiscoverEndpoint
{
    private readonly XooDbContext _db;
    private readonly IUserContextService _userContext;

    public DiscoverEndpoint(XooDbContext db, IUserContextService userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    [Route("/api/{locale}/creature-builder/discover")] // POST
    public static async Task<Results<Ok<DiscoverResponseDto>, UnauthorizedHttpResult, BadRequest<DiscoverResponseDto>>> HandlePost(
        [FromRoute] string locale,
        [FromServices] DiscoverEndpoint ep,
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
        var item = await ep._db.BestiaryItems.FirstOrDefaultAsync(d =>
            d.ArmsKey == arms && d.BodyKey == body && d.HeadKey == head, ct);

        if (item == null)
        {
            return TypedResults.BadRequest(new DiscoverResponseDto(false, false, null, "Combination not found", null));
        }

        // 2) Check if already discovered
        var existing = await ep._db.UserBestiary
            .AnyAsync(ud => ud.UserId == userId.Value && ud.BestiaryItemId == item.Id, ct);
        bool alreadyDiscovered = existing;
        if (!alreadyDiscovered)
        {
            // Spend one discovery credit only when this combination is first discovered
            var wallet = await ep._db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId.Value, ct);
            if (wallet == null)
                return TypedResults.BadRequest(new DiscoverResponseDto(false, false, null, "Wallet not found", null));
            if (wallet.DiscoveryBalance <= 0)
                return TypedResults.BadRequest(new DiscoverResponseDto(false, false, null, "Insufficient discovery credits", wallet.DiscoveryBalance));

            wallet.DiscoveryBalance -= 1;
            wallet.UpdatedAt = DateTime.UtcNow;

            ep._db.UserBestiary.Add(new UserBestiary
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
                BestiaryItemId = item.Id,
                BestiaryType = "discovery",
                DiscoveredAt = DateTime.UtcNow
            });
            await ep._db.SaveChangesAsync(ct);
        }

        // Build response item
        // Build image path from combination keys (Name or parts) with .jpg
        string normalize(string s) => s == "—" ? "None" : s;
        string imageUrl = $"{normalize(item.ArmsKey)}{normalize(item.BodyKey)}{normalize(item.HeadKey)}.jpg";

        var responseItem = new DiscoverItemDto(
            item.Id,
            item.Name,
            imageUrl,
            item.Story
        );

        // Get updated wallet balance
        var updatedWallet = await ep._db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId.Value, ct);
        var updatedBalance = updatedWallet?.DiscoveryBalance ?? 0;

        return TypedResults.Ok(new DiscoverResponseDto(true, alreadyDiscovered, responseItem, null, updatedBalance));
    }
}
