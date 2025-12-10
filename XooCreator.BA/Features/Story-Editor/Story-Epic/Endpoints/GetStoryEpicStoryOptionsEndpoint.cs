using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class GetStoryEpicStoryOptionsEndpoint
{
    private readonly XooDbContext _context;
    private readonly IUserContextService _userContext;

    public GetStoryEpicStoryOptionsEndpoint(XooDbContext context, IUserContextService userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    /// <summary>
    /// Returns a lightweight list of published stories created by the current user,
    /// filtered by optional search query. Used by Story Epic editor when attaching
    /// stories to regions.
    /// </summary>
    /// <param name="locale">Request locale (e.g., ro-ro)</param>
    /// <param name="query">Optional search term (matches title or storyId)</param>
    [Route("/api/{locale}/story-editor/epics/story-options")]
    [Authorize]
    public static async Task<Results<Ok<GetStoryEpicStoryOptionsResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromQuery] string? query,
        [FromServices] GetStoryEpicStoryOptionsEndpoint ep,
        CancellationToken ct)
    {
        var userId = ep._userContext.GetCurrentUserId();
        if (userId == null)
        {
            return TypedResults.Unauthorized();
        }

        var normalizedLocale = (locale ?? "ro-ro").ToLowerInvariant();

        // Base query: published stories created by current user
        var storiesQuery = ep._context.UserCreatedStories
            .Include(ucs => ucs.StoryDefinition)
                .ThenInclude(sd => sd.Translations)
            .Where(ucs =>
                ucs.UserId == userId &&
                ucs.IsPublished &&
                ucs.StoryDefinition.IsActive);

        // Apply optional search filter on title or StoryId (case-insensitive)
        if (!string.IsNullOrWhiteSpace(query))
        {
            var searchTerm = query.Trim();

            storiesQuery = storiesQuery.Where(ucs =>
                // Match StoryId
                EF.Functions.ILike(ucs.StoryDefinition.StoryId, $"%{searchTerm}%") ||
                // Match base Title
                (ucs.StoryDefinition.Title != null &&
                 EF.Functions.ILike(ucs.StoryDefinition.Title, $"%{searchTerm}%")) ||
                // Match translated Title
                (ucs.StoryDefinition.Translations != null &&
                 ucs.StoryDefinition.Translations.Any(t =>
                     t.LanguageCode == normalizedLocale &&
                     t.Title != null &&
                     EF.Functions.ILike(t.Title, $"%{searchTerm}%"))));
        }

        // Limit results to avoid huge payloads
        var stories = await storiesQuery
            .OrderByDescending(ucs => ucs.PublishedAt ?? ucs.CreatedAt)
            .Take(50)
            .Select(ucs => new StoryEpicStoryOptionDto
            {
                StoryId = ucs.StoryDefinition.StoryId,
                Title = ucs.StoryDefinition.Translations
                    .Where(t => t.LanguageCode == normalizedLocale && t.Title != null)
                    .Select(t => t.Title!)
                    .FirstOrDefault()
                    ?? ucs.StoryDefinition.Title
                    ?? ucs.StoryDefinition.StoryId,
                CoverImageUrl = ucs.StoryDefinition.CoverImageUrl
            })
            .ToListAsync(ct);

        var response = new GetStoryEpicStoryOptionsResponse
        {
            Stories = stories
        };

        return TypedResults.Ok(response);
    }
}


