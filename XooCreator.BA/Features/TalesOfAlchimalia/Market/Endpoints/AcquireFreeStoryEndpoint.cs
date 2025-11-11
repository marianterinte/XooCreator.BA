using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public class AcquireFreeStoryEndpoint
{
    private readonly XooDbContext _context;
    private readonly IUserContextService _userContext;

    public AcquireFreeStoryEndpoint(XooDbContext context, IUserContextService userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public record GetFreeStoryResponse(bool Success, string? ErrorMessage);
    public record AcquireFreeStoryRequest(string StoryId);

    [Route("/api/{locale}/tales-of-alchimalia/market/acquire-free-story")]
    [Authorize]
    public static async Task<Results<Ok<GetFreeStoryResponse>, BadRequest<GetFreeStoryResponse>, UnauthorizedHttpResult, NotFound>> HandlePost(
        [FromRoute] string locale,
        [FromBody] AcquireFreeStoryRequest? body,
        [FromServices] AcquireFreeStoryEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var finalStoryId = body?.StoryId;
        if (string.IsNullOrWhiteSpace(finalStoryId))
            return TypedResults.BadRequest(new GetFreeStoryResponse(false, "Missing storyId"));

        var def = await ep._context.StoryDefinitions
            .FirstOrDefaultAsync(s => s.StoryId == finalStoryId && s.IsActive);
        if (def == null) return TypedResults.NotFound();

        var price = await GetPriceFromJsonOrDefaultAsync(finalStoryId);
        if (price > 0)
        {
            return TypedResults.BadRequest(new GetFreeStoryResponse(false, "Story is not free"));
        }

        var alreadyOwned = await ep._context.UserOwnedStories
            .AnyAsync(uos => uos.UserId == userId && uos.StoryDefinitionId == def.Id);
        if (alreadyOwned)
        {
            return TypedResults.Ok(new GetFreeStoryResponse(true, null));
        }

        ep._context.UserOwnedStories.Add(new UserOwnedStories
        {
            Id = Guid.NewGuid(),
            UserId = userId.Value,
            StoryDefinitionId = def.Id,
            PurchasedAt = DateTime.UtcNow,
            PurchasePrice = 0,
            PurchaseReference = "FREE_GET"
        });
        await ep._context.SaveChangesAsync();

        return TypedResults.Ok(new GetFreeStoryResponse(true, null));
    }

    private static async Task<int> GetPriceFromJsonOrDefaultAsync(string storyId)
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var locales = new[] { "ro-ro", "en-us", "hu-hu" };
        foreach (var locale in locales)
        {
            var dir = Path.Combine(baseDir, "Data", "SeedData", "Stories", "seed@alchimalia.com", "independent", "i18n", locale);
            var filePath = Path.Combine(dir, $"{storyId}.json");
            if (File.Exists(filePath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(filePath);
                    var seed = JsonSerializer.Deserialize<StorySeedDataProbe>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });
                    if (seed?.Price.HasValue == true) return seed.Price.Value;
                }
                catch { }
            }
        }
        
        return 0;
    }

    private sealed class StorySeedDataProbe
    {
        public int? Price { get; set; }
    }
}


