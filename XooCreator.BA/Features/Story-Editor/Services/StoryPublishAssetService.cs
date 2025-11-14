using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Infrastructure.Services.Blob;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Service responsible for collecting and copying story assets during publish.
/// Handles extraction of assets for all languages and copying from draft to published container.
/// </summary>
public interface IStoryPublishAssetService
{
    /// <summary>
    /// Collects all assets from the craft for all available languages.
    /// Images are common (extracted once), Audio/Video are language-specific (extracted per language).
    /// </summary>
    List<StoryAssetPathMapper.AssetInfo> CollectAllAssets(StoryCraft craft);

    /// <summary>
    /// Copies all assets from draft container to published container.
    /// </summary>
    Task<AssetCopyResult> CopyAssetsToPublishedAsync(
        List<StoryAssetPathMapper.AssetInfo> assets,
        string userEmail,
        string storyId,
        CancellationToken ct);
}

public class StoryPublishAssetService : IStoryPublishAssetService
{
    private readonly IBlobSasService _sas;
    private readonly ILogger<StoryPublishAssetService> _logger;

    public StoryPublishAssetService(IBlobSasService sas, ILogger<StoryPublishAssetService> logger)
    {
        _sas = sas;
        _logger = logger;
    }

    public List<StoryAssetPathMapper.AssetInfo> CollectAllAssets(StoryCraft craft)
    {
        var allAssets = new List<StoryAssetPathMapper.AssetInfo>();
        var processedImages = new HashSet<string>(); // Deduplicate images (they're common)
        
        // Extract images once (they're common for all languages)
        if (!string.IsNullOrWhiteSpace(craft.CoverImageUrl) && !processedImages.Contains(craft.CoverImageUrl))
        {
            allAssets.Add(new StoryAssetPathMapper.AssetInfo(craft.CoverImageUrl, StoryAssetPathMapper.AssetType.Image, null));
            processedImages.Add(craft.CoverImageUrl);
        }

        foreach (var tile in craft.Tiles)
        {
            // Image is common for all languages (extract once)
            if (!string.IsNullOrWhiteSpace(tile.ImageUrl) && !processedImages.Contains(tile.ImageUrl))
            {
                allAssets.Add(new StoryAssetPathMapper.AssetInfo(tile.ImageUrl, StoryAssetPathMapper.AssetType.Image, null));
                processedImages.Add(tile.ImageUrl);
            }

            // Audio and Video are language-specific - extract for each language
            foreach (var tileTranslation in tile.Translations)
            {
                var lang = tileTranslation.LanguageCode;
                
                if (!string.IsNullOrWhiteSpace(tileTranslation.AudioUrl))
                {
                    allAssets.Add(new StoryAssetPathMapper.AssetInfo(tileTranslation.AudioUrl, StoryAssetPathMapper.AssetType.Audio, lang));
                }

                if (!string.IsNullOrWhiteSpace(tileTranslation.VideoUrl))
                {
                    allAssets.Add(new StoryAssetPathMapper.AssetInfo(tileTranslation.VideoUrl, StoryAssetPathMapper.AssetType.Video, lang));
                }
            }
        }

        _logger.LogInformation(
            "Collected assets for publish: storyId={StoryId} totalAssets={Count} images={ImageCount} audioVideo={AudioVideoCount}",
            craft.StoryId,
            allAssets.Count,
            allAssets.Count(a => a.Type == StoryAssetPathMapper.AssetType.Image),
            allAssets.Count(a => a.Type == StoryAssetPathMapper.AssetType.Audio || a.Type == StoryAssetPathMapper.AssetType.Video));

        return allAssets;
    }

    public async Task<AssetCopyResult> CopyAssetsToPublishedAsync(
        List<StoryAssetPathMapper.AssetInfo> assets,
        string userEmail,
        string storyId,
        CancellationToken ct)
    {
        foreach (var asset in assets)
        {
            var sourceResult = await FindSourceAssetAsync(asset, userEmail, storyId, ct);
            if (sourceResult.Result.HasError) return sourceResult.Result;

            var copyResult = await CopyAssetWithPollingAsync(
                sourceResult.SourceBlobPath!,
                asset,
                userEmail,
                storyId,
                ct);
            if (copyResult.HasError) return copyResult;
        }

        return AssetCopyResult.Success();
    }

