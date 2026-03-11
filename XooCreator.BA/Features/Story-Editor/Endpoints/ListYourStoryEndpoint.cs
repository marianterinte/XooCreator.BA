using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

/// <summary>
/// Lists private stories (your-story) for the current user and returns full-story generation credits balance.
/// </summary>
[Endpoint]
public class ListYourStoryEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;

    public ListYourStoryEndpoint(XooDbContext db, IAuth0UserService auth0)
    {
        _db = db;
        _auth0 = auth0;
    }

    [Route("/api/{locale}/your-story")]
    [Authorize]
    public static async Task<Results<Ok<YourStoryListResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] ListYourStoryEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        var wallet = await ep._db.CreditWallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.UserId == user.Id, ct);
        var fullStoryCredits = wallet?.FullStoryGenerationBalance ?? 0;

        var normalizedLocale = (locale ?? "ro-ro").Trim().ToLowerInvariant();
        var defs = await ep._db.StoryDefinitions
            .AsNoTracking()
            .Include(s => s.Translations)
            .Where(s => s.IsPrivate && s.CreatedBy == user.Id && s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

        var stories = defs.Select(def =>
        {
            var translation = def.Translations?.FirstOrDefault(t => string.Equals(t.LanguageCode, normalizedLocale, StringComparison.OrdinalIgnoreCase))
                ?? def.Translations?.FirstOrDefault();
            var title = translation?.Title ?? def.Title ?? def.StoryId;
            return new YourStoryItemDto
            {
                StoryId = def.StoryId,
                Title = title,
                CoverImageUrl = def.CoverImageUrl,
                Summary = def.Summary,
                CreatedAt = def.CreatedAt
            };
        }).ToList();

        return TypedResults.Ok(new YourStoryListResponse
        {
            Stories = stories,
            FullStoryGenerationCredits = fullStoryCredits
        });
    }
}

public sealed class YourStoryListResponse
{
    public List<YourStoryItemDto> Stories { get; init; } = new();
    public double FullStoryGenerationCredits { get; init; }
}

public sealed class YourStoryItemDto
{
    public string StoryId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? CoverImageUrl { get; init; }
    public string? Summary { get; init; }
    public DateTime CreatedAt { get; init; }
}
