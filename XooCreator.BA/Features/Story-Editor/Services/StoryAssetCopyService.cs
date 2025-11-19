using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
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
            results.Add(new StoryAssetPathMapper.AssetInfo(craft.CoverImageUrl, StoryAssetPathMapper.AssetType.Image, null));
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
        }

        return results;
    }

    public List<StoryAssetPathMapper.AssetInfo> CollectFromDefinition(StoryDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        var results = new List<StoryAssetPathMapper.AssetInfo>();

        if (!string.IsNullOrWhiteSpace(definition.CoverImageUrl))
        {
            results.Add(new StoryAssetPathMapper.AssetInfo(Path.GetFileName(definition.CoverImageUrl), StoryAssetPathMapper.AssetType.Image, null));
        }

        foreach (var tile in definition.Tiles)
        {
            if (!string.IsNullOrWhiteSpace(tile.ImageUrl))
            {
                results.Add(new StoryAssetPathMapper.AssetInfo(Path.GetFileName(tile.ImageUrl), StoryAssetPathMapper.AssetType.Image, null));
            }

            foreach (var translation in tile.Translations)
            {
                if (!string.IsNullOrWhiteSpace(translation.AudioUrl))
                {
                    results.Add(new StoryAssetPathMapper.AssetInfo(Path.GetFileName(translation.AudioUrl), StoryAssetPathMapper.AssetType.Audio, translation.LanguageCode));
                }
                if (!string.IsNullOrWhiteSpace(translation.VideoUrl))
                {
                    results.Add(new StoryAssetPathMapper.AssetInfo(Path.GetFileName(translation.VideoUrl), StoryAssetPathMapper.AssetType.Video, translation.LanguageCode));
                }
            }
        }

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
            var sourceClient = _sasService.GetBlobClient(_sasService.PublishedContainer, sourcePath);

            if (!await sourceClient.ExistsAsync(ct))
            {
                _logger.LogWarning(
                    "Source asset not found in published: owner={OwnerEmail} storyId={StoryId} filename={Filename} path={Path}",
                    publishedOwnerEmail, sourceStoryId, asset.Filename, sourcePath);
                continue;
            }

            var targetPath = StoryAssetPathMapper.BuildDraftPath(asset, targetEmail, targetStoryId);
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

