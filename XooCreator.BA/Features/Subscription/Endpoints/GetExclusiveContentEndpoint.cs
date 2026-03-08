using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.Subscription.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.Subscription.Endpoints;

[Endpoint]
public class GetExclusiveContentEndpoint
{
    private readonly IExclusiveContentService _exclusiveContent;
    private readonly IAuth0UserService _auth0;

    public GetExclusiveContentEndpoint(IExclusiveContentService exclusiveContent, IAuth0UserService auth0)
    {
        _exclusiveContent = exclusiveContent;
        _auth0 = auth0;
    }

    public record ExclusiveContentResponse(IReadOnlyList<string> StoryIds, IReadOnlyList<string> EpicIds);

    [Route("/api/{locale}/user/exclusive-content")]
    [Authorize]
    public static async Task<Results<Ok<ExclusiveContentResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetExclusiveContentEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        var (storyIds, epicIds) = await ep._exclusiveContent.GetUserExclusiveContentAsync(user.Id, ct);
        return TypedResults.Ok(new ExclusiveContentResponse(storyIds, epicIds));
    }
}
