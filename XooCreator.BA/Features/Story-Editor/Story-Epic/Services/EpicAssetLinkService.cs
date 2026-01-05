using System.IO;
using System.Security.Cryptography;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Images;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IEpicAssetLinkService
{
    Task SyncCoverImageAsync(StoryEpicCraft craft, string ownerEmail, CancellationToken ct);
    Task SyncRewardImageAsync(StoryEpicCraft craft, string storyId, string ownerEmail, CancellationToken ct);
    Task SyncAllAssetsAsync(StoryEpicCraft craft, string ownerEmail, CancellationToken ct);
    Task RemoveCoverImageAsync(string epicId, CancellationToken ct);
    Task RemoveRewardImageAsync(string epicId, string storyId, CancellationToken ct);
    Task RemoveAllAssetsAsync(string epicId, CancellationToken ct);
}

public class EpicAssetLinkService : IEpicAssetLinkService
{
    private const string CoverImageEntityId = "__cover_image__";
    private const string RewardImageEntityIdPrefix = "__reward_image__";

    private readonly XooDbContext _db;
    private readonly IBlobSasService _blobSas;
    private readonly IImageCompressionService _imageCompression;
    private readonly ILogger<EpicAssetLinkService> _logger;

    public EpicAssetLinkService(
        XooDbContext db,
        IBlobSasService blobSas,
        IImageCompressionService imageCompression,
        ILogger<EpicAssetLinkService> logger)
    {
        _db = db;
        _blobSas = blobSas;
        _imageCompression = imageCompression;
        _logger = logger;
    }

    public async Task SyncAllAssetsAsync(StoryEpicCraft craft, string ownerEmail, CancellationToken ct)
    {
        await SyncCoverImageAsync(craft, ownerEmail, ct);

        // Load craft with story nodes if not already loaded
        craft = await _db.StoryEpicCrafts
            .Include(c => c.StoryNodes)
            .FirstOrDefaultAsync(c => c.Id == craft.Id, ct) ?? craft;

        foreach (var storyNode in craft.StoryNodes)
        {
            if (!string.IsNullOrWhiteSpace(storyNode.RewardImageUrl))
            {
                await SyncRewardImageAsync(craft, storyNode.StoryId, ownerEmail, ct);
            }
        }
    }

    public async Task SyncCoverImageAsync(StoryEpicCraft craft, string ownerEmail, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            await RemoveCoverImageAsync(craft.Id, ct);
            return;
        }

        var draftPath = NormalizeBlobPath(craft.CoverImageUrl);
        var publishedPath = await PublishCoverImageAsync(craft.Id, draftPath, ownerEmail, ct);

        if (string.IsNullOrWhiteSpace(publishedPath))
        {
            _logger.LogWarning("Failed to publish cover image for epicId={EpicId}", craft.Id);
            return;
        }

