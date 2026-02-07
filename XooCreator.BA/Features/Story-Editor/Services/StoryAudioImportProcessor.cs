using System.IO.Compression;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure.Services.Blob;
using static XooCreator.BA.Features.StoryEditor.Mappers.StoryAssetPathMapper;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryAudioImportProcessor
{
    Task<ImportAudioResult> ProcessAsync(Stream zipStream, string storyId, string locale, string ownerEmail, CancellationToken ct = default);
}

public record ImportAudioResult(bool Success, List<string> Errors, List<string> Warnings, int ImportedCount, int TotalPages);

public class StoryAudioImportProcessor : IStoryAudioImportProcessor
{
    private readonly XooDbContext _db;
    private readonly IStoryCraftsRepository _crafts;
    private readonly IBlobSasService _sas;
    private readonly ILogger<StoryAudioImportProcessor> _logger;

    private const long MaxAudioFileSizeBytes = 10 * 1024 * 1024; // 10MB per audio file
    private static readonly HashSet<string> AllowedAudioExtensions = new(StringComparer.OrdinalIgnoreCase) { ".wav", ".mp3", ".m4a" };

    public StoryAudioImportProcessor(
        XooDbContext db,
        IStoryCraftsRepository crafts,
        IBlobSasService sas,
        ILogger<StoryAudioImportProcessor> logger)
    {
        _db = db;
        _crafts = crafts;
        _sas = sas;
        _logger = logger;
    }

    public async Task<ImportAudioResult> ProcessAsync(Stream zipStream, string storyId, string locale, string ownerEmail, CancellationToken ct = default)
    {
        var errors = new List<string>();
        var warnings = new List<string>();
        var importedCount = 0;

        var craft = await _crafts.GetWithLanguageAsync(storyId, locale, ct);
        if (craft == null)
        {
            errors.Add("Story not found");
            return new ImportAudioResult(false, errors, warnings, 0, 0);
        }

        var pageTiles = craft.Tiles
            .Where(t => t.Type.Equals("page", StringComparison.OrdinalIgnoreCase))
            .OrderBy(t => t.SortOrder)
            .ToList();

        if (pageTiles.Count == 0)
        {
            errors.Add("Story has no page tiles");
            return new ImportAudioResult(false, errors, warnings, 0, 0);
        }

        var normalizedEmail = string.IsNullOrWhiteSpace(ownerEmail) ? string.Empty : Uri.UnescapeDataString(ownerEmail);

        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: false);

        var audioEntries = zip.Entries
            .Where(e => !string.IsNullOrWhiteSpace(e.Name))
            .Where(e =>
            {
                var fileName = Path.GetFileName(e.Name);
                return !string.IsNullOrWhiteSpace(fileName) && AllowedAudioExtensions.Contains(Path.GetExtension(fileName));
            })
            .OrderBy(e => ExtractPageNumber(Path.GetFileName(e.Name)))
            .ToList();

        if (audioEntries.Count == 0)
        {
            errors.Add("No audio files found in ZIP archive");
            return new ImportAudioResult(false, errors, warnings, 0, pageTiles.Count);
        }

        var containerClient = _sas.GetContainerClient(_sas.DraftContainer);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        foreach (var entry in audioEntries)
        {
            var entryFileName = Path.GetFileName(entry.Name);
            var pageNumber = ExtractPageNumber(entryFileName);
            if (pageNumber < 1 || pageNumber > pageTiles.Count)
            {
                warnings.Add($"Audio file '{entry.Name}' has page number {pageNumber}, but story only has {pageTiles.Count} pages. Skipping.");
                continue;
            }

            var tile = pageTiles[pageNumber - 1];
            var tileTranslation = tile.Translations.FirstOrDefault(t =>
                !string.IsNullOrWhiteSpace(t.LanguageCode) &&
                t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));

            if (tileTranslation == null)
            {
                tileTranslation = new StoryCraftTileTranslation
                {
                    Id = Guid.NewGuid(),
                    StoryCraftTileId = tile.Id,
                    LanguageCode = locale
                };
                tile.Translations.Add(tileTranslation);
                _db.StoryCraftTileTranslations.Add(tileTranslation);
            }

            await using var entryStream = entry.Open();
            if (entry.Length > MaxAudioFileSizeBytes)
            {
                warnings.Add($"Audio file '{entry.Name}' exceeds maximum size of {MaxAudioFileSizeBytes / (1024 * 1024)}MB. Skipping.");
                continue;
            }

            using var memoryStream = new MemoryStream();
            await entryStream.CopyToAsync(memoryStream, ct);
            var audioBytes = memoryStream.ToArray();

            var audioFilename = entryFileName;
            var draftPath = BuildDraftPath(new AssetInfo(audioFilename, AssetType.Audio, locale), normalizedEmail, craft.StoryId);

            if (!string.IsNullOrWhiteSpace(tileTranslation.AudioUrl) && tileTranslation.AudioUrl != audioFilename)
            {
                try
                {
                    var oldPath = BuildDraftPath(new AssetInfo(tileTranslation.AudioUrl, AssetType.Audio, locale), normalizedEmail, craft.StoryId);
                    var oldBlobClient = _sas.GetBlobClient(_sas.DraftContainer, oldPath);
                    await oldBlobClient.DeleteIfExistsAsync(cancellationToken: ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete old audio file: {OldPath}", tileTranslation.AudioUrl);
                }
            }

            try
            {
                var blobClient = _sas.GetBlobClient(_sas.DraftContainer, draftPath);
                await blobClient.UploadAsync(new BinaryData(audioBytes), overwrite: true, cancellationToken: ct);
                tileTranslation.AudioUrl = audioFilename;
                importedCount++;
                _logger.LogInformation("Imported audio for page {PageNumber} (tile {TileId}): {Filename}", pageNumber, tile.Id, audioFilename);
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to upload audio file '{entry.Name}' for page {pageNumber}: {ex.Message}");
                _logger.LogError(ex, "Failed to upload audio file: {Filename}", entry.Name);
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
                _logger.LogError(ex, "Failed to save audio import changes");
            }
        }

        return new ImportAudioResult(errors.Count == 0, errors, warnings, importedCount, pageTiles.Count);
    }

    private static int ExtractPageNumber(string filename)
    {
        var match = Regex.Match(filename, @"^(\d+)");
        return match.Success && int.TryParse(match.Groups[1].Value, out var pageNumber) ? pageNumber : 0;
    }
}
