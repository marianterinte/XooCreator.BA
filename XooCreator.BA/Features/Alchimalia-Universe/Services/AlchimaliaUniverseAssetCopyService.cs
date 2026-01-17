using Azure.Storage.Blobs.Models;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.AlchimaliaUniverse.Mappers;
using XooCreator.BA.Features.StoryEditor.Models; // Reusing AssetCopyResult
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public interface IAlchimaliaUniverseAssetCopyService
{
    Task<AssetCopyResult> CopyDraftToPublishedAsync(
        IEnumerable<AlchimaliaUniverseAssetPathMapper.AssetInfo> assets,
        string userEmail,
        string entityId,
        AlchimaliaUniverseAssetPathMapper.EntityType entityType,
        CancellationToken ct);

    Task<AssetCopyResult> CopyDraftToDraftAsync(
        IEnumerable<AlchimaliaUniverseAssetPathMapper.AssetInfo> assets,
        string sourceEmail,
        string sourceId,
        string targetEmail,
        string targetId,
        AlchimaliaUniverseAssetPathMapper.EntityType entityType,
        CancellationToken ct);
        
    Task<AssetCopyResult> CopyPublishedToDraftAsync(
        IEnumerable<AlchimaliaUniverseAssetPathMapper.AssetInfo> assets,
        string publishedOwnerEmail,
        string sourceId,
        string targetEmail,
        string targetId,
        AlchimaliaUniverseAssetPathMapper.EntityType entityType,
        CancellationToken ct);

    string GetPublishedUrl(string path);
    string GetDraftUrl(string path);
}

public class AlchimaliaUniverseAssetCopyService : IAlchimaliaUniverseAssetCopyService
{
    private readonly IBlobSasService _sasService;
    private readonly ILogger<AlchimaliaUniverseAssetCopyService> _logger;

    public AlchimaliaUniverseAssetCopyService(
        IBlobSasService sasService,
        ILogger<AlchimaliaUniverseAssetCopyService> logger)
    {
        _sasService = sasService;
        _logger = logger;
    }

    public string GetPublishedUrl(string path)
    {
         var client = _sasService.GetBlobClient(_sasService.PublishedContainer, path);
         return client.Uri.ToString();
    }

    public string GetDraftUrl(string path)
    {
         var client = _sasService.GetBlobClient(_sasService.DraftContainer, path);
         return client.Uri.ToString();
    }

    public async Task<AssetCopyResult> CopyDraftToPublishedAsync(
        IEnumerable<AlchimaliaUniverseAssetPathMapper.AssetInfo> assets,
        string userEmail,
        string entityId,
        AlchimaliaUniverseAssetPathMapper.EntityType entityType,
        CancellationToken ct)
    {
        foreach (var asset in assets)
        {
            var sourcePath = AlchimaliaUniverseAssetPathMapper.BuildDraftPath(asset, userEmail, entityId, entityType);
            var targetPath = AlchimaliaUniverseAssetPathMapper.BuildPublishedPath(asset, userEmail, entityId, entityType);

            var sourceClient = _sasService.GetBlobClient(_sasService.DraftContainer, sourcePath);
            if (!await sourceClient.ExistsAsync(ct))
            {
               _logger.LogWarning("Source asset not found: {SourcePath}", sourcePath);
               continue; 
            }

            var result = await CopyAssetWithPollingAsync(sourcePath, targetPath, asset.Filename, entityId, 
                _sasService.DraftContainer, _sasService.PublishedContainer, ct);
            
            if (result.HasError) return result;
        }
        return AssetCopyResult.Success();
    }

    public async Task<AssetCopyResult> CopyDraftToDraftAsync(
        IEnumerable<AlchimaliaUniverseAssetPathMapper.AssetInfo> assets,
        string sourceEmail,
        string sourceId,
        string targetEmail,
        string targetId,
        AlchimaliaUniverseAssetPathMapper.EntityType entityType,
        CancellationToken ct)
    {
        foreach (var asset in assets)
        {
            var sourcePath = AlchimaliaUniverseAssetPathMapper.BuildDraftPath(asset, sourceEmail, sourceId, entityType);
            var targetPath = AlchimaliaUniverseAssetPathMapper.BuildDraftPath(asset, targetEmail, targetId, entityType);

            var sourceClient = _sasService.GetBlobClient(_sasService.DraftContainer, sourcePath);
            if (!await sourceClient.ExistsAsync(ct)) continue;

            var result = await CopyAssetWithPollingAsync(sourcePath, targetPath, asset.Filename, targetId, 
                _sasService.DraftContainer, _sasService.DraftContainer, ct);
            
            if (result.HasError) return result;
        }
        return AssetCopyResult.Success();
    }

    public async Task<AssetCopyResult> CopyPublishedToDraftAsync(
        IEnumerable<AlchimaliaUniverseAssetPathMapper.AssetInfo> assets,
        string publishedOwnerEmail,
        string sourceId,
        string targetEmail,
        string targetId,
        AlchimaliaUniverseAssetPathMapper.EntityType entityType,
        CancellationToken ct)
    {
        foreach (var asset in assets)
        {
            var sourcePath = AlchimaliaUniverseAssetPathMapper.BuildPublishedPath(asset, publishedOwnerEmail, sourceId, entityType);
            var targetPath = AlchimaliaUniverseAssetPathMapper.BuildDraftPath(asset, targetEmail, targetId, entityType);

            var sourceClient = _sasService.GetBlobClient(_sasService.PublishedContainer, sourcePath);
            if (!await sourceClient.ExistsAsync(ct)) continue;

            var result = await CopyAssetWithPollingAsync(sourcePath, targetPath, asset.Filename, targetId, 
                _sasService.PublishedContainer, _sasService.DraftContainer, ct);
            
            if (result.HasError) return result;
        }
        return AssetCopyResult.Success();
    }

    private async Task<AssetCopyResult> CopyAssetWithPollingAsync(
        string sourceBlobPath,
        string targetBlobPath,
        string filename,
        string entityId,
        string sourceContainer,
        string targetContainer,
        CancellationToken ct)
    {
        try
        {
            var sourceSas = await _sasService.GetReadSasAsync(sourceContainer, sourceBlobPath, TimeSpan.FromMinutes(10), ct);
            var targetClient = _sasService.GetBlobClient(targetContainer, targetBlobPath);
            var pollUntil = DateTime.UtcNow.AddSeconds(30);

            var copyOperation = await targetClient.StartCopyFromUriAsync(sourceSas, cancellationToken: ct);

            while (true)
            {
                var props = await targetClient.GetPropertiesAsync(cancellationToken: ct);
                var copyStatus = props.Value.CopyStatus;

                if (copyStatus == CopyStatus.Success) break;

                if (copyStatus == CopyStatus.Failed || copyStatus == CopyStatus.Aborted)
                {
                    return AssetCopyResult.CopyFailed(filename, entityId, $"CopyStatus: {copyStatus}");
                }

                if (DateTime.UtcNow > pollUntil)
                {
                    return AssetCopyResult.CopyTimeout(filename, entityId);
                }

                await Task.Delay(250, ct);
            }
            return AssetCopyResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during asset copy: {Filename}", filename);
            return AssetCopyResult.CopyFailed(filename, entityId, ex.Message);
        }
    }
}
