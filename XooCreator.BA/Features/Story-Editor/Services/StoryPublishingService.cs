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
    Task<int> UpsertFromCraftAsync(StoryCraft craft, string ownerEmail, string langTag, bool forceFullPublish, CancellationToken ct);
}

public class StoryPublishingService : IStoryPublishingService
{
    private readonly XooDbContext _db;
    private readonly ILogger<StoryPublishingService> _logger;
    private readonly IStoryAssetLinkService _assetLinks;

    public StoryPublishingService(
        XooDbContext db,
        ILogger<StoryPublishingService> logger,
        IStoryAssetLinkService assetLinks)
    {
        _db = db;
        _logger = logger;
        _assetLinks = assetLinks;
    }

    public async Task<int> UpsertFromCraftAsync(StoryCraft craft, string ownerEmail, string langTag, bool forceFullPublish, CancellationToken ct)
    {
        if (craft == null) throw new ArgumentNullException(nameof(craft));
        var storyId = craft.StoryId;

        var def = await _db.StoryDefinitions
            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Translations)
            .Include(d => d.Tiles).ThenInclude(t => t.Translations)
            .Include(d => d.Tiles).ThenInclude(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.Translations)
            .Include(d => d.Tiles).ThenInclude(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.OutgoingEdges).ThenInclude(e => e.Translations)
            .Include(d => d.Tiles).ThenInclude(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.OutgoingEdges).ThenInclude(e => e.Tokens)
            .Include(d => d.Translations)
            .Include(d => d.Topics)
            .Include(d => d.AgeGroups)
            .Include(d => d.CoAuthors)
            .Include(d => d.DialogParticipants)
            .FirstOrDefaultAsync(d => d.StoryId == storyId, ct);
        
        // Load craft with topics, age groups, and co-authors
        craft = await _db.StoryCrafts
            .Include(c => c.Translations)
            .Include(c => c.Tiles).ThenInclude(t => t.Translations)
            .Include(c => c.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Translations)
            .Include(c => c.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(c => c.Tiles).ThenInclude(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.Translations)
            .Include(c => c.Tiles).ThenInclude(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.OutgoingEdges).ThenInclude(e => e.Translations)
            .Include(c => c.Tiles).ThenInclude(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.OutgoingEdges).ThenInclude(e => e.Tokens)
            .Include(c => c.Topics).ThenInclude(t => t.StoryTopic)
            .Include(c => c.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .Include(c => c.CoAuthors).ThenInclude(ca => ca.User)
            .Include(c => c.DialogParticipants)
            .FirstOrDefaultAsync(c => c.Id == craft.Id, ct) ?? craft;

        var requiresFullPublish = forceFullPublish || def == null;
        List<StoryPublishChangeLog>? pendingLogs = null;

        if (!requiresFullPublish && def != null)
        {
            pendingLogs = await _db.StoryPublishChangeLogs
                .Where(x => x.StoryId == storyId && x.DraftVersion > def.LastPublishedVersion)
                .OrderBy(x => x.DraftVersion)
                .ThenBy(x => x.CreatedAt)
                .ToListAsync(ct);

            if (pendingLogs.Count == 0)
            {
                requiresFullPublish = true;
            }
        }

        if (!requiresFullPublish && def != null && pendingLogs != null)
        {
            var deltaApplied = await TryApplyDeltaPublishAsync(def, craft, pendingLogs, ownerEmail, langTag, ct);
            if (deltaApplied)
            {
                await UpsertUserCreatedStoryAsync(def, craft.OwnerUserId, ct);
                LogPublication(def, craft.OwnerUserId, ownerEmail, StoryPublicationAction.Publish, null);
                await _db.SaveChangesAsync(ct);
                await CleanupChangeLogsAsync(storyId, craft.LastDraftVersion, ct);
                return def.Version;
            }

            requiresFullPublish = true;
        }

        var newVersion = await ApplyFullPublishAsync(def, craft, ownerEmail, langTag, ct);
        await CleanupChangeLogsAsync(storyId, craft.LastDraftVersion, ct);
        return newVersion;
    }

    private async Task<int> ApplyFullPublishAsync(StoryDefinition? existingDefinition, StoryCraft craft, string ownerEmail, string langTag, CancellationToken ct)
    {
        var storyId = craft.StoryId;
        var def = existingDefinition;
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
                IsEvaluative = craft.IsEvaluative,
                IsPartOfEpic = craft.IsPartOfEpic,
                SortOrder = 0,
                Version = 1,
                PriceInCredits = craft.PriceInCredits,
                CreatedBy = craft.OwnerUserId,
                UpdatedBy = craft.OwnerUserId
            };
            _db.StoryDefinitions.Add(def);
        }

