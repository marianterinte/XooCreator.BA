using Microsoft.Extensions.Logging;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryPublishedAssetCleanupService
{
    Task DeletePublishedAssetsAsync(string ownerEmail, string storyId, CancellationToken ct);
}

public class StoryPublishedAssetCleanupService : IStoryPublishedAssetCleanupService
{
    private static readonly string[] Categories = { "images", "audio", "video" };

    private readonly IBlobSasService _sas;
    private readonly ILogger<StoryPublishedAssetCleanupService> _logger;

    public StoryPublishedAssetCleanupService(
        IBlobSasService sas,
        ILogger<StoryPublishedAssetCleanupService> logger)
    {
        _sas = sas;
        _logger = logger;
    }

    public async Task DeletePublishedAssetsAsync(string ownerEmail, string storyId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(ownerEmail) || string.IsNullOrWhiteSpace(storyId))
        {
            _logger.LogWarning("Cannot cleanup published assets. Missing ownerEmail or storyId. storyId={StoryId}", storyId);
            return;
        }

        try
        {
            var container = _sas.GetContainerClient(_sas.PublishedContainer);
            var deleted = 0;

            foreach (var category in Categories)
            {
                var prefix = $"{category}/tales-of-alchimalia/stories/{ownerEmail}/{storyId}/";
                await foreach (var blob in container.GetBlobsAsync(prefix: prefix, cancellationToken: ct))
                {
                    var blobClient = container.GetBlobClient(blob.Name);
                    await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
                    deleted++;
                }
            }

            _logger.LogInformation(
                "Published assets cleanup complete: storyId={StoryId} owner={Owner} deleted={Count}",
                storyId,
                ownerEmail,
                deleted);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Published assets cleanup cancelled: storyId={StoryId}", storyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup published assets: storyId={StoryId}", storyId);
        }
    }
}

