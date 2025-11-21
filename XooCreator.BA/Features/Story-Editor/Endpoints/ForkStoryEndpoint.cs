using System.Linq;
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
public class ForkStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly IStoryIdGenerator _storyIdGenerator;
    private readonly IStoryCopyService _storyCopyService;
    private readonly IStoryAssetCopyService _storyAssetCopyService;
    private readonly ILogger<ForkStoryEndpoint> _logger;

    public ForkStoryEndpoint(
        IStoryCraftsRepository crafts,
        IAuth0UserService auth0,
        XooDbContext db,
        IStoryIdGenerator storyIdGenerator,
        IStoryCopyService storyCopyService,
        IStoryAssetCopyService storyAssetCopyService,
        ILogger<ForkStoryEndpoint> logger)
    {
        _crafts = crafts;
        _auth0 = auth0;
        _db = db;
        _storyIdGenerator = storyIdGenerator;
        _storyCopyService = storyCopyService;
        _storyAssetCopyService = storyAssetCopyService;
        _logger = logger;
    }

    public record ForkStoryRequest
    {
        public bool CopyAssets { get; init; } = true;
    }

    public record ForkStoryResponse
    {
        public required string StoryId { get; init; }
        public required string OriginalStoryId { get; init; }
    }

    [Route("/api/stories/{storyId}/fork")]
    [Authorize]
    public static async Task<
        Results<
            Ok<ForkStoryResponse>,
            BadRequest<string>,
            NotFound,
            UnauthorizedHttpResult,
            ForbidHttpResult,
            Conflict<string>>> HandlePost(
        [FromRoute] string storyId,
        [FromBody] ForkStoryRequest request,
        [FromServices] ForkStoryEndpoint ep,
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

        var (craft, definition) = await ep.LoadSourceStoryAsync(storyId, ct);
        if (craft == null && definition == null)
        {
            return TypedResults.NotFound();
        }

        var newStoryId = await ep._storyIdGenerator.GenerateNextAsync(currentUser.Id, currentUser.FirstName, currentUser.LastName, ct);
        StoryCraft newCraft;

        if (craft != null)
        {
            newCraft = await ep._storyCopyService.CreateCopyFromCraftAsync(craft, currentUser.Id, newStoryId, ct);
        }
        else
        {
            newCraft = await ep._storyCopyService.CreateCopyFromDefinitionAsync(definition!, currentUser.Id, newStoryId, ct);
        }

        ep._logger.LogInformation(
            "Fork story completed: userId={UserId} source={SourceStoryId} newStoryId={NewStoryId} copyAssets={CopyAssets}",
            currentUser.Id,
            storyId,
            newStoryId,
            request.CopyAssets);

        if (request.CopyAssets)
        {
            await ep.TryCopyAssetsAsync(currentUser, craft, definition, newStoryId, ct);
        }

        return TypedResults.Ok(new ForkStoryResponse
        {
            StoryId = newStoryId,
            OriginalStoryId = storyId
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
            _logger.LogWarning("Fork story forbidden: userId={UserId}", user.Id);
            return (user, AuthorizationOutcome.Forbidden);
        }

        return (user, AuthorizationOutcome.Ok);
    }

    private bool IsValidStoryId(string storyId)
    {
        return !(string.IsNullOrWhiteSpace(storyId) || storyId.Equals("new", StringComparison.OrdinalIgnoreCase));
    }

    private async Task<(StoryCraft? Craft, StoryDefinition? Definition)> LoadSourceStoryAsync(string storyId, CancellationToken ct)
    {
        var craft = await _crafts.GetAsync(storyId, ct);
        if (craft != null)
        {
            return (craft, null);
        }

        var definition = await _db.StoryDefinitions
            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(d => d.Tiles).ThenInclude(t => t.Translations)
            .Include(d => d.Translations)
            .Include(d => d.Topics).ThenInclude(t => t.StoryTopic)
            .Include(d => d.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .FirstOrDefaultAsync(d => d.StoryId == storyId, ct);

        return (null, definition);
    }

    private async Task TryCopyAssetsAsync(AlchimaliaUser currentUser, StoryCraft? craft, StoryDefinition? definition, string newStoryId, CancellationToken ct)
    {
        try
        {
            if (craft != null)
            {
                var sourceEmail = await ResolveOwnerEmailAsync(craft.OwnerUserId, currentUser, ct);
                if (sourceEmail == null)
                {
                    _logger.LogWarning("Skipping fork asset copy for storyId={StoryId}: cannot resolve source email", craft.StoryId);
                    return;
                }

                var assets = _storyAssetCopyService.CollectFromCraft(craft);
                await _storyAssetCopyService.CopyDraftToDraftAsync(
                    assets,
                    sourceEmail,
                    craft.StoryId,
                    currentUser.Email,
                    newStoryId,
                    ct);
                return;
            }

            if (definition != null)
            {
                if (!definition.CreatedBy.HasValue)
                {
                    _logger.LogWarning("Skipping fork asset copy for published storyId={StoryId}: CreatedBy missing", definition.StoryId);
                    return;
                }

                var sourceEmail = await ResolveOwnerEmailAsync(definition.CreatedBy.Value, currentUser, ct);
                if (string.IsNullOrWhiteSpace(sourceEmail))
                {
                    _logger.LogWarning("Skipping fork asset copy for published storyId={StoryId}: cannot resolve owner email", definition.StoryId);
                    return;
                }

                var assets = _storyAssetCopyService.CollectFromDefinition(definition);
                await _storyAssetCopyService.CopyPublishedToDraftAsync(
                    assets,
                    sourceEmail,
                    definition.StoryId,
                    currentUser.Email,
                    newStoryId,
                    ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Asset copy failed during fork for storyId={StoryId}", newStoryId);
        }
    }

    private async Task<string?> ResolveOwnerEmailAsync(Guid ownerId, AlchimaliaUser currentUser, CancellationToken ct)
    {
        if (ownerId == currentUser.Id)
        {
            return currentUser.Email;
        }

        return await _db.AlchimaliaUsers
            .AsNoTracking()
            .Where(u => u.Id == ownerId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync(ct);
    }
}

