using global::System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.StoryEditor.Services.Content;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class SaveStoryEditEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IStoryEditorService _editorService;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;
    private readonly IStoryIdGenerator _storyIdGenerator;
    private readonly ILogger<SaveStoryEditEndpoint> _logger;

    public SaveStoryEditEndpoint(IStoryCraftsRepository crafts, IStoryEditorService editorService, IUserContextService userContext, IAuth0UserService auth0, IStoryIdGenerator storyIdGenerator, ILogger<SaveStoryEditEndpoint> logger)
    {
        _crafts = crafts;
        _editorService = editorService;
        _userContext = userContext;
        _auth0 = auth0;
        _storyIdGenerator = storyIdGenerator;
        _logger = logger;
    }

    public record SaveResponse 
    { 
        public bool Success { get; init; } = true;
        public string? StoryId { get; init; }
    }

    [Route("/api/stories/{storyId}/edit")]
    [Authorize]
    public static async Task<Results<Ok<SaveResponse>, BadRequest<string>, Conflict<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandlePut(
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

        // Generate storyId if "new" or empty
        string finalStoryId = storyId?.Trim() ?? string.Empty;
        bool isNewStory = string.IsNullOrWhiteSpace(finalStoryId) || finalStoryId.Equals("new", StringComparison.OrdinalIgnoreCase);
        
        if (isNewStory)
        {
            // Generate storyId using firstname/lastname + timestamp
            finalStoryId = await ep._storyIdGenerator.GenerateNextAsync(user.Id, user.FirstName, user.LastName, ct);
            ep._logger.LogInformation("Generated storyId: {StoryId} for userId={UserId}", finalStoryId, user.Id);
        }

        // Deserialize JSON to EditableStoryDto
        EditableStoryDto? dto;
        try
        {
            dto = JsonSerializer.Deserialize<EditableStoryDto>(body.RootElement.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (dto == null)
            {
                return TypedResults.BadRequest("Invalid story data");
            }
            dto.Id = finalStoryId; // Ensure ID matches
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "Failed to deserialize story data");
            return TypedResults.BadRequest("Invalid JSON format");
        }

        // Language must be provided in payload (agnostic of UI language)
        if (string.IsNullOrWhiteSpace(dto.Language))
        {
            return TypedResults.BadRequest("Language is required in request body.");
        }

        var langTag = dto.Language.ToLowerInvariant();

        var craft = await ep._crafts.GetAsync(finalStoryId, ct);
        
        // Check if craft exists and verify ownership/status
        if (craft != null)
        {
            // Only owner can edit (unless Admin)
            if (craft.OwnerUserId != user.Id && !ep._auth0.HasRole(user, UserRole.Admin))
            {
                ep._logger.LogWarning("Save forbidden: userId={UserId} storyId={StoryId} not owner", user.Id, finalStoryId);
                return TypedResults.Forbid();
            }

            // Disallow edits in SentForApproval/InReview/Approved/Published (Admin override optional, but keeping workflow strict for now unless critical)
            // Actually, Admin might need to edit even in other states, but let's keep it safe. 
            // If Admin acts as user, they should follow workflow. Admin specific override can be added if requested.
            // For now, assume Admin follows same state rules for "Edit".
            var status = (craft.Status ?? "draft").ToLowerInvariant();
            if (status is "sent_for_approval" or "in_review" or "approved" or "published")
            {
                ep._logger.LogWarning("Save read-only: storyId={StoryId} status={Status}", finalStoryId, status);
                return TypedResults.Conflict("Story is read-only in the current status.");
            }
        }

        // Save using the new structure
        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        await ep._editorService.SaveDraftAsync(user.Id, finalStoryId, langTag, dto, isAdmin, ct);
        ep._logger.LogInformation("Save story draft: storyId={StoryId} lang={Lang}", finalStoryId, langTag);

        return TypedResults.Ok(new SaveResponse { StoryId = finalStoryId });
    }
}


