using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Infrastructure.Services.Blob;
using static XooCreator.BA.Features.StoryEditor.Mappers.StoryAssetPathMapper;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryExportService : IStoryExportService
{
    private readonly IBlobSasService _sas;
    private readonly ILogger<StoryExportService> _logger;

    public StoryExportService(
        IBlobSasService sas,
        ILogger<StoryExportService> logger)
    {
        _sas = sas;
        _logger = logger;
    }

    public async Task<ExportResult> ExportPublishedStoryAsync(StoryDefinition def, string locale, CancellationToken ct)
    {
        var primaryLang = (locale ?? string.Empty).Trim().ToLowerInvariant();
        var exportObj = BuildExportJson(def, primaryLang);
        var exportJson = JsonSerializer.Serialize(exportObj, new JsonSerializerOptions { WriteIndented = true });
        var fileName = $"{def.StoryId}-v{def.Version}.zip";
        var manifestPrefix = $"manifest/{def.StoryId}/v{def.Version}/";

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            // Add manifest JSON (with optional split: dialogs/ when story has dialog tiles with nodes)
            var manifestJson = ApplySplitFormatIfDialogsPresent(exportJson, zip, manifestPrefix);
            var manifestEntry = zip.CreateEntry(manifestPrefix + "story.json", CompressionLevel.Fastest);
            await using (var writer = new StreamWriter(manifestEntry.Open(), new UTF8Encoding(false)))
            {
                await writer.WriteAsync(manifestJson);
            }

            // Collect media paths from definition (already in published layout)
            var mediaPaths = CollectPublishedMediaPaths(def);
            foreach (var path in mediaPaths)
            {
                try
                {
                    // Download from published container and add to ZIP preserving relative path under "media/"
                    var client = _sas.GetBlobClient(_sas.PublishedContainer, path);
                    if (!await client.ExistsAsync(ct))
                    {
                        _logger.LogWarning("Asset not found in published storage: {Path}", path);
                        continue;
                    }

                    var entry = zip.CreateEntry($"media/{path}".Replace('\\', '/'), CompressionLevel.Fastest);
                    await using var entryStream = entry.Open();
                    var download = await client.DownloadStreamingAsync(cancellationToken: ct);
                    await download.Value.Content.CopyToAsync(entryStream, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to download asset from published storage: {Path}", path);
                    // Continue with other assets even if one fails
                }
            }
        }

        var zipBytes = ms.ToArray();
        return new ExportResult
        {
            ZipBytes = zipBytes,
            FileName = fileName,
            MediaCount = CollectPublishedMediaPaths(def).Count,
            LanguageCount = def.Translations.Select(t => t.LanguageCode).Distinct().Count(),
            ZipSizeBytes = zipBytes.Length
        };
    }

    public async Task<ExportResult> ExportDraftStoryAsync(StoryCraft craft, string locale, string ownerEmail, CancellationToken ct)
    {
        // Build export JSON
        var primaryLang = (locale ?? string.Empty).Trim().ToLowerInvariant();
        var exportObj = BuildExportJson(craft, primaryLang);
        var exportJson = JsonSerializer.Serialize(exportObj, new JsonSerializerOptions { WriteIndented = true });
        var fileName = $"{craft.StoryId}-draft-export.zip";

        // Collect all assets for all languages with metadata
        var allAssets = new List<(AssetInfo Asset, string BlobPath, bool IsCover)>();

        // Important: languages may exist in tile/answer translations even if story-level translation is missing.
        var availableLanguages = craft.Translations.Select(t => t.LanguageCode)
            .Concat(craft.Tiles.SelectMany(t => t.Translations.Select(tr => tr.LanguageCode)))
            .Concat(craft.Tiles.SelectMany(t => (t.Answers ?? new()).SelectMany(a => (a.Translations ?? new()).Select(at => at.LanguageCode))))
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l!.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();

        // Extract assets for each language
        foreach (var lang in availableLanguages)
        {
            var assets = StoryAssetPathMapper.ExtractAssets(craft, lang);
            foreach (var asset in assets)
            {
                var blobPath = StoryAssetPathMapper.BuildDraftPath(asset, ownerEmail, craft.StoryId);
                // Cover can be image or video (language-agnostic)
                var isCover = !string.IsNullOrWhiteSpace(craft.CoverImageUrl) &&
                              asset.Filename.Equals(craft.CoverImageUrl, StringComparison.OrdinalIgnoreCase);
                allAssets.Add((asset, blobPath, isCover));
            }
        }

        // Remove duplicates (same blob path)
        var uniqueAssets = allAssets
            .GroupBy(a => a.BlobPath, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        // Build ZIP
        var manifestPrefix = $"manifest/{craft.StoryId}/";
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            // Add manifest JSON (with optional split: dialogs/ when story has dialog tiles with nodes)
            var manifestJson = ApplySplitFormatIfDialogsPresent(exportJson, zip, manifestPrefix);
            var manifestEntry = zip.CreateEntry(manifestPrefix + "story.json", CompressionLevel.Fastest);
            await using (var writer = new StreamWriter(manifestEntry.Open(), new UTF8Encoding(false)))
            {
                await writer.WriteAsync(manifestJson);
            }

            // Download and add media files from draft container
            foreach (var (asset, blobPath, isCover) in uniqueAssets)
            {
                try
                {
                    var client = _sas.GetBlobClient(_sas.DraftContainer, blobPath);

                    // Check if blob exists
                    if (!await client.ExistsAsync(ct))
                    {
                        _logger.LogWarning("Asset not found in draft storage: {BlobPath}", blobPath);
                        continue;
                    }

                    // Build ZIP entry path using asset metadata
                    var zipEntryPath = BuildZipEntryPath(asset, isCover);
                    var entry = zip.CreateEntry(zipEntryPath, CompressionLevel.Fastest);

                    await using var entryStream = entry.Open();
                    var download = await client.DownloadStreamingAsync(cancellationToken: ct);
                    await download.Value.Content.CopyToAsync(entryStream, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to download asset from draft: {BlobPath}", blobPath);
                    // Continue with other assets even if one fails
                }
            }
        }

        var zipBytes = ms.ToArray();
        return new ExportResult
        {
            ZipBytes = zipBytes,
            FileName = fileName,
            MediaCount = uniqueAssets.Count,
            LanguageCount = availableLanguages.Count,
            ZipSizeBytes = zipBytes.Length
        };
    }

    /// <summary>
    /// If the export JSON contains any dialog tile with nodes, writes split format: main manifest with
    /// dialogRef + empty dialogNodes for those tiles, and separate dialogs/{tileId}.json files.
    /// Returns the (possibly modified) manifest JSON string to write to story.json.
    /// </summary>
    private static string ApplySplitFormatIfDialogsPresent(string exportJson, ZipArchive zip, string manifestPrefix)
    {
        var root = JsonNode.Parse(exportJson);
        if (root == null) return exportJson;

        var tiles = root["tiles"] as JsonArray;
        if (tiles == null || tiles.Count == 0) return exportJson;

        var hasAnyDialogWithNodes = false;
        foreach (var tile in tiles)
        {
            if (tile is not JsonObject tileObj) continue;
            var type = tileObj["type"]?.GetValue<string>();
            if (!string.Equals(type, "dialog", StringComparison.OrdinalIgnoreCase)) continue;
            var dialogNodes = tileObj["dialogNodes"] as JsonArray;
            if (dialogNodes == null || dialogNodes.Count == 0) continue;

            hasAnyDialogWithNodes = true;
            break;
        }

        if (!hasAnyDialogWithNodes) return exportJson;

        var serializerOptions = new JsonSerializerOptions { WriteIndented = true };

        foreach (var tile in tiles)
        {
            if (tile is not JsonObject tileObj) continue;
            var type = tileObj["type"]?.GetValue<string>();
            if (!string.Equals(type, "dialog", StringComparison.OrdinalIgnoreCase)) continue;
            var dialogNodes = tileObj["dialogNodes"] as JsonArray;
            if (dialogNodes == null || dialogNodes.Count == 0) continue;

            var tileId = tileObj["id"]?.GetValue<string>() ?? "dialog";
            var dialogRootNodeId = tileObj["dialogRootNodeId"]?.GetValue<string>() ?? string.Empty;

            // Clone dialog nodes for the separate file (JsonNode allows only one parent)
            var dialogNodesCopy = JsonNode.Parse(dialogNodes.ToJsonString());
            var dialogPayload = new JsonObject
            {
                ["dialogRootNodeId"] = dialogRootNodeId,
                ["dialogNodes"] = dialogNodesCopy
            };
            var dialogJson = dialogPayload.ToJsonString(serializerOptions);
            var dialogEntry = zip.CreateEntry(manifestPrefix + "dialogs/" + tileId + ".json", CompressionLevel.Fastest);
            using (var writer = new StreamWriter(dialogEntry.Open(), new UTF8Encoding(false)))
            {
                writer.Write(dialogJson);
            }

            tileObj["dialogRef"] = "dialogs/" + tileId + ".json";
            tileObj["dialogNodes"] = new JsonArray();
        }

        return root.ToJsonString(serializerOptions);
    }

    private static object BuildExportJson(StoryDefinition def, string primaryLang)
    {
        // Extract topic IDs
        var topicIds = def.Topics?
            .Where(t => t.StoryTopic != null)
            .Select(t => t.StoryTopic!.TopicId)
            .ToList() ?? new List<string>();

        // Extract age group IDs
        var ageGroupIds = def.AgeGroups?
            .Where(ag => ag.StoryAgeGroup != null)
            .Select(ag => ag.StoryAgeGroup!.AgeGroupId)
            .ToList() ?? new List<string>();

        return new
        {
            id = def.StoryId,
            version = def.Version,
            // Published definitions only have translated Title (no translated Summary field on StoryDefinitionTranslation)
            title = def.Translations.FirstOrDefault(t => t.LanguageCode == primaryLang)?.Title ?? def.Title,
            summary = def.Summary,
            storyType = def.StoryType,
            coverImageUrl = def.CoverImageUrl,
            storyTopic = def.StoryTopic,
            topicIds = topicIds,
            ageGroupIds = ageGroupIds,
            authorName = def.AuthorName,
            classicAuthorId = def.ClassicAuthorId,
            priceInCredits = def.PriceInCredits,
            isEvaluative = def.IsEvaluative,
            isFullyInteractive = def.IsFullyInteractive,
            audioLanguages = def.AudioLanguages ?? new List<string>(),
            dialogParticipants = def.DialogParticipants.OrderBy(p => p.SortOrder).Select(p => p.HeroId).ToList(),
            translations = def.Translations.Select(t => new
            {
                lang = t.LanguageCode,
                title = t.Title
            }).ToList(),
            tiles = def.Tiles
                .OrderBy(t => t.SortOrder)
                .Select(t => new
                {
                    id = t.TileId,
                    type = t.Type,
                    branchId = t.BranchId,
                    sortOrder = t.SortOrder,
                    imageUrl = t.ImageUrl,
                    translations = t.Translations.Select(tr => new
                    {
                        lang = tr.LanguageCode,
                        caption = tr.Caption,
                        text = tr.Text,
                        question = tr.Question,
                        audioUrl = tr.AudioUrl,
                        videoUrl = tr.VideoUrl
                    }).ToList(),
                    dialogRootNodeId = t.DialogTile?.RootNodeId,
                    dialogNodes = t.DialogTile?.Nodes.OrderBy(n => n.SortOrder).Select(n => new
                    {
                        nodeId = n.NodeId,
                        speakerType = n.SpeakerType,
                        speakerHeroId = n.SpeakerHeroId,
                        translations = n.Translations.Select(nt => new { lang = nt.LanguageCode, text = nt.Text }).ToList(),
                        options = n.OutgoingEdges.OrderBy(e => e.OptionOrder).Select(e => new
                        {
                            id = e.EdgeId,
                            nextNodeId = e.ToNodeId,
                            jumpToTileId = e.JumpToTileId,
                            setBranchId = e.SetBranchId,
                            sortOrder = e.OptionOrder,
                            tokens = (e.Tokens ?? new()).Select(tok => new { type = tok.Type, value = tok.Value, quantity = tok.Quantity }).ToList(),
                            translations = e.Translations.Select(et => new { lang = et.LanguageCode, text = et.OptionText }).ToList()
                        }).ToList()
                    }).ToList(),
                    answers = (t.Answers ?? new()).OrderBy(a => a.SortOrder).Select(a => new
                    {
                        id = a.AnswerId,
                        sortOrder = a.SortOrder,
                        isCorrect = a.IsCorrect,
                        text = a.Text,
                        tokens = (a.Tokens ?? new()).Select(tok => new { type = tok.Type, value = tok.Value, quantity = tok.Quantity }).ToList(),
                        translations = (a.Translations ?? new()).Select(at => new
                        {
                            lang = at.LanguageCode,
                            text = at.Text
                        }).ToList()
                    }).ToList()
                }).ToList()
        };
    }

    private static object BuildExportJson(StoryCraft craft, string primaryLang)
    {
        // Get primary translation (first one or default)
        var primaryTranslation = craft.Translations.FirstOrDefault(t => t.LanguageCode == primaryLang)
                                 ?? craft.Translations.FirstOrDefault()
                                 ?? new StoryCraftTranslation
        {
            LanguageCode = "ro-ro",
            Title = string.Empty,
            Summary = null
        };

        // Extract topic IDs
        var topicIds = craft.Topics?
            .Where(t => t.StoryTopic != null)
            .Select(t => t.StoryTopic!.TopicId)
            .ToList() ?? new List<string>();

        // Extract age group IDs
        var ageGroupIds = craft.AgeGroups?
            .Where(ag => ag.StoryAgeGroup != null)
            .Select(ag => ag.StoryAgeGroup!.AgeGroupId)
            .ToList() ?? new List<string>();

        // Load UnlockedStoryHeroes from many-to-many table
        var unlockedHeroes = craft.UnlockedHeroes.Select(h => h.HeroId).ToList();

        return new
        {
            id = craft.StoryId,
            version = craft.BaseVersion > 0 ? craft.BaseVersion : 0,
            title = primaryTranslation.Title,
            summary = primaryTranslation.Summary ?? craft.StoryTopic,
            storyType = craft.StoryType,
            coverImageUrl = craft.CoverImageUrl,
            storyTopic = craft.StoryTopic,
            topicIds = topicIds,
            ageGroupIds = ageGroupIds,
            authorName = craft.AuthorName,
            classicAuthorId = craft.ClassicAuthorId,
            priceInCredits = craft.PriceInCredits,
            isEvaluative = craft.IsEvaluative,
            isFullyInteractive = craft.IsFullyInteractive,
            audioLanguages = craft.AudioLanguages ?? new List<string>(),
            unlockedStoryHeroes = unlockedHeroes,
            dialogParticipants = craft.DialogParticipants.OrderBy(p => p.SortOrder).Select(p => p.HeroId).ToList(),
            translations = craft.Translations.Select(t => new
            {
                lang = t.LanguageCode,
                title = t.Title,
                summary = t.Summary
            }).ToList(),
            tiles = craft.Tiles
                .OrderBy(t => t.SortOrder)
                .Select(t => new
                {
                    id = t.TileId,
                    type = t.Type,
                    branchId = t.BranchId,
                    sortOrder = t.SortOrder,
                    imageUrl = t.ImageUrl,
                    translations = t.Translations.Select(tr => new
                    {
                        lang = tr.LanguageCode,
                        caption = tr.Caption,
                        text = tr.Text,
                        question = tr.Question,
                        audioUrl = tr.AudioUrl,
                        videoUrl = tr.VideoUrl
                    }).ToList(),
                    dialogRootNodeId = t.DialogTile?.RootNodeId,
                    dialogNodes = t.DialogTile?.Nodes.OrderBy(n => n.SortOrder).Select(n => new
                    {
                        nodeId = n.NodeId,
                        speakerType = n.SpeakerType,
                        speakerHeroId = n.SpeakerHeroId,
                        translations = n.Translations.Select(nt => new { lang = nt.LanguageCode, text = nt.Text }).ToList(),
                        options = n.OutgoingEdges.OrderBy(e => e.OptionOrder).Select(e => new
                        {
                            id = e.EdgeId,
                            nextNodeId = e.ToNodeId,
                            jumpToTileId = e.JumpToTileId,
                            setBranchId = e.SetBranchId,
                            sortOrder = e.OptionOrder,
                            tokens = (e.Tokens ?? new()).Select(tok => new { type = tok.Type, value = tok.Value, quantity = tok.Quantity }).ToList(),
                            translations = e.Translations.Select(et => new { lang = et.LanguageCode, text = et.OptionText }).ToList()
                        }).ToList()
                    }).ToList(),
                    answers = (t.Answers ?? new()).OrderBy(a => a.SortOrder).Select(a => new
                    {
                        id = a.AnswerId,
                        sortOrder = a.SortOrder,
                        isCorrect = a.IsCorrect,
                        // Convenience field: export answer text for the primary language at top-level.
                        // (Draft answers store text per-language in Translations.)
                        text = (a.Translations ?? new()).FirstOrDefault(at => at.LanguageCode == primaryTranslation.LanguageCode)?.Text
                               ?? (a.Translations ?? new()).FirstOrDefault()?.Text
                               ?? string.Empty,
                        tokens = (a.Tokens ?? new()).Select(tok => new { type = tok.Type, value = tok.Value, quantity = tok.Quantity }).ToList(),
                        translations = (a.Translations ?? new()).Select(at => new
                        {
                            lang = at.LanguageCode,
                            text = at.Text
                        }).ToList()
                    }).ToList()
                }).ToList()
        };
    }

    private static List<string> CollectPublishedMediaPaths(StoryDefinition def)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(def.CoverImageUrl)) result.Add(Normalize(def.CoverImageUrl));
        foreach (var t in def.Tiles)
        {
            // Image is common for all languages
            if (!string.IsNullOrWhiteSpace(t.ImageUrl)) result.Add(Normalize(t.ImageUrl));
            
            // Audio and Video are now language-specific (in translations)
            foreach (var tr in t.Translations)
            {
                if (!string.IsNullOrWhiteSpace(tr.AudioUrl)) result.Add(Normalize(tr.AudioUrl));
                if (!string.IsNullOrWhiteSpace(tr.VideoUrl)) result.Add(Normalize(tr.VideoUrl));
            }
        }
        return result.ToList();
    }

    private static string Normalize(string path)
    {
        return path.TrimStart('/').Replace('\\', '/');
    }

    private static string BuildZipEntryPath(AssetInfo asset, bool isCover)
    {
        var mediaType = asset.Type switch
        {
            StoryAssetPathMapper.AssetType.Image => "images",
            StoryAssetPathMapper.AssetType.Audio => "audio",
            StoryAssetPathMapper.AssetType.Video => "video",
            _ => "images"
        };

        // Cover (image or video) is language-agnostic
        if (isCover)
        {
            return $"media/{mediaType}/cover/{asset.Filename}";
        }

        // Tile images are language-agnostic
        if (asset.Type == StoryAssetPathMapper.AssetType.Image)
        {
            return $"media/{mediaType}/tiles/{asset.Filename}";
        }

        // Audio and Video are language-specific
        if (!string.IsNullOrWhiteSpace(asset.Lang))
        {
            return $"media/{mediaType}/{asset.Lang}/{asset.Filename}";
        }

        return $"media/{mediaType}/{asset.Filename}";
    }
}
