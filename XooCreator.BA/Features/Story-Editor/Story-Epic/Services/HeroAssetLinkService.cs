using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Images;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IHeroAssetLinkService
{
    Task SyncImageAsync(EpicHeroCraft craft, string ownerEmail, CancellationToken ct);
    Task SyncGreetingAudioAsync(EpicHeroCraft craft, string languageCode, string ownerEmail, CancellationToken ct);
    Task RemoveImageAsync(string heroId, CancellationToken ct);
    Task RemoveGreetingAudioAsync(string heroId, string languageCode, CancellationToken ct);
}

public class HeroAssetLinkService : IHeroAssetLinkService
{
    private const string ImageEntityId = "__image__";

    private readonly XooDbContext _db;
    private readonly IBlobSasService _blobSas;
    private readonly IImageCompressionService _imageCompression;
    private readonly ILogger<HeroAssetLinkService> _logger;

    public HeroAssetLinkService(
        XooDbContext db,
        IBlobSasService blobSas,
        IImageCompressionService imageCompression,
        ILogger<HeroAssetLinkService> logger)
    {
        _db = db;
        _blobSas = blobSas;
        _imageCompression = imageCompression;
        _logger = logger;
    }

    public async Task SyncImageAsync(EpicHeroCraft craft, string ownerEmail, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(craft.ImageUrl))
        {
            await RemoveImageAsync(craft.Id, ct);
            return;
        }

        var draftPath = NormalizeBlobPath(craft.ImageUrl);
        var publishedPath = await PublishImageAsync(craft.Id, draftPath, ownerEmail, ct);

        if (string.IsNullOrWhiteSpace(publishedPath))
        {
            _logger.LogWarning("Failed to publish image for heroId={HeroId}", craft.Id);
            return;
        }

