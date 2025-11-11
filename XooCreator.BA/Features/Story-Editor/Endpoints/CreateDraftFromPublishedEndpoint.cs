using Microsoft.AspNetCore.Authorization;
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
using Microsoft.Extensions.Logging;
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

    [Route("/api/{locale}/stories/{storyId}/create-draft-from-published")]
    [Authorize]
    public static async Task<Results<Ok<CreateDraftResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] CreateDraftFromPublishedEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Only Creator or Admin can create drafts
        if (!ep._auth0.HasRole(user, Data.Enums.UserRole.Creator) && !ep._auth0.HasRole(user, Data.Enums.UserRole.Admin))
        {
            ep._logger.LogWarning("User {UserId} attempted to create draft without Creator/Admin role", user.Id);
            return TypedResults.Forbid();
        }

        if (string.IsNullOrWhiteSpace(storyId) || storyId.Equals("new", StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("storyId is required and cannot be 'new'");
        }

        // Load published StoryDefinition
        var def = await ep._db.StoryDefinitions
            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(d => d.Tiles).ThenInclude(t => t.Translations)
            .Include(d => d.Translations)
            .FirstOrDefaultAsync(d => d.StoryId == storyId, ct);

        if (def == null)
        {
            ep._logger.LogWarning("Story not found: {StoryId}", storyId);
            return TypedResults.NotFound();
        }

        // Check if story is published
        if (def.Status != StoryStatus.Published)
        {
            return TypedResults.BadRequest($"Story is not published (status: {def.Status})");
        }

        // Check ownership (unless Admin)
        if (!ep._auth0.HasRole(user, Data.Enums.UserRole.Admin))
        {
            if (def.CreatedBy != user.Id)
            {
                ep._logger.LogWarning("User {UserId} attempted to create draft for story owned by {OwnerId}", user.Id, def.CreatedBy);
                return TypedResults.Forbid();
            }
        }

        // Check if draft already exists
        var existingCraft = await ep._crafts.GetAsync(storyId, ct);
        if (existingCraft != null && existingCraft.Status != StoryStatus.Published.ToDb())
        {
            return TypedResults.Conflict("A draft already exists for this story. Please edit the existing draft or publish it first.");
        }

        // Create new StoryCraft from published StoryDefinition
        var craft = new StoryCraft
        {
            StoryId = storyId,
            OwnerUserId = user.Id,
            Status = StoryStatus.Draft.ToDb(),
            StoryType = def.StoryType,
            CoverImageUrl = def.CoverImageUrl,
            BaseVersion = def.Version, // Set base version to current published version
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Copy translations
        foreach (var defTr in def.Translations)
        {
            craft.Translations.Add(new StoryCraftTranslation
            {
                LanguageCode = defTr.LanguageCode,
                Title = defTr.Title,
                Summary = def.Summary // Summary is on StoryDefinition, not translation
            });
        }

        // Copy tiles
        var sortOrder = 0;
        foreach (var defTile in def.Tiles.OrderBy(t => t.SortOrder))
        {
            var craftTile = new StoryCraftTile
            {
                TileId = defTile.TileId,
                Type = defTile.Type,
                ImageUrl = defTile.ImageUrl ?? string.Empty,
                AudioUrl = defTile.AudioUrl ?? string.Empty,
                VideoUrl = defTile.VideoUrl ?? string.Empty,
                SortOrder = sortOrder++
            };

            // Copy tile translations (Caption, Text, Question are in translations, not on tile)
            foreach (var tileTr in defTile.Translations)
            {
                craftTile.Translations.Add(new StoryCraftTileTranslation
                {
                    LanguageCode = tileTr.LanguageCode,
                    Caption = tileTr.Caption ?? string.Empty,
                    Text = tileTr.Text ?? string.Empty,
                    Question = tileTr.Question ?? string.Empty
                });
            }

            // Copy answers
            var answerSortOrder = 0;
            foreach (var defAnswer in defTile.Answers.OrderBy(a => a.SortOrder))
            {
                var craftAnswer = new StoryCraftAnswer
                {
                    AnswerId = defAnswer.AnswerId,
                    SortOrder = answerSortOrder++
                };

                // Copy answer translations (Text is in translations, not on answer)
                foreach (var answerTr in defAnswer.Translations)
                {
                    craftAnswer.Translations.Add(new StoryCraftAnswerTranslation
                    {
                        LanguageCode = answerTr.LanguageCode,
                        Text = answerTr.Text ?? string.Empty
                    });
                }

                // Copy tokens (correct class name is StoryCraftAnswerToken)
                foreach (var token in defAnswer.Tokens)
                {
                    craftAnswer.Tokens.Add(new StoryCraftAnswerToken
                    {
                        Type = token.Type ?? string.Empty,
                        Value = token.Value ?? string.Empty,
                        Quantity = token.Quantity
                    });
                }

                craftTile.Answers.Add(craftAnswer);
            }

            craft.Tiles.Add(craftTile);
        }

        // Save the new craft
        await ep._crafts.SaveAsync(craft, ct);
        ep._logger.LogInformation("Created draft from published version: userId={UserId} storyId={StoryId} baseVersion={BaseVersion}", user.Id, storyId, def.Version);

        return TypedResults.Ok(new CreateDraftResponse
        {
            StoryId = storyId,
            BaseVersion = def.Version
        });
    }
}

