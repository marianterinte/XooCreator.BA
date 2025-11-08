using global::System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class SaveStoryEditEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IStoryEditorService _editorService;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;

    public SaveStoryEditEndpoint(IStoryCraftsRepository crafts, IStoryEditorService editorService, IUserContextService userContext, IAuth0UserService auth0)
    {
        _crafts = crafts;
        _editorService = editorService;
        _userContext = userContext;
        _auth0 = auth0;
    }

    public record SaveResponse { public bool Success { get; init; } = true; }

    [Route("/api/{locale}/stories/{storyId}/edit")]
    [Authorize]
    public static async Task<Results<Ok<SaveResponse>, BadRequest<string>, UnauthorizedHttpResult>> HandlePut(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] SaveStoryEditEndpoint ep,
        [FromBody] JsonDocument body,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (string.IsNullOrWhiteSpace(storyId))
        {
            return TypedResults.BadRequest("Missing storyId.");
        }

        var langTag = ep._userContext.GetRequestLocaleOrDefault("ro-ro");
        var lang = LanguageCodeExtensions.FromTag(langTag);

        // Persist raw JSON from editor; status stays draft unless changed elsewhere
        var json = body.RootElement.GetRawText();
        await ep._editorService.SaveDraftJsonAsync(user.Id, storyId, lang, json, ct);
        return TypedResults.Ok(new SaveResponse());
    }
}


