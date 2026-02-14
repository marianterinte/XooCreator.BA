using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.SeedData;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Infrastructure.Services.Blob;
using Azure.Storage.Blobs.Models;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryAssetCopyService
{
    List<StoryAssetPathMapper.AssetInfo> CollectFromCraft(StoryCraft craft);
    List<StoryAssetPathMapper.AssetInfo> CollectFromDefinition(StoryDefinition definition);

    Task<AssetCopyResult> CopyDraftToDraftAsync(
        IEnumerable<StoryAssetPathMapper.AssetInfo> assets,
        string sourceEmail,
        string sourceStoryId,
        string targetEmail,
        string targetStoryId,
        CancellationToken ct);

    Task<AssetCopyResult> CopyPublishedToDraftAsync(
        IEnumerable<StoryAssetPathMapper.AssetInfo> assets,
        string publishedOwnerEmail,
        string sourceStoryId,
        string targetEmail,
        string targetStoryId,
        CancellationToken ct);
}

/// <summary>
/// New asset copy service dedicated to the Copy/Fork/Version flows.
/// Implementation is intentionally minimal for now; logic will be filled
/// once Copy/Fork endpoints are wired end-to-end.
/// </summary>
public class StoryAssetCopyService : IStoryAssetCopyService
{
    private readonly IBlobSasService _sasService;
    private readonly ILogger<StoryAssetCopyService> _logger;

    public StoryAssetCopyService(
        IBlobSasService sasService,
        ILogger<StoryAssetCopyService> logger)
    {
        _sasService = sasService;
        _logger = logger;
    }

    public List<StoryAssetPathMapper.AssetInfo> CollectFromCraft(StoryCraft craft)
    {
        ArgumentNullException.ThrowIfNull(craft);
        var results = new List<StoryAssetPathMapper.AssetInfo>();

        if (!string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            var coverType = StoryAssetPathMapper.GetCoverAssetType(craft.CoverImageUrl);
            results.Add(new StoryAssetPathMapper.AssetInfo(craft.CoverImageUrl, coverType, null));
        }

        foreach (var tile in craft.Tiles)
        {
            if (!string.IsNullOrWhiteSpace(tile.ImageUrl))
            {
                results.Add(new StoryAssetPathMapper.AssetInfo(tile.ImageUrl, StoryAssetPathMapper.AssetType.Image, null));
            }

            foreach (var translation in tile.Translations)
            {
                if (!string.IsNullOrWhiteSpace(translation.AudioUrl))
                {
                    results.Add(new StoryAssetPathMapper.AssetInfo(translation.AudioUrl, StoryAssetPathMapper.AssetType.Audio, translation.LanguageCode));
                }
                if (!string.IsNullOrWhiteSpace(translation.VideoUrl))
                {
                    results.Add(new StoryAssetPathMapper.AssetInfo(translation.VideoUrl, StoryAssetPathMapper.AssetType.Video, translation.LanguageCode));
                }
            }

            if (tile.DialogTile != null)
            {
                foreach (var node in tile.DialogTile.Nodes)
                {
                    foreach (var nodeTr in node.Translations)
                    {
                        if (!string.IsNullOrWhiteSpace(nodeTr.AudioUrl))
                        {
                            results.Add(new StoryAssetPathMapper.AssetInfo(nodeTr.AudioUrl, StoryAssetPathMapper.AssetType.Audio, nodeTr.LanguageCode));
                        }
                    }
                    // Option audio not used: only node (replica) audio
                }
            }
        }

        return results;
    }

