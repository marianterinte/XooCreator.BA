using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.User.DTOs;
using XooCreator.BA.Features.User.Services;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.User.Endpoints;

[Endpoint]
public class SendNewsletterEndpoint
{
    private readonly INewsletterSendingService _newsletterSendingService;
    private readonly IAuth0UserService _auth0UserService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public SendNewsletterEndpoint(
        INewsletterSendingService newsletterSendingService,
        IAuth0UserService auth0UserService,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _newsletterSendingService = newsletterSendingService;
        _auth0UserService = auth0UserService;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    [Route("/api/{locale}/user/newsletter/send")]
    [Authorize]
    public static async Task<Results<Ok<SendNewsletterResponse>, UnauthorizedHttpResult, BadRequest<SendNewsletterResponse>>> HandlePost(
        [FromRoute] string locale,
        [FromServices] SendNewsletterEndpoint ep,
        [FromServices] IAuth0UserService auth0UserService,
        [FromBody] SendNewsletterRequest request,
        CancellationToken ct)
    {
        // Check if user is admin
        var currentUser = await auth0UserService.GetCurrentUserAsync(ct);
        if (currentUser == null)
            return TypedResults.Unauthorized();

        if (!auth0UserService.HasAnyRole(currentUser, UserRole.Admin))
        {
            return TypedResults.BadRequest(new SendNewsletterResponse
            {
                Success = false,
                ErrorMessage = "Only administrators can send newsletters",
                TotalRecipients = 0,
                SentCount = 0,
                FailedCount = 0
            });
        }

        // If stories/epics are provided, generate template; otherwise use provided content
        string htmlContent = request.HtmlContent;
        string? textContent = request.TextContent;

        if ((request.SelectedStoryIds != null && request.SelectedStoryIds.Count > 0) ||
            (request.SelectedEpicIds != null && request.SelectedEpicIds.Count > 0))
        {
            using var scope = ep._serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
            var templateService = scope.ServiceProvider.GetRequiredService<INewsletterTemplateService>();
            var frontendUrl = ep._configuration["Frontend:BaseUrl"] ?? "https://alchimalia.com";
            var unsubscribeUrl = $"{frontendUrl}/user-menu?unsubscribe=true";

            var stories = new List<NewStoryItem>();
            var epics = new List<NewEpicItem>();

            if (request.SelectedStoryIds != null && request.SelectedStoryIds.Count > 0)
            {
                var storyDefs = await db.StoryDefinitions
                    .Include(s => s.Translations)
                    .Where(s => request.SelectedStoryIds.Contains(s.StoryId) && s.IsActive && s.Status == StoryStatus.Published)
                    .ToListAsync(ct);

                foreach (var story in storyDefs)
                {
                    var translation = story.Translations.FirstOrDefault(t => t.LanguageCode == locale.ToLowerInvariant())
                        ?? story.Translations.FirstOrDefault();
                    
                    stories.Add(new NewStoryItem
                    {
                        Title = translation?.Title ?? story.Title,
                        Description = story.Summary,
                        Url = $"{frontendUrl}/tales-of-alchimalia/story-details/{story.StoryId}",
                        CoverImageUrl = story.CoverImageUrl
                    });
                }
            }

            if (request.SelectedEpicIds != null && request.SelectedEpicIds.Count > 0)
            {
                var epicDefs = await db.StoryEpicDefinitions
                    .Include(e => e.Translations)
                    .Where(e => request.SelectedEpicIds.Contains(e.Id) && e.IsActive && e.Status == "published")
                    .ToListAsync(ct);

                foreach (var epic in epicDefs)
                {
                    var translation = epic.Translations.FirstOrDefault(t => t.LanguageCode == locale.ToLowerInvariant())
                        ?? epic.Translations.FirstOrDefault();
                    
                    epics.Add(new NewEpicItem
                    {
                        Name = translation?.Name ?? epic.Name,
                        Description = translation?.Description ?? epic.Description,
                        Url = $"{frontendUrl}/tales-of-alchimalia/epic-details/{epic.Id}",
                        CoverImageUrl = epic.CoverImageUrl
                    });
                }
            }

            htmlContent = templateService.GenerateNewsletterHtml(stories, epics, locale);
            textContent = templateService.GenerateNewsletterText(stories, epics, locale);

            // Replace unsubscribe URL placeholder
            htmlContent = htmlContent.Replace("{unsubscribe_url}", unsubscribeUrl);
            textContent = textContent?.Replace("{unsubscribe_url}", unsubscribeUrl);
        }

        var result = await ep._newsletterSendingService.SendNewsletterToSubscribersAsync(
            request.Subject,
            htmlContent,
            textContent,
            ct);

        var response = new SendNewsletterResponse
        {
            Success = result.Success,
            ErrorMessage = result.ErrorMessage,
            TotalRecipients = result.TotalRecipients,
            SentCount = result.SentCount,
            FailedCount = result.FailedCount
        };

        return result.Success
            ? TypedResults.Ok(response)
            : TypedResults.BadRequest(response);
    }
}

[Endpoint]
public class GetSubscribedUsersEndpoint
{
    private readonly INewsletterSendingService _newsletterSendingService;
    private readonly IAuth0UserService _auth0UserService;

    public GetSubscribedUsersEndpoint(
        INewsletterSendingService newsletterSendingService,
        IAuth0UserService auth0UserService)
    {
        _newsletterSendingService = newsletterSendingService;
        _auth0UserService = auth0UserService;
    }

    [Route("/api/{locale}/user/newsletter/subscribers")]
    [Authorize]
    public static async Task<Results<Ok<GetSubscribedUsersResponse>, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetSubscribedUsersEndpoint ep,
        [FromServices] IAuth0UserService auth0UserService,
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

        var emails = await ep._newsletterSendingService.GetSubscribedUserEmailsAsync(ct);

        return TypedResults.Ok(new GetSubscribedUsersResponse
        {
            Success = true,
            Emails = emails,
            Count = emails.Count
        });
    }
}
