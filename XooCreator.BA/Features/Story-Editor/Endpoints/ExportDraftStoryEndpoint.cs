using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using static XooCreator.BA.Features.StoryEditor.Mappers.StoryAssetPathMapper;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class ExportDraftStoryEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IUserContextService _userContext;
    private readonly IBlobSasService _sas;
    private readonly IStoryCraftsRepository _crafts;
    private readonly ILogger<ExportDraftStoryEndpoint> _logger;
    private readonly TelemetryClient? _telemetryClient;

    public ExportDraftStoryEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IUserContextService userContext,
        IBlobSasService sas,
        IStoryCraftsRepository crafts,
        ILogger<ExportDraftStoryEndpoint> logger,
        TelemetryClient? telemetryClient = null)
    {
        _db = db;
        _auth0 = auth0;
        _userContext = userContext;
        _sas = sas;
        _crafts = crafts;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    [Route("/api/{locale}/stories/{storyId}/export-draft")]
    [Authorize]
    public static async Task<Results<FileContentHttpResult, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] ExportDraftStoryEndpoint ep,
        CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        var outcome = "Unknown";
        AlchimaliaUser? currentUser = null;
        string? userId = null;
        string? userEmail = null;
        var isAdmin = false;
        var isCreator = false;
        var isOwner = false;
        var languageCount = 0;
        var uniqueAssetCount = 0;
        long zipBytes = 0;
        string? ownerEmail = null;

        try
        {
            currentUser = await ep._auth0.GetCurrentUserAsync(ct);
            if (currentUser == null)
            {
                outcome = "Unauthorized";
                return TypedResults.Unauthorized();
            }

            userId = currentUser.Id.ToString();
            userEmail = currentUser.Email;

            // Allow Creator (owner) or Admin to export
            isAdmin = ep._auth0.HasRole(currentUser, UserRole.Admin);
            isCreator = ep._auth0.HasRole(currentUser, UserRole.Creator);

            if (!isAdmin && !isCreator)
            {
                outcome = "Forbidden";
                return TypedResults.Forbid();
            }

            var craft = await ep._crafts.GetAsync(storyId, ct);
            if (craft == null)
            {
                outcome = "NotFound";
                return TypedResults.NotFound();
            }

            isOwner = craft.OwnerUserId == currentUser.Id;

            // If not Admin, verify ownership
            if (!isAdmin && !isOwner)
            {
                ep._logger.LogWarning("Export draft forbidden: userId={UserId} storyId={StoryId} not owner", currentUser.Id, storyId);
                outcome = "Forbidden";
                return TypedResults.Forbid();
            }

            // Resolve owner email for building blob paths
            ownerEmail = await ep.ResolveOwnerEmailAsync(craft.OwnerUserId, currentUser, ct);
            if (string.IsNullOrWhiteSpace(ownerEmail))
            {
                ep._logger.LogWarning("Export draft failed: cannot resolve owner email for storyId={StoryId} ownerId={OwnerId}", storyId, craft.OwnerUserId);
                outcome = "NotFound";
                return TypedResults.NotFound();
            }

            // Build export JSON
            var exportObj = BuildExportJson(craft);
            var exportJson = JsonSerializer.Serialize(exportObj, new JsonSerializerOptions { WriteIndented = true });
            var fileName = $"{craft.StoryId}-draft-export.zip";

            // Collect all assets for all languages with metadata
            var allAssets = new List<(AssetInfo Asset, string BlobPath, bool IsCoverImage)>();
            var availableLanguages = craft.Translations.Select(t => t.LanguageCode).Distinct().ToList();
            languageCount = availableLanguages.Count;

            // Extract assets for each language
            foreach (var lang in availableLanguages)
            {
                var assets = StoryAssetPathMapper.ExtractAssets(craft, lang);
                foreach (var asset in assets)
                {
                    var blobPath = StoryAssetPathMapper.BuildDraftPath(asset, ownerEmail, craft.StoryId);
                    // Check if this is the cover image
                    var isCoverImage = asset.Type == StoryAssetPathMapper.AssetType.Image &&
                                       !string.IsNullOrWhiteSpace(craft.CoverImageUrl) &&
                                       asset.Filename.Equals(craft.CoverImageUrl, StringComparison.OrdinalIgnoreCase);
                    allAssets.Add((asset, blobPath, isCoverImage));
                }
            }

            // Remove duplicates (same blob path)
            var uniqueAssets = allAssets
                .GroupBy(a => a.BlobPath, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();
            uniqueAssetCount = uniqueAssets.Count;

            // Build ZIP
            using var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
            {
                // Add manifest JSON
                var manifestEntry = zip.CreateEntry($"manifest/{craft.StoryId}/story.json", CompressionLevel.Fastest);
                await using (var writer = new StreamWriter(manifestEntry.Open(), new UTF8Encoding(false)))
                {
                    await writer.WriteAsync(exportJson);
                }

                // Download and add media files from draft container
                foreach (var (asset, blobPath, isCoverImage) in uniqueAssets)
                {
                    try
                    {
                        var client = ep._sas.GetBlobClient(ep._sas.DraftContainer, blobPath);

                        // Check if blob exists
                        if (!await client.ExistsAsync(ct))
                        {
                            ep._logger.LogWarning("Asset not found in draft storage: {BlobPath}", blobPath);
                            continue;
                        }

                        // Build ZIP entry path using asset metadata
                        var zipEntryPath = BuildZipEntryPath(asset, isCoverImage);
                        var entry = zip.CreateEntry(zipEntryPath, CompressionLevel.Fastest);

                        await using var entryStream = entry.Open();
                        var download = await client.DownloadStreamingAsync(cancellationToken: ct);
                        await download.Value.Content.CopyToAsync(entryStream, ct);
                    }
                    catch (Exception ex)
                    {
                        ep._logger.LogWarning(ex, "Failed to download asset from draft: {BlobPath}", blobPath);
                        // Continue with other assets even if one fails
                    }
                }
            }

            zipBytes = ms.Length;
            var bytes = ms.ToArray();

            outcome = "Success";
            return TypedResults.File(bytes, "application/zip", fileName);
        }
        finally
        {
            stopwatch.Stop();
            ep.TrackExportTelemetry(
                durationMs: stopwatch.ElapsedMilliseconds,
                outcome: outcome,
                locale: locale,
                storyId: storyId,
                userId: userId,
                userEmail: userEmail,
                isAdmin: isAdmin,
                isCreator: isCreator,
                isOwner: isOwner,
                languageCount: languageCount,
                assetCount: uniqueAssetCount,
                zipBytes: zipBytes);
        }
    }

    private async Task<string?> ResolveOwnerEmailAsync(Guid ownerId, AlchimaliaUser currentUser, CancellationToken ct)
    {
        if (ownerId == currentUser.Id)
        {
            return currentUser.Email;
        }

        return await _db.AlchimaliaUsers
            .AsNoTracking()
            .Where(u => u.Id == ownerId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync(ct);
    }

    private void TrackExportTelemetry(
        long durationMs,
        string outcome,
        string locale,
        string storyId,
        string? userId,
        string? userEmail,
        bool isAdmin,
        bool isCreator,
        bool isOwner,
        int languageCount,
        int assetCount,
        long zipBytes)
    {
        var zipMb = zipBytes / 1024d / 1024d;

        _logger.LogInformation(
            "Export draft telemetry | storyId={StoryId} locale={Locale} outcome={Outcome} durationMs={DurationMs} languages={Languages} assets={Assets} zipSizeMB={ZipSizeMB:F2} isAdmin={IsAdmin} isCreator={IsCreator} isOwner={IsOwner}",
            storyId,
            locale,
            outcome,
            durationMs,
            languageCount,
            assetCount,
            zipMb,
            isAdmin,
            isCreator,
            isOwner);

        var properties = new Dictionary<string, string?>
        {
            ["IsDraft"] = bool.TrueString,
            ["Outcome"] = outcome,
            ["Locale"] = locale,
            ["StoryId"] = storyId,
            ["UserId"] = userId,
            ["UserEmail"] = userEmail,
            ["Languages"] = languageCount.ToString(CultureInfo.InvariantCulture),
            ["Assets"] = assetCount.ToString(CultureInfo.InvariantCulture),
            ["IsAdmin"] = isAdmin.ToString(CultureInfo.InvariantCulture),
            ["IsCreator"] = isCreator.ToString(CultureInfo.InvariantCulture),
            ["IsOwner"] = isOwner.ToString(CultureInfo.InvariantCulture)
        };

        TrackMetric("ExportStory_Duration", durationMs, properties);
        TrackMetric("ExportStory_LanguageCount", languageCount, properties);
        TrackMetric("ExportStory_AssetCount", assetCount, properties);
        if (zipBytes > 0)
        {
            TrackMetric("ExportStory_ZipSizeMB", zipMb, properties);
        }
    }

    private void TrackMetric(string metricName, double value, IReadOnlyDictionary<string, string?> properties)
    {
        if (_telemetryClient == null)
        {
            return;
        }

        var metric = new MetricTelemetry(metricName, value);
        foreach (var kvp in properties)
        {
            if (!string.IsNullOrEmpty(kvp.Value))
            {
                metric.Properties[kvp.Key] = kvp.Value!;
            }
        }

        _telemetryClient.TrackMetric(metric);
    }

    private static object BuildExportJson(StoryCraft craft)
    {
        // Get primary translation (first one or default)
        var primaryTranslation = craft.Translations.FirstOrDefault() ?? new StoryCraftTranslation
        {
            LanguageCode = "ro-ro",
            Title = string.Empty,
            Summary = null
        };

        // Extract topic IDs
        var topicIds = craft.Topics?
            .Where(t => t.StoryTopic != null)
            .Select(t => t.StoryTopic!.TopicId)
            .ToList() ?? new List<string>();

        // Extract age group IDs
        var ageGroupIds = craft.AgeGroups?
            .Where(ag => ag.StoryAgeGroup != null)
            .Select(ag => ag.StoryAgeGroup!.AgeGroupId)
            .ToList() ?? new List<string>();

        return new
        {
            id = craft.StoryId,
            version = craft.BaseVersion > 0 ? craft.BaseVersion : 0,
            title = primaryTranslation.Title,
            summary = primaryTranslation.Summary ?? craft.StoryTopic,
            storyType = craft.StoryType,
            coverImageUrl = craft.CoverImageUrl,
            storyTopic = craft.StoryTopic,
            topicIds = topicIds,
            ageGroupIds = ageGroupIds,
            authorName = craft.AuthorName,
            classicAuthorId = craft.ClassicAuthorId,
            priceInCredits = craft.PriceInCredits,
            translations = craft.Translations.Select(t => new
            {
                lang = t.LanguageCode,
                title = t.Title,
                summary = t.Summary
            }).ToList(),
            tiles = craft.Tiles
                .OrderBy(t => t.SortOrder)
                .Select(t => new
                {
                    id = t.TileId,
                    type = t.Type,
                    sortOrder = t.SortOrder,
                    imageUrl = t.ImageUrl,
                    // Audio and Video are language-specific (in translations)
                    translations = t.Translations.Select(tr => new
                    {
                        lang = tr.LanguageCode,
                        caption = tr.Caption,
                        text = tr.Text,
                        question = tr.Question,
                        audioUrl = tr.AudioUrl,
                        videoUrl = tr.VideoUrl
                    }).ToList(),
                    answers = (t.Answers ?? new()).OrderBy(a => a.SortOrder).Select(a => new
                    {
                        id = a.AnswerId,
                        sortOrder = a.SortOrder,
                        tokens = (a.Tokens ?? new()).Select(tok => new { type = tok.Type, value = tok.Value, quantity = tok.Quantity }).ToList(),
                        translations = (a.Translations ?? new()).Select(at => new
                        {
                            lang = at.LanguageCode,
                            text = at.Text
                        }).ToList()
                    }).ToList()
                }).ToList()
        };
    }

    /// <summary>
    /// Builds the ZIP entry path from asset info, organizing media by type and language.
    /// Structure: media/{type}/{lang?}/{filename}
    /// - Cover Images: media/images/cover/{filename} (no lang, common for all languages)
    /// - Tile Images: media/images/tiles/{filename} (no lang, common for all languages)
    /// - Audio: media/audio/{lang}/{filename}
    /// - Video: media/video/{lang}/{filename}
    /// </summary>
    private static string BuildZipEntryPath(AssetInfo asset, bool isCoverImage)
    {
        var mediaType = asset.Type switch
        {
            StoryAssetPathMapper.AssetType.Image => "images",
            StoryAssetPathMapper.AssetType.Audio => "audio",
            StoryAssetPathMapper.AssetType.Video => "video",
            _ => "images"
        };

        // Images are language-agnostic
        if (asset.Type == StoryAssetPathMapper.AssetType.Image)
        {
            if (isCoverImage)
            {
                return $"media/{mediaType}/cover/{asset.Filename}";
            }
            // Otherwise it's a tile image
            return $"media/{mediaType}/tiles/{asset.Filename}";
        }

        // Audio and Video are language-specific
        if (!string.IsNullOrWhiteSpace(asset.Lang))
        {
            return $"media/{mediaType}/{asset.Lang}/{asset.Filename}";
        }

        // Fallback (shouldn't happen for audio/video)
        return $"media/{mediaType}/{asset.Filename}";
    }
}