    public List<StoryAssetPathMapper.AssetInfo> CollectFromDefinition(StoryDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        var results = new List<StoryAssetPathMapper.AssetInfo>();

        var isSeeded = LooksLikeSeededStory(definition);

        _logger.LogInformation(
            "CollectFromDefinition started: storyId={StoryId} tiles={TileCount} looksSeeded={IsSeeded}",
            definition.StoryId, definition.Tiles.Count, isSeeded);

        if (!string.IsNullOrWhiteSpace(definition.CoverImageUrl))
        {
            var coverFilename = Path.GetFileName(definition.CoverImageUrl);
            var coverType = StoryAssetPathMapper.GetCoverAssetType(definition.CoverImageUrl);
            results.Add(new StoryAssetPathMapper.AssetInfo(coverFilename, coverType, null));
            _logger.LogInformation("Collected cover: {Filename} type={Type}", coverFilename, coverType);
        }

        foreach (var tile in definition.Tiles)
        {
            var tileImageFilename = Path.GetFileName(tile.ImageUrl);

            if (!string.IsNullOrWhiteSpace(tileImageFilename))
            {
                results.Add(new StoryAssetPathMapper.AssetInfo(tileImageFilename, StoryAssetPathMapper.AssetType.Image, null));
                _logger.LogInformation("Collected tile image: {Filename}", tileImageFilename);
            }

            _logger.LogInformation(
                "Processing tile: tileId={TileId} translations={TranslationCount}",
                tile.TileId, tile.Translations.Count);

            foreach (var translation in tile.Translations)
            {
                _logger.LogInformation(
                    "Processing translation: lang={Lang} audioUrl={AudioUrl} videoUrl={VideoUrl}",
                    translation.LanguageCode,
                    translation.AudioUrl ?? "null",
                    translation.VideoUrl ?? "null");

                var audioFilename = Path.GetFileName(translation.AudioUrl);
                if (string.IsNullOrWhiteSpace(audioFilename) && isSeeded)
                {
                    audioFilename = DeriveSeededAudioFilename(tileImageFilename);
                    if (!string.IsNullOrWhiteSpace(audioFilename))
                    {
                        _logger.LogInformation(
                            "Seeded fallback audio detected: tileId={TileId} lang={Lang} derivedFilename={Filename}",
                            tile.TileId,
                            translation.LanguageCode,
                            audioFilename);
                    }
                }

                if (!string.IsNullOrWhiteSpace(audioFilename))
                {
                    results.Add(new StoryAssetPathMapper.AssetInfo(audioFilename, StoryAssetPathMapper.AssetType.Audio, translation.LanguageCode));
                }

                if (!string.IsNullOrWhiteSpace(translation.VideoUrl))
                {
                    var filename = Path.GetFileName(translation.VideoUrl);
                    results.Add(new StoryAssetPathMapper.AssetInfo(filename, StoryAssetPathMapper.AssetType.Video, translation.LanguageCode));
                    _logger.LogInformation(
                        "Collected video: filename={Filename} lang={Lang} fullUrl={FullUrl}",
                        filename, translation.LanguageCode, translation.VideoUrl);
                }
            }

            if (tile.DialogTile != null)
            {
                foreach (var node in tile.DialogTile.Nodes)
                {
                    foreach (var nodeTr in node.Translations)
                    {
                        if (!string.IsNullOrWhiteSpace(nodeTr.AudioUrl))
                        {
                            var dialogAudioFilename = Path.GetFileName(nodeTr.AudioUrl);
                            results.Add(new StoryAssetPathMapper.AssetInfo(dialogAudioFilename, StoryAssetPathMapper.AssetType.Audio, nodeTr.LanguageCode));
                        }
                    }
                    // Option audio not used: only node (replica) audio
                }
            }
        }

        _logger.LogInformation(
            "CollectFromDefinition completed: storyId={StoryId} totalAssets={AssetCount}",
            definition.StoryId, results.Count);

        return results;
    }

    public async Task<AssetCopyResult> CopyDraftToDraftAsync(
        IEnumerable<StoryAssetPathMapper.AssetInfo> assets,
        string sourceEmail,
        string sourceStoryId,
        string targetEmail,
        string targetStoryId,
        CancellationToken ct)
    {
        _logger.LogInformation(
            "Draft → Draft asset copy started: source={SourceEmail}/{SourceStoryId} target={TargetEmail}/{TargetStoryId} assets={Count}",
            sourceEmail,
            sourceStoryId,
            targetEmail,
            targetStoryId,
            assets.Count());

        foreach (var asset in assets)
        {
            var sourcePath = StoryAssetPathMapper.BuildDraftPath(asset, sourceEmail, sourceStoryId);
            var sourceClient = _sasService.GetBlobClient(_sasService.DraftContainer, sourcePath);

            if (!await sourceClient.ExistsAsync(ct))
            {
                _logger.LogWarning(
                    "Source asset not found in draft: source={SourceEmail}/{SourceStoryId} filename={Filename} path={Path}",
                    sourceEmail, sourceStoryId, asset.Filename, sourcePath);
                continue;
            }

            var targetPath = StoryAssetPathMapper.BuildDraftPath(asset, targetEmail, targetStoryId);
            var copyResult = await CopyAssetWithPollingAsync(
                sourcePath,
                targetPath,
                asset,
                targetEmail,
                targetStoryId,
                _sasService.DraftContainer,
                _sasService.DraftContainer,
                ct);

            if (copyResult.HasError)
            {
                return copyResult;
            }
        }

        _logger.LogInformation(
            "Draft → Draft asset copy completed: source={SourceEmail}/{SourceStoryId} target={TargetEmail}/{TargetStoryId}",
            sourceEmail, sourceStoryId, targetEmail, targetStoryId);

        return AssetCopyResult.Success();
    }

    private static bool LooksLikeSeededStory(StoryDefinition definition)
    {
        if (definition == null) return false;
        return LooksLikeSeededPath(definition.CoverImageUrl)
               || definition.Tiles.Any(t => LooksLikeSeededPath(t.ImageUrl));
    }

