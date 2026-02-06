using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Text.RegularExpressions;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using static XooCreator.BA.Features.StoryEditor.Mappers.StoryAssetPathMapper;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public partial class ImportAudioEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IBlobSasService _sas;
    private readonly IStoryCraftsRepository _crafts;
    private readonly ILogger<ImportAudioEndpoint> _logger;
    private const long MaxZipSizeBytes = 100 * 1024 * 1024; // 100MB
    private const long MaxAudioFileSizeBytes = 10 * 1024 * 1024; // 10MB per audio file
    private static readonly HashSet<string> AllowedAudioExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".wav", ".mp3", ".m4a"
    };

    public ImportAudioEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IBlobSasService sas,
        IStoryCraftsRepository crafts,
        ILogger<ImportAudioEndpoint> logger)
    {
        _db = db;
        _auth0 = auth0;
        _sas = sas;
        _crafts = crafts;
        _logger = logger;
    }

    public record ImportAudioResponse
    {
        public bool Success { get; init; }
        public List<string> Errors { get; init; } = new();
        public List<string> Warnings { get; init; } = new();
        public int ImportedCount { get; init; }
        public int TotalPages { get; init; }
    }

    [Route("/api/{locale}/stories/{storyId}/import-audio")]
    [Authorize]
    [DisableRequestSizeLimit]
    public static async Task<Results<Ok<ImportAudioResponse>, BadRequest<ImportAudioResponse>, UnauthorizedHttpResult, ForbidHttpResult, NotFound>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] ImportAudioEndpoint ep,
        HttpRequest request,
        CancellationToken ct)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        if (!isAdmin)
        {
            ep._logger.LogWarning("Audio import forbidden: userId={UserId} storyId={StoryId} not admin", user.Id, storyId);
            return TypedResults.Forbid();
        }

        if (!request.HasFormContentType)
        {
            errors.Add("Request must be multipart/form-data");
            return TypedResults.BadRequest(new ImportAudioResponse { Success = false, Errors = errors });
        }

        var form = await request.ReadFormAsync(new Microsoft.AspNetCore.Http.Features.FormOptions
        {
            MultipartBodyLengthLimit = 120 * 1024 * 1024,
            ValueLengthLimit = int.MaxValue,
            KeyLengthLimit = int.MaxValue
        }, ct);

        var file = form.Files.GetFile("file");
        if (file == null || file.Length == 0)
        {
            errors.Add("No file provided");
            return TypedResults.BadRequest(new ImportAudioResponse { Success = false, Errors = errors });
        }

        if (file.Length > MaxZipSizeBytes)
        {
            errors.Add($"File size exceeds maximum allowed size of {MaxZipSizeBytes / (1024 * 1024)}MB");
            return TypedResults.BadRequest(new ImportAudioResponse { Success = false, Errors = errors });
        }

        if (!file.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("File must be a ZIP archive");
            return TypedResults.BadRequest(new ImportAudioResponse { Success = false, Errors = errors });
        }

        // Get story craft
        var craft = await ep._crafts.GetWithLanguageAsync(storyId, locale, ct);
        if (craft == null)
        {
            return TypedResults.NotFound();
        }

        // Get page tiles ordered by SortOrder
        var pageTiles = craft.Tiles
            .Where(t => t.Type.Equals("page", StringComparison.OrdinalIgnoreCase))
            .OrderBy(t => t.SortOrder)
            .ToList();

        if (pageTiles.Count == 0)
        {
            errors.Add("Story has no page tiles");
            return TypedResults.BadRequest(new ImportAudioResponse { Success = false, Errors = errors });
        }

        await using var zipStream = file.OpenReadStream();
        if (!zipStream.CanSeek)
        {
            errors.Add("Unable to process file stream for import.");
            return TypedResults.BadRequest(new ImportAudioResponse { Success = false, Errors = errors });
        }

        // Determine which email to use for asset path:
        // - Admin can import audio for any story, so use owner's email (preserve original author)
        // - This ensures assets are stored in the correct owner's folder
        string emailToUse = user.Email ?? string.Empty;
        if (isAdmin && craft.OwnerUserId != Guid.Empty && craft.OwnerUserId != user.Id)
        {
            // Admin importing for another user - use owner's email
            var ownerEmail = await ep._db.AlchimaliaUsers
                .AsNoTracking()
                .Where(u => u.Id == craft.OwnerUserId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync(ct);
            
            if (!string.IsNullOrWhiteSpace(ownerEmail))
            {
                emailToUse = ownerEmail;
                ep._logger.LogInformation("Admin importing audio for another user: storyId={StoryId} ownerEmail={OwnerEmail} adminEmail={AdminEmail}",
                    storyId, emailToUse, user.Email);
            }
        }
        else if (isAdmin && craft.OwnerUserId == user.Id)
        {
            // Admin importing for their own story - use their own email
            ep._logger.LogInformation("Admin importing audio for their own story: storyId={StoryId} email={Email}",
                storyId, emailToUse);
        }

        // Normalize locale to lowercase to match manual uploads (e.g., "ro-RO" -> "ro-ro")
        var normalizedLocale = locale?.ToLowerInvariant() ?? locale;
        
        var result = await ep.ProcessAudioImportAsync(zipStream, craft, pageTiles, normalizedLocale, emailToUse, ct);

        return TypedResults.Ok(result);
    }

    private async Task<ImportAudioResponse> ProcessAudioImportAsync(
        Stream zipStream,
        StoryCraft craft,
        List<StoryCraftTile> pageTiles,
        string locale,
        string userEmail,
        CancellationToken ct)
    {
        var errors = new List<string>();
        var warnings = new List<string>();
        var importedCount = 0;
        var normalizedEmail = string.IsNullOrWhiteSpace(userEmail)
            ? string.Empty
            : Uri.UnescapeDataString(userEmail);

        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: false);

        // Extract audio files from ZIP (expecting format: 1.wav, 2.wav, etc.)
        var audioEntries = zip.Entries
            .Where(e => !string.IsNullOrWhiteSpace(e.Name))
            .Where(e =>
            {
                var fileName = Path.GetFileName(e.Name);
                return !string.IsNullOrWhiteSpace(fileName) &&
                       AllowedAudioExtensions.Contains(Path.GetExtension(fileName));
            })
            .OrderBy(e => ExtractPageNumber(Path.GetFileName(e.Name)))
            .ToList();

        if (audioEntries.Count == 0)
        {
            errors.Add("No audio files found in ZIP archive");
            return new ImportAudioResponse { Success = false, Errors = errors };
        }

        var containerClient = _sas.GetContainerClient(_sas.DraftContainer);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        // Process each audio file
        foreach (var entry in audioEntries)
        {
            var entryFileName = Path.GetFileName(entry.Name);
            var pageNumber = ExtractPageNumber(entryFileName);
            if (pageNumber < 1 || pageNumber > pageTiles.Count)
            {
                warnings.Add($"Audio file '{entry.Name}' has page number {pageNumber}, but story only has {pageTiles.Count} pages. Skipping.");
                continue;
            }

            var tile = pageTiles[pageNumber - 1]; // Convert to 0-based index
            var tileTranslation = tile.Translations.FirstOrDefault(t =>
                !string.IsNullOrWhiteSpace(t.LanguageCode) &&
                t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));

            if (tileTranslation == null)
            {
                // Create translation if it doesn't exist
                tileTranslation = new StoryCraftTileTranslation
                {
                    Id = Guid.NewGuid(),
                    StoryCraftTileId = tile.Id,
                    LanguageCode = locale
                };
                tile.Translations.Add(tileTranslation);
                _db.StoryCraftTileTranslations.Add(tileTranslation);
            }

            // Extract audio file from ZIP
            await using var entryStream = entry.Open();
            if (entry.Length > MaxAudioFileSizeBytes)
            {
                warnings.Add($"Audio file '{entry.Name}' exceeds maximum size of {MaxAudioFileSizeBytes / (1024 * 1024)}MB. Skipping.");
                continue;
            }

            // Read audio bytes
            using var memoryStream = new MemoryStream();
            await entryStream.CopyToAsync(memoryStream, ct);
            var audioBytes = memoryStream.ToArray();

            // Build draft path for audio
            var audioFilename = entryFileName;
            var draftPath = BuildDraftPath(new AssetInfo(audioFilename, AssetType.Audio, locale), normalizedEmail, craft.StoryId);

            // Delete old audio file if exists
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

            // Upload new audio file
            try
            {
                var blobClient = _sas.GetBlobClient(_sas.DraftContainer, draftPath);
                await blobClient.UploadAsync(new BinaryData(audioBytes), overwrite: true, cancellationToken: ct);

                // Update tile translation with new audio filename
                tileTranslation.AudioUrl = audioFilename;
                importedCount++;

                _logger.LogInformation("Imported audio for page {PageNumber} (tile {TileId}): {Filename}",
                    pageNumber, tile.Id, audioFilename);
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to upload audio file '{entry.Name}' for page {pageNumber}: {ex.Message}");
                _logger.LogError(ex, "Failed to upload audio file: {Filename}", entry.Name);
            }
        }

        // Save changes to database
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

        return new ImportAudioResponse
        {
            Success = errors.Count == 0,
            Errors = errors,
            Warnings = warnings,
            ImportedCount = importedCount,
            TotalPages = pageTiles.Count
        };
    }

    private static int ExtractPageNumber(string filename)
    {
        // Extract number from filename (e.g., "1.wav" -> 1, "10.wav" -> 10)
        var match = Regex.Match(filename, @"^(\d+)");
        if (match.Success && int.TryParse(match.Groups[1].Value, out var pageNumber))
        {
            return pageNumber;
        }
        return 0; // Invalid format
    }
}
