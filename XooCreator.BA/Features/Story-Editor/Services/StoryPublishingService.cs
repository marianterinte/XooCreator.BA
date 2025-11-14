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
            .FirstOrDefaultAsync(d => d.StoryId == storyId, ct);

        var isNew = def == null;
        if (isNew)
        {
            def = new StoryDefinition
            {
                StoryId = storyId,
                Title = craft.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Title ?? storyId,
                StoryType = craft.StoryType,
                Status = StoryStatus.Published,
                IsActive = true,
                SortOrder = 0,
                Version = 1
            };
            _db.StoryDefinitions.Add(def);
        }

        // Update header
        def.Title = craft.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Title
            ?? def.Title;
        def.Summary = craft.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Summary ?? def.Summary;
        def.StoryType = craft.StoryType;
        def.Status = StoryStatus.Published;
        def.UpdatedAt = DateTime.UtcNow;
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

        await _db.SaveChangesAsync(ct);
        return def.Version;
    }
}


