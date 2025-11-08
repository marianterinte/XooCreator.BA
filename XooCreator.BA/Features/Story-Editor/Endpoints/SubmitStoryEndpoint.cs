using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class SubmitStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IStoryEditorService _editorService;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<SubmitStoryEndpoint> _logger;

    public SubmitStoryEndpoint(IStoryCraftsRepository crafts, IStoryEditorService editorService, IUserContextService userContext, IAuth0UserService auth0, ILogger<SubmitStoryEndpoint> logger)
    {
        _crafts = crafts;
        _editorService = editorService;
        _userContext = userContext;
        _auth0 = auth0;
        _logger = logger;
    }

    public record SubmitResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "SentForApproval";
    }

    [Route("/api/{locale}/stories/{storyId}/submit")]
    [Authorize]
    public static async Task<Results<Ok<SubmitResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] SubmitStoryEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, Data.Enums.UserRole.Creator))
        {
            ep._logger.LogWarning("Submit forbidden: userId={UserId} roles={Roles}", user?.Id, string.Join(",", user?.Roles ?? new List<UserRole> { user?.Role ?? UserRole.Reader }));
            return TypedResults.Forbid();
        }

        var langTag = ep._userContext.GetRequestLocaleOrDefault("ro-ro");
        var lang = LanguageCodeExtensions.FromTag(langTag);

        var craft = await ep._crafts.GetAsync(storyId, lang, ct);
        if (craft == null) return TypedResults.NotFound();

        if (craft.OwnerUserId != user.Id)
        {
            ep._logger.LogWarning("Submit not owner: userId={UserId} storyId={StoryId}", user.Id, storyId);
            return TypedResults.Forbid();
        }

        var current = StoryStatusExtensions.FromDb(craft.Status);
        if (current != StoryStatus.Draft && current != StoryStatus.ChangesRequested)
        {
            ep._logger.LogWarning("Submit invalid state: storyId={StoryId} state={State}", storyId, current);
            return TypedResults.Conflict("Invalid state transition. Expected Draft or ChangesRequested.");
        }

        craft.Status = StoryStatus.SentForApproval.ToDb();
        await ep._crafts.SaveAsync(craft, ct);
        ep._logger.LogInformation("Submit: storyId={StoryId} lang={Lang} from={From} to={To}", storyId, langTag, current, StoryStatus.SentForApproval);
        return TypedResults.Ok(new SubmitResponse());
    }
}

