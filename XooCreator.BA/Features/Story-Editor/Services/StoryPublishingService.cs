using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Extensions;
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
        
        // Reload craft only if not already fully loaded (e.g. when called from queue worker it is already loaded)
        if (craft.Tiles == null || (craft.Topics == null && craft.AgeGroups == null))
        {
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
        }

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
        var lightChanges = craft.LightChanges;
        var previousCoverImageUrl = def?.CoverImageUrl;
        var previousTileAssetsByTileId = BuildPublishedTileAssetSnapshots(def);

        var effectiveTranslations = craft.Translations;

        var resolvedTitle = effectiveTranslations.FirstOrDefault(t => string.Equals(t.LanguageCode, langTag, StringComparison.OrdinalIgnoreCase))?.Title
                            ?? effectiveTranslations.FirstOrDefault()?.Title
                            ?? string.Empty;

        if (isNew)
        {
            def = new StoryDefinition
            {
                StoryId = storyId,
                Title = resolvedTitle,
                StoryTopic = craft.StoryTopic,
                AuthorName = craft.AuthorName,
                ClassicAuthorId = craft.ClassicAuthorId,
                StoryType = craft.StoryType,
                Status = StoryStatus.Published,
                IsActive = true,
                IsEvaluative = craft.IsEvaluative,
                IsPartOfEpic = craft.IsPartOfEpic,
                IsFullyInteractive = craft.IsFullyInteractive,
                AlwaysShowInStoriesList = craft.AlwaysShowInStoriesList,
                SortOrder = 0,
                Version = 1,
                PriceInCredits = craft.PriceInCredits,
                CreatedBy = craft.OwnerUserId,
                UpdatedBy = craft.OwnerUserId
            };
            _db.StoryDefinitions.Add(def);
        }

        def!.Title = effectiveTranslations.FirstOrDefault(t => string.Equals(t.LanguageCode, langTag, StringComparison.OrdinalIgnoreCase))?.Title 
                     ?? effectiveTranslations.FirstOrDefault()?.Title 
                     ?? def.Title;
        def.Summary = effectiveTranslations.FirstOrDefault(t => string.Equals(t.LanguageCode, langTag, StringComparison.OrdinalIgnoreCase))?.Summary 
                      ?? effectiveTranslations.FirstOrDefault()?.Summary 
                      ?? def.Summary;
        def.StoryTopic = craft.StoryTopic ?? def.StoryTopic;
        def.AuthorName = craft.AuthorName ?? def.AuthorName;
        def.ClassicAuthorId = craft.ClassicAuthorId ?? def.ClassicAuthorId;
        def.StoryType = craft.StoryType;
        def.IsEvaluative = craft.IsEvaluative;
        def.IsPartOfEpic = craft.IsPartOfEpic;
        def.IsFullyInteractive = craft.IsFullyInteractive;
        def.AlwaysShowInStoriesList = craft.AlwaysShowInStoriesList;
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

        var selectedLangs = StoryPublishLanguageMergeHelper.GetSelectedLanguagesFromCraft(craft);
        var useLanguageMerge = StoryPublishLanguageMergeHelper.ShouldUseLanguageMerge(craft, isNew);

        if (useLanguageMerge)
        {
            await ApplyFullPublishWithLanguageMergeAsync(def, craft, ownerEmail, langTag, selectedLangs!, previousTileAssetsByTileId, ct);
        }
        else
        {
            await RemoveExistingDefinitionContentAsync(def, ct, keepAssets: lightChanges);

            if (lightChanges)
            {
                def.CoverImageUrl = previousCoverImageUrl;
            }
            else
            {
            var coverSynced = await _assetLinks.SyncCoverAsync(craft, ownerEmail, ct);
            if (coverSynced && !string.IsNullOrWhiteSpace(craft.CoverImageUrl))
            {
                var coverType = StoryAssetPathMapper.GetCoverAssetType(craft.CoverImageUrl);
                var asset = new StoryAssetPathMapper.AssetInfo(craft.CoverImageUrl, coverType, null);
                def.CoverImageUrl = StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, storyId);
            }
            else if (!string.IsNullOrWhiteSpace(previousCoverImageUrl))
            {
                // Granular version: draft has no cover (was not copied); keep published cover.
                def.CoverImageUrl = previousCoverImageUrl;
            }
            else
            {
                def.CoverImageUrl = null;
            }
        }

        foreach (var tr in effectiveTranslations)
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
            previousTileAssetsByTileId.TryGetValue(craftTile.TileId, out var previousTileAssets);
            await UpsertTileFromCraftAsync(
                def,
                craft,
                craftTile,
                ownerEmail,
                langTag,
                ct,
                removeExisting: false,
                keepAssets: lightChanges,
                previousPublishedAssets: previousTileAssets);
        }

        await ReplaceDefinitionTopicsAsync(def, craft, ct);
        await ReplaceDefinitionAgeGroupsAsync(def, craft, ct);
        await ReplaceDefinitionCoAuthorsAsync(def, craft, ct);
        await ReplaceDefinitionUnlockedHeroesAsync(def, craft, ct);
        await ReplaceDefinitionDialogParticipantsAsync(def, craft, ct);
        }

        def.LastPublishedVersion = craft.LastDraftVersion;

        await UpsertUserCreatedStoryAsync(def, craft.OwnerUserId, ct);
        LogPublication(def, craft.OwnerUserId, ownerEmail, StoryPublicationAction.Publish, null);
        await _db.SaveChangesAsync(ct);
        return def.Version;
    }

    private async Task ApplyFullPublishWithLanguageMergeAsync(
        StoryDefinition def,
        StoryCraft craft,
        string ownerEmail,
        string langTag,
        HashSet<string> selectedLangs,
        Dictionary<string, PublishedTileAssetSnapshot> previousTileAssetsByTileId,
        CancellationToken ct)
    {
        var storyId = def.StoryId;

        // Merge story-level translations: remove only selected langs, add from craft
        await MergeDefinitionTranslationsForSelectedLanguagesAsync(def, craft, selectedLangs, ct);

        // Merge AudioLanguages: keep def languages not in selected, add craft's
        var defAudioLangs = def.AudioLanguages ?? new List<string>();
        var craftAudioLangs = craft.AudioLanguages ?? new List<string>();
        def.AudioLanguages = defAudioLangs
            .Where(l => !selectedLangs.Contains(l.ToLowerInvariant()))
            .Concat(craftAudioLangs)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Cover: use craft's if present, else keep published (asset preservation)
        // Do NOT call SyncCoverAsync when craft has no cover - that would remove published cover
        if (!string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            var coverSynced = await _assetLinks.SyncCoverAsync(craft, ownerEmail, ct);
            if (coverSynced)
            {
                var coverType = StoryAssetPathMapper.GetCoverAssetType(craft.CoverImageUrl);
                var asset = new StoryAssetPathMapper.AssetInfo(craft.CoverImageUrl, coverType, null);
                def.CoverImageUrl = StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, storyId);
            }
        }
        // else: def.CoverImageUrl unchanged (keep published cover)

        // Merge tile translations for selected languages only
        await MergeTilesForSelectedLanguagesAsync(def, craft, ownerEmail, selectedLangs, previousTileAssetsByTileId, ct);

        await ReplaceDefinitionTopicsAsync(def, craft, ct);
        await ReplaceDefinitionAgeGroupsAsync(def, craft, ct);
        await ReplaceDefinitionCoAuthorsAsync(def, craft, ct);
        await ReplaceDefinitionUnlockedHeroesAsync(def, craft, ct);
        await ReplaceDefinitionDialogParticipantsAsync(def, craft, ct);
    }

    private async Task MergeDefinitionTranslationsForSelectedLanguagesAsync(
        StoryDefinition def,
        StoryCraft craft,
        HashSet<string> selectedLangs,
        CancellationToken ct)
    {
        var existing = await _db.StoryDefinitionTranslations
            .Where(t => t.StoryDefinitionId == def.Id)
            .ToListAsync(ct);

        var toRemove = existing.Where(t => selectedLangs.Contains(t.LanguageCode.ToLowerInvariant())).ToList();
        if (toRemove.Count > 0)
        {
            _db.StoryDefinitionTranslations.RemoveRange(toRemove);
        }

        foreach (var tr in craft.Translations.Where(t => selectedLangs.Contains(t.LanguageCode.ToLowerInvariant())))
        {
            _db.StoryDefinitionTranslations.Add(new StoryDefinitionTranslation
            {
                StoryDefinitionId = def.Id,
                LanguageCode = tr.LanguageCode.ToLowerInvariant(),
                Title = tr.Title ?? string.Empty
            });
        }
    }

    private async Task MergeTilesForSelectedLanguagesAsync(
        StoryDefinition def,
        StoryCraft craft,
        string ownerEmail,
        HashSet<string> selectedLangs,
        Dictionary<string, PublishedTileAssetSnapshot> previousTileAssetsByTileId,
        CancellationToken ct)
    {
        var defTiles = await _db.StoryTiles
            .Include(t => t.Translations)
            .Include(t => t.Answers).ThenInclude(a => a.Translations)
            .Include(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.Translations)
            .Include(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.OutgoingEdges).ThenInclude(e => e.Translations)
            .Where(t => t.StoryDefinitionId == def.Id)
            .ToListAsync(ct);

        var craftTilesByTileId = craft.Tiles.ToDictionary(t => t.TileId, t => t, StringComparer.OrdinalIgnoreCase);

        foreach (var defTile in defTiles)
        {
            if (!craftTilesByTileId.TryGetValue(defTile.TileId, out var craftTile))
            {
                continue;
            }

            previousTileAssetsByTileId.TryGetValue(defTile.TileId, out var previousAssets);
            await MergeSingleTileForSelectedLanguagesAsync(def, craft, craftTile, ownerEmail, selectedLangs, previousAssets, ct);
            await _assetLinks.SyncTileAssetsAsync(craft, craftTile, ownerEmail, ct, preserveExistingWhenEmpty: true);
        }
    }

    private async Task MergeSingleTileForSelectedLanguagesAsync(
        StoryDefinition def,
        StoryCraft craft,
        StoryCraftTile craftTile,
        string ownerEmail,
        HashSet<string> selectedLangs,
        PublishedTileAssetSnapshot? previousAssets,
        CancellationToken ct)
    {
        var defTile = await _db.StoryTiles
            .Include(t => t.Translations)
            .Include(t => t.Answers).ThenInclude(a => a.Translations)
            .Include(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.Translations)
            .Include(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.OutgoingEdges).ThenInclude(e => e.Translations)
            .FirstOrDefaultAsync(t => t.StoryDefinitionId == def.Id && t.TileId == craftTile.TileId, ct);

        if (defTile == null)
        {
            return;
        }

        // Tile-level ImageUrl: use craft's if non-empty, else keep def's (asset preservation)
        if (!string.IsNullOrWhiteSpace(craftTile.ImageUrl))
        {
            var imageFilename = craftTile.ImageUrl.GetFilenameOnly();
            if (!string.IsNullOrWhiteSpace(imageFilename))
            {
                var asset = new StoryAssetPathMapper.AssetInfo(imageFilename!, StoryAssetPathMapper.AssetType.Image, null);
                defTile.ImageUrl = StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, def.StoryId);
            }
        }

        // Merge tile translations: remove selected langs, add from craft
        var tileTransToRemove = defTile.Translations
            .Where(tr => selectedLangs.Contains(tr.LanguageCode.ToLowerInvariant()))
            .ToList();
        foreach (var t in tileTransToRemove)
        {
            _db.StoryTileTranslations.Remove(t);
        }

        foreach (var tr in craftTile.Translations.Where(t => selectedLangs.Contains(t.LanguageCode.ToLowerInvariant())))
        {
            var lang = tr.LanguageCode.ToLowerInvariant();
            string? publishedAudioUrl = null;
            string? publishedVideoUrl = null;

            PublishedTranslationMedia? existingMedia = null;
            if (previousAssets?.TranslationMediaByLang.TryGetValue(lang, out var media) == true)
            {
                existingMedia = media;
            }

            if (!string.IsNullOrWhiteSpace(tr.AudioUrl))
            {
                var audioFilename = tr.AudioUrl.GetFilenameOnly();
                var audioAsset = new StoryAssetPathMapper.AssetInfo(audioFilename!, StoryAssetPathMapper.AssetType.Audio, lang);
                publishedAudioUrl = StoryAssetPathMapper.BuildPublishedPath(audioAsset, ownerEmail, def.StoryId);
            }
            else if (existingMedia != null)
            {
                publishedAudioUrl = existingMedia.AudioUrl;
            }

            if (!string.IsNullOrWhiteSpace(tr.VideoUrl))
            {
                var videoFilename = tr.VideoUrl.GetFilenameOnly();
                var videoAsset = new StoryAssetPathMapper.AssetInfo(videoFilename!, StoryAssetPathMapper.AssetType.Video, lang);
                publishedVideoUrl = StoryAssetPathMapper.BuildPublishedPath(videoAsset, ownerEmail, def.StoryId);
            }
            else if (existingMedia != null)
            {
                publishedVideoUrl = existingMedia.VideoUrl;
            }

            _db.StoryTileTranslations.Add(new StoryTileTranslation
            {
                StoryTile = defTile,
                LanguageCode = lang,
                Caption = tr.Caption,
                Text = tr.Text,
                Question = tr.Question,
                AudioUrl = publishedAudioUrl,
                VideoUrl = publishedVideoUrl
            });
        }

        // Merge answer translations for selected langs
        var defAnswers = defTile.Answers?.ToList() ?? new List<StoryAnswer>();
        foreach (var defAnswer in defAnswers)
        {
            var craftAnswer = craftTile.Answers?.FirstOrDefault(a =>
                string.Equals(a.AnswerId, defAnswer.AnswerId, StringComparison.OrdinalIgnoreCase));
            if (craftAnswer == null)
            {
                continue;
            }

            var answerTransToRemove = defAnswer.Translations?
                .Where(tr => selectedLangs.Contains(tr.LanguageCode.ToLowerInvariant()))
                .ToList() ?? new List<StoryAnswerTranslation>();
            foreach (var t in answerTransToRemove)
            {
                _db.StoryAnswerTranslations.Remove(t);
            }

            foreach (var tr in craftAnswer.Translations.Where(t => selectedLangs.Contains(t.LanguageCode.ToLowerInvariant())))
            {
                _db.StoryAnswerTranslations.Add(new StoryAnswerTranslation
                {
                    StoryAnswer = defAnswer,
                    LanguageCode = tr.LanguageCode.ToLowerInvariant(),
                    Text = tr.Text ?? string.Empty
                });
            }
        }

        // Merge dialog node/edge translations for selected langs
        if (string.Equals(craftTile.Type, "dialog", StringComparison.OrdinalIgnoreCase)
            && craftTile.DialogTile != null
            && defTile.DialogTile != null)
        {
            await MergeDialogTranslationsForSelectedLanguagesAsync(
                defTile,
                craftTile,
                def.StoryId,
                ownerEmail,
                selectedLangs,
                previousAssets,
                ct);
        }
    }

    private async Task MergeDialogTranslationsForSelectedLanguagesAsync(
        StoryTile defTile,
        StoryCraftTile craftTile,
        string storyId,
        string ownerEmail,
        HashSet<string> selectedLangs,
        PublishedTileAssetSnapshot? previousAssets,
        CancellationToken ct)
    {
        var craftDialogTileId = craftTile.DialogTile!.Id;
        var craftNodes = await _db.StoryCraftDialogNodes
            .AsNoTracking()
            .Include(n => n.Translations)
            .Include(n => n.OutgoingEdges).ThenInclude(e => e.Translations)
            .Where(n => n.StoryCraftDialogTileId == craftDialogTileId)
            .OrderBy(n => n.SortOrder)
            .ToListAsync(ct);

        var defNodes = defTile.DialogTile!.Nodes.OrderBy(n => n.SortOrder).ToList();

        for (var i = 0; i < craftNodes.Count && i < defNodes.Count; i++)
        {
            var craftNode = craftNodes[i];
            var defNode = defNodes[i];

            // Merge node translations
            var nodeTransToRemove = defNode.Translations?
                .Where(tr => selectedLangs.Contains(tr.LanguageCode.ToLowerInvariant()))
                .ToList() ?? new List<StoryDialogNodeTranslation>();
            foreach (var t in nodeTransToRemove)
            {
                _db.StoryDialogNodeTranslations.Remove(t);
            }

            foreach (var nodeTr in craftNode.Translations.Where(t => selectedLangs.Contains(t.LanguageCode.ToLowerInvariant())))
            {
                var nodeLang = nodeTr.LanguageCode.ToLowerInvariant();
                string? publishedNodeAudioUrl = null;
                if (!string.IsNullOrWhiteSpace(nodeTr.AudioUrl))
                {
                    var nodeAudioFilename = nodeTr.AudioUrl.GetFilenameOnly();
                    publishedNodeAudioUrl = !string.IsNullOrWhiteSpace(nodeAudioFilename)
                        ? StoryAssetPathMapper.BuildPublishedPath(
                            new StoryAssetPathMapper.AssetInfo(nodeAudioFilename!, StoryAssetPathMapper.AssetType.Audio, nodeLang),
                            ownerEmail, storyId)
                        : null;
                }
                else if (previousAssets?.DialogNodeAudioByNodeAndLang.TryGetValue(
                    BuildNodeAudioKey(craftNode.NodeId, nodeLang),
                    out var fallback) == true)
                {
                    publishedNodeAudioUrl = fallback;
                }

                _db.StoryDialogNodeTranslations.Add(new StoryDialogNodeTranslation
                {
                    Id = Guid.NewGuid(),
                    StoryDialogNode = defNode,
                    LanguageCode = nodeLang,
                    Text = nodeTr.Text ?? string.Empty,
                    AudioUrl = publishedNodeAudioUrl
                });
            }

            // Merge edge translations
            var craftEdges = craftNode.OutgoingEdges.OrderBy(e => e.OptionOrder).ToList();
            var defEdges = defNode.OutgoingEdges.OrderBy(e => e.OptionOrder).ToList();
            for (var j = 0; j < craftEdges.Count && j < defEdges.Count; j++)
            {
                var craftEdge = craftEdges[j];
                var defEdge = defEdges[j];

                var edgeTransToRemove = defEdge.Translations?
                    .Where(tr => selectedLangs.Contains(tr.LanguageCode.ToLowerInvariant()))
                    .ToList() ?? new List<StoryDialogEdgeTranslation>();
                foreach (var t in edgeTransToRemove)
                {
                    _db.StoryDialogEdgeTranslations.Remove(t);
                }

                foreach (var edgeTr in craftEdge.Translations.Where(t => selectedLangs.Contains(t.LanguageCode.ToLowerInvariant())))
                {
                    _db.StoryDialogEdgeTranslations.Add(new StoryDialogEdgeTranslation
                    {
                        Id = Guid.NewGuid(),
                        StoryDialogEdge = defEdge,
                        LanguageCode = edgeTr.LanguageCode.ToLowerInvariant(),
                        OptionText = edgeTr.OptionText ?? string.Empty,
                        AudioUrl = null
                    });
                }
            }
        }

        await Task.CompletedTask;
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

        var selectedLangsForDelta = StoryPublishLanguageMergeHelper.GetSelectedLanguagesFromCraft(craft);
        var useLanguageMergeForDelta = selectedLangsForDelta != null && selectedLangsForDelta.Count > 0
            && string.Equals(craft.VersionCopyLanguageMode, "selected", StringComparison.OrdinalIgnoreCase);

        if (headerChanged)
        {
            if (useLanguageMergeForDelta)
            {
                await ApplyDefinitionMetadataDeltaWithLanguageMergeAsync(def, craft, ownerEmail, langTag, selectedLangsForDelta!, ct);
            }
            else
            {
                await ApplyDefinitionMetadataDeltaAsync(def, craft, ownerEmail, langTag, ct);
            }
        }

        foreach (var change in tileChanges)
        {
            var applied = useLanguageMergeForDelta
                ? await ApplyTileChangeWithLanguageMergeAsync(def, craft, change, ownerEmail, langTag, selectedLangsForDelta!, ct)
                : await ApplyTileChangeAsync(def, craft, change, ownerEmail, langTag, ct);
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

    private async Task ApplyDefinitionMetadataDeltaWithLanguageMergeAsync(
        StoryDefinition def,
        StoryCraft craft,
        string ownerEmail,
        string langTag,
        HashSet<string> selectedLangs,
        CancellationToken ct)
    {
        def.Title = craft.Translations.FirstOrDefault(t => string.Equals(t.LanguageCode, langTag, StringComparison.OrdinalIgnoreCase))?.Title
                    ?? craft.Translations.FirstOrDefault()?.Title
                    ?? def.Title;
        def.Summary = craft.Translations.FirstOrDefault(t => string.Equals(t.LanguageCode, langTag, StringComparison.OrdinalIgnoreCase))?.Summary
                      ?? craft.Translations.FirstOrDefault()?.Summary
                      ?? def.Summary;
        def.StoryTopic = craft.StoryTopic ?? def.StoryTopic;
        def.AuthorName = craft.AuthorName ?? def.AuthorName;
        def.ClassicAuthorId = craft.ClassicAuthorId ?? def.ClassicAuthorId;
        def.StoryType = craft.StoryType;
        def.IsEvaluative = craft.IsEvaluative;
        def.IsPartOfEpic = craft.IsPartOfEpic;
        def.IsFullyInteractive = craft.IsFullyInteractive;
        def.AlwaysShowInStoriesList = craft.AlwaysShowInStoriesList;
        def.PriceInCredits = craft.PriceInCredits;
        var defAudioLangs = def.AudioLanguages ?? new List<string>();
        var craftAudioLangs = craft.AudioLanguages ?? new List<string>();
        def.AudioLanguages = defAudioLangs
            .Where(l => !selectedLangs.Contains(l.ToLowerInvariant()))
            .Concat(craftAudioLangs)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        def.Status = StoryStatus.Published;
        def.IsActive = true;
        def.UpdatedAt = DateTime.UtcNow;
        def.UpdatedBy = craft.OwnerUserId;
        if (!def.CreatedBy.HasValue)
        {
            def.CreatedBy = craft.OwnerUserId;
        }

        if (!craft.LightChanges && !string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            var coverSynced = await _assetLinks.SyncCoverAsync(craft, ownerEmail, ct);
            if (coverSynced)
            {
                var coverType = StoryAssetPathMapper.GetCoverAssetType(craft.CoverImageUrl);
                var asset = new StoryAssetPathMapper.AssetInfo(craft.CoverImageUrl, coverType, null);
                def.CoverImageUrl = StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, craft.StoryId);
            }
        }
        // else: keep def.CoverImageUrl unchanged (asset preservation)

        await MergeDefinitionTranslationsForSelectedLanguagesAsync(def, craft, selectedLangs, ct);
        await ReplaceDefinitionTopicsAsync(def, craft, ct);
        await ReplaceDefinitionAgeGroupsAsync(def, craft, ct);
        await ReplaceDefinitionCoAuthorsAsync(def, craft, ct);
        await ReplaceDefinitionUnlockedHeroesAsync(def, craft, ct);
        await ReplaceDefinitionDialogParticipantsAsync(def, craft, ct);
    }

    private async Task ApplyDefinitionMetadataDeltaAsync(StoryDefinition def, StoryCraft craft, string ownerEmail, string langTag, CancellationToken ct)
    {
        def.Title = craft.Translations.FirstOrDefault(t => string.Equals(t.LanguageCode, langTag, StringComparison.OrdinalIgnoreCase))?.Title 
                    ?? craft.Translations.FirstOrDefault()?.Title 
                    ?? def.Title;
        def.Summary = craft.Translations.FirstOrDefault(t => string.Equals(t.LanguageCode, langTag, StringComparison.OrdinalIgnoreCase))?.Summary 
                      ?? craft.Translations.FirstOrDefault()?.Summary 
                      ?? def.Summary;
        def.StoryTopic = craft.StoryTopic ?? def.StoryTopic;
        def.AuthorName = craft.AuthorName ?? def.AuthorName;
        def.ClassicAuthorId = craft.ClassicAuthorId ?? def.ClassicAuthorId;
        def.StoryType = craft.StoryType;
        def.IsEvaluative = craft.IsEvaluative;
        def.IsPartOfEpic = craft.IsPartOfEpic;
        def.IsFullyInteractive = craft.IsFullyInteractive;
        def.AlwaysShowInStoriesList = craft.AlwaysShowInStoriesList;
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

        if (!craft.LightChanges)
        {
            var coverSynced = await _assetLinks.SyncCoverAsync(craft, ownerEmail, ct);
            if (coverSynced && !string.IsNullOrWhiteSpace(craft.CoverImageUrl))
            {
                var coverType = StoryAssetPathMapper.GetCoverAssetType(craft.CoverImageUrl);
                var asset = new StoryAssetPathMapper.AssetInfo(craft.CoverImageUrl, coverType, null);
                def.CoverImageUrl = StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, craft.StoryId);
            }
            else
            {
                def.CoverImageUrl = null;
            }
        }

        await ReplaceDefinitionTranslationsAsync(def, craft, ct);
        await ReplaceDefinitionTopicsAsync(def, craft, ct);
        await ReplaceDefinitionAgeGroupsAsync(def, craft, ct);
        await ReplaceDefinitionCoAuthorsAsync(def, craft, ct);
        await ReplaceDefinitionUnlockedHeroesAsync(def, craft, ct);
        await ReplaceDefinitionDialogParticipantsAsync(def, craft, ct);
    }

    private async Task RemoveExistingDefinitionContentAsync(StoryDefinition def, CancellationToken ct, bool keepAssets = false)
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
            await RemoveTileAsync(def, tileId, ct, keepAssets);
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

        if (!keepAssets)
        {
            await _assetLinks.RemoveCoverAsync(def.StoryId, ct);
        }

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

    private async Task<bool> ApplyTileChangeWithLanguageMergeAsync(
        StoryDefinition def,
        StoryCraft craft,
        StoryPublishChangeLog change,
        string ownerEmail,
        string langTag,
        HashSet<string> selectedLangs,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(change.EntityId))
        {
            return true;
        }

        var tileId = change.EntityId;
        if (string.Equals(change.ChangeType, StoryPublishChangeTypes.Removed, StringComparison.OrdinalIgnoreCase))
        {
            await RemoveTileAsync(def, tileId, ct, keepAssets: craft.LightChanges);
            return true;
        }

        var craftTile = craft.Tiles.FirstOrDefault(t => string.Equals(t.TileId, tileId, StringComparison.OrdinalIgnoreCase));
        if (craftTile == null)
        {
            _logger.LogWarning("Delta publish failed: storyId={StoryId} tileId={TileId} missing in craft.", def.StoryId, tileId);
            return false;
        }

        var previousAssets = await LoadPublishedTileAssetSnapshotAsync(def, tileId, ct);
        await MergeSingleTileForSelectedLanguagesAsync(def, craft, craftTile, ownerEmail, selectedLangs, previousAssets, ct);
        await _assetLinks.SyncTileAssetsAsync(craft, craftTile, ownerEmail, ct, preserveExistingWhenEmpty: true);
        return true;
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
            await RemoveTileAsync(def, tileId, ct, keepAssets: craft.LightChanges);
            return true;
        }

        var craftTile = craft.Tiles.FirstOrDefault(t => string.Equals(t.TileId, tileId, StringComparison.OrdinalIgnoreCase));
        if (craftTile == null)
        {
            _logger.LogWarning("Delta publish failed: storyId={StoryId} tileId={TileId} missing in craft.", def.StoryId, tileId);
            return false;
        }

        PublishedTileAssetSnapshot? previousAssets = null;
        if (craft.LightChanges)
        {
            previousAssets = await LoadPublishedTileAssetSnapshotAsync(def, tileId, ct);
        }

        await UpsertTileFromCraftAsync(
            def,
            craft,
            craftTile,
            ownerEmail,
            langTag,
            ct,
            removeExisting: true,
            keepAssets: craft.LightChanges,
            previousPublishedAssets: previousAssets);
        return true;
    }

    private async Task UpsertTileFromCraftAsync(
        StoryDefinition def,
        StoryCraft craft,
        StoryCraftTile craftTile,
        string ownerEmail,
        string langTag,
        CancellationToken ct,
        bool removeExisting = true,
        bool keepAssets = false,
        PublishedTileAssetSnapshot? previousPublishedAssets = null)
    {
        if (removeExisting)
        {
            await RemoveTileAsync(def, craftTile.TileId, ct, keepAssets);
        }

        var tile = new StoryTile
        {
            StoryDefinitionId = def.Id,
            TileId = craftTile.TileId,
            Type = craftTile.Type,
            SortOrder = craftTile.SortOrder,
            BranchId = craftTile.BranchId,
            AvailableHeroIdsJson = craftTile.AvailableHeroIdsJson
        };

        if (keepAssets)
        {
            tile.ImageUrl = previousPublishedAssets?.ImageUrl;
        }
        else if (!string.IsNullOrWhiteSpace(craftTile.ImageUrl))
        {
            var imageFilename = craftTile.ImageUrl.GetFilenameOnly();
            var asset = new StoryAssetPathMapper.AssetInfo(imageFilename!, StoryAssetPathMapper.AssetType.Image, null);
            tile.ImageUrl = StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, def.StoryId);
        }
        else if (!string.IsNullOrWhiteSpace(previousPublishedAssets?.ImageUrl))
        {
            // Granular version: craft has no image (was not copied); keep published image.
            tile.ImageUrl = previousPublishedAssets.ImageUrl;
        }

        _db.StoryTiles.Add(tile);

        foreach (var tr in craftTile.Translations)
        {
            var translationLang = tr.LanguageCode.ToLowerInvariant();
            string? publishedAudioUrl = null;
            string? publishedVideoUrl = null;

            PublishedTranslationMedia? existingTranslationMedia = null;
            if (previousPublishedAssets != null
                && previousPublishedAssets.TranslationMediaByLang.TryGetValue(translationLang, out var media))
            {
                existingTranslationMedia = media;
            }

            if (keepAssets)
            {
                if (existingTranslationMedia != null)
                {
                    publishedAudioUrl = existingTranslationMedia.AudioUrl;
                    publishedVideoUrl = existingTranslationMedia.VideoUrl;
                }
            }
            else if (!string.IsNullOrWhiteSpace(tr.AudioUrl))
            {
                var audioFilename = tr.AudioUrl.GetFilenameOnly();
                var audioAsset = new StoryAssetPathMapper.AssetInfo(audioFilename!, StoryAssetPathMapper.AssetType.Audio, translationLang);
                publishedAudioUrl = StoryAssetPathMapper.BuildPublishedPath(audioAsset, ownerEmail, def.StoryId);
            }
            else if (!string.IsNullOrWhiteSpace(existingTranslationMedia?.AudioUrl))
            {
                publishedAudioUrl = existingTranslationMedia.AudioUrl;
            }

            if (!keepAssets && !string.IsNullOrWhiteSpace(tr.VideoUrl))
            {
                var videoFilename = tr.VideoUrl.GetFilenameOnly();
                var videoAsset = new StoryAssetPathMapper.AssetInfo(videoFilename!, StoryAssetPathMapper.AssetType.Video, translationLang);
                publishedVideoUrl = StoryAssetPathMapper.BuildPublishedPath(videoAsset, ownerEmail, def.StoryId);
            }
            else if (!string.IsNullOrWhiteSpace(existingTranslationMedia?.VideoUrl))
            {
                publishedVideoUrl = existingTranslationMedia.VideoUrl;
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

            // Load dialog nodes with translations from DB so we never miss AudioUrl (craft in-memory may be stale or not fully loaded)
            var craftDialogTileId = craftTile.DialogTile.Id;
            var dialogNodesWithTranslations = await _db.StoryCraftDialogNodes
                .AsNoTracking()
                .Include(n => n.Translations)
                .Include(n => n.OutgoingEdges).ThenInclude(e => e.Translations)
                .Include(n => n.OutgoingEdges).ThenInclude(e => e.Tokens)
                .Where(n => n.StoryCraftDialogTileId == craftDialogTileId)
                .OrderBy(n => n.SortOrder)
                .ToListAsync(ct);

            foreach (var craftNode in dialogNodesWithTranslations)
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
                    var nodeLang = nodeTranslation.LanguageCode.ToLowerInvariant();
                    string? publishedNodeAudioUrl;
                    if (keepAssets && previousPublishedAssets != null)
                    {
                        previousPublishedAssets.DialogNodeAudioByNodeAndLang.TryGetValue(
                            BuildNodeAudioKey(craftNode.NodeId, nodeLang),
                            out publishedNodeAudioUrl);
                    }
                    else if (!string.IsNullOrWhiteSpace(nodeTranslation.AudioUrl))
                    {
                        var nodeAudioFilename = nodeTranslation.AudioUrl.GetFilenameOnly();
                        publishedNodeAudioUrl = !string.IsNullOrWhiteSpace(nodeAudioFilename)
                            ? StoryAssetPathMapper.BuildPublishedPath(
                                new StoryAssetPathMapper.AssetInfo(nodeAudioFilename!, StoryAssetPathMapper.AssetType.Audio, nodeLang),
                                ownerEmail, def.StoryId)
                            : null;
                    }
                    else if (previousPublishedAssets?.DialogNodeAudioByNodeAndLang.TryGetValue(
                                 BuildNodeAudioKey(craftNode.NodeId, nodeLang),
                                 out var fallbackNodeAudio) == true
                             && !string.IsNullOrWhiteSpace(fallbackNodeAudio))
                    {
                        publishedNodeAudioUrl = fallbackNodeAudio;
                    }
                    else
                    {
                        publishedNodeAudioUrl = null;
                    }
                    _db.StoryDialogNodeTranslations.Add(new StoryDialogNodeTranslation
                    {
                        Id = Guid.NewGuid(),
                        StoryDialogNode = node,
                        LanguageCode = nodeLang,
                        Text = nodeTranslation.Text ?? string.Empty,
                        AudioUrl = publishedNodeAudioUrl
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
                        HideIfBranchSet = craftEdge.HideIfBranchSet,
                        ShowOnlyIfBranchesSet = craftEdge.ShowOnlyIfBranchesSet,
                        OptionOrder = craftEdge.OptionOrder
                    };
                    _db.StoryDialogEdges.Add(edge);

                    foreach (var edgeTranslation in craftEdge.Translations)
                    {
                        var edgeLang = edgeTranslation.LanguageCode.ToLowerInvariant();
                        _db.StoryDialogEdgeTranslations.Add(new StoryDialogEdgeTranslation
                        {
                            Id = Guid.NewGuid(),
                            StoryDialogEdge = edge,
                            LanguageCode = edgeLang,
                            OptionText = edgeTranslation.OptionText ?? string.Empty,
                            AudioUrl = null // Audio only on node (replica), not on options
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

        if (!keepAssets)
        {
            await _assetLinks.SyncTileAssetsAsync(craft, craftTile, ownerEmail, ct);
        }

        var answers = craftTile.Answers.OrderBy(a => a.SortOrder).ToList();
        foreach (var craftAnswer in answers)
        {
            var answer = new StoryAnswer
            {
                StoryTile = tile,
                AnswerId = craftAnswer.AnswerId,
                Text = craftAnswer.Translations.FirstOrDefault(t => string.Equals(t.LanguageCode, langTag, StringComparison.OrdinalIgnoreCase))?.Text ?? string.Empty,
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

    private async Task RemoveTileAsync(StoryDefinition def, string tileId, CancellationToken ct, bool keepAssets = false)
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

        if (!keepAssets)
        {
            await _assetLinks.RemoveTileAssetsAsync(def.StoryId, tile.TileId, ct);
        }

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

    private static Dictionary<string, PublishedTileAssetSnapshot> BuildPublishedTileAssetSnapshots(StoryDefinition? definition)
    {
        if (definition?.Tiles == null || definition.Tiles.Count == 0)
        {
            return new Dictionary<string, PublishedTileAssetSnapshot>(StringComparer.OrdinalIgnoreCase);
        }

        return definition.Tiles
            .ToDictionary(
                t => t.TileId,
                t => BuildPublishedTileAssetSnapshotFromTile(t),
                StringComparer.OrdinalIgnoreCase);
    }

    private async Task<PublishedTileAssetSnapshot?> LoadPublishedTileAssetSnapshotAsync(
        StoryDefinition def,
        string tileId,
        CancellationToken ct)
    {
        var tile = await _db.StoryTiles
            .AsNoTracking()
            .Include(t => t.Translations)
            .Include(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.Translations)
            .FirstOrDefaultAsync(t => t.StoryDefinitionId == def.Id && t.TileId == tileId, ct);

        return tile == null ? null : BuildPublishedTileAssetSnapshotFromTile(tile);
    }

    private static PublishedTileAssetSnapshot BuildPublishedTileAssetSnapshotFromTile(StoryTile tile)
    {
        var translationMedia = tile.Translations
            .ToDictionary(
                tr => tr.LanguageCode.ToLowerInvariant(),
                tr => new PublishedTranslationMedia(tr.AudioUrl, tr.VideoUrl),
                StringComparer.OrdinalIgnoreCase);

        var dialogNodeAudio = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        if (tile.DialogTile?.Nodes != null)
        {
            foreach (var node in tile.DialogTile.Nodes)
            {
                foreach (var translation in node.Translations)
                {
                    var key = BuildNodeAudioKey(node.NodeId, translation.LanguageCode.ToLowerInvariant());
                    dialogNodeAudio[key] = translation.AudioUrl;
                }
            }
        }

        return new PublishedTileAssetSnapshot(tile.ImageUrl, translationMedia, dialogNodeAudio);
    }

    private static string BuildNodeAudioKey(string nodeId, string lang)
        => $"{nodeId.ToLowerInvariant()}::{lang.ToLowerInvariant()}";

    private sealed record PublishedTranslationMedia(string? AudioUrl, string? VideoUrl);

    private sealed record PublishedTileAssetSnapshot(
        string? ImageUrl,
        Dictionary<string, PublishedTranslationMedia> TranslationMediaByLang,
        Dictionary<string, string?> DialogNodeAudioByNodeAndLang);
}


