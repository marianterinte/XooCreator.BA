using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.Services;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

public record CreateStoryRequest
{
    public string? StoryId { get; init; }
    public string? Language { get; init; } // Standardized: use "language" instead of "lang"
    public string? Title { get; init; } // reserved for future
    public int? StoryType { get; init; } // reserved for future
}

public record CreateStoryResponse
{
    public required string StoryId { get; init; }
}

[Endpoint]
public class CreateStoryEndpoint
{
    private readonly IStoryEditorService _editorService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CreateStoryEndpoint> _logger;
    private readonly IStoryIdGenerator _storyIdGenerator;

    public CreateStoryEndpoint(
        IStoryEditorService editorService,
        IAuth0UserService auth0,
        IStoryIdGenerator storyIdGenerator,
        ILogger<CreateStoryEndpoint> logger)
    {
        _editorService = editorService;
        _auth0 = auth0;
        _storyIdGenerator = storyIdGenerator;
        _logger = logger;
    }

    [Route("/api/stories")]
    [Authorize]
    public static async Task<Results<Ok<CreateStoryResponse>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromServices] CreateStoryEndpoint ep,
        [FromBody] CreateStoryRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Creator-only guard
        if (!ep._auth0.HasRole(user, Data.Enums.UserRole.Creator))
        {
            ep._logger.LogWarning("CreateStory forbidden: userId={UserId} roles={Roles}", user?.Id, string.Join(",", user?.Roles ?? new List<UserRole> { user?.Role ?? UserRole.Reader }));
            return TypedResults.Forbid();
        }

        // Language must be provided in payload (agnostic of UI language)
        if (string.IsNullOrWhiteSpace(req.Language))
        {
            return TypedResults.BadRequest("Language is required in request body.");
        }

        var langTag = req.Language!.ToLowerInvariant();
        
        // Generate storyId if not provided
        string storyId = (req.StoryId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(storyId))
        {
            storyId = await ep._storyIdGenerator.GenerateNextAsync(user.Id, user.Email, ct);
            ep._logger.LogInformation("Generated storyId: {StoryId} for userId={UserId}", storyId, user.Id);
        }

        // Ensure draft exists and create translation for the requested language
        // Use req.StoryType if provided, otherwise default to Indie (1)
        var storyType = req.StoryType.HasValue ? (StoryType)req.StoryType.Value : StoryType.Indie;
        await ep._editorService.EnsureDraftAsync(user.Id, storyId, storyType, ct);
        await ep._editorService.EnsureTranslationAsync(user.Id, storyId, langTag, req.Title, ct);
        ep._logger.LogInformation("CreateStory: userId={UserId} storyId={StoryId} lang={Lang} title={Title}", user.Id, storyId, langTag, req.Title ?? "(empty)");
        return TypedResults.Ok(new CreateStoryResponse { StoryId = storyId });
    }
}


