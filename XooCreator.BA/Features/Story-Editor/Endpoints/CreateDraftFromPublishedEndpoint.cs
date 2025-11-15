using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using StoryCraftAnswerToken = XooCreator.BA.Data.Entities.StoryCraftAnswerToken;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class CreateDraftFromPublishedEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly ILogger<CreateDraftFromPublishedEndpoint> _logger;

    public CreateDraftFromPublishedEndpoint(
        IStoryCraftsRepository crafts,
        IUserContextService userContext,
        IAuth0UserService auth0,
        XooDbContext db,
        ILogger<CreateDraftFromPublishedEndpoint> logger)
    {
        _crafts = crafts;
        _userContext = userContext;
        _auth0 = auth0;
        _db = db;
        _logger = logger;
    }

    public record CreateDraftResponse
    {
        public bool Ok { get; init; } = true;
        public required string StoryId { get; init; }
        public int BaseVersion { get; init; }
    }

    [Route("/api/stories/{storyId}/create-draft-from-published")]
    [Authorize]
    public static async Task<Results<Ok<CreateDraftResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>> HandlePost(
        [FromRoute] string storyId,
        [FromServices] CreateDraftFromPublishedEndpoint ep,
        CancellationToken ct)
    {
        var authResult = await ep.ValidateAuthorizationAsync(ct);
        if (authResult.Result != null) return (Results<Ok<CreateDraftResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>)authResult.Result;
        var user = authResult.User!;

        var validationResult = ep.ValidateStoryId(storyId);
        if (validationResult != null) return (Results<Ok<CreateDraftResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>)validationResult;

        var storyResult = await ep.LoadAndValidateStoryAsync(storyId, ct);
        if (storyResult.Result != null) return (Results<Ok<CreateDraftResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>)storyResult.Result;
        var def = storyResult.Story!;

        var ownershipResult = ep.ValidateOwnership(user, def);
        if (ownershipResult != null) return (Results<Ok<CreateDraftResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>)ownershipResult;

        var draftCheckResult = await ep.CheckExistingDraftAsync(storyId, ct);
        if (draftCheckResult != null) return (Results<Ok<CreateDraftResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>)draftCheckResult;

        // Create draft from published story
        var craft = ep.CreateStoryCraftFromDefinition(def, storyId, user.Id);

        // Save the new craft
        await ep._crafts.SaveAsync(craft, ct);
        ep._logger.LogInformation("Created draft from published version: userId={UserId} storyId={StoryId} baseVersion={BaseVersion}", user.Id, storyId, def.Version);

        return TypedResults.Ok(new CreateDraftResponse
        {
            StoryId = storyId,
            BaseVersion = def.Version
        });
    }

    private async Task<(AlchimaliaUser? User, IResult? Result)> ValidateAuthorizationAsync(CancellationToken ct)
    {
        var user = await _auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return (null, TypedResults.Unauthorized());
        }

        if (!_auth0.HasRole(user, Data.Enums.UserRole.Creator) && !_auth0.HasRole(user, Data.Enums.UserRole.Admin))
        {
            _logger.LogWarning("User {UserId} attempted to create draft without Creator/Admin role", user.Id);
            return (null, TypedResults.Forbid());
        }

        return (user, null);
    }

    private IResult? ValidateStoryId(string storyId)
    {
        if (string.IsNullOrWhiteSpace(storyId) || storyId.Equals("new", StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("storyId is required and cannot be 'new'");
        }

        return null;
    }

    private async Task<(StoryDefinition? Story, IResult? Result)> LoadAndValidateStoryAsync(string storyId, CancellationToken ct)
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
            _logger.LogWarning("Story not found: {StoryId}", storyId);
            return (null, TypedResults.NotFound());
        }

        if (def.Status != StoryStatus.Published)
        {
            return (null, TypedResults.BadRequest($"Story is not published (status: {def.Status})"));
        }

        return (def, null);
    }

    private IResult? ValidateOwnership(AlchimaliaUser user, StoryDefinition def)
    {
        if (!_auth0.HasRole(user, Data.Enums.UserRole.Admin))
        {
            if (def.CreatedBy != user.Id)
            {
                _logger.LogWarning("User {UserId} attempted to create draft for story owned by {OwnerId}", user.Id, def.CreatedBy);
                return TypedResults.Forbid();
            }
        }

        return null;
    }

    private async Task<IResult?> CheckExistingDraftAsync(string storyId, CancellationToken ct)
    {
        var existingCraft = await _crafts.GetAsync(storyId, ct);
        if (existingCraft != null && existingCraft.Status != StoryStatus.Published.ToDb())
        {
            return TypedResults.Conflict("A draft already exists for this story. Please edit the existing draft or publish it first.");
        }

        return null;
    }

    private StoryCraft CreateStoryCraftFromDefinition(StoryDefinition def, string storyId, Guid userId)
    {
        var craft = new StoryCraft
        {
            StoryId = storyId,
            OwnerUserId = userId,
            Status = StoryStatus.Draft.ToDb(),
            StoryType = def.StoryType,
            CoverImageUrl = ExtractFileName(def.CoverImageUrl),
            StoryTopic = def.StoryTopic,
            BaseVersion = def.Version,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        CopyTranslations(craft, def);
        CopyTiles(craft, def);
        CopyTopics(craft, def);
        CopyAgeGroups(craft, def);

        return craft;
    }

    private static void CopyTopics(StoryCraft craft, StoryDefinition def)
    {
        foreach (var defTopic in def.Topics)
        {
            craft.Topics.Add(new StoryCraftTopic
            {
                StoryCraftId = craft.Id,
                StoryTopicId = defTopic.StoryTopicId,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private static void CopyAgeGroups(StoryCraft craft, StoryDefinition def)
    {
        foreach (var defAgeGroup in def.AgeGroups)
        {
            craft.AgeGroups.Add(new StoryCraftAgeGroup
            {
                StoryCraftId = craft.Id,
                StoryAgeGroupId = defAgeGroup.StoryAgeGroupId,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private static void CopyTranslations(StoryCraft craft, StoryDefinition def)
    {
        foreach (var defTr in def.Translations)
        {
            craft.Translations.Add(new StoryCraftTranslation
            {
                LanguageCode = defTr.LanguageCode,
                Title = defTr.Title,
                Summary = def.Summary // Summary is on StoryDefinition, not translation
            });
        }
    }

    private static void CopyTiles(StoryCraft craft, StoryDefinition def)
    {
        var sortOrder = 0;
        foreach (var defTile in def.Tiles.OrderBy(t => t.SortOrder))
        {
            var craftTile = CreateCraftTile(defTile, sortOrder++);
            craft.Tiles.Add(craftTile);
        }
    }

    private static StoryCraftTile CreateCraftTile(StoryTile defTile, int sortOrder)
    {
        var craftTile = new StoryCraftTile
        {
            TileId = defTile.TileId,
            Type = defTile.Type,
            // Image is common for all languages
            ImageUrl = ExtractFileName(defTile.ImageUrl),
            // Audio and Video are now language-specific (stored in translation)
            SortOrder = sortOrder
        };

        CopyTileTranslations(craftTile, defTile);
        CopyAnswers(craftTile, defTile);

        return craftTile;
    }

    private static void CopyTileTranslations(StoryCraftTile craftTile, StoryTile defTile)
    {
        foreach (var tileTr in defTile.Translations)
        {
            craftTile.Translations.Add(new StoryCraftTileTranslation
            {
                LanguageCode = tileTr.LanguageCode,
                Caption = tileTr.Caption ?? string.Empty,
                Text = tileTr.Text ?? string.Empty,
                Question = tileTr.Question ?? string.Empty,
                // Audio and Video are language-specific (extract filename from published path)
                AudioUrl = ExtractFileName(tileTr.AudioUrl),
                VideoUrl = ExtractFileName(tileTr.VideoUrl)
            });
        }
    }

    private static void CopyAnswers(StoryCraftTile craftTile, StoryTile defTile)
    {
        var answerSortOrder = 0;
        foreach (var defAnswer in defTile.Answers.OrderBy(a => a.SortOrder))
        {
            var craftAnswer = CreateCraftAnswer(defAnswer, answerSortOrder++);
            craftTile.Answers.Add(craftAnswer);
        }
    }

    private static StoryCraftAnswer CreateCraftAnswer(StoryAnswer defAnswer, int sortOrder)
    {
        var craftAnswer = new StoryCraftAnswer
        {
            AnswerId = defAnswer.AnswerId,
            SortOrder = sortOrder
        };

        CopyAnswerTranslations(craftAnswer, defAnswer);
        CopyTokens(craftAnswer, defAnswer);

        return craftAnswer;
    }

    private static void CopyAnswerTranslations(StoryCraftAnswer craftAnswer, StoryAnswer defAnswer)
    {
        foreach (var answerTr in defAnswer.Translations)
        {
            craftAnswer.Translations.Add(new StoryCraftAnswerTranslation
            {
                LanguageCode = answerTr.LanguageCode,
                Text = answerTr.Text ?? string.Empty
            });
        }
    }

    private static void CopyTokens(StoryCraftAnswer craftAnswer, StoryAnswer defAnswer)
    {
        foreach (var token in defAnswer.Tokens)
        {
            craftAnswer.Tokens.Add(new StoryCraftAnswerToken
            {
                Type = token.Type ?? string.Empty,
                Value = token.Value ?? string.Empty,
                Quantity = token.Quantity
            });
        }
    }

    private static string? ExtractFileName(string? path)
    {
        return string.IsNullOrWhiteSpace(path) ? null : Path.GetFileName(path);
    }
}
