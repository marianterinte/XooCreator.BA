using System.IO.Compression;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure.Services.Blob;
using static XooCreator.BA.Features.StoryEditor.Mappers.StoryAssetPathMapper;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryAudioImportProcessor
{
    Task<ImportAudioResult> ProcessAsync(Stream zipStream, string storyId, string locale, string ownerEmail, CancellationToken ct = default);
    Task<ImportAudioResult> ProcessFromStagingAsync(AudioBatchMappingPayload payload, string storyId, string locale, string ownerEmail, CancellationToken ct = default);
}

public record ImportAudioResult(bool Success, List<string> Errors, List<string> Warnings, int ImportedCount, int TotalPages);

public class StoryAudioImportProcessor : IStoryAudioImportProcessor
{
    /// <summary>One slot per page/quiz tile or per dialog node (same order as full audio export).</summary>
    private sealed record AudioImportTarget(StoryCraftTile Tile, StoryCraftTileTranslation? TileTranslation, StoryCraftDialogNode? Node);

    private readonly XooDbContext _db;
    private readonly IStoryCraftsRepository _crafts;
    private readonly IBlobSasService _sas;
    private readonly ILogger<StoryAudioImportProcessor> _logger;

    private const long MaxAudioFileSizeBytes = 100 * 1024 * 1024; // 100MB per audio file
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
        var changedTileIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var craft = await _crafts.GetWithLanguageAsync(storyId, locale, ct);
        if (craft == null)
        {
            errors.Add("Story not found");
            return new ImportAudioResult(false, errors, warnings, 0, 0);
        }

        var localeNorm = (locale ?? string.Empty).Trim().ToLowerInvariant();
        // Same slot list as full audio export: one per page/quiz with text, one per dialog node with text
        var targets = BuildAudioImportTargets(craft, localeNorm);
        if (targets.Count == 0)
        {
            errors.Add("Story has no page, quiz, or dialog tiles with text");
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
            return new ImportAudioResult(false, errors, warnings, 0, targets.Count);
        }