    private async Task<(string? SourceBlobPath, AssetCopyResult Result)> FindSourceAssetAsync(
        StoryAssetPathMapper.AssetInfo asset,
        string userEmail,
        string storyId,
        CancellationToken ct)
    {
        var sourceBlobPath = StoryAssetPathMapper.BuildDraftPath(asset, userEmail, storyId);
        var sourceClient = _sas.GetBlobClient(_sas.DraftContainer, sourceBlobPath);

        if (await sourceClient.ExistsAsync(ct))
        {
            return (sourceBlobPath, AssetCopyResult.Success());
        }

        // Try fallback path for images
        if (asset.Type == StoryAssetPathMapper.AssetType.Image)
        {
            var altPath = StoryAssetPathMapper.BuildDraftPath(
                new StoryAssetPathMapper.AssetInfo(asset.Filename, asset.Type, null),
                userEmail,
                storyId);
            var altClient = _sas.GetBlobClient(_sas.DraftContainer, altPath);
            if (await altClient.ExistsAsync(ct))
            {
                return (altPath, AssetCopyResult.Success());
            }
        }

        _logger.LogWarning(
            "Publish asset not found in draft container: storyId={StoryId} filename={Filename} type={Type} userEmail={UserEmail}",
            storyId, asset.Filename, asset.Type, userEmail);
        
        return (null, AssetCopyResult.AssetNotFound(asset.Filename, storyId));
    }

    private async Task<AssetCopyResult> CopyAssetWithPollingAsync(
        string sourceBlobPath,
        StoryAssetPathMapper.AssetInfo asset,
        string userEmail,
        string storyId,
        CancellationToken ct)
    {
        try
        {
            var targetBlobPath = StoryAssetPathMapper.BuildPublishedPath(asset, userEmail, storyId);
            var sourceSas = await _sas.GetReadSasAsync(_sas.DraftContainer, sourceBlobPath, TimeSpan.FromMinutes(10), ct);

            var targetClient = _sas.GetBlobClient(_sas.PublishedContainer, targetBlobPath);
            var pollUntil = DateTime.UtcNow.AddSeconds(30);

            _logger.LogDebug(
                "Starting asset copy: storyId={StoryId} filename={Filename} source={SourcePath} target={TargetPath}",
                storyId, asset.Filename, sourceBlobPath, targetBlobPath);

            var copyOperation = await targetClient.StartCopyFromUriAsync(sourceSas, cancellationToken: ct);

            while (true)
            {
                var props = await targetClient.GetPropertiesAsync(cancellationToken: ct);
                var copyStatus = props.Value.CopyStatus;

                if (copyStatus == CopyStatus.Success)
                {
                    _logger.LogDebug(
                        "Asset copy succeeded: storyId={StoryId} filename={Filename}",
                        storyId, asset.Filename);
                    break;
                }

                if (copyStatus == CopyStatus.Failed || copyStatus == CopyStatus.Aborted)
                {
                    var errorDetails = $"CopyStatus: {copyStatus}";
                    _logger.LogError(
                        "Publish copy failed: storyId={StoryId} filename={Filename} status={Status} source={SourcePath} target={TargetPath}",
                        storyId, asset.Filename, copyStatus, sourceBlobPath, targetBlobPath);
                    return AssetCopyResult.CopyFailed(asset.Filename, storyId, errorDetails);
                }

                if (DateTime.UtcNow > pollUntil)
                {
                    _logger.LogError(
                        "Publish copy timeout: storyId={StoryId} filename={Filename} source={SourcePath} target={TargetPath}",
                        storyId, asset.Filename, sourceBlobPath, targetBlobPath);
                    return AssetCopyResult.CopyTimeout(asset.Filename, storyId);
                }

                await Task.Delay(250, ct);
            }

            return AssetCopyResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Exception during asset copy: storyId={StoryId} filename={Filename} source={SourcePath}",
                storyId, asset.Filename, sourceBlobPath);
            return AssetCopyResult.CopyFailed(asset.Filename, storyId, ex.Message);
        }
    }
}

