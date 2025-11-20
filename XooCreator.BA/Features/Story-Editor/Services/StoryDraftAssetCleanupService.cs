using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryDraftAssetCleanupService
{
    Task DeleteDraftAssetsAsync(string userEmail, string storyId, CancellationToken ct);
}

public class StoryDraftAssetCleanupService : IStoryDraftAssetCleanupService
{
    private readonly IBlobSasService _sas;
    private readonly ILogger<StoryDraftAssetCleanupService> _logger;

    public StoryDraftAssetCleanupService(IBlobSasService sas, ILogger<StoryDraftAssetCleanupService> logger)
    {
        _sas = sas;
        _logger = logger;
    }

    public async Task DeleteDraftAssetsAsync(string userEmail, string storyId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(userEmail) || string.IsNullOrWhiteSpace(storyId))
        {
            _logger.LogWarning("Cannot cleanup draft assets. Missing userEmail or storyId. storyId={StoryId}", storyId);
            return;
        }

        var emailEscaped = Uri.EscapeDataString(userEmail);
        var prefix = $"draft/u/{emailEscaped}/stories/{storyId}/";

        try
        {
            var containerClient = _sas.GetContainerClient(_sas.DraftContainer);
            var deletedCount = 0;

            await foreach (var blob in containerClient.GetBlobsAsync(prefix: prefix, cancellationToken: ct))
            {
                var blobClient = containerClient.GetBlobClient(blob.Name);
                await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
                deletedCount++;
            }

            _logger.LogInformation(
                "Draft assets cleanup complete: storyId={StoryId} prefix={Prefix} deletedCount={Count}",
                storyId,
                prefix,
                deletedCount);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Draft assets cleanup cancelled: storyId={StoryId}", storyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup draft assets: storyId={StoryId}", storyId);
            // Cleanup failures should not block publish flow
        }
    }
}

