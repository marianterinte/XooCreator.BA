using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Extensions;
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

    public async Task<ExportResult> ExportPublishedStoryAsync(StoryDefinition def, string locale, ExportOptions options, CancellationToken ct)
    {
        var primaryLang = (locale ?? string.Empty).Trim().ToLowerInvariant();
        var fileName = $"{def.StoryId}-v{def.Version}.zip";
        var manifestPrefix = $"manifest/{def.StoryId}/";
        var hasDialogTiles = def.Tiles.Any(t => t.DialogTile?.Nodes != null && t.DialogTile.Nodes.Count > 0);
        var allMediaPaths = CollectPublishedMediaPaths(def);
        var mediaPaths = FilterPublishedMediaPathsByOptions(allMediaPaths, options).ToList();

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var zipData = BuildExportV2ZipData(def);
            WriteV2ZipFiles(zip, manifestPrefix, zipData);

            // Add only media paths that pass the options filter
            foreach (var path in mediaPaths)
            {
                try
                {
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
                }
            }
        }

        var zipBytes = ms.ToArray();
        return new ExportResult
        {
            ZipBytes = zipBytes,
            FileName = fileName,
            MediaCount = mediaPaths.Count,
            LanguageCount = def.Translations.Select(t => t.LanguageCode).Distinct().Count(),
            ZipSizeBytes = zipBytes.Length
        };
    }

    private static IEnumerable<string> FilterPublishedMediaPathsByOptions(List<string> paths, ExportOptions options)
    {
        foreach (var path in paths)
        {
            var kind = ClassifyPublishedPath(path);
            if (kind == AssetType.Video && !options.IncludeVideo) continue;
            if (kind == AssetType.Audio && !options.IncludeAudio) continue;
            if (kind == AssetType.Image && !options.IncludeImages) continue;
            yield return path;
        }
    }

    private static AssetType ClassifyPublishedPath(string path)
    {
        var p = (path ?? string.Empty).Trim().Replace('\\', '/').ToLowerInvariant();
        if (p.Contains("/audio/")) return AssetType.Audio;
        if (p.Contains("/video/")) return AssetType.Video;
        var ext = Path.GetExtension(p);
        if (ext is ".mp3" or ".wav" or ".ogg" or ".m4a") return AssetType.Audio;
        if (ext is ".mp4" or ".webm") return AssetType.Video;
        return AssetType.Image;
    }

    private static bool IncludeAssetByOptions(AssetType type, ExportOptions options)
    {
        return type switch
        {
            AssetType.Video => options.IncludeVideo,
            AssetType.Audio => options.IncludeAudio,
            AssetType.Image => options.IncludeImages,
            _ => options.IncludeImages
        };
    }

    public async Task<ExportResult> ExportDraftStoryAsync(StoryCraft craft, string locale, string ownerEmail, ExportOptions options, CancellationToken ct)
    {
        // Build export JSON
        // Build export JSON
        var primaryLang = (locale ?? string.Empty).Trim().ToLowerInvariant();
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
                var isCover = !string.IsNullOrWhiteSpace(craft.CoverImageUrl) &&
                              asset.Filename.Equals(craft.CoverImageUrl, StringComparison.OrdinalIgnoreCase);
                allAssets.Add((asset, blobPath, isCover));
            }
        }

        // Remove duplicates (same blob path) and filter by options
        var uniqueAssets = allAssets
            .GroupBy(a => a.BlobPath, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .Where(a => IncludeAssetByOptions(a.Asset.Type, options))
            .ToList();

        // Build ZIP
        var manifestPrefix = $"manifest/{craft.StoryId}/";
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var zipData = BuildExportV2ZipData(craft);
            WriteV2ZipFiles(zip, manifestPrefix, zipData);

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
    /// Writes the V2 files to the zip archive
    /// </summary>
    private static void WriteV2ZipFiles(ZipArchive zip, string manifestPrefix, (object Master, Dictionary<string, object> Languages, Dictionary<string, object> Dialogs) zipData)
    {
        var serializerOptions = new JsonSerializerOptions { WriteIndented = true };

        // Write master.json
        var masterEntry = zip.CreateEntry(manifestPrefix + "master.json", CompressionLevel.Fastest);
        using (var writer = new StreamWriter(masterEntry.Open(), new UTF8Encoding(false)))
        {
            writer.Write(JsonSerializer.Serialize(zipData.Master, serializerOptions));
        }

        // Write languages/{lang}.json
        foreach (var langPair in zipData.Languages)
        {
            var langEntry = zip.CreateEntry($"{manifestPrefix}languages/{langPair.Key}.json", CompressionLevel.Fastest);
            using (var writer = new StreamWriter(langEntry.Open(), new UTF8Encoding(false)))
            {
                writer.Write(JsonSerializer.Serialize(langPair.Value, serializerOptions));
            }
        }

        // Write dialogs/{lang}/{tileId}.json
        foreach (var dialogPair in zipData.Dialogs)
        {
            var dialogEntry = zip.CreateEntry($"{manifestPrefix}dialogs/{dialogPair.Key}.json", CompressionLevel.Fastest);
            using (var writer = new StreamWriter(dialogEntry.Open(), new UTF8Encoding(false)))
            {
                writer.Write(JsonSerializer.Serialize(dialogPair.Value, serializerOptions));
            }
        }
    }

    private static (object Master, Dictionary<string, object> Languages, Dictionary<string, object> Dialogs) BuildExportV2ZipData(StoryDefinition def)
    {
        var topicIds = def.Topics?.Where(t => t.StoryTopic != null).Select(t => t.StoryTopic!.TopicId).ToList() ?? new List<string>();
        var ageGroupIds = def.AgeGroups?.Where(ag => ag.StoryAgeGroup != null).Select(ag => ag.StoryAgeGroup!.AgeGroupId).ToList() ?? new List<string>();

        var masterTiles = def.Tiles.OrderBy(t => t.SortOrder).Select((t, i) => new
        {
            tileId = t.TileId,
            type = t.Type,
            sortOrder = i + 1,
            branchId = t.BranchId,
            imageUrl = t.ImageUrl.ToExportAssetUrl(),
            answers = t.Type != "quiz" ? null : (t.Answers ?? new()).OrderBy(a => a.SortOrder).Select((a, ai) => new
            {
                answerId = a.AnswerId,
                isCorrect = a.IsCorrect,
                tokens = (a.Tokens ?? new()).Select(tok => new { type = tok.Type, value = tok.Value, quantity = tok.Quantity }).ToList(),
                sortOrder = ai + 1
            }).ToList()
        }).ToList();

        var master = new
        {
            formatVersion = 2,
            id = def.StoryId,
            storyType = def.StoryType,
            coverImageUrl = def.CoverImageUrl.ToExportAssetUrl(),
            storyTopic = def.StoryTopic,
            topicIds = topicIds,
            ageGroupIds = ageGroupIds,
            priceInCredits = def.PriceInCredits,
            unlockedStoryHeroes = new List<string>(), // Simplified
            dialogParticipants = def.DialogParticipants.OrderBy(p => p.SortOrder).Select(p => p.HeroId).ToList(),
            audioLanguages = def.AudioLanguages ?? new List<string>(),
            tiles = masterTiles
        };

        var availableLangs = def.Translations.Select(t => t.LanguageCode).Distinct().ToList();
        var languages = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        var dialogs = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        foreach (var lang in availableLangs)
        {
            var lowerLang = lang.ToLowerInvariant();
            var storyTr = def.Translations.FirstOrDefault(t => t.LanguageCode.Equals(lang, StringComparison.OrdinalIgnoreCase));
            
            var langTiles = def.Tiles.Select(t =>
            {
                var tr = t.Translations.FirstOrDefault(x => x.LanguageCode.Equals(lang, StringComparison.OrdinalIgnoreCase));
                return new
                {
                    tileId = t.TileId,
                    caption = tr?.Caption ?? string.Empty,
                    text = tr?.Text ?? string.Empty,
                    question = tr?.Question ?? string.Empty,
                    audioUrl = tr?.AudioUrl.ToExportAssetUrl() ?? string.Empty,
                    videoUrl = tr?.VideoUrl.ToExportAssetUrl() ?? string.Empty,
                    answers = t.Type != "quiz" ? null : (t.Answers ?? new()).Select(a => new
                    {
                        answerId = a.AnswerId,
                        text = (a.Translations ?? new()).FirstOrDefault(at => at.LanguageCode.Equals(lang, StringComparison.OrdinalIgnoreCase))?.Text ?? string.Empty
                    })
                };
            }).ToList();

            languages[lowerLang] = new
            {
                formatVersion = 2,
                storyId = def.StoryId,
                languageCode = lowerLang,
                title = storyTr?.Title ?? def.Title,
                summary = def.Summary ?? string.Empty,
                tiles = langTiles
            };

            // Build dialogs manually for this lang
            foreach (var tile in def.Tiles.Where(t => t.Type == "dialog" && t.DialogTile?.Nodes?.Count > 0))
            {
                var dialogNodes = tile.DialogTile!.Nodes.OrderBy(n => n.SortOrder).Select(n =>
                {
                    var nodeTr = n.Translations.FirstOrDefault(nt => nt.LanguageCode.Equals(lang, StringComparison.OrdinalIgnoreCase));
                    return new
                    {
                        nodeId = n.NodeId,
                        speakerType = n.SpeakerType,
                        speakerHeroId = n.SpeakerHeroId,
                        text = nodeTr?.Text ?? string.Empty,
                        audioUrl = nodeTr?.AudioUrl.ToExportAssetUrl() ?? string.Empty,
                        options = n.OutgoingEdges.OrderBy(e => e.OptionOrder).Select(e =>
                        {
                            var optionTr = e.Translations.FirstOrDefault(et => et.LanguageCode.Equals(lang, StringComparison.OrdinalIgnoreCase));
                            return new
                            {
                                id = e.EdgeId,
                                nextNodeId = e.ToNodeId,
                                jumpToTileId = e.JumpToTileId,
                                setBranchId = e.SetBranchId,
                                hideIfBranchSet = e.HideIfBranchSet,
                                showOnlyIfBranchesSet = ParseShowOnlyIfBranchesSet(e.ShowOnlyIfBranchesSet),
                                sortOrder = e.OptionOrder,
                                tokens = (e.Tokens ?? new()).Select(tok => new { type = tok.Type, value = tok.Value, quantity = tok.Quantity }).ToList(),
                                text = optionTr?.OptionText ?? string.Empty
                            };
                        }).ToList()
                    };
                }).ToList();

                var key = $"{lowerLang}/{tile.TileId}";
                dialogs[key] = new
                {
                    formatVersion = 2,
                    storyId = def.StoryId,
                    languageCode = lowerLang,
                    tileId = tile.TileId,
                    dialogRootNodeId = tile.DialogTile.RootNodeId,
                    dialogNodes = dialogNodes
                };
            }
        }

        return (master, languages, dialogs);
    }

    private static (object Master, Dictionary<string, object> Languages, Dictionary<string, object> Dialogs) BuildExportV2ZipData(StoryCraft craft)
    {
        var topicIds = craft.Topics?.Where(t => t.StoryTopic != null).Select(t => t.StoryTopic!.TopicId).ToList() ?? new List<string>();
        var ageGroupIds = craft.AgeGroups?.Where(ag => ag.StoryAgeGroup != null).Select(ag => ag.StoryAgeGroup!.AgeGroupId).ToList() ?? new List<string>();
        var unlockedHeroes = craft.UnlockedHeroes.Select(h => h.HeroId).ToList();

        var masterTiles = craft.Tiles.OrderBy(t => t.SortOrder).Select((t, i) => new
        {
            tileId = t.TileId,
            type = t.Type,
            sortOrder = i + 1,
            branchId = t.BranchId,
            imageUrl = t.ImageUrl.ToExportAssetUrl(),
            answers = t.Type != "quiz" ? null : (t.Answers ?? new()).OrderBy(a => a.SortOrder).Select((a, ai) => new
            {
                answerId = a.AnswerId,
                isCorrect = a.IsCorrect,
                tokens = (a.Tokens ?? new()).Select(tok => new { type = tok.Type, value = tok.Value, quantity = tok.Quantity }).ToList(),
                sortOrder = ai + 1
            }).ToList()
        }).ToList();

        var master = new
        {
            formatVersion = 2,
            id = craft.StoryId,
            storyType = craft.StoryType,
            coverImageUrl = craft.CoverImageUrl.ToExportAssetUrl(),
            storyTopic = craft.StoryTopic,
            topicIds = topicIds,
            ageGroupIds = ageGroupIds,
            priceInCredits = craft.PriceInCredits,
            unlockedStoryHeroes = unlockedHeroes,
            dialogParticipants = craft.DialogParticipants.OrderBy(p => p.SortOrder).Select(p => p.HeroId).ToList(),
            audioLanguages = craft.AudioLanguages ?? new List<string>(),
            tiles = masterTiles
        };

        var availableLangs = craft.Translations.Select(t => t.LanguageCode).Distinct().ToList();
        var languages = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        var dialogs = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        // In draft, some language data might only exist in the tile translations. Ensure we gather all active languages.
        var allTileLangs = craft.Tiles.SelectMany(t => t.Translations.Select(tr => tr.LanguageCode)).Distinct().ToList();
        foreach (var l in allTileLangs)
        {
            if (!availableLangs.Contains(l, StringComparer.OrdinalIgnoreCase))
            {
                availableLangs.Add(l);
            }
        }

        foreach (var lang in availableLangs)
        {
            var lowerLang = lang.ToLowerInvariant();
            var storyTr = craft.Translations.FirstOrDefault(t => t.LanguageCode.Equals(lang, StringComparison.OrdinalIgnoreCase));
            
            var langTiles = craft.Tiles.Select(t =>
            {
                var tr = t.Translations.FirstOrDefault(x => x.LanguageCode.Equals(lang, StringComparison.OrdinalIgnoreCase));
                return new
                {
                    tileId = t.TileId,
                    caption = tr?.Caption ?? string.Empty,
                    text = tr?.Text ?? string.Empty,
                    question = tr?.Question ?? string.Empty,
                    audioUrl = tr?.AudioUrl.ToExportAssetUrl() ?? string.Empty,
                    videoUrl = tr?.VideoUrl.ToExportAssetUrl() ?? string.Empty,
                    answers = t.Type != "quiz" ? null : (t.Answers ?? new()).Select(a => new
                    {
                        answerId = a.AnswerId,
                        text = (a.Translations ?? new()).FirstOrDefault(at => at.LanguageCode.Equals(lang, StringComparison.OrdinalIgnoreCase))?.Text ?? string.Empty
                    })
                };
            }).ToList();

            languages[lowerLang] = new
            {
                formatVersion = 2,
                storyId = craft.StoryId,
                languageCode = lowerLang,
                title = storyTr?.Title ?? string.Empty,
                summary = storyTr?.Summary ?? string.Empty, // Draft has summary localized
                tiles = langTiles
            };

            // Build dialogs manually for this lang
            foreach (var tile in craft.Tiles.Where(t => t.Type == "dialog" && t.DialogTile?.Nodes?.Count > 0))
            {
                var dialogNodes = tile.DialogTile!.Nodes.OrderBy(n => n.SortOrder).Select(n =>
                {
                    var nodeTr = n.Translations.FirstOrDefault(nt => nt.LanguageCode.Equals(lang, StringComparison.OrdinalIgnoreCase));
                    return new
                    {
                        nodeId = n.NodeId,
                        speakerType = n.SpeakerType,
                        speakerHeroId = n.SpeakerHeroId,
                        text = nodeTr?.Text ?? string.Empty,
                        audioUrl = nodeTr?.AudioUrl.ToExportAssetUrl() ?? string.Empty,
                        options = n.OutgoingEdges.OrderBy(e => e.OptionOrder).Select(e =>
                        {
                            var optionTr = e.Translations.FirstOrDefault(et => et.LanguageCode.Equals(lang, StringComparison.OrdinalIgnoreCase));
                            return new
                            {
                                id = e.EdgeId,
                                nextNodeId = e.ToNodeId,
                                jumpToTileId = e.JumpToTileId,
                                setBranchId = e.SetBranchId,
                                hideIfBranchSet = e.HideIfBranchSet,
                                showOnlyIfBranchesSet = ParseShowOnlyIfBranchesSet(e.ShowOnlyIfBranchesSet),
                                sortOrder = e.OptionOrder,
                                tokens = (e.Tokens ?? new()).Select(tok => new { type = tok.Type, value = tok.Value, quantity = tok.Quantity }).ToList(),
                                text = optionTr?.OptionText ?? string.Empty
                            };
                        }).ToList()
                    };
                }).ToList();

                var key = $"{lowerLang}/{tile.TileId}";
                dialogs[key] = new
                {
                    formatVersion = 2,
                    storyId = craft.StoryId,
                    languageCode = lowerLang,
                    tileId = tile.TileId,
                    dialogRootNodeId = tile.DialogTile.RootNodeId,
                    dialogNodes = dialogNodes
                };
            }
        }

        return (master, languages, dialogs);
    }

    private static List<string>? ParseShowOnlyIfBranchesSet(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json);
        }
        catch
        {
            return null;
        }
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

            // Dialog node audio (per node translation)
            if (t.DialogTile?.Nodes != null)
            {
                foreach (var node in t.DialogTile.Nodes)
                {
                    foreach (var nodeTr in node.Translations ?? new List<StoryDialogNodeTranslation>())
                    {
                        if (!string.IsNullOrWhiteSpace(nodeTr.AudioUrl))
                            result.Add(Normalize(nodeTr.AudioUrl));
                    }
                }
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
