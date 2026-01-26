using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.User.DTOs;
using XooCreator.BA.Features.User.Services;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.User.Endpoints;

[Endpoint]
public class UpdateNewsletterSubscriptionEndpoint
{
    private readonly INewsletterSubscriptionService _newsletterService;
    private readonly IUserContextService _userContext;

    public UpdateNewsletterSubscriptionEndpoint(
        INewsletterSubscriptionService newsletterService,
        IUserContextService userContext)
    {
        _newsletterService = newsletterService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/user/newsletter-subscription")]
    [Authorize]
    public static async Task<Results<Ok<UpdateNewsletterSubscriptionResponse>, UnauthorizedHttpResult, BadRequest<UpdateNewsletterSubscriptionResponse>>> HandlePut(
        [FromRoute] string locale,
        [FromServices] UpdateNewsletterSubscriptionEndpoint ep,
        [FromBody] UpdateNewsletterSubscriptionRequest request,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var result = await ep._newsletterService.UpdateSubscriptionAsync(userId.Value, request.IsSubscribed, ct);

        return result.Success
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}

[Endpoint]
public class GetNewsletterSubscriptionEndpoint
{
    private readonly INewsletterSubscriptionService _newsletterService;
    private readonly IUserContextService _userContext;

    public GetNewsletterSubscriptionEndpoint(
        INewsletterSubscriptionService newsletterService,
        IUserContextService userContext)
    {
        _newsletterService = newsletterService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/user/newsletter-subscription")]
    [Authorize]
    public static async Task<Results<Ok<GetNewsletterSubscriptionResponse>, UnauthorizedHttpResult, NotFound>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetNewsletterSubscriptionEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var result = await ep._newsletterService.GetSubscriptionStatusAsync(userId.Value, ct);

        if (!result.Success)
            return TypedResults.NotFound();

        return TypedResults.Ok(result);
    }
}
