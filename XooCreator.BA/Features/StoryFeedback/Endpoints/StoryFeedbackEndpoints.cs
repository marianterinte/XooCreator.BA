using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryFeedback.DTOs;
using XooCreator.BA.Features.StoryFeedback.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryFeedback.Endpoints;

[Endpoint]
public class SubmitStoryFeedbackEndpoint
{
    private readonly IStoryFeedbackService _feedbackService;
    private readonly IAuth0UserService _auth0UserService;

    public SubmitStoryFeedbackEndpoint(IStoryFeedbackService feedbackService, IAuth0UserService auth0UserService)
    {
        _feedbackService = feedbackService;
        _auth0UserService = auth0UserService;
    }

    [Route("/api/{locale}/story-feedback/submit")]
    [Authorize]
    public static async Task<Results<Ok<SubmitStoryFeedbackResponse>, BadRequest<SubmitStoryFeedbackResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromBody] SubmitStoryFeedbackRequest request,
        [FromServices] SubmitStoryFeedbackEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0UserService.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        var response = await ep._feedbackService.SubmitFeedbackAsync(user.Id, user.Email, request, ct);
        
        if (!response.Success)
            return TypedResults.BadRequest(response);

        return TypedResults.Ok(response);
    }
}

[Endpoint]
public class SetFeedbackPreferenceEndpoint
{
    private readonly IStoryFeedbackService _feedbackService;
    private readonly IAuth0UserService _auth0UserService;

    public SetFeedbackPreferenceEndpoint(IStoryFeedbackService feedbackService, IAuth0UserService auth0UserService)
    {
        _feedbackService = feedbackService;
        _auth0UserService = auth0UserService;
    }

    [Route("/api/{locale}/story-feedback/preference")]
    [Authorize]
    public static async Task<Results<Ok<SetFeedbackPreferenceResponse>, BadRequest<SetFeedbackPreferenceResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromBody] SetFeedbackPreferenceRequest request,
        [FromServices] SetFeedbackPreferenceEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0UserService.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        var response = await ep._feedbackService.SetPreferenceAsync(user.Id, request, ct);
        
        if (!response.Success)
            return TypedResults.BadRequest(response);

        return TypedResults.Ok(response);
    }
}

[Endpoint]
public class CheckFeedbackStatusEndpoint
{
    private readonly IStoryFeedbackService _feedbackService;
    private readonly IAuth0UserService _auth0UserService;

    public CheckFeedbackStatusEndpoint(IStoryFeedbackService feedbackService, IAuth0UserService auth0UserService)
    {
        _feedbackService = feedbackService;
        _auth0UserService = auth0UserService;
    }

    [Route("/api/{locale}/story-feedback/status/{storyId}")]
    [Authorize]
    public static async Task<Results<Ok<CheckFeedbackStatusResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] CheckFeedbackStatusEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0UserService.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        var response = await ep._feedbackService.CheckStatusAsync(user.Id, storyId, ct);
        return TypedResults.Ok(response);
    }
}

