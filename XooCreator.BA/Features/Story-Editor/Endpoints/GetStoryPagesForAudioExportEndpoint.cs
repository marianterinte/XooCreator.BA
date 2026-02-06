using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GetStoryPagesForAudioExportEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetStoryPagesForAudioExportEndpoint> _logger;

    public GetStoryPagesForAudioExportEndpoint(
        IStoryCraftsRepository crafts,
        IAuth0UserService auth0,
        ILogger<GetStoryPagesForAudioExportEndpoint> logger)
    {
        _crafts = crafts;
        _auth0 = auth0;
        _logger = logger;
    }

    public record PageInfo
    {
        public required Guid TileId { get; init; }
        public required int PageNumber { get; init; }
        public required string Text { get; init; }
        public required int SortOrder { get; init; }
    }

    public record StoryPagesResponse
    {
        public required List<PageInfo> Pages { get; init; }
    }

    [Route("/api/stories/{storyId}/pages-for-audio-export")]
    [Authorize]
    public static async Task<Results<Ok<StoryPagesResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string storyId,
        [FromQuery] string locale,
        [FromServices] GetStoryPagesForAudioExportEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);

        if (!isAdmin)
        {
            ep._logger.LogWarning("Get pages for audio export forbidden: userId={UserId} storyId={StoryId} not admin", user.Id, storyId);
            return TypedResults.Forbid();
        }

        var craft = await ep._crafts.GetWithLanguageAsync(storyId, locale, ct);
        if (craft == null)
        {
            return TypedResults.NotFound();
        }

        var pages = craft.Tiles
            .Where(t => t.Type.Equals("page", StringComparison.OrdinalIgnoreCase))
            .OrderBy(t => t.SortOrder)
            .Select((tile, index) => new PageInfo
            {
                TileId = tile.Id,
                PageNumber = index + 1,
                Text = ResolveTileText(tile, locale),
                SortOrder = tile.SortOrder
            })
            .ToList();

        return TypedResults.Ok(new StoryPagesResponse
        {
            Pages = pages
        });
    }

    private static string ResolveTileText(StoryCraftTile tile, string locale)
    {
        var lang = (locale ?? string.Empty).Trim().ToLowerInvariant();
        var translation = tile.Translations.FirstOrDefault(tr => tr.LanguageCode == lang)
                          ?? tile.Translations.FirstOrDefault();
        return (translation?.Text ?? translation?.Caption ?? string.Empty).Trim();
    }
}
