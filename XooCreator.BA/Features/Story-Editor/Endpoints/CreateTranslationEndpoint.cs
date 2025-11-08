using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class CreateTranslationEndpoint
{
    private readonly IStoryEditorService _editorService;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CreateTranslationEndpoint> _logger;

    public CreateTranslationEndpoint(IStoryEditorService editorService, IUserContextService userContext, IAuth0UserService auth0, ILogger<CreateTranslationEndpoint> logger)
    {
        _editorService = editorService;
        _userContext = userContext;
        _auth0 = auth0;
        _logger = logger;
    }

    public record CreateTranslationRequest
    {
        public required string Lang { get; init; }
    }

    public record CreateTranslationResponse
    {
        public bool Ok { get; init; } = true;
        public required string StoryId { get; init; }
        public required string Lang { get; init; }
    }

    [Route("/api/{locale}/stories/{storyId}/translations")]
    [Authorize]
    public static async Task<Results<Ok<CreateTranslationResponse>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] CreateTranslationEndpoint ep,
        [FromBody] CreateTranslationRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (user.Role != Data.Enums.UserRole.Creator)
        {
            return TypedResults.Forbid();
        }

        if (string.IsNullOrWhiteSpace(req.Lang))
        {
            return TypedResults.BadRequest("lang is required");
        }

        var lang = LanguageCodeExtensions.FromTag(req.Lang);
        await ep._editorService.EnsureDraftAsync(user.Id, storyId, lang, ct);
        ep._logger.LogInformation("Create translation draft: userId={UserId} storyId={StoryId} lang={Lang}", user.Id, storyId, req.Lang.ToLowerInvariant());
        return TypedResults.Ok(new CreateTranslationResponse { StoryId = storyId, Lang = req.Lang.ToLowerInvariant() });
    }
}

