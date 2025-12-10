using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage.Blobs.Specialized;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class StoryEpicPublishingService : IStoryEpicPublishingService
{
    private readonly XooDbContext _context;
    private readonly IBlobSasService _blobSas;
    private readonly ILogger<StoryEpicPublishingService> _logger;

    public StoryEpicPublishingService(
        XooDbContext context,
        IBlobSasService blobSas,
        ILogger<StoryEpicPublishingService> logger)
    {
        _context = context;
        _blobSas = blobSas;
        _logger = logger;
    }

    public async Task<DateTime?> PublishAsync(Guid ownerUserId, string epicId, CancellationToken ct = default)
    {
        var epic = await _context.StoryEpics
            .Include(e => e.Regions)
            .Include(e => e.StoryNodes)
            .FirstOrDefaultAsync(e => e.Id == epicId, ct);

        if (epic == null)
        {
            throw new InvalidOperationException($"Epic '{epicId}' was not found.");
        }

        if (epic.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException("You do not own this epic.");
        }

        if (!epic.Regions.Any())
        {
            throw new InvalidOperationException("Cannot publish an epic without regions.");
        }

        if (!epic.StoryNodes.Any())
        {
            throw new InvalidOperationException("Cannot publish an epic without stories.");
        }

        foreach (var region in epic.Regions)
        {
            if (string.IsNullOrWhiteSpace(region.ImageUrl)) continue;

            var currentPath = NormalizeBlobPath(region.ImageUrl);
            if (IsAlreadyPublished(currentPath)) continue;

            var publishedPath = await PublishRegionImageAsync(epic.Id, region.RegionId, currentPath, ct);
            region.ImageUrl = publishedPath;
            region.UpdatedAt = DateTime.UtcNow;
        }

        epic.Status = "published";
        epic.UpdatedAt = DateTime.UtcNow;
        epic.PublishedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return epic.PublishedAtUtc;
    }

    private async Task<string> PublishRegionImageAsync(string epicId, string regionId, string draftPath, CancellationToken ct)
    {
        var sourceClient = _blobSas.GetBlobClient(_blobSas.DraftContainer, draftPath);
        if (!await sourceClient.ExistsAsync(ct))
        {
            throw new InvalidOperationException($"Draft image '{draftPath}' does not exist.");
        }

        var fileName = Path.GetFileName(draftPath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = $"{regionId}-background.png";
        }

        var destinationPath = $"images/epics/{epicId}/regions/{regionId}/{fileName}";
        var destinationClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, destinationPath);

        _logger.LogInformation("Copying epic region image from {Source} to {Destination}", draftPath, destinationPath);

        var sasUri = sourceClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(10));
        var operation = await destinationClient.StartCopyFromUriAsync(sasUri, cancellationToken: ct);
        await operation.WaitForCompletionAsync(cancellationToken: ct);

        return destinationPath;
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
}
