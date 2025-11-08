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
public class ReviewStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;

    public ReviewStoryEndpoint(IStoryCraftsRepository crafts, IUserContextService userContext, IAuth0UserService auth0)
    {
        _crafts = crafts;
        _userContext = userContext;
        _auth0 = auth0;
    }

    public record ReviewRequest
    {
        public bool Approve { get; init; }
        public string? Notes { get; init; }
    }

    public record ReviewResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "Approved";
    }

    [Route("/api/{locale}/stories/{storyId}/review")]
    [Authorize]
    public static async Task<Results<Ok<ReviewResponse>, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] ReviewStoryEndpoint ep,
        [FromBody] ReviewRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (user.Role != Data.Enums.UserRole.Reviewer)
        {
            return TypedResults.Forbid();
        }

        var langTag = ep._userContext.GetRequestLocaleOrDefault("ro-ro");
        var lang = LanguageCodeExtensions.FromTag(langTag);

        var craft = await ep._crafts.GetAsync(storyId, lang, ct);
        if (craft == null) return TypedResults.NotFound();

        var current = (craft.Status ?? "draft").ToLowerInvariant();
        if (current != "in_review")
        {
            return TypedResults.BadRequest("Invalid state transition. Expected InReview.");
        }

        var newStatus = req.Approve ? "approved" : "changes_requested";
        await ep._crafts.UpsertAsync(craft.OwnerUserId, storyId, lang, newStatus, string.Empty, ct);
        return TypedResults.Ok(new ReviewResponse { Status = req.Approve ? "Approved" : "ChangesRequested" });
    }
}