        def!.Title = craft.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Title ?? def.Title;
        def.Summary = craft.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Summary ?? def.Summary;
        def.StoryTopic = craft.StoryTopic ?? def.StoryTopic;
        def.AuthorName = craft.AuthorName ?? def.AuthorName;
        def.ClassicAuthorId = craft.ClassicAuthorId ?? def.ClassicAuthorId;
        def.StoryType = craft.StoryType;
        def.IsEvaluative = craft.IsEvaluative;
        def.IsPartOfEpic = craft.IsPartOfEpic;
        def.PriceInCredits = craft.PriceInCredits;
        def.AudioLanguages = craft.AudioLanguages ?? new List<string>();
        def.Status = StoryStatus.Published;
        def.IsActive = true;
        def.UpdatedAt = DateTime.UtcNow;
        def.UpdatedBy = craft.OwnerUserId;
        if (!def.CreatedBy.HasValue)
        {
            def.CreatedBy = craft.OwnerUserId;
        }
        if (!isNew)
        {
            def.Version = def.Version <= 0 ? 1 : def.Version + 1;
        }

        if (!string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            var coverType = StoryAssetPathMapper.GetCoverAssetType(craft.CoverImageUrl);
            var asset = new StoryAssetPathMapper.AssetInfo(craft.CoverImageUrl, coverType, null);
            def.CoverImageUrl = StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, storyId);
        }
        else
        {
            def.CoverImageUrl = null;
        }

        await RemoveExistingDefinitionContentAsync(def, ct);
        await _assetLinks.SyncCoverAsync(craft, ownerEmail, ct);

        foreach (var tr in craft.Translations)
        {
            _db.StoryDefinitionTranslations.Add(new StoryDefinitionTranslation
            {
                StoryDefinition = def,
                LanguageCode = tr.LanguageCode.ToLowerInvariant(),
                Title = tr.Title ?? string.Empty
            });
        }

        var tilesBySort = craft.Tiles.OrderBy(t => t.SortOrder).ToList();
        _logger.LogInformation("Publishing story: storyId={StoryId} tilesCount={Count}", storyId, tilesBySort.Count);

        foreach (var craftTile in tilesBySort)
        {
            await UpsertTileFromCraftAsync(def, craft, craftTile, ownerEmail, langTag, ct, removeExisting: false);
        }

        await ReplaceDefinitionTopicsAsync(def, craft, ct);
        await ReplaceDefinitionAgeGroupsAsync(def, craft, ct);
        await ReplaceDefinitionCoAuthorsAsync(def, craft, ct);
        await ReplaceDefinitionUnlockedHeroesAsync(def, craft, ct);
        await ReplaceDefinitionDialogParticipantsAsync(def, craft, ct);

        def.LastPublishedVersion = craft.LastDraftVersion;

        await UpsertUserCreatedStoryAsync(def, craft.OwnerUserId, ct);
        LogPublication(def, craft.OwnerUserId, ownerEmail, StoryPublicationAction.Publish, null);
        await _db.SaveChangesAsync(ct);
        return def.Version;
    }

    private async Task<bool> TryApplyDeltaPublishAsync(
        StoryDefinition def,
        StoryCraft craft,
        IReadOnlyCollection<StoryPublishChangeLog> changeLogs,
        string ownerEmail,
        string langTag,
        CancellationToken ct)
    {
        if (changeLogs.Count == 0)
        {
            return false;
        }

        var headerChanged = changeLogs.Any(l =>
            string.Equals(l.EntityType, StoryPublishEntityTypes.Header, StringComparison.OrdinalIgnoreCase));

        var tileChanges = changeLogs
            .Where(l => string.Equals(l.EntityType, StoryPublishEntityTypes.Tile, StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(l.EntityId))
            .GroupBy(l => l.EntityId!)
            .Select(g => g.OrderBy(l => l.DraftVersion).ThenBy(l => l.CreatedAt).Last())
            .ToList();

        if (!headerChanged && tileChanges.Count == 0)
        {
            return false;
        }

        if (headerChanged)
        {
            await ApplyDefinitionMetadataDeltaAsync(def, craft, ownerEmail, langTag, ct);
        }

        foreach (var change in tileChanges)
        {
            var applied = await ApplyTileChangeAsync(def, craft, change, ownerEmail, langTag, ct);
            if (!applied)
            {
                return false;
            }
        }

        // Sync dialog participants from craft on every delta publish (not only on header change)
        await ReplaceDefinitionDialogParticipantsAsync(def, craft, ct);

        def.LastPublishedVersion = craft.LastDraftVersion;
        def.Version = def.Version <= 0 ? 1 : def.Version + 1;
        def.Status = StoryStatus.Published;
        def.IsActive = true;
        def.UpdatedAt = DateTime.UtcNow;
        def.UpdatedBy = craft.OwnerUserId;

        return true;
    }

    private async Task ApplyDefinitionMetadataDeltaAsync(StoryDefinition def, StoryCraft craft, string ownerEmail, string langTag, CancellationToken ct)
    {
        def.Title = craft.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Title ?? def.Title;
        def.Summary = craft.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Summary ?? def.Summary;
        def.StoryTopic = craft.StoryTopic ?? def.StoryTopic;
        def.AuthorName = craft.AuthorName ?? def.AuthorName;
        def.ClassicAuthorId = craft.ClassicAuthorId ?? def.ClassicAuthorId;
        def.StoryType = craft.StoryType;
        def.IsEvaluative = craft.IsEvaluative;
        def.IsPartOfEpic = craft.IsPartOfEpic;
        def.PriceInCredits = craft.PriceInCredits;
        def.AudioLanguages = craft.AudioLanguages ?? new List<string>();
        def.Status = StoryStatus.Published;
        def.IsActive = true;
        def.UpdatedAt = DateTime.UtcNow;
        def.UpdatedBy = craft.OwnerUserId;
        if (!def.CreatedBy.HasValue)
        {
            def.CreatedBy = craft.OwnerUserId;
        }

        if (!string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            var coverType = StoryAssetPathMapper.GetCoverAssetType(craft.CoverImageUrl);
            var asset = new StoryAssetPathMapper.AssetInfo(craft.CoverImageUrl, coverType, null);
            def.CoverImageUrl = StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, craft.StoryId);
        }
        else
        {
            def.CoverImageUrl = null;
        }

        await ReplaceDefinitionTranslationsAsync(def, craft, ct);
        await ReplaceDefinitionTopicsAsync(def, craft, ct);
        await ReplaceDefinitionAgeGroupsAsync(def, craft, ct);
        await ReplaceDefinitionCoAuthorsAsync(def, craft, ct);
        await ReplaceDefinitionUnlockedHeroesAsync(def, craft, ct);
        await ReplaceDefinitionDialogParticipantsAsync(def, craft, ct);
        await _assetLinks.SyncCoverAsync(craft, ownerEmail, ct);
    }

    private async Task RemoveExistingDefinitionContentAsync(StoryDefinition def, CancellationToken ct)
    {
        if (def.Id == Guid.Empty)
        {
            return;
        }

        var existingTileIds = await _db.StoryTiles
            .Where(t => t.StoryDefinitionId == def.Id)
            .Select(t => t.TileId)
            .ToListAsync(ct);
        foreach (var tileId in existingTileIds)
        {
            await RemoveTileAsync(def, tileId, ct);
        }

        var existingTranslations = await _db.StoryDefinitionTranslations
            .Where(t => t.StoryDefinitionId == def.Id)
            .ToListAsync(ct);
        if (existingTranslations.Count > 0)
        {
            _db.StoryDefinitionTranslations.RemoveRange(existingTranslations);
        }

        var existingTopics = await _db.StoryDefinitionTopics
            .Where(t => t.StoryDefinitionId == def.Id)
            .ToListAsync(ct);
        if (existingTopics.Count > 0)
        {
            _db.StoryDefinitionTopics.RemoveRange(existingTopics);
        }

        var existingAgeGroups = await _db.StoryDefinitionAgeGroups
            .Where(ag => ag.StoryDefinitionId == def.Id)
            .ToListAsync(ct);
        if (existingAgeGroups.Count > 0)
        {
            _db.StoryDefinitionAgeGroups.RemoveRange(existingAgeGroups);
        }

        await _assetLinks.RemoveCoverAsync(def.StoryId, ct);

        var existingDialogParticipants = await _db.StoryDialogParticipants
            .Where(p => p.StoryDefinitionId == def.Id)
            .ToListAsync(ct);
        if (existingDialogParticipants.Count > 0)
        {
            _db.StoryDialogParticipants.RemoveRange(existingDialogParticipants);
        }
    }

    private async Task ReplaceDefinitionTranslationsAsync(StoryDefinition def, StoryCraft craft, CancellationToken ct)
    {
        var existingTranslations = await _db.StoryDefinitionTranslations
            .Where(t => t.StoryDefinitionId == def.Id)
            .ToListAsync(ct);
        if (existingTranslations.Count > 0)
        {
            _db.StoryDefinitionTranslations.RemoveRange(existingTranslations);
        }

        foreach (var tr in craft.Translations)
        {
            _db.StoryDefinitionTranslations.Add(new StoryDefinitionTranslation
            {
                StoryDefinitionId = def.Id,
                LanguageCode = tr.LanguageCode.ToLowerInvariant(),
                Title = tr.Title ?? string.Empty
            });
        }
    }

    private async Task ReplaceDefinitionTopicsAsync(StoryDefinition def, StoryCraft craft, CancellationToken ct)
    {
        var existingTopics = await _db.StoryDefinitionTopics
            .Where(t => t.StoryDefinitionId == def.Id)
            .ToListAsync(ct);
        if (existingTopics.Count > 0)
        {
            _db.StoryDefinitionTopics.RemoveRange(existingTopics);
        }

        foreach (var craftTopic in craft.Topics)
        {
            _db.StoryDefinitionTopics.Add(new StoryDefinitionTopic
            {
                StoryDefinitionId = def.Id,
                StoryTopicId = craftTopic.StoryTopicId,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private async Task ReplaceDefinitionAgeGroupsAsync(StoryDefinition def, StoryCraft craft, CancellationToken ct)
    {
        var existingAgeGroups = await _db.StoryDefinitionAgeGroups
            .Where(ag => ag.StoryDefinitionId == def.Id)
            .ToListAsync(ct);
        if (existingAgeGroups.Count > 0)
        {
            _db.StoryDefinitionAgeGroups.RemoveRange(existingAgeGroups);
        }

        foreach (var craftAgeGroup in craft.AgeGroups)
        {
            _db.StoryDefinitionAgeGroups.Add(new StoryDefinitionAgeGroup
            {
                StoryDefinitionId = def.Id,
                StoryAgeGroupId = craftAgeGroup.StoryAgeGroupId,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private async Task ReplaceDefinitionCoAuthorsAsync(StoryDefinition def, StoryCraft craft, CancellationToken ct)
    {
        var existing = await _db.StoryDefinitionCoAuthors
            .Where(c => c.StoryDefinitionId == def.Id)
            .ToListAsync(ct);
        if (existing.Count > 0)
        {
            _db.StoryDefinitionCoAuthors.RemoveRange(existing);
        }

        if (craft.CoAuthors == null || craft.CoAuthors.Count == 0)
            return;

        var sortOrder = 0;
        foreach (var craftCoAuthor in craft.CoAuthors.OrderBy(c => c.SortOrder))
        {
            _db.StoryDefinitionCoAuthors.Add(new StoryDefinitionCoAuthor
            {
                Id = Guid.NewGuid(),
                StoryDefinitionId = def.Id,
                UserId = craftCoAuthor.UserId,
                DisplayName = craftCoAuthor.UserId.HasValue ? null : craftCoAuthor.DisplayName,
                SortOrder = sortOrder++
            });
        }
    }

    private async Task ReplaceDefinitionUnlockedHeroesAsync(StoryDefinition def, StoryCraft craft, CancellationToken ct)
    {
        var existingUnlockedHeroes = await _db.StoryDefinitionUnlockedHeroes
            .Where(h => h.StoryDefinitionId == def.Id)
            .ToListAsync(ct);
        if (existingUnlockedHeroes.Count > 0)
        {
            _db.StoryDefinitionUnlockedHeroes.RemoveRange(existingUnlockedHeroes);
        }

        // Load craft with UnlockedHeroes if not already loaded
        if (craft.UnlockedHeroes == null || craft.UnlockedHeroes.Count == 0)
        {
            craft.UnlockedHeroes = await _db.StoryCraftUnlockedHeroes
                .Where(h => h.StoryCraftId == craft.Id)
                .ToListAsync(ct);
        }

        foreach (var craftUnlockedHero in craft.UnlockedHeroes)
        {
            _db.StoryDefinitionUnlockedHeroes.Add(new StoryDefinitionUnlockedHero
            {
                StoryDefinitionId = def.Id,
                HeroId = craftUnlockedHero.HeroId,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private async Task ReplaceDefinitionDialogParticipantsAsync(StoryDefinition def, StoryCraft craft, CancellationToken ct)
    {
        var existing = await _db.StoryDialogParticipants
            .Where(h => h.StoryDefinitionId == def.Id)
            .ToListAsync(ct);
        if (existing.Count > 0)
        {
            _db.StoryDialogParticipants.RemoveRange(existing);
        }

        var participants = craft.DialogParticipants
            .OrderBy(p => p.SortOrder)
            .ToList();

        for (var i = 0; i < participants.Count; i++)
        {
            _db.StoryDialogParticipants.Add(new StoryDialogParticipant
            {
                Id = Guid.NewGuid(),
                StoryDefinitionId = def.Id,
                HeroId = participants[i].HeroId,
                SortOrder = i,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private async Task<bool> ApplyTileChangeAsync(
        StoryDefinition def,
        StoryCraft craft,
        StoryPublishChangeLog change,
        string ownerEmail,
        string langTag,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(change.EntityId))
        {
            return true;
        }

        var tileId = change.EntityId;
        if (string.Equals(change.ChangeType, StoryPublishChangeTypes.Removed, StringComparison.OrdinalIgnoreCase))
        {
            await RemoveTileAsync(def, tileId, ct);
            return true;
        }

        var craftTile = craft.Tiles.FirstOrDefault(t => string.Equals(t.TileId, tileId, StringComparison.OrdinalIgnoreCase));
        if (craftTile == null)
        {
            _logger.LogWarning("Delta publish failed: storyId={StoryId} tileId={TileId} missing in craft.", def.StoryId, tileId);
            return false;
        }

        await UpsertTileFromCraftAsync(def, craft, craftTile, ownerEmail, langTag, ct);
        return true;
    }

    private async Task UpsertTileFromCraftAsync(
        StoryDefinition def,
        StoryCraft craft,
        StoryCraftTile craftTile,
        string ownerEmail,
        string langTag,
        CancellationToken ct,
        bool removeExisting = true)
    {
        if (removeExisting)
        {
            await RemoveTileAsync(def, craftTile.TileId, ct);
        }

        var tile = new StoryTile
        {
            StoryDefinitionId = def.Id,
            TileId = craftTile.TileId,
            Type = craftTile.Type,
            SortOrder = craftTile.SortOrder,
            BranchId = craftTile.BranchId
        };

        if (!string.IsNullOrWhiteSpace(craftTile.ImageUrl))
        {
            var asset = new StoryAssetPathMapper.AssetInfo(craftTile.ImageUrl, StoryAssetPathMapper.AssetType.Image, null);
            tile.ImageUrl = StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, def.StoryId);
        }

        _db.StoryTiles.Add(tile);

        foreach (var tr in craftTile.Translations)
        {
            var translationLang = tr.LanguageCode.ToLowerInvariant();
            string? publishedAudioUrl = null;
            string? publishedVideoUrl = null;

            if (!string.IsNullOrWhiteSpace(tr.AudioUrl))
            {
                var audioAsset = new StoryAssetPathMapper.AssetInfo(tr.AudioUrl, StoryAssetPathMapper.AssetType.Audio, translationLang);
                publishedAudioUrl = StoryAssetPathMapper.BuildPublishedPath(audioAsset, ownerEmail, def.StoryId);
            }

            if (!string.IsNullOrWhiteSpace(tr.VideoUrl))
            {
                var videoAsset = new StoryAssetPathMapper.AssetInfo(tr.VideoUrl, StoryAssetPathMapper.AssetType.Video, translationLang);
                publishedVideoUrl = StoryAssetPathMapper.BuildPublishedPath(videoAsset, ownerEmail, def.StoryId);
            }

            _db.StoryTileTranslations.Add(new StoryTileTranslation
            {
                StoryTile = tile,
                LanguageCode = translationLang,
                Caption = tr.Caption,
                Text = tr.Text,
                Question = tr.Question,
                AudioUrl = publishedAudioUrl,
                VideoUrl = publishedVideoUrl
            });
        }

        if (string.Equals(craftTile.Type, "dialog", StringComparison.OrdinalIgnoreCase) && craftTile.DialogTile != null)
        {
            var dialogTile = new StoryDialogTile
            {
                Id = Guid.NewGuid(),
                StoryDefinitionId = def.Id,
                StoryTile = tile,
                RootNodeId = craftTile.DialogTile.RootNodeId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.StoryDialogTiles.Add(dialogTile);

            foreach (var craftNode in craftTile.DialogTile.Nodes.OrderBy(n => n.SortOrder))
            {
                var node = new StoryDialogNode
                {
                    Id = Guid.NewGuid(),
                    StoryDialogTile = dialogTile,
                    NodeId = craftNode.NodeId,
                    SpeakerType = craftNode.SpeakerType,
                    SpeakerHeroId = craftNode.SpeakerHeroId,
                    SortOrder = craftNode.SortOrder
                };
                _db.StoryDialogNodes.Add(node);

                foreach (var nodeTranslation in craftNode.Translations)
                {
                    _db.StoryDialogNodeTranslations.Add(new StoryDialogNodeTranslation
                    {
                        Id = Guid.NewGuid(),
                        StoryDialogNode = node,
                        LanguageCode = nodeTranslation.LanguageCode.ToLowerInvariant(),
                        Text = nodeTranslation.Text ?? string.Empty
                    });
                }

                foreach (var craftEdge in craftNode.OutgoingEdges.OrderBy(e => e.OptionOrder))
                {
                    var edge = new StoryDialogEdge
                    {
                        Id = Guid.NewGuid(),
                        StoryDialogNode = node,
                        EdgeId = craftEdge.EdgeId,
                        ToNodeId = craftEdge.ToNodeId,
                        JumpToTileId = craftEdge.JumpToTileId,
                        SetBranchId = craftEdge.SetBranchId,
                        OptionOrder = craftEdge.OptionOrder
                    };
                    _db.StoryDialogEdges.Add(edge);

                    foreach (var edgeTranslation in craftEdge.Translations)
                    {
                        _db.StoryDialogEdgeTranslations.Add(new StoryDialogEdgeTranslation
                        {
                            Id = Guid.NewGuid(),
                            StoryDialogEdge = edge,
                            LanguageCode = edgeTranslation.LanguageCode.ToLowerInvariant(),
                            OptionText = edgeTranslation.OptionText ?? string.Empty
                        });
                    }

                    foreach (var edgeToken in craftEdge.Tokens)
                    {
                        _db.StoryDialogEdgeTokens.Add(new StoryDialogEdgeToken
                        {
                            Id = Guid.NewGuid(),
                            StoryDialogEdge = edge,
                            Type = edgeToken.Type,
                            Value = edgeToken.Value,
                            Quantity = edgeToken.Quantity
                        });
                    }
                }
            }
        }

        await _assetLinks.SyncTileAssetsAsync(craft, craftTile, ownerEmail, ct);

        var answers = craftTile.Answers.OrderBy(a => a.SortOrder).ToList();
        foreach (var craftAnswer in answers)
        {
            var answer = new StoryAnswer
            {
                StoryTile = tile,
                AnswerId = craftAnswer.AnswerId,
                Text = craftAnswer.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Text ?? string.Empty,
                IsCorrect = craftAnswer.IsCorrect,
                SortOrder = craftAnswer.SortOrder
            };
            _db.StoryAnswers.Add(answer);

            foreach (var token in craftAnswer.Tokens)
            {
                _db.StoryAnswerTokens.Add(new StoryAnswerToken
                {
                    StoryAnswer = answer,
                    Type = token.Type,
                    Value = token.Value,
                    Quantity = token.Quantity
                });
            }

            foreach (var translation in craftAnswer.Translations)
            {
                _db.StoryAnswerTranslations.Add(new StoryAnswerTranslation
                {
                    StoryAnswer = answer,
                    LanguageCode = translation.LanguageCode.ToLowerInvariant(),
                    Text = translation.Text ?? string.Empty
                });
            }
        }
    }

    private async Task RemoveTileAsync(StoryDefinition def, string tileId, CancellationToken ct)
    {
        var tile = await _db.StoryTiles
            .Include(t => t.Translations)
            .Include(t => t.Answers).ThenInclude(a => a.Translations)
            .Include(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.Translations)
            .Include(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.OutgoingEdges).ThenInclude(e => e.Translations)
            .Include(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.OutgoingEdges).ThenInclude(e => e.Tokens)
            .FirstOrDefaultAsync(t => t.StoryDefinitionId == def.Id && t.TileId == tileId, ct);

        if (tile == null)
        {
            return;
        }

        await _assetLinks.RemoveTileAssetsAsync(def.StoryId, tile.TileId, ct);

        _db.StoryTileTranslations.RemoveRange(tile.Translations);
        foreach (var answer in tile.Answers)
        {
            _db.StoryAnswerTranslations.RemoveRange(answer.Translations);
            _db.StoryAnswerTokens.RemoveRange(answer.Tokens);
        }
        if (tile.DialogTile != null)
        {
            foreach (var node in tile.DialogTile.Nodes)
            {
                _db.StoryDialogNodeTranslations.RemoveRange(node.Translations);
                foreach (var edge in node.OutgoingEdges)
                {
                    _db.StoryDialogEdgeTranslations.RemoveRange(edge.Translations);
                    _db.StoryDialogEdgeTokens.RemoveRange(edge.Tokens);
                }
                _db.StoryDialogEdges.RemoveRange(node.OutgoingEdges);
            }
            _db.StoryDialogNodes.RemoveRange(tile.DialogTile.Nodes);
            _db.StoryDialogTiles.Remove(tile.DialogTile);
        }
        _db.StoryAnswers.RemoveRange(tile.Answers);
        _db.StoryTiles.Remove(tile);
    }

    private async Task CleanupChangeLogsAsync(string storyId, int lastDraftVersion, CancellationToken ct)
    {
        if (lastDraftVersion <= 0)
        {
            return;
        }

        await _db.StoryPublishChangeLogs
            .Where(x => x.StoryId == storyId && x.DraftVersion <= lastDraftVersion)
            .ExecuteDeleteAsync(ct);
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

    private static class StoryPublishEntityTypes
    {
        public const string Header = "Header";
        public const string Tile = "Tile";
    }

    private static class StoryPublishChangeTypes
    {
        public const string Added = "Added";
        public const string Updated = "Updated";
        public const string Removed = "Removed";
    }
}


