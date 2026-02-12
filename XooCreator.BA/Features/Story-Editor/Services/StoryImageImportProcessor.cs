using System.IO.Compression;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure.Services.Blob;
using static XooCreator.BA.Features.StoryEditor.Mappers.StoryAssetPathMapper;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryImageImportProcessor
{
    Task<ImportImageResult> ProcessAsync(Stream zipStream, string storyId, string locale, string ownerEmail, CancellationToken ct = default);
}

public record ImportImageResult(bool Success, List<string> Errors, List<string> Warnings, int ImportedCount, int TotalPages);

public class StoryImageImportProcessor : IStoryImageImportProcessor
{
    private readonly XooDbContext _db;
    private readonly IStoryCraftsRepository _crafts;
    private readonly IBlobSasService _sas;
    private readonly ILogger<StoryImageImportProcessor> _logger;
    private readonly long _maxImageFileSizeBytes;

    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".webp"
    };

    public StoryImageImportProcessor(
        XooDbContext db,
        IStoryCraftsRepository crafts,
        IBlobSasService sas,
        IConfiguration config,
        ILogger<StoryImageImportProcessor> logger)
    {
        _db = db;
        _crafts = crafts;
        _sas = sas;
        _logger = logger;
        _maxImageFileSizeBytes = config.GetValue<long?>("Uploads:MaxImageBytes") ?? 10 * 1024 * 1024;
    }

    public async Task<ImportImageResult> ProcessAsync(Stream zipStream, string storyId, string locale, string ownerEmail, CancellationToken ct = default)
    {
        var errors = new List<string>();
        var warnings = new List<string>();
        var importedCount = 0;

        var craft = await _crafts.GetAsync(storyId, ct);
        if (craft == null)
        {
            errors.Add("Story not found");
            return new ImportImageResult(false, errors, warnings, 0, 0);
        }

        var pageOrQuizTypes = new[] { "page", "quiz", "dialog" };
        var pageTiles = craft.Tiles
            .Where(t => pageOrQuizTypes.Contains(t.Type, StringComparer.OrdinalIgnoreCase))
            .OrderBy(t => t.SortOrder)
            .ToList();

        if (pageTiles.Count == 0)
        {
            errors.Add("Story has no page, quiz, or dialog tiles");
            return new ImportImageResult(false, errors, warnings, 0, 0);
        }

        var normalizedEmail = string.IsNullOrWhiteSpace(ownerEmail) ? string.Empty : Uri.UnescapeDataString(ownerEmail);

        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: false);

        var imageEntries = zip.Entries
            .Where(e => !string.IsNullOrWhiteSpace(e.Name))
            .Where(e =>
            {
                var fileName = Path.GetFileName(e.Name);
                return !string.IsNullOrWhiteSpace(fileName) && AllowedImageExtensions.Contains(Path.GetExtension(fileName));
            })
            .OrderBy(e => ExtractPageNumber(Path.GetFileName(e.Name)))
            .ToList();

        if (imageEntries.Count == 0)
        {
            errors.Add("No image files found in ZIP archive");
            return new ImportImageResult(false, errors, warnings, 0, pageTiles.Count);
        }

        var containerClient = _sas.GetContainerClient(_sas.DraftContainer);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        foreach (var entry in imageEntries)
        {
            var entryFileName = Path.GetFileName(entry.Name);
            var pageNumber = ExtractPageNumber(entryFileName);
            if (pageNumber < 1 || pageNumber > pageTiles.Count)
            {
                warnings.Add($"Image file '{entry.Name}' has page number {pageNumber}, but story only has {pageTiles.Count} pages. Skipping.");
                continue;
            }

            var tile = pageTiles[pageNumber - 1];

            await using var entryStream = entry.Open();
            if (entry.Length > _maxImageFileSizeBytes)
            {
                warnings.Add($"Image file '{entry.Name}' exceeds maximum size of {_maxImageFileSizeBytes / (1024 * 1024)}MB. Skipping.");
                continue;
            }

            using var memoryStream = new MemoryStream();
            await entryStream.CopyToAsync(memoryStream, ct);
            var imageBytes = memoryStream.ToArray();

            var imageFilename = entryFileName;
            var draftPath = BuildDraftPath(new AssetInfo(imageFilename, AssetType.Image, null), normalizedEmail, craft.StoryId);

            if (!string.IsNullOrWhiteSpace(tile.ImageUrl) && !tile.ImageUrl.Equals(imageFilename, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var oldPath = BuildDraftPath(new AssetInfo(tile.ImageUrl, AssetType.Image, null), normalizedEmail, craft.StoryId);
                    var oldBlobClient = _sas.GetBlobClient(_sas.DraftContainer, oldPath);
                    await oldBlobClient.DeleteIfExistsAsync(cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete old image file: {OldPath}", tile.ImageUrl);
                }
            }

            try
            {
                var blobClient = _sas.GetBlobClient(_sas.DraftContainer, draftPath);
                await blobClient.UploadAsync(new BinaryData(imageBytes), overwrite: true, cancellationToken: ct);
                tile.ImageUrl = imageFilename;
                importedCount++;
                _logger.LogInformation("Imported image for page {PageNumber} (tile {TileId}): {Filename}", pageNumber, tile.Id, imageFilename);
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to upload image file '{entry.Name}' for page {pageNumber}: {ex.Message}");
                _logger.LogError(ex, "Failed to upload image file: {Filename}", entry.Name);
            }
        }

        if (importedCount > 0)
        {
            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to save changes to database: {ex.Message}");
                _logger.LogError(ex, "Failed to save image import changes");
            }
        }

        return new ImportImageResult(errors.Count == 0, errors, warnings, importedCount, pageTiles.Count);
    }

    private static int ExtractPageNumber(string filename)
    {
        var match = Regex.Match(filename, @"^(\d+)");
        return match.Success && int.TryParse(match.Groups[1].Value, out var pageNumber) ? pageNumber : 0;
    }
}
