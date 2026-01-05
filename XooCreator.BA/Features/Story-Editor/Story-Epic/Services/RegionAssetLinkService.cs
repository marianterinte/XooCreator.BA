using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Images;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IRegionAssetLinkService
{
    Task SyncImageAsync(StoryRegionCraft craft, string ownerEmail, CancellationToken ct);
    Task RemoveImageAsync(string regionId, CancellationToken ct);
}

public class RegionAssetLinkService : IRegionAssetLinkService
{
    private const string ImageEntityId = "__image__";

    private readonly XooDbContext _db;
    private readonly IBlobSasService _blobSas;
    private readonly IImageCompressionService _imageCompression;
    private readonly ILogger<RegionAssetLinkService> _logger;

    public RegionAssetLinkService(
        XooDbContext db,
        IBlobSasService blobSas,
        IImageCompressionService imageCompression,
        ILogger<RegionAssetLinkService> logger)
    {
        _db = db;
        _blobSas = blobSas;
        _imageCompression = imageCompression;
        _logger = logger;
    }

    public async Task SyncImageAsync(StoryRegionCraft craft, string ownerEmail, CancellationToken ct)
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
            _logger.LogWarning("Failed to publish image for regionId={RegionId}", craft.Id);
            return;
        }

        await UpsertLinkAsync(
            craft.Id,
            craft.LastDraftVersion,
            ImageEntityId,
            draftPath,
            publishedPath,
            ct);
    }

    public async Task RemoveImageAsync(string regionId, CancellationToken ct)
    {
        var links = await _db.RegionAssetLinks
            .Where(x => x.RegionId == regionId && x.EntityId == ImageEntityId)
            .ToListAsync(ct);

        if (links.Count == 0)
        {
            return;
        }

        foreach (var link in links)
        {
            await DeletePublishedAssetAsync(link.PublishedPath, ct);
        }

        _db.RegionAssetLinks.RemoveRange(links);
        await _db.SaveChangesAsync(ct);
    }

    private async Task<string> PublishImageAsync(string regionId, string draftPath, string ownerEmail, CancellationToken ct)
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
            fileName = $"{regionId}-image.png";
        }

        // Path format: images/regions/{regionId}/{fileName}
        var destinationPath = $"images/regions/{regionId}/{fileName}";
        var destinationClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, destinationPath);

        _logger.LogInformation("Copying region image from {Source} to {Destination}", draftPath, destinationPath);

        try
        {
            var sasUri = sourceClient.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(10));
            var operation = await destinationClient.StartCopyFromUriAsync(sasUri, cancellationToken: ct);
            await operation.WaitForCompletionAsync(cancellationToken: ct);

            // Generate s/m variants for region image (non-blocking)
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
                _logger.LogWarning(ex, "Failed to generate region image variants: path={Path}", destinationPath);
            }

            return destinationPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to copy region image: {Path}", draftPath);
            return string.Empty;
        }
    }

    private async Task UpsertLinkAsync(
        string regionId,
        int draftVersion,
        string entityId,
        string draftPath,
        string publishedPath,
        CancellationToken ct)
    {
        var hash = ComputeAssetHash(draftPath, draftVersion);

        // Check by DraftPath in database to avoid duplicate key violations
        var existing = await _db.RegionAssetLinks
            .FirstOrDefaultAsync(x => x.DraftPath == draftPath, ct);

        if (existing == null)
        {
            _db.RegionAssetLinks.Add(new RegionAssetLink
            {
                Id = Guid.NewGuid(),
                RegionId = regionId,
                DraftVersion = draftVersion,
                AssetType = "Image",
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
            existing.RegionId = regionId;
            existing.DraftVersion = draftVersion;
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

    private static string NormalizeBlobPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return string.Empty;

        var trimmed = path.Trim();
        if (trimmed.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(trimmed);
            trimmed = uri.AbsolutePath.TrimStart('/');
        }

        return trimmed.TrimStart('/');
    }

    private static bool IsAlreadyPublished(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        return path.StartsWith("images/", StringComparison.OrdinalIgnoreCase);
    }

    private static string ComputeAssetHash(string draftPath, int draftVersion)
    {
        var raw = $"Image|{draftPath}|v{draftVersion}";
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(raw)));
    }
}

