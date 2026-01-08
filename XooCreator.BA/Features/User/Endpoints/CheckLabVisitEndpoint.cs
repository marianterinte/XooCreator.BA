
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Features.Stories.Repositories;

namespace XooCreator.BA.Features.User.Endpoints;

// Define a DTO for the response
public class FirstVisitDto
{
    public bool IsFirstVisit { get; set; }
}

[Endpoint]
public class CheckLabVisitEndpoint
{
    [Route("/api/{locale}/user/check-lab-visit")] // GET
    [Authorize]
    public static async Task<IResult> HandleGet(
        [FromRoute] string locale,
        [FromServices] IUserContextService userContext,
        [FromServices] XooDbContext dbContext,
        [FromServices] IStoriesRepository storiesRepository,
        CancellationToken ct)
    {
        var userId = await userContext.GetUserIdAsync();
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        var user = await dbContext.AlchimaliaUsers.FindAsync(userId.Value);
        if (user == null)
        {
            return Results.NotFound("User not found.");
        }

        // Check if user has completed the loi-intro story
        const string introStoryId = "loi-intro";
        var completionInfo = await storiesRepository.GetStoryCompletionStatusAsync(userId.Value, introStoryId);
        var hasCompletedIntroStory = completionInfo.IsCompleted;

        // If user has completed the intro story, it's not a first visit
        // Otherwise, check the legacy flag
        var isFirstVisit = !hasCompletedIntroStory && !user.HasVisitedImaginationLaboratory;

        return Results.Ok(new FirstVisitDto { IsFirstVisit = isFirstVisit });
    }
}
