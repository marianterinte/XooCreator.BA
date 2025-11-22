using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Mappers;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryPublishingService
{
    // Returns the new global version after publish
    Task<int> UpsertFromCraftAsync(StoryCraft craft, string ownerEmail, string langTag, CancellationToken ct);
}

public class StoryPublishingService : IStoryPublishingService
{
    private readonly XooDbContext _db;
    private readonly ILogger<StoryPublishingService> _logger;

    public StoryPublishingService(XooDbContext db, ILogger<StoryPublishingService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<int> UpsertFromCraftAsync(StoryCraft craft, string ownerEmail, string langTag, CancellationToken ct)
    {
        if (craft == null) throw new ArgumentNullException(nameof(craft));
        var storyId = craft.StoryId;

        var def = await _db.StoryDefinitions
            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(d => d.Tiles).ThenInclude(t => t.Translations)
            .Include(d => d.Translations)
            .Include(d => d.Topics)
            .Include(d => d.AgeGroups)
            .FirstOrDefaultAsync(d => d.StoryId == storyId, ct);
        
        // Load craft with topics and age groups
        craft = await _db.StoryCrafts
            .Include(c => c.Topics).ThenInclude(t => t.StoryTopic)
            .Include(c => c.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .FirstOrDefaultAsync(c => c.Id == craft.Id, ct) ?? craft;

        var isNew = def == null;
        if (isNew)
        {
            def = new StoryDefinition
            {
                StoryId = storyId,
                Title = craft.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Title ?? storyId,
                StoryTopic = craft.StoryTopic,
                AuthorName = craft.AuthorName,
                ClassicAuthorId = craft.ClassicAuthorId,
                StoryType = craft.StoryType,
                Status = StoryStatus.Published,
                IsActive = true,
                SortOrder = 0,
                Version = 1,
                PriceInCredits = craft.PriceInCredits,
                CreatedBy = craft.OwnerUserId,
                UpdatedBy = craft.OwnerUserId
            };
            _db.StoryDefinitions.Add(def);
        }

        // Update header
        def.Title = craft.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Title
            ?? def.Title;
        def.Summary = craft.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Summary ?? def.Summary;
        def.StoryTopic = craft.StoryTopic ?? def.StoryTopic;
        def.AuthorName = craft.AuthorName ?? def.AuthorName;
        def.ClassicAuthorId = craft.ClassicAuthorId ?? def.ClassicAuthorId;
        def.StoryType = craft.StoryType;
        def.PriceInCredits = craft.PriceInCredits;
        def.Status = StoryStatus.Published;
        def.IsActive = true;
        def.UpdatedAt = DateTime.UtcNow;
        def.UpdatedBy = craft.OwnerUserId;
        if (!def.CreatedBy.HasValue)
        {
            def.CreatedBy = craft.OwnerUserId;
        }
        // Version bump for existing definitions
        if (!isNew)
        {
            def.Version = def.Version <= 0 ? 1 : def.Version + 1;
        }

        if (!string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            var asset = new StoryAssetPathMapper.AssetInfo(craft.CoverImageUrl, StoryAssetPathMapper.AssetType.Image, null);
            def.CoverImageUrl = StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, storyId);
        }
        if (def.Id != Guid.Empty)
        {
            var existingTiles = await _db.StoryTiles.Where(t => t.StoryDefinitionId == def.Id).ToListAsync(ct);
            if (existingTiles.Count > 0)
            {
                _db.StoryTiles.RemoveRange(existingTiles);
            }

            var existingDefTr = await _db.StoryDefinitionTranslations.Where(t => t.StoryDefinitionId == def.Id).ToListAsync(ct);
            if (existingDefTr.Count > 0)
            {
                _db.StoryDefinitionTranslations.RemoveRange(existingDefTr);
            }

            // Remove existing topics and age groups
            var existingTopics = await _db.StoryDefinitionTopics.Where(t => t.StoryDefinitionId == def.Id).ToListAsync(ct);
            if (existingTopics.Count > 0)
            {
                _db.StoryDefinitionTopics.RemoveRange(existingTopics);
            }

            var existingAgeGroups = await _db.StoryDefinitionAgeGroups.Where(ag => ag.StoryDefinitionId == def.Id).ToListAsync(ct);
            if (existingAgeGroups.Count > 0)
            {
                _db.StoryDefinitionAgeGroups.RemoveRange(existingAgeGroups);
            }
        }

        // Add definition translations from craft
        foreach (var tr in craft.Translations)
        {
            _db.StoryDefinitionTranslations.Add(new StoryDefinitionTranslation
            {
                StoryDefinition = def,
                LanguageCode = tr.LanguageCode.ToLowerInvariant(),
                Title = tr.Title ?? string.Empty
            });
        }

        // Tiles
        var tilesBySort = craft.Tiles.OrderBy(t => t.SortOrder).ToList();
        _logger.LogInformation("Publishing story: storyId={StoryId} tilesCount={Count}", storyId, tilesBySort.Count);
        
        foreach (var ctile in tilesBySort)
        {
            _logger.LogInformation("Processing tile: tileId={TileId} imageUrl={ImageUrl}", 
                ctile.TileId, ctile.ImageUrl ?? "(null)");
            
            var tile = new StoryTile
            {
                StoryDefinition = def,
                TileId = ctile.TileId,
                Type = ctile.Type,
                SortOrder = ctile.SortOrder,
                Caption = null,
                Text = null,
                Question = null,
                // Image is common for all languages
                ImageUrl = ctile.ImageUrl
                // Audio and Video are now language-specific (stored in translation)
            };

            // Build published path for image (common for all languages)
            if (!string.IsNullOrWhiteSpace(ctile.ImageUrl))
            {
                var asset = new StoryAssetPathMapper.AssetInfo(ctile.ImageUrl, StoryAssetPathMapper.AssetType.Image, null);
                tile.ImageUrl = StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, storyId);
            }

            _db.StoryTiles.Add(tile);

            // Translations (including audio/video per language)
            foreach (var tr in ctile.Translations)
            {
                var translationLang = tr.LanguageCode.ToLowerInvariant();
                string? publishedAudioUrl = null;
                string? publishedVideoUrl = null;

                // Build published paths for audio (language-specific)
                if (!string.IsNullOrWhiteSpace(tr.AudioUrl))
                {
                    var audioAsset = new StoryAssetPathMapper.AssetInfo(tr.AudioUrl, StoryAssetPathMapper.AssetType.Audio, translationLang);
                    publishedAudioUrl = StoryAssetPathMapper.BuildPublishedPath(audioAsset, ownerEmail, storyId);
                }

                // Build published paths for video (language-specific)
                if (!string.IsNullOrWhiteSpace(tr.VideoUrl))
                {
                    var videoAsset = new StoryAssetPathMapper.AssetInfo(tr.VideoUrl, StoryAssetPathMapper.AssetType.Video, translationLang);
                    publishedVideoUrl = StoryAssetPathMapper.BuildPublishedPath(videoAsset, ownerEmail, storyId);
                }

                _db.StoryTileTranslations.Add(new StoryTileTranslation
                {
                    StoryTile = tile,
                    LanguageCode = translationLang,
                    Caption = tr.Caption,
                    Text = tr.Text,
                    Question = tr.Question,
                    // Audio and Video are language-specific (full published path)
                    AudioUrl = publishedAudioUrl,
                    VideoUrl = publishedVideoUrl
                });
            }

            // Answers
            var answers = ctile.Answers.OrderBy(a => a.SortOrder).ToList();
            foreach (var ca in answers)
            {
                var ans = new StoryAnswer
                {
                    StoryTile = tile,
                    AnswerId = ca.AnswerId,
                    Text = ca.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Text ?? string.Empty,
                    SortOrder = ca.SortOrder
                };
                _db.StoryAnswers.Add(ans);

                foreach (var tok in ca.Tokens)
                {
                    _db.StoryAnswerTokens.Add(new StoryAnswerToken
                    {
                        StoryAnswer = ans,
                        Type = tok.Type,
                        Value = tok.Value,
                        Quantity = tok.Quantity
                    });
                }

                foreach (var atr in ca.Translations)
                {
                    _db.StoryAnswerTranslations.Add(new StoryAnswerTranslation
                    {
                        StoryAnswer = ans,
                        LanguageCode = atr.LanguageCode.ToLowerInvariant(),
                        Text = atr.Text ?? string.Empty
                    });
                }
            }
        }

        // Copy topics from craft to definition
        foreach (var craftTopic in craft.Topics)
        {
            _db.StoryDefinitionTopics.Add(new StoryDefinitionTopic
            {
                StoryDefinitionId = def.Id,
                StoryTopicId = craftTopic.StoryTopicId,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Copy age groups from craft to definition
        foreach (var craftAgeGroup in craft.AgeGroups)
        {
            _db.StoryDefinitionAgeGroups.Add(new StoryDefinitionAgeGroup
            {
                StoryDefinitionId = def.Id,
                StoryAgeGroupId = craftAgeGroup.StoryAgeGroupId,
                CreatedAt = DateTime.UtcNow
            });
        }

        await UpsertUserCreatedStoryAsync(def, craft.OwnerUserId, ct);
        LogPublication(def, craft.OwnerUserId, ownerEmail, StoryPublicationAction.Publish, null);
        await _db.SaveChangesAsync(ct);
        return def.Version;
    }

    private async Task UpsertUserCreatedStoryAsync(StoryDefinition def, Guid ownerUserId, CancellationToken ct)
    {
        var userStory = await _db.UserCreatedStories
            .FirstOrDefaultAsync(x => x.StoryDefinitionId == def.Id, ct);

        if (userStory == null)
        {
            userStory = new UserCreatedStories
            {
                Id = Guid.NewGuid(),
                StoryDefinitionId = def.Id,
                UserId = ownerUserId,
                CreatedAt = DateTime.UtcNow
            };
            _db.UserCreatedStories.Add(userStory);
        }

        userStory.IsPublished = true;
        userStory.PublishedAt = DateTime.UtcNow;
    }

    private void LogPublication(
        StoryDefinition def,
        Guid performedByUserId,
        string performedByEmail,
        StoryPublicationAction action,
        string? notes)
    {
        _db.StoryPublicationAudits.Add(new StoryPublicationAudit
        {
            Id = Guid.NewGuid(),
            StoryDefinitionId = def.Id,
            StoryId = def.StoryId,
            PerformedByUserId = performedByUserId,
            PerformedByEmail = performedByEmail,
            Action = action,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        });
    }
}


