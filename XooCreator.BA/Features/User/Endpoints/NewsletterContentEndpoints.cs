using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.User.DTOs;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.User.Endpoints;

[Endpoint]
public class GetNewsletterStoriesEndpoint
{
    private readonly IAuth0UserService _auth0UserService;

    public GetNewsletterStoriesEndpoint(IAuth0UserService auth0UserService)
    {
        _auth0UserService = auth0UserService;
    }

    [Route("/api/{locale}/user/newsletter/stories")]
    [Authorize]
    public static async Task<Results<Ok<GetNewsletterStoriesResponse>, UnauthorizedHttpResult, ForbiddenHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetNewsletterStoriesEndpoint ep,
        [FromServices] IAuth0UserService auth0UserService,
        [FromServices] XooDbContext db,
        CancellationToken ct)
    {
        // Check if user is admin
        var currentUser = await auth0UserService.GetCurrentUserAsync(ct);
        if (currentUser == null)
            return TypedResults.Unauthorized();

        if (!auth0UserService.HasAnyRole(currentUser, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var normalizedLocale = locale.ToLowerInvariant();

        var stories = await db.StoryDefinitions
            .Include(s => s.Translations)
            .Where(s => s.IsActive && s.Status == StoryStatus.Published && !s.IsPartOfEpic)
            .OrderBy(s => s.Title)
            .Select(s => new NewsletterStoryItem
            {
                StoryId = s.StoryId,
                Title = s.Translations
                    .FirstOrDefault(t => t.LanguageCode == normalizedLocale) != null
                    ? s.Translations.First(t => t.LanguageCode == normalizedLocale).Title
                    : s.Title,
                Summary = s.Summary,
                CoverImageUrl = s.CoverImageUrl
            })
            .ToListAsync(ct);

        return TypedResults.Ok(new GetNewsletterStoriesResponse
        {
            Success = true,
            Stories = stories
        });
    }
}

[Endpoint]
public class GetNewsletterEpicsEndpoint
{
    private readonly IAuth0UserService _auth0UserService;

    public GetNewsletterEpicsEndpoint(IAuth0UserService auth0UserService)
    {
        _auth0UserService = auth0UserService;
    }

    [Route("/api/{locale}/user/newsletter/epics")]
    [Authorize]
    public static async Task<Results<Ok<GetNewsletterEpicsResponse>, UnauthorizedHttpResult, ForbiddenHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetNewsletterEpicsEndpoint ep,
        [FromServices] IAuth0UserService auth0UserService,
        [FromServices] XooDbContext db,
        CancellationToken ct)
    {
        // Check if user is admin
        var currentUser = await auth0UserService.GetCurrentUserAsync(ct);
        if (currentUser == null)
            return TypedResults.Unauthorized();

        if (!auth0UserService.HasAnyRole(currentUser, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var normalizedLocale = locale.ToLowerInvariant();

        var epics = await db.StoryEpicDefinitions
            .Include(e => e.Translations)
            .Where(e => e.IsActive && e.Status == "published")
            .OrderBy(e => e.Name)
            .Select(e => new NewsletterEpicItem
            {
                EpicId = e.Id,
                Name = e.Translations
                    .FirstOrDefault(t => t.LanguageCode == normalizedLocale) != null
                    ? e.Translations.First(t => t.LanguageCode == normalizedLocale).Name
                    : e.Name,
                Description = e.Translations
                    .FirstOrDefault(t => t.LanguageCode == normalizedLocale) != null
                    ? e.Translations.First(t => t.LanguageCode == normalizedLocale).Description
                    : e.Description,
                CoverImageUrl = e.CoverImageUrl
            })
            .ToListAsync(ct);

        return TypedResults.Ok(new GetNewsletterEpicsResponse
        {
            Success = true,
            Epics = epics
        });
    }
}
