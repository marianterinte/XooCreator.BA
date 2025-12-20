using Microsoft.Extensions.Logging;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IHeroPublishedAssetCleanupService
{
    Task DeletePublishedAssetsAsync(string heroId, CancellationToken ct);
}

public class HeroPublishedAssetCleanupService : IHeroPublishedAssetCleanupService
{
    private static readonly string[] Prefixes =
    {
        // NEW published structure for hero assets
        "images/heroes/",
        "audio/heroes/",
        "video/heroes/"
    };

    private readonly IBlobSasService _sas;
    private readonly ILogger<HeroPublishedAssetCleanupService> _logger;

    public HeroPublishedAssetCleanupService(
        IBlobSasService sas,
        ILogger<HeroPublishedAssetCleanupService> logger)
    {
        _sas = sas;
        _logger = logger;
    }

    public async Task DeletePublishedAssetsAsync(string heroId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(heroId))
        {
            _logger.LogWarning("Cannot cleanup published hero assets. Missing heroId.");
            return;
        }

        try
        {
            var container = _sas.GetContainerClient(_sas.PublishedContainer);
            var deleted = 0;

            foreach (var prefixBase in Prefixes)
            {
                var prefix = $"{prefixBase}{heroId}/";
                await foreach (var blob in container.GetBlobsAsync(prefix: prefix, cancellationToken: ct))
                {
                    var blobClient = container.GetBlobClient(blob.Name);
                    await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
                    deleted++;
                }
            }

            _logger.LogInformation(
                "Published hero assets cleanup complete: heroId={HeroId} deletedCount={Count}",
                heroId,
                deleted);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Published hero assets cleanup was cancelled: heroId={HeroId}", heroId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup published hero assets: heroId={HeroId}", heroId);
            throw;
        }
    }
}

