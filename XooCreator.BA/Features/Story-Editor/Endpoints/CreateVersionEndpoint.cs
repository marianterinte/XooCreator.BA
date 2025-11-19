using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class CreateVersionEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly IStoryCopyService _storyCopyService;
    private readonly IStoryAssetCopyService _storyAssetCopyService;
    private readonly ILogger<CreateVersionEndpoint> _logger;

    public CreateVersionEndpoint(
        IStoryCraftsRepository crafts,
        IAuth0UserService auth0,
        XooDbContext db,
        IStoryCopyService storyCopyService,
        IStoryAssetCopyService storyAssetCopyService,
        ILogger<CreateVersionEndpoint> logger)
    {
        _crafts = crafts;
        _auth0 = auth0;
        _db = db;
        _storyCopyService = storyCopyService;
        _storyAssetCopyService = storyAssetCopyService;
        _logger = logger;
    }

    public record CreateVersionResponse
    {
        public bool Ok { get; init; } = true;
        public required string StoryId { get; init; }
        public int BaseVersion { get; init; }
    }

    [Route("/api/stories/{storyId}/create-version")]
    [Authorize]
    public static async Task<
        Results<
            Ok<CreateVersionResponse>,
            BadRequest<string>,
            NotFound,
            UnauthorizedHttpResult,
            ForbidHttpResult,
            Conflict<string>>> HandlePost(
        [FromRoute] string storyId,
        [FromServices] CreateVersionEndpoint ep,
        CancellationToken ct)
    {
        var (user, outcome) = await ep.AuthorizeCreatorAsync(ct);
        if (outcome == AuthorizationOutcome.Unauthorized) return TypedResults.Unauthorized();
        if (outcome == AuthorizationOutcome.Forbidden) return TypedResults.Forbid();
        var currentUser = user!;

        if (!ep.IsValidStoryId(storyId))
        {
            return TypedResults.BadRequest("storyId is required and cannot be 'new'");
        }

        var (definition, validationResult) = await ep.LoadPublishedStoryAsync(storyId, ct);
        if (validationResult != null) return validationResult;

        if (!ep.HasOwnership(currentUser, definition!))
        {
            return TypedResults.Forbid();
        }

        var existingDraftResult = await ep.CheckExistingDraftAsync(storyId, ct);
        if (existingDraftResult != null) return existingDraftResult;

        var newCraft = await ep._storyCopyService.CreateCopyFromDefinitionAsync(definition!, currentUser.Id, storyId, ct);

        await ep.TryCopyAssetsFromPublishedAsync(currentUser.Email, definition!, storyId, ct);

        ep._logger.LogInformation(
            "Create version completed: userId={UserId} storyId={StoryId} baseVersion={BaseVersion}",
            currentUser.Id,
            storyId,
            definition!.Version);

        return TypedResults.Ok(new CreateVersionResponse
        {
            StoryId = storyId,
            BaseVersion = definition!.Version
        });
    }

    private enum AuthorizationOutcome
    {
        Ok,
        Unauthorized,
        Forbidden
    }

    private async Task<(AlchimaliaUser? User, AuthorizationOutcome Outcome)> AuthorizeCreatorAsync(CancellationToken ct)
    {
        var user = await _auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return (null, AuthorizationOutcome.Unauthorized);
        }

        if (!_auth0.HasRole(user, UserRole.Creator) && !_auth0.HasRole(user, UserRole.Admin))
        {
            _logger.LogWarning("Create version forbidden: userId={UserId}", user.Id);
            return (user, AuthorizationOutcome.Forbidden);
        }

        return (user, AuthorizationOutcome.Ok);
    }

    private bool IsValidStoryId(string storyId)
    {
        return !(string.IsNullOrWhiteSpace(storyId) || storyId.Equals("new", StringComparison.OrdinalIgnoreCase));
    }

    private async Task<(StoryDefinition? Definition, Results<Ok<CreateVersionResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>? Error)> LoadPublishedStoryAsync(string storyId, CancellationToken ct)
    {
        var def = await _db.StoryDefinitions
            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(d => d.Tiles).ThenInclude(t => t.Translations)
            .Include(d => d.Translations)
            .Include(d => d.Topics).ThenInclude(t => t.StoryTopic)
            .Include(d => d.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .FirstOrDefaultAsync(d => d.StoryId == storyId, ct);

        if (def == null)
        {
            _logger.LogWarning("Create version failed. Story not found: {StoryId}", storyId);
            return (null, TypedResults.NotFound());
        }

        if (def.Status != StoryStatus.Published)
        {
            return (null, TypedResults.BadRequest($"Story is not published (status: {def.Status})"));
        }

        return (def, null);
    }

    private bool HasOwnership(AlchimaliaUser user, StoryDefinition definition)
    {
        if (_auth0.HasRole(user, UserRole.Admin))
        {
            return true;
        }

        return definition.CreatedBy == user.Id;
    }

    private async Task<Results<Ok<CreateVersionResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>?> CheckExistingDraftAsync(string storyId, CancellationToken ct)
    {
        var existingCraft = await _crafts.GetAsync(storyId, ct);
        if (existingCraft != null && existingCraft.Status != StoryStatus.Published.ToDb())
        {
            return TypedResults.Conflict("A draft already exists for this story. Please edit or publish it first.");
        }

        return null;
    }

    private async Task TryCopyAssetsFromPublishedAsync(string ownerEmail, StoryDefinition definition, string storyId, CancellationToken ct)
    {
        try
        {
            var assets = _storyAssetCopyService.CollectFromDefinition(definition);
            await _storyAssetCopyService.CopyPublishedToDraftAsync(
                assets,
                ownerEmail,
                definition.StoryId,
                ownerEmail,
                storyId,
                ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to copy assets during create version for storyId={StoryId}", storyId);
        }
    }
}

