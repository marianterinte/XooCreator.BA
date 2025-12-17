using Microsoft.Extensions.Logging;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IRegionPublishedAssetCleanupService
{
    Task DeletePublishedAssetsAsync(string ownerEmail, string regionId, CancellationToken ct);
}

public class RegionPublishedAssetCleanupService : IRegionPublishedAssetCleanupService
{
    private static readonly string[] Categories = { "images", "audio", "video" };

    private readonly IBlobSasService _sas;
    private readonly ILogger<RegionPublishedAssetCleanupService> _logger;

    public RegionPublishedAssetCleanupService(
        IBlobSasService sas,
        ILogger<RegionPublishedAssetCleanupService> logger)
    {
        _sas = sas;
        _logger = logger;
    }

    public async Task DeletePublishedAssetsAsync(string ownerEmail, string regionId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(ownerEmail) || string.IsNullOrWhiteSpace(regionId))
        {
            _logger.LogWarning("Cannot cleanup published region assets. Missing ownerEmail or regionId. regionId={RegionId}", regionId);
            return;
        }

        try
        {
            var container = _sas.GetContainerClient(_sas.PublishedContainer);
            var deleted = 0;

            foreach (var category in Categories)
            {
                var prefix = $"{category}/tales-of-alchimalia/regions/{ownerEmail}/{regionId}/";
                await foreach (var blob in container.GetBlobsAsync(prefix: prefix, cancellationToken: ct))
                {
                    var blobClient = container.GetBlobClient(blob.Name);
                    await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
                    deleted++;
                }
            }

            _logger.LogInformation(
                "Published region assets cleanup complete: regionId={RegionId} ownerEmail={OwnerEmail} deletedCount={Count}",
                regionId,
                ownerEmail,
                deleted);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Published region assets cleanup was cancelled: regionId={RegionId}", regionId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup published region assets: regionId={RegionId} ownerEmail={OwnerEmail}", regionId, ownerEmail);
            throw;
        }
    }
}

