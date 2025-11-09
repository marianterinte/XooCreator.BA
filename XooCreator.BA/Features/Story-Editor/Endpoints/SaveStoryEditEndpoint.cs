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
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class SaveStoryEditEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IStoryEditorService _editorService;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<SaveStoryEditEndpoint> _logger;

    public SaveStoryEditEndpoint(IStoryCraftsRepository crafts, IStoryEditorService editorService, IUserContextService userContext, IAuth0UserService auth0, ILogger<SaveStoryEditEndpoint> logger)
    {
        _crafts = crafts;
        _editorService = editorService;
        _userContext = userContext;
        _auth0 = auth0;
        _logger = logger;
    }

    public record SaveResponse 
    { 
        public bool Success { get; init; } = true;
        public string? StoryId { get; init; }
    }

    [Route("/api/{locale}/stories/{storyId}/edit")]
    [Authorize]
    public static async Task<Results<Ok<SaveResponse>, BadRequest<string>, Conflict<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandlePut(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] SaveStoryEditEndpoint ep,
        [FromBody] JsonDocument body,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, Data.Enums.UserRole.Creator))
        {
            ep._logger.LogWarning("Save forbidden: userId={UserId} not a creator", user.Id);
            return TypedResults.Forbid();
        }

        var langTag = ep._userContext.GetRequestLocaleOrDefault("ro-ro");
        var lang = LanguageCodeExtensions.FromTag(langTag);

        // Generate storyId if "new" or empty
        string finalStoryId = storyId?.Trim() ?? string.Empty;
        bool isNewStory = string.IsNullOrWhiteSpace(finalStoryId) || finalStoryId.Equals("new", StringComparison.OrdinalIgnoreCase);
        
        if (isNewStory)
        {
            // Generate storyId: email-s1, email-s2, etc.
            var storyCount = await ep._crafts.CountDistinctStoryIdsByOwnerAsync(user.Id, ct);
            finalStoryId = $"{user.Email}-s{storyCount + 1}";
            ep._logger.LogInformation("Generated storyId: {StoryId} for userId={UserId}", finalStoryId, user.Id);
        }

        var craft = await ep._crafts.GetAsync(finalStoryId, lang, ct);
        
        // If craft doesn't exist, create it
        if (craft == null)
        {
            craft = await ep._crafts.CreateAsync(user.Id, finalStoryId, lang, "draft", body.RootElement.GetRawText(), ct);
            ep._logger.LogInformation("Created new story draft: storyId={StoryId} lang={Lang}", finalStoryId, langTag);
        }
        else
        {
            // Only owner can edit
            if (craft.OwnerUserId != user.Id)
            {
                ep._logger.LogWarning("Save forbidden: userId={UserId} storyId={StoryId} not owner", user.Id, finalStoryId);
                return TypedResults.Forbid();
            }

            // Disallow edits in SentForApproval/InReview/Approved/Published
            var status = (craft.Status ?? "draft").ToLowerInvariant();
            if (status is "sent_for_approval" or "in_review" or "approved" or "published")
            {
                ep._logger.LogWarning("Save read-only: storyId={StoryId} status={Status}", finalStoryId, status);
                return TypedResults.Conflict("Story is read-only in the current status.");
            }

            // Persist raw JSON from editor without changing current status
            craft.Json = body.RootElement.GetRawText();
            await ep._crafts.SaveAsync(craft, ct);
            ep._logger.LogInformation("Save story draft: storyId={StoryId} lang={Lang}", finalStoryId, langTag);
        }

        return TypedResults.Ok(new SaveResponse { StoryId = finalStoryId });
    }
}


