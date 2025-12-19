using Microsoft.Extensions.Logging;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IEpicPublishedAssetCleanupService
{
    Task DeletePublishedAssetsAsync(string ownerEmail, string epicId, CancellationToken ct);
}

public class EpicPublishedAssetCleanupService : IEpicPublishedAssetCleanupService
{
    private static readonly string[] Categories = { "images", "audio", "video" };

    private readonly IBlobSasService _sas;
    private readonly ILogger<EpicPublishedAssetCleanupService> _logger;

    public EpicPublishedAssetCleanupService(
        IBlobSasService sas,
        ILogger<EpicPublishedAssetCleanupService> logger)
    {
        _sas = sas;
        _logger = logger;
    }

    public async Task DeletePublishedAssetsAsync(string ownerEmail, string epicId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(ownerEmail) || string.IsNullOrWhiteSpace(epicId))
        {
            _logger.LogWarning("Cannot cleanup published epic assets. Missing ownerEmail or epicId. epicId={EpicId}", epicId);
            return;
        }

        try
        {
            var container = _sas.GetContainerClient(_sas.PublishedContainer);
            var deleted = 0;

            // Delete all assets from the epic's folder (includes cover image, reward images, etc.)
            foreach (var category in Categories)
            {
                var prefix = $"{category}/tales-of-alchimalia/epics/{ownerEmail}/{epicId}/";
                await foreach (var blob in container.GetBlobsAsync(prefix: prefix, cancellationToken: ct))
                {
                    var blobClient = container.GetBlobClient(blob.Name);
                    await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
                    deleted++;
                }
            }

            _logger.LogInformation(
                "Published epic assets cleanup complete: epicId={EpicId} ownerEmail={OwnerEmail} deletedCount={Count}",
                epicId,
                ownerEmail,
                deleted);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Published epic assets cleanup was cancelled: epicId={EpicId}", epicId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup published epic assets: epicId={EpicId} ownerEmail={OwnerEmail}", epicId, ownerEmail);
            throw;
        }
    }
}