    private static bool LooksLikeSeededPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        return path.Contains(SeedUserHelper.SeedUserEmail, StringComparison.OrdinalIgnoreCase)
               || path.Contains("/tol/stories/", StringComparison.OrdinalIgnoreCase);
    }

    private static string? DeriveSeededAudioFilename(string? imageFilename)
    {
        if (string.IsNullOrWhiteSpace(imageFilename)) return null;
        var name = Path.GetFileNameWithoutExtension(imageFilename);
        if (string.IsNullOrWhiteSpace(name)) return null;
        return $"{name}.wav";
    }

    public async Task<AssetCopyResult> CopyPublishedToDraftAsync(
        IEnumerable<StoryAssetPathMapper.AssetInfo> assets,
        string publishedOwnerEmail,
        string sourceStoryId,
        string targetEmail,
        string targetStoryId,
        CancellationToken ct)
    {
        _logger.LogInformation(
            "Published → Draft asset copy started: publishedOwner={OwnerEmail} source={SourceStoryId} target={TargetEmail}/{TargetStoryId} assets={Count}",
            publishedOwnerEmail,
            sourceStoryId,
            targetEmail,
            targetStoryId,
            assets.Count());

        foreach (var asset in assets)
        {
            var sourcePath = StoryAssetPathMapper.BuildPublishedPath(asset, publishedOwnerEmail, sourceStoryId);
            
            _logger.LogInformation(
                "Processing asset: filename={Filename} type={Type} lang={Lang} sourcePath={SourcePath}",
                asset.Filename, asset.Type, asset.Lang ?? "null", sourcePath);
            
            var sourceClient = _sasService.GetBlobClient(_sasService.PublishedContainer, sourcePath);

            if (!await sourceClient.ExistsAsync(ct))
            {
                _logger.LogWarning(
                    "Source asset not found in published: owner={OwnerEmail} storyId={StoryId} filename={Filename} path={Path}",
                    publishedOwnerEmail, sourceStoryId, asset.Filename, sourcePath);
                continue;
            }

            _logger.LogInformation("Source asset exists, copying: {SourcePath}", sourcePath);

            var targetPath = StoryAssetPathMapper.BuildDraftPath(asset, targetEmail, targetStoryId);
            _logger.LogInformation("Target path: {TargetPath}", targetPath);
            
            var copyResult = await CopyAssetWithPollingAsync(
                sourcePath,
                targetPath,
                asset,
                targetEmail,
                targetStoryId,
                _sasService.PublishedContainer,
                _sasService.DraftContainer,
                ct);

            if (copyResult.HasError)
            {
                return copyResult;
            }
        }

        _logger.LogInformation(
            "Published → Draft asset copy completed: owner={OwnerEmail} source={SourceStoryId} target={TargetEmail}/{TargetStoryId}",
            publishedOwnerEmail, sourceStoryId, targetEmail, targetStoryId);

        return AssetCopyResult.Success();
    }

    private async Task<AssetCopyResult> CopyAssetWithPollingAsync(
        string sourceBlobPath,
        string targetBlobPath,
        StoryAssetPathMapper.AssetInfo asset,
        string targetEmail,
        string targetStoryId,
        string sourceContainer,
        string targetContainer,
        CancellationToken ct)
    {
        try
        {
            var sourceSas = await _sasService.GetReadSasAsync(sourceContainer, sourceBlobPath, TimeSpan.FromMinutes(10), ct);
            var targetClient = _sasService.GetBlobClient(targetContainer, targetBlobPath);
            var pollUntil = DateTime.UtcNow.AddSeconds(30);

            _logger.LogDebug(
                "Starting asset copy: storyId={StoryId} filename={Filename} source={SourcePath} target={TargetPath}",
                targetStoryId, asset.Filename, sourceBlobPath, targetBlobPath);

            var copyOperation = await targetClient.StartCopyFromUriAsync(sourceSas, cancellationToken: ct);

            while (true)
            {
                var props = await targetClient.GetPropertiesAsync(cancellationToken: ct);
                var copyStatus = props.Value.CopyStatus;

                if (copyStatus == CopyStatus.Success)
                {
                    _logger.LogDebug(
                        "Asset copy succeeded: storyId={StoryId} filename={Filename}",
                        targetStoryId, asset.Filename);
                    break;
                }

                if (copyStatus == CopyStatus.Failed || copyStatus == CopyStatus.Aborted)
                {
                    var errorDetails = $"CopyStatus: {copyStatus}";
                    _logger.LogError(
                        "Asset copy failed: storyId={StoryId} filename={Filename} status={Status} source={SourcePath} target={TargetPath}",
                        targetStoryId, asset.Filename, copyStatus, sourceBlobPath, targetBlobPath);
                    return AssetCopyResult.CopyFailed(asset.Filename, targetStoryId, errorDetails);
                }

                if (DateTime.UtcNow > pollUntil)
                {
                    _logger.LogError(
                        "Asset copy timeout: storyId={StoryId} filename={Filename} source={SourcePath} target={TargetPath}",
                        targetStoryId, asset.Filename, sourceBlobPath, targetBlobPath);
                    return AssetCopyResult.CopyTimeout(asset.Filename, targetStoryId);
                }

                await Task.Delay(250, ct);
            }

            return AssetCopyResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Exception during asset copy: storyId={StoryId} filename={Filename} source={SourcePath}",
                targetStoryId, asset.Filename, sourceBlobPath);
            return AssetCopyResult.CopyFailed(asset.Filename, targetStoryId, ex.Message);
        }
    }
}

