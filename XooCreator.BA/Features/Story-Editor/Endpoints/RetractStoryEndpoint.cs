using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class RetractStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;

    public RetractStoryEndpoint(IStoryCraftsRepository crafts, IUserContextService userContext, IAuth0UserService auth0)
    {
        _crafts = crafts;
        _userContext = userContext;
        _auth0 = auth0;
    }

    public record RetractResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "Draft";
    }

    [Route("/api/{locale}/stories/{storyId}/retract")]
    [Authorize]
    public static async Task<Results<Ok<RetractResponse>, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] RetractStoryEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (user.Role != Data.Enums.UserRole.Creator)
        {
            return TypedResults.Forbid();
        }

        var langTag = ep._userContext.GetRequestLocaleOrDefault("ro-ro");
        var lang = LanguageCodeExtensions.FromTag(langTag);

        var craft = await ep._crafts.GetAsync(storyId, lang, ct);
        if (craft == null) return TypedResults.NotFound();

        if (craft.OwnerUserId != user.Id)
        {
            return TypedResults.BadRequest("Only the owner can retract this story.");
        }

        var current = (craft.Status ?? "draft").ToLowerInvariant();
        if (current != "sent_for_approval" && current != "in_review")
        {
            return TypedResults.BadRequest("Invalid state transition. Expected SentForApproval or InReview.");
        }

        await ep._crafts.UpsertAsync(craft.OwnerUserId, storyId, lang, "draft", string.Empty, ct);
        return TypedResults.Ok(new RetractResponse());
    }
}

