using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.HeroStoryRewards.DTOs;
using XooCreator.BA.Features.HeroStoryRewards.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.HeroStoryRewards.Endpoints;

[Endpoint]
public class CompleteStoryRewardEndpoint
{
    private readonly IHeroStoryRewardsService _rewardService;
    private readonly IAuth0UserService _auth0;

    public CompleteStoryRewardEndpoint(
        IHeroStoryRewardsService rewardService,
        IAuth0UserService auth0)
    {
        _rewardService = rewardService;
        _auth0 = auth0;
    }

    [Route("/api/story-rewards/complete")]
    [Authorize]
    public static async Task<Results<Ok<CompleteStoryRewardResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromBody] CompleteStoryRewardRequest? request,
        [FromServices] CompleteStoryRewardEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (request == null || string.IsNullOrWhiteSpace(request.StoryId))
        {
            return TypedResults.Ok(new CompleteStoryRewardResponse
            {
                Completed = true,
                TokensAwarded = false,
                Warnings = new List<string> { "Missing storyId." }
            });
        }

        var response = await ep._rewardService.AwardStoryRewardsAsync(user.Id, request, ct);
        return TypedResults.Ok(response);
    }
}