        var containerClient = _sas.GetContainerClient(_sas.DraftContainer);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        foreach (var entry in audioEntries)
        {
            var entryFileName = Path.GetFileName(entry.Name);
            var pageNumber = ExtractPageNumber(entryFileName);
            if (pageNumber < 1 || pageNumber > targets.Count)
            {
                warnings.Add($"Audio file '{entry.Name}' has index {pageNumber}, but story has {targets.Count} audio slots. Skipping.");
                continue;
            }

            var target = targets[pageNumber - 1];

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

            if (target.Node != null)
            {
                var nodeTranslation = target.Node.Translations.FirstOrDefault(nt =>
                    !string.IsNullOrWhiteSpace(nt.LanguageCode) && nt.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
                if (nodeTranslation == null)
                {
                    nodeTranslation = new StoryCraftDialogNodeTranslation
                    {
                        Id = Guid.NewGuid(),
                        StoryCraftDialogNodeId = target.Node.Id,
                        LanguageCode = locale
                    };
                    target.Node.Translations.Add(nodeTranslation);
                    _db.StoryCraftDialogNodeTranslations.Add(nodeTranslation);
                }

                if (!string.IsNullOrWhiteSpace(nodeTranslation.AudioUrl) && nodeTranslation.AudioUrl != audioFilename)
                {
                    try
                    {
                        var oldPath = BuildDraftPath(new AssetInfo(nodeTranslation.AudioUrl, AssetType.Audio, locale), normalizedEmail, craft.StoryId);
                        var oldBlobClient = _sas.GetBlobClient(_sas.DraftContainer, oldPath);
                        await oldBlobClient.DeleteIfExistsAsync(cancellationToken: ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete old dialog node audio: {OldPath}", nodeTranslation.AudioUrl);
                    }
                }

                try
                {
                    var blobClient = _sas.GetBlobClient(_sas.DraftContainer, draftPath);
                    await blobClient.UploadAsync(new BinaryData(audioBytes), overwrite: true, cancellationToken: ct);
                    nodeTranslation.AudioUrl = audioFilename;
                    importedCount++;
                    changedTileIds.Add(target.Tile.TileId);
                    _logger.LogInformation("Imported audio for slot {PageNumber} (dialog node {NodeId}): {Filename}", pageNumber, target.Node.NodeId, audioFilename);
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to upload audio file '{entry.Name}' for slot {pageNumber}: {ex.Message}");
                    _logger.LogError(ex, "Failed to upload audio file: {Filename}", entry.Name);
                }
            }
            else
            {
                var tile = target.Tile;
                var tileTranslation = target.TileTranslation ?? tile.Translations.FirstOrDefault(t =>
                    !string.IsNullOrWhiteSpace(t.LanguageCode) && t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
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
                    changedTileIds.Add(tile.TileId);
                    _logger.LogInformation("Imported audio for slot {PageNumber} (tile {TileId}): {Filename}", pageNumber, tile.Id, audioFilename);
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to upload audio file '{entry.Name}' for slot {pageNumber}: {ex.Message}");
                    _logger.LogError(ex, "Failed to upload audio file: {Filename}", entry.Name);
                }
            }
        }

        if (importedCount > 0)
        {
            try
            {
                await _db.SaveChangesAsync(ct);
                await AppendTileChangeLogsAsync(craft, localeNorm, changedTileIds, ct);
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to save changes to database: {ex.Message}");
                _logger.LogError(ex, "Failed to save audio import changes");
            }
        }

        return new ImportAudioResult(errors.Count == 0, errors, warnings, importedCount, targets.Count);
    }

    public async Task<ImportAudioResult> ProcessFromStagingAsync(AudioBatchMappingPayload payload, string storyId, string locale, string ownerEmail, CancellationToken ct = default)
    {
        var errors = new List<string>();
        var warnings = new List<string>();
        var importedCount = 0;
        var changedTileIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var craft = await _crafts.GetWithLanguageAsync(storyId, locale, ct);
        if (craft == null)
        {
            errors.Add("Story not found");
            return new ImportAudioResult(false, errors, warnings, 0, 0);
        }

        var localeNorm = (locale ?? string.Empty).Trim().ToLowerInvariant();
        var targets = BuildAudioImportTargets(craft, localeNorm);
        if (targets.Count == 0)
        {
            errors.Add("Story has no page, quiz, or dialog tiles with text");
            return new ImportAudioResult(false, errors, warnings, 0, 0);
        }

        var files = payload.Files ?? new List<StagedMediaFile>();
        if (files.Count == 0)
        {
            errors.Add("No staged audio files found.");
            return new ImportAudioResult(false, errors, warnings, 0, targets.Count);
        }

        var filesByClientId = files
            .Where(f => !string.IsNullOrWhiteSpace(f.ClientFileId))
            .GroupBy(f => f.ClientFileId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        var assignments = new Dictionary<int, StagedMediaFile>();
        var validFiles = new List<StagedMediaFile>();
        foreach (var file in files)
        {
            var ext = Path.GetExtension(file.FileName);
            if (!AllowedAudioExtensions.Contains(ext))
            {
                warnings.Add($"Unsupported audio format '{file.FileName}'. Skipping.");
                continue;
            }
            validFiles.Add(file);
            var slot = ExtractPageNumber(Path.GetFileName(file.FileName));
            if (slot >= 1 && slot <= targets.Count)
                assignments[slot] = file;
        }

        // Fallback: assign files with no leading number (e.g. intro.wav, scene2.wav) to remaining slots in 1:1 order.
        // This fixes multi-select import when user does not set overrides and filenames are not like "1.wav", "2.wav".
        var assignedFiles = new HashSet<StagedMediaFile>(assignments.Values);
        var unassignedFiles = validFiles.Where(f => !assignedFiles.Contains(f)).ToList();
        var unassignedSlots = Enumerable.Range(1, targets.Count).Where(s => !assignments.ContainsKey(s)).ToList();
        for (var i = 0; i < unassignedFiles.Count && i < unassignedSlots.Count; i++)
            assignments[unassignedSlots[i]] = unassignedFiles[i];

        foreach (var ov in payload.Overrides ?? new List<AudioImportOverride>())
        {
            if (string.IsNullOrWhiteSpace(ov.ClientFileId) || ov.TargetIndex < 1 || ov.TargetIndex > targets.Count)
                continue;
            if (!filesByClientId.TryGetValue(ov.ClientFileId, out var file))
            {
                warnings.Add($"Override file not found for clientFileId '{ov.ClientFileId}'.");
                continue;
            }
            assignments[ov.TargetIndex] = file;
        }

        var normalizedEmail = string.IsNullOrWhiteSpace(ownerEmail) ? string.Empty : Uri.UnescapeDataString(ownerEmail);
        var containerClient = _sas.GetContainerClient(_sas.DraftContainer);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        foreach (var kv in assignments.OrderBy(k => k.Key))
        {
            var slot = kv.Key;
            var file = kv.Value;
            var target = targets[slot - 1];

            try
            {
                var sourceBlobClient = _sas.GetBlobClient(_sas.DraftContainer, file.BlobPath);
                if (!await sourceBlobClient.ExistsAsync(ct))
                {
                    warnings.Add($"Staged file missing: {file.FileName}");
                    continue;
                }
                var props = await sourceBlobClient.GetPropertiesAsync(cancellationToken: ct);
                if (props.Value.ContentLength > MaxAudioFileSizeBytes)
                {
                    warnings.Add($"Audio file '{file.FileName}' exceeds maximum size of {MaxAudioFileSizeBytes / (1024 * 1024)}MB. Skipping.");
                    continue;
                }

                var audioFilename = Path.GetFileName(file.FileName);
                var draftPath = BuildDraftPath(new AssetInfo(audioFilename, AssetType.Audio, locale), normalizedEmail, craft.StoryId);

                if (target.Node != null)
                {
                    var nodeTranslation = target.Node.Translations.FirstOrDefault(nt =>
                        !string.IsNullOrWhiteSpace(nt.LanguageCode) && nt.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
                    if (nodeTranslation == null)
                    {
                        nodeTranslation = new StoryCraftDialogNodeTranslation
                        {
                            Id = Guid.NewGuid(),
                            StoryCraftDialogNodeId = target.Node.Id,
                            LanguageCode = locale
                        };
                        target.Node.Translations.Add(nodeTranslation);
                        _db.StoryCraftDialogNodeTranslations.Add(nodeTranslation);
                    }

                    if (!string.IsNullOrWhiteSpace(nodeTranslation.AudioUrl) && nodeTranslation.AudioUrl != audioFilename)
                    {
                        try
                        {
                            var oldPath = BuildDraftPath(new AssetInfo(nodeTranslation.AudioUrl, AssetType.Audio, locale), normalizedEmail, craft.StoryId);
                            var oldBlobClient = _sas.GetBlobClient(_sas.DraftContainer, oldPath);
                            await oldBlobClient.DeleteIfExistsAsync(cancellationToken: ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete old dialog node audio: {OldPath}", nodeTranslation.AudioUrl);
                        }
                    }

                    var download = await sourceBlobClient.DownloadStreamingAsync(cancellationToken: ct);
                    await using var sourceStream = download.Value.Content;
                    var destBlobClient = _sas.GetBlobClient(_sas.DraftContainer, draftPath);
                    await destBlobClient.UploadAsync(sourceStream, overwrite: true, cancellationToken: ct);
                    nodeTranslation.AudioUrl = audioFilename;
                    importedCount++;
                    changedTileIds.Add(target.Tile.TileId);
                }
                else
                {
                    var tile = target.Tile;
                    var tileTranslation = target.TileTranslation ?? tile.Translations.FirstOrDefault(t =>
                        !string.IsNullOrWhiteSpace(t.LanguageCode) && t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
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

                    var download = await sourceBlobClient.DownloadStreamingAsync(cancellationToken: ct);
                    await using var sourceStream = download.Value.Content;
                    var destBlobClient = _sas.GetBlobClient(_sas.DraftContainer, draftPath);
                    await destBlobClient.UploadAsync(sourceStream, overwrite: true, cancellationToken: ct);
                    tileTranslation.AudioUrl = audioFilename;
                    importedCount++;
                    changedTileIds.Add(tile.TileId);
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to import staged audio '{file.FileName}' for slot {slot}: {ex.Message}");
                _logger.LogError(ex, "Failed to import staged audio: {FileName}", file.FileName);
            }
        }

        if (importedCount > 0)
        {
            try
            {
                await _db.SaveChangesAsync(ct);
                await AppendTileChangeLogsAsync(craft, localeNorm, changedTileIds, ct);
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to save changes to database: {ex.Message}");
                _logger.LogError(ex, "Failed to save staged audio import changes");
            }
        }

        return new ImportAudioResult(errors.Count == 0, errors, warnings, importedCount, targets.Count);
    }

    private async Task AppendTileChangeLogsAsync(
        StoryCraft craft,
        string locale,
        IReadOnlyCollection<string> changedTileIds,
        CancellationToken ct)
    {
        if (changedTileIds == null || changedTileIds.Count == 0)
        {
            return;
        }

        var nextVersion = craft.LastDraftVersion + 1;
        var now = DateTime.UtcNow;
        var userId = craft.OwnerUserId;

        foreach (var tileId in changedTileIds.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            _db.StoryPublishChangeLogs.Add(new StoryPublishChangeLog
            {
                Id = Guid.NewGuid(),
                StoryId = craft.StoryId,
                DraftVersion = nextVersion,
                LanguageCode = locale,
                EntityType = "Tile",
                EntityId = tileId,
                ChangeType = "Updated",
                CreatedAt = now,
                CreatedBy = userId
            });
        }

        craft.LastDraftVersion = nextVersion;
        await _db.SaveChangesAsync(ct);
    }

    /// <summary>Builds the same ordered list as full audio export: one slot per page/quiz with text, one per dialog node with text.</summary>
    private static List<AudioImportTarget> BuildAudioImportTargets(StoryCraft craft, string locale)
    {
        var pageOrQuizOrDialog = new[] { "page", "quiz", "dialog" };
        var list = new List<AudioImportTarget>();
        foreach (var tile in craft.Tiles
            .Where(t => pageOrQuizOrDialog.Contains(t.Type, StringComparer.OrdinalIgnoreCase))
            .OrderBy(t => t.SortOrder))
        {
            if (string.Equals(tile.Type, "dialog", StringComparison.OrdinalIgnoreCase) && tile.DialogTile != null)
            {
                foreach (var node in tile.DialogTile.Nodes.OrderBy(n => n.SortOrder))
                {
                    var nodeText = node.Translations
                        .FirstOrDefault(nt => string.Equals(nt.LanguageCode, locale, StringComparison.OrdinalIgnoreCase))
                        ?.Text?.Trim() ?? node.Translations.FirstOrDefault()?.Text?.Trim() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(nodeText))
                    {
                        list.Add(new AudioImportTarget(tile, null, node));
                    }
                }
            }
            else
            {
                var tileText = ResolveTileText(tile, locale);
                if (!string.IsNullOrWhiteSpace(tileText))
                {
                    var tileTranslation = tile.Translations.FirstOrDefault(t =>
                        !string.IsNullOrWhiteSpace(t.LanguageCode) && t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
                    list.Add(new AudioImportTarget(tile, tileTranslation, null));
                }
            }
        }
        return list;
    }

    private static string ResolveTileText(StoryCraftTile tile, string locale)
    {
        var tr = tile.Translations.FirstOrDefault(t => string.Equals(t.LanguageCode, locale, StringComparison.OrdinalIgnoreCase))
                 ?? tile.Translations.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(tr?.Text)) return tr.Text.Trim();
        if (!string.IsNullOrWhiteSpace(tr?.Question)) return tr.Question.Trim();
        if (!string.IsNullOrWhiteSpace(tr?.Caption)) return tr.Caption.Trim();
        return string.Empty;
    }

    /// <summary>Slot index from filename. Supports both simple (1.wav, 4.wav) and standardized export names (01_page.wav, 04_dialog_n1_hero.wav).</summary>
    private static int ExtractPageNumber(string filename)
    {
        var match = Regex.Match(filename, @"^(\d+)");
        return match.Success && int.TryParse(match.Groups[1].Value, out var pageNumber) ? pageNumber : 0;
    }
}
