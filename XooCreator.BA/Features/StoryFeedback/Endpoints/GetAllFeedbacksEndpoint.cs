using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryFeedback.DTOs;
using XooCreator.BA.Features.StoryFeedback.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryFeedback.Endpoints;

[Endpoint]
public class GetAllFeedbacksEndpoint
{
    private readonly IStoryFeedbackService _feedbackService;
    private readonly IAuth0UserService _auth0UserService;

    public GetAllFeedbacksEndpoint(IStoryFeedbackService feedbackService, IAuth0UserService auth0UserService)
    {
        _feedbackService = feedbackService;
        _auth0UserService = auth0UserService;
    }

    [Route("/api/{locale}/story-feedback/all")]
    [Authorize]
    public static async Task<Results<Ok<GetAllFeedbacksResponse>, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetAllFeedbacksEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0UserService.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        // Only Admin can access all feedbacks
        if (!ep._auth0UserService.HasRole(user, UserRole.Admin))
            return TypedResults.Forbid();

        var response = await ep._feedbackService.GetAllFeedbacksAsync(ct);
        return TypedResults.Ok(response);
    }
}