        await UpsertLinkAsync(
            craft.Id,
            craft.LastDraftVersion,
            ImageEntityId,
            null, // Image is not language-specific
            draftPath,
            publishedPath,
            ct);
    }

    public async Task SyncGreetingAudioAsync(EpicHeroCraft craft, string languageCode, string ownerEmail, CancellationToken ct)
    {
        var translation = craft.Translations.FirstOrDefault(t => t.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
        if (translation == null || string.IsNullOrWhiteSpace(translation.GreetingAudioUrl))
        {
            await RemoveGreetingAudioAsync(craft.Id, languageCode, ct);
            return;
        }

        var draftPath = NormalizeBlobPath(translation.GreetingAudioUrl);
        var publishedPath = await PublishGreetingAudioAsync(craft.Id, draftPath, languageCode, ownerEmail, ct);

        if (string.IsNullOrWhiteSpace(publishedPath))
        {
            _logger.LogWarning("Failed to publish greeting audio for heroId={HeroId} lang={Lang}", craft.Id, languageCode);
            return;
        }

        await UpsertLinkAsync(
            craft.Id,
            craft.LastDraftVersion,
            $"__greeting_{languageCode}__",
            languageCode,
            draftPath,
            publishedPath,
            ct);
    }

    public async Task RemoveImageAsync(string heroId, CancellationToken ct)
    {
        var links = await _db.HeroAssetLinks
            .Where(x => x.HeroId == heroId && x.EntityId == ImageEntityId)
            .ToListAsync(ct);

        if (links.Count == 0)
        {
            return;
        }

        foreach (var link in links)
        {
            await DeletePublishedAssetAsync(link.PublishedPath, ct);
        }

        _db.HeroAssetLinks.RemoveRange(links);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveGreetingAudioAsync(string heroId, string languageCode, CancellationToken ct)
    {
        var entityId = $"__greeting_{languageCode}__";
        var links = await _db.HeroAssetLinks
            .Where(x => x.HeroId == heroId && x.EntityId == entityId)
            .ToListAsync(ct);

        if (links.Count == 0)
        {
            return;
        }

        foreach (var link in links)
        {
            await DeletePublishedAssetAsync(link.PublishedPath, ct);
        }

        _db.HeroAssetLinks.RemoveRange(links);
        await _db.SaveChangesAsync(ct);
    }

    private async Task<string> PublishImageAsync(string heroId, string draftPath, string ownerEmail, CancellationToken ct)
    {
        if (IsAlreadyPublished(draftPath))
        {
            return draftPath; // Already published
        }

        var sourceClient = _blobSas.GetBlobClient(_blobSas.DraftContainer, draftPath);
        if (!await sourceClient.ExistsAsync(ct))
        {
            _logger.LogWarning("Draft image does not exist: {Path}", draftPath);
            return string.Empty;
        }

        var fileName = Path.GetFileName(draftPath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = $"{heroId}-image.png";
        }

        // Path format: images/heroes/{heroId}/{fileName}
        var destinationPath = $"images/heroes/{heroId}/{fileName}";
        var destinationClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, destinationPath);

        _logger.LogInformation("Copying hero image from {Source} to {Destination}", draftPath, destinationPath);

        try
        {
            var sasUri = sourceClient.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(10));
            var operation = await destinationClient.StartCopyFromUriAsync(sasUri, cancellationToken: ct);
            await operation.WaitForCompletionAsync(cancellationToken: ct);

            // Generate s/m variants for hero image (non-blocking)
            try
            {
                var (basePath, fileName2) = SplitPath(destinationPath);
                await _imageCompression.EnsureStorySizeVariantsAsync(
                    sourceBlobPath: destinationPath,
                    targetBasePath: basePath,
                    filename: fileName2,
                    overwriteExisting: true,
                    ct: ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to generate hero image variants: path={Path}", destinationPath);
            }

            return destinationPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to copy hero image: {Path}", draftPath);
            return string.Empty;
        }
    }

    private async Task<string> PublishGreetingAudioAsync(string heroId, string draftPath, string languageCode, string ownerEmail, CancellationToken ct)
    {
        // If already published under the new structure, keep it
        if (draftPath.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
        {
            var publishedClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, draftPath);
            if (await publishedClient.ExistsAsync(ct))
            {
                return draftPath;
            }
            _logger.LogWarning("Audio path starts with audio/ but doesn't exist in published container, will copy from draft: {Path}", draftPath);
        }

        var sourceClient = _blobSas.GetBlobClient(_blobSas.DraftContainer, draftPath);
        if (!await sourceClient.ExistsAsync(ct))
        {
            _logger.LogWarning("Draft greeting audio does not exist: {Path}", draftPath);
            return string.Empty;
        }

        var fileName = Path.GetFileName(draftPath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = $"{heroId}-greeting-{languageCode}.mp3";
        }

        var lang = string.IsNullOrWhiteSpace(languageCode) ? "en-us" : languageCode.Trim().ToLowerInvariant();
        // Path format: audio/heroes/{heroId}/{languageCode}/{fileName}
        var destinationPath = $"audio/heroes/{heroId}/{lang}/{fileName}";
        var destinationClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, destinationPath);

        // Check if file already exists in published container (skip copy if already published)
        if (await destinationClient.ExistsAsync(ct))
        {
            return destinationPath; // Already published, return the path
        }

        _logger.LogInformation("Copying hero greeting audio: {Source} → {Destination}", draftPath, destinationPath);

        try
        {
            var sasUri = sourceClient.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(10));
            var operation = await destinationClient.StartCopyFromUriAsync(sasUri, cancellationToken: ct);
            await operation.WaitForCompletionAsync(cancellationToken: ct);

            // Verify copy was successful
            if (!await destinationClient.ExistsAsync(ct))
            {
                _logger.LogWarning("Failed to copy greeting audio to published container. Source: {Source}, Destination: {Destination}", draftPath, destinationPath);
                return string.Empty;
            }

            return destinationPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to copy hero greeting audio: {Path}", draftPath);
            return string.Empty;
        }
    }

    private async Task UpsertLinkAsync(
        string heroId,
        int draftVersion,
        string entityId,
        string? languageCode,
        string draftPath,
        string publishedPath,
        CancellationToken ct)
    {
        var hash = ComputeAssetHash(draftPath, draftVersion);

        // Check by DraftPath in database to avoid duplicate key violations
        var existing = await _db.HeroAssetLinks
            .FirstOrDefaultAsync(x => x.DraftPath == draftPath, ct);

        if (existing == null)
        {
            _db.HeroAssetLinks.Add(new HeroAssetLink
            {
                Id = Guid.NewGuid(),
                HeroId = heroId,
                DraftVersion = draftVersion,
                LanguageCode = languageCode,
                AssetType = entityId.StartsWith("__greeting_") ? "Audio" : "Image",
                EntityId = entityId,
                DraftPath = draftPath,
                PublishedPath = publishedPath,
                ContentHash = hash,
                LastSyncedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            // Update existing link
            existing.HeroId = heroId;
            existing.DraftVersion = draftVersion;
            existing.LanguageCode = languageCode;
            existing.PublishedPath = publishedPath;
            existing.ContentHash = hash;
            existing.LastSyncedAt = DateTime.UtcNow;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
    }

    private async Task DeletePublishedAssetAsync(string? publishedPath, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(publishedPath))
        {
            return;
        }

        try
        {
            var client = _blobSas.GetBlobClient(_blobSas.PublishedContainer, publishedPath);
            await client.DeleteIfExistsAsync(cancellationToken: ct);

            // Also delete derived s/m variants for images
            if (publishedPath.StartsWith("images/", StringComparison.OrdinalIgnoreCase) &&
                !publishedPath.Contains("/s/", StringComparison.OrdinalIgnoreCase) &&
                !publishedPath.Contains("/m/", StringComparison.OrdinalIgnoreCase))
            {
                var (basePath, fileName) = SplitPath(publishedPath);
                if (!string.IsNullOrWhiteSpace(basePath) && !string.IsNullOrWhiteSpace(fileName))
                {
                    var small = $"{basePath.TrimEnd('/')}/s/{fileName}";
                    var medium = $"{basePath.TrimEnd('/')}/m/{fileName}";
                    await _blobSas.GetBlobClient(_blobSas.PublishedContainer, small).DeleteIfExistsAsync(cancellationToken: ct);
                    await _blobSas.GetBlobClient(_blobSas.PublishedContainer, medium).DeleteIfExistsAsync(cancellationToken: ct);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete published asset path={Path}", publishedPath);
        }
    }

    private static (string BasePath, string FileName) SplitPath(string blobPath)
    {
        var trimmed = (blobPath ?? string.Empty).Trim().TrimStart('/');
        var idx = trimmed.LastIndexOf('/');
        if (idx < 0)
        {
            return (string.Empty, trimmed);
        }
        var basePath = trimmed.Substring(0, idx);
        var fileName = trimmed.Substring(idx + 1);
        return (basePath, fileName);
    }

    private string NormalizeBlobPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return string.Empty;

        var trimmed = path.Trim();
        if (trimmed.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(trimmed);
            trimmed = uri.AbsolutePath.TrimStart('/');
        }

        trimmed = trimmed.TrimStart('/');

        // If a full URL was provided, AbsolutePath includes the container as the first segment
        var draftPrefix = _blobSas.DraftContainer.Trim('/') + "/";
        var publishedPrefix = _blobSas.PublishedContainer.Trim('/') + "/";
        if (trimmed.StartsWith(draftPrefix, StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed.Substring(draftPrefix.Length);
        }
        else if (trimmed.StartsWith(publishedPrefix, StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed.Substring(publishedPrefix.Length);
        }

        return trimmed;
    }

    private static bool IsAlreadyPublished(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        return path.StartsWith("images/", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("audio/", StringComparison.OrdinalIgnoreCase);
    }

    private static string ComputeAssetHash(string draftPath, int draftVersion)
    {
        var raw = $"Asset|{draftPath}|v{draftVersion}";
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(raw)));
    }
}