        await UpsertLinkAsync(
            craft.Id,
            craft.LastDraftVersion,
            CoverImageEntityId,
            null,
            draftPath,
            publishedPath,
            ct);
    }

    public async Task SyncRewardImageAsync(StoryEpicCraft craft, string storyId, string ownerEmail, CancellationToken ct)
    {
        // Load craft with story nodes if not already loaded
        craft = await _db.StoryEpicCrafts
            .Include(c => c.StoryNodes)
            .FirstOrDefaultAsync(c => c.Id == craft.Id, ct) ?? craft;

        var storyNode = craft.StoryNodes.FirstOrDefault(n => n.StoryId == storyId);
        if (storyNode == null || string.IsNullOrWhiteSpace(storyNode.RewardImageUrl))
        {
            await RemoveRewardImageAsync(craft.Id, storyId, ct);
            return;
        }

        var draftPath = NormalizeBlobPath(storyNode.RewardImageUrl);
        var publishedPath = await PublishRewardImageAsync(craft.Id, storyId, draftPath, ownerEmail, ct);

        if (string.IsNullOrWhiteSpace(publishedPath))
        {
            _logger.LogWarning("Failed to publish reward image for epicId={EpicId} storyId={StoryId}", craft.Id, storyId);
            return;
        }

        var entityId = $"{RewardImageEntityIdPrefix}{storyId}";
        await UpsertLinkAsync(
            craft.Id,
            craft.LastDraftVersion,
            entityId,
            storyId,
            draftPath,
            publishedPath,
            ct);
    }

    public async Task RemoveCoverImageAsync(string epicId, CancellationToken ct)
    {
        var links = await _db.EpicAssetLinks
            .Where(x => x.EpicId == epicId && x.EntityId == CoverImageEntityId)
            .ToListAsync(ct);

        if (links.Count == 0)
        {
            return;
        }

        foreach (var link in links)
        {
            await DeletePublishedAssetAsync(link.PublishedPath, ct);
        }

        _db.EpicAssetLinks.RemoveRange(links);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveRewardImageAsync(string epicId, string storyId, CancellationToken ct)
    {
        var entityId = $"{RewardImageEntityIdPrefix}{storyId}";
        var links = await _db.EpicAssetLinks
            .Where(x => x.EpicId == epicId && x.EntityId == entityId)
            .ToListAsync(ct);

        if (links.Count == 0)
        {
            return;
        }

        foreach (var link in links)
        {
            await DeletePublishedAssetAsync(link.PublishedPath, ct);
        }

        _db.EpicAssetLinks.RemoveRange(links);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveAllAssetsAsync(string epicId, CancellationToken ct)
    {
        var links = await _db.EpicAssetLinks
            .Where(x => x.EpicId == epicId)
            .ToListAsync(ct);

        if (links.Count == 0)
        {
            return;
        }

        foreach (var link in links)
        {
            await DeletePublishedAssetAsync(link.PublishedPath, ct);
        }

        _db.EpicAssetLinks.RemoveRange(links);
        await _db.SaveChangesAsync(ct);
    }

    private async Task<string> PublishCoverImageAsync(string epicId, string draftPath, string ownerEmail, CancellationToken ct)
    {
        if (IsAlreadyPublished(draftPath))
        {
            return draftPath; // Already published
        }

        var sourceClient = _blobSas.GetBlobClient(_blobSas.DraftContainer, draftPath);
        if (!await sourceClient.ExistsAsync(ct))
        {
            _logger.LogWarning("Draft cover image does not exist: {Path}", draftPath);
            return string.Empty;
        }

        var extension = Path.GetExtension(draftPath);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".png"; // Default extension
        }

        var publishedPath = $"images/epics/{epicId}/cover{extension}";
        var destinationClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, publishedPath);

        _logger.LogInformation("Publishing epic cover image: {Source} → {Destination}", draftPath, publishedPath);

        try
        {
            var sourceSas = sourceClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(10));
            var copyOperation = await destinationClient.StartCopyFromUriAsync(sourceSas, cancellationToken: ct);
            await copyOperation.WaitForCompletionAsync(cancellationToken: ct);

            // Generate s/m variants for images (non-blocking)
            if (publishedPath.StartsWith("images/", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var (basePath, fileName) = SplitPath(publishedPath);
                    await _imageCompression.EnsureStorySizeVariantsAsync(
                        sourceBlobPath: publishedPath,
                        targetBasePath: basePath,
                        filename: fileName,
                        overwriteExisting: true,
                        ct: ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to generate cover image variants: path={Path}", publishedPath);
                }
            }

            return publishedPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish cover image: {Path}", draftPath);
            return string.Empty;
        }
    }

    private async Task<string> PublishRewardImageAsync(string epicId, string storyId, string draftPath, string ownerEmail, CancellationToken ct)
    {
        if (IsAlreadyPublished(draftPath))
        {
            return draftPath; // Already published
        }

        var sourceClient = _blobSas.GetBlobClient(_blobSas.DraftContainer, draftPath);
        if (!await sourceClient.ExistsAsync(ct))
        {
            _logger.LogWarning("Draft reward image does not exist: {Path}", draftPath);
            return string.Empty;
        }

        var extension = Path.GetExtension(draftPath);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".png"; // Default extension
        }

        var publishedPath = $"images/epics/{epicId}/stories/{storyId}/reward{extension}";
        var destinationClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, publishedPath);

        _logger.LogInformation("Publishing epic reward image: {Source} → {Destination}", draftPath, publishedPath);

        try
        {
            var sourceSas = sourceClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(10));
            var copyOperation = await destinationClient.StartCopyFromUriAsync(sourceSas, cancellationToken: ct);
            await copyOperation.WaitForCompletionAsync(cancellationToken: ct);

            // Generate s/m variants for images (non-blocking)
            if (publishedPath.StartsWith("images/", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var (basePath, fileName) = SplitPath(publishedPath);
                    await _imageCompression.EnsureStorySizeVariantsAsync(
                        sourceBlobPath: publishedPath,
                        targetBasePath: basePath,
                        filename: fileName,
                        overwriteExisting: true,
                        ct: ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to generate reward image variants: path={Path}", publishedPath);
                }
            }

            return publishedPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish reward image: {Path}", draftPath);
            return string.Empty;
        }
    }

    private async Task UpsertLinkAsync(
        string epicId,
        int draftVersion,
        string entityId,
        string? storyId,
        string draftPath,
        string publishedPath,
        CancellationToken ct)
    {
        var hash = ComputeAssetHash(draftPath, draftVersion);

        // Check by DraftPath in database to avoid duplicate key violations
        var existing = await _db.EpicAssetLinks
            .FirstOrDefaultAsync(x => x.DraftPath == draftPath, ct);

        if (existing == null)
        {
            _db.EpicAssetLinks.Add(new EpicAssetLink
            {
                Id = Guid.NewGuid(),
                EpicId = epicId,
                DraftVersion = draftVersion,
                AssetType = entityId == CoverImageEntityId ? "Cover" : "Reward",
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
            existing.EpicId = epicId;
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

