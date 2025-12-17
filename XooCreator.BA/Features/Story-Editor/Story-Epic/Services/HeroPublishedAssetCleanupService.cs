using Microsoft.Extensions.Logging;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IHeroPublishedAssetCleanupService
{
    Task DeletePublishedAssetsAsync(string ownerEmail, string heroId, CancellationToken ct);
}

public class HeroPublishedAssetCleanupService : IHeroPublishedAssetCleanupService
{
    private static readonly string[] Categories = { "images", "audio", "video" };

    private readonly IBlobSasService _sas;
    private readonly ILogger<HeroPublishedAssetCleanupService> _logger;

    public HeroPublishedAssetCleanupService(
        IBlobSasService sas,
        ILogger<HeroPublishedAssetCleanupService> logger)
    {
        _sas = sas;
        _logger = logger;
    }

    public async Task DeletePublishedAssetsAsync(string ownerEmail, string heroId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(ownerEmail) || string.IsNullOrWhiteSpace(heroId))
        {
            _logger.LogWarning("Cannot cleanup published hero assets. Missing ownerEmail or heroId. heroId={HeroId}", heroId);
            return;
        }

        try
        {
            var container = _sas.GetContainerClient(_sas.PublishedContainer);
            var deleted = 0;

            foreach (var category in Categories)
            {
                var prefix = $"{category}/tales-of-alchimalia/heroes/{ownerEmail}/{heroId}/";
                await foreach (var blob in container.GetBlobsAsync(prefix: prefix, cancellationToken: ct))
                {
                    var blobClient = container.GetBlobClient(blob.Name);
                    await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
                    deleted++;
                }
            }

            _logger.LogInformation(
                "Published hero assets cleanup complete: heroId={HeroId} ownerEmail={OwnerEmail} deletedCount={Count}",
                heroId,
                ownerEmail,
                deleted);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Published hero assets cleanup was cancelled: heroId={HeroId}", heroId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup published hero assets: heroId={HeroId} ownerEmail={OwnerEmail}", heroId, ownerEmail);
            throw;
        }
    }
}

