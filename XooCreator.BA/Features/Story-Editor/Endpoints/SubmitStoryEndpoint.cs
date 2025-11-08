using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class SubmitStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IStoryEditorService _editorService;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;

    public SubmitStoryEndpoint(IStoryCraftsRepository crafts, IStoryEditorService editorService, IUserContextService userContext, IAuth0UserService auth0)
    {
        _crafts = crafts;
        _editorService = editorService;
        _userContext = userContext;
        _auth0 = auth0;
    }

    public record SubmitResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "SentForApproval";
    }

    [Route("/api/{locale}/stories/{storyId}/submit")]
    [Authorize]
    public static async Task<Results<Ok<SubmitResponse>, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] SubmitStoryEndpoint ep,
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
            return TypedResults.BadRequest("Only the owner can submit the story for approval.");
        }

        var current = (craft.Status ?? "draft").ToLowerInvariant();
        if (current != "draft" && current != "changes_requested")
        {
            return TypedResults.BadRequest("Invalid state transition. Expected Draft or ChangesRequested.");
        }

        await ep._crafts.UpsertAsync(user.Id, storyId, lang, "sent_for_approval", string.Empty, ct);
        return TypedResults.Ok(new SubmitResponse());
    }
}

