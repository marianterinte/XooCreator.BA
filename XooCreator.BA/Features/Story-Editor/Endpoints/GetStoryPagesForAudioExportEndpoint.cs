using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.Stories.Services;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GetStoryPagesForAudioExportEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IStoriesService _storiesService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetStoryPagesForAudioExportEndpoint> _logger;

    public GetStoryPagesForAudioExportEndpoint(
        IStoryCraftsRepository crafts,
        IStoriesService storiesService,
        IAuth0UserService auth0,
        ILogger<GetStoryPagesForAudioExportEndpoint> logger)
    {
        _crafts = crafts;
        _storiesService = storiesService;
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

        // Use the same endpoint flow as the story editor: GetStoryForEditAsync returns tiles with
        // Text, Question, Caption (and Answers) exactly as the editor sees them.
        var normalizedLocale = (locale ?? string.Empty).Trim().ToLowerInvariant();
        var editable = await ep._storiesService.GetStoryForEditAsync(storyId, normalizedLocale);
        if (editable == null)
        {
            return TypedResults.NotFound();
        }

        // We need tile Guid for export job; editor DTO only has tile Id (string). Load craft to map Id -> Guid.
        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null)
        {
            return TypedResults.NotFound();
        }

        var pageOrQuizTypes = new[] { "page", "quiz", "dialog" };
        var tileById = craft.Tiles
            .Where(t => pageOrQuizTypes.Contains(t.Type, StringComparer.OrdinalIgnoreCase))
            .OrderBy(t => t.SortOrder)
            .ToDictionary(t => t.TileId, t => t, StringComparer.OrdinalIgnoreCase);

        var pageNumber = 0;
        var pages = new List<PageInfo>();
        foreach (var tile in editable.Tiles)
        {
            if (!pageOrQuizTypes.Contains(tile.Type ?? "page", StringComparer.OrdinalIgnoreCase))
                continue;
            if (!tileById.TryGetValue(tile.Id ?? string.Empty, out var craftTile))
                continue;

            pageNumber++;
            var text = ResolveDisplayText(tile.Text, tile.Question, tile.Caption);
            pages.Add(new PageInfo
            {
                TileId = craftTile.Id,
                PageNumber = pageNumber,
                Text = text,
                SortOrder = craftTile.SortOrder
            });
        }

        return TypedResults.Ok(new StoryPagesResponse
        {
            Pages = pages
        });
    }

    private static string ResolveDisplayText(string? text, string? question, string? caption)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            return text.Trim();
        }

        if (!string.IsNullOrWhiteSpace(question))
        {
            return question.Trim();
        }

        if (!string.IsNullOrWhiteSpace(caption))
        {
            return caption.Trim();
        }

        return string.Empty;
    }
}
