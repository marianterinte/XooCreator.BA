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
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class ExportPublishedStoryEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IUserContextService _userContext;
    private readonly IBlobSasService _sas;
    private readonly ILogger<ExportPublishedStoryEndpoint> _logger;
    private readonly TelemetryClient? _telemetryClient;

    public ExportPublishedStoryEndpoint(
        XooDbContext db, 
        IAuth0UserService auth0, 
        IUserContextService userContext, 
        IBlobSasService sas,
        ILogger<ExportPublishedStoryEndpoint> logger,
        TelemetryClient? telemetryClient = null)
    {
        _db = db;
        _auth0 = auth0;
        _userContext = userContext;
        _sas = sas;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    [Route("/api/{locale}/stories/{storyId}/export")]
    [Authorize]
    public static async Task<Results<FileContentHttpResult, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] ExportPublishedStoryEndpoint ep,
        CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        var outcome = "Unknown";
        string? userId = null;
        string? userEmail = null;
        var isAdmin = false;
        var isCreator = false;
        var isOwner = false;
        int version = 0;
        var mediaCount = 0;
        long zipBytes = 0;

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            outcome = "Unauthorized";
            return TypedResults.Unauthorized();
        }
        userId = user.Id.ToString();
        userEmail = user.Email;

        // Allow Creator (owner) or Admin to export
        isAdmin = ep._auth0.HasRole(user, Data.Enums.UserRole.Admin);
        isCreator = ep._auth0.HasRole(user, Data.Enums.UserRole.Creator);
        
        if (!isAdmin && !isCreator)
        {
            outcome = "Forbidden";
            return TypedResults.Forbid();
        }

        var def = await ep._db.StoryDefinitions
            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(d => d.Tiles).ThenInclude(t => t.Translations)
            .Include(d => d.Translations)
            .Include(d => d.Topics).ThenInclude(t => t.StoryTopic)
            .Include(d => d.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .FirstOrDefaultAsync(d => d.StoryId == storyId, ct);
        if (def == null)
        {
            outcome = "NotFound";
            return TypedResults.NotFound();
        }

        version = def.Version;

        isOwner = def.CreatedBy.HasValue && def.CreatedBy.Value == user.Id;

        // If not Admin, verify ownership
        if (!isAdmin)
        {
            if (!isOwner)
            {
                ep._logger.LogWarning("Export forbidden: userId={UserId} storyId={StoryId} not owner", user.Id, storyId);
                outcome = "Forbidden";
                return TypedResults.Forbid();
            }
        }

        try
        {
            var exportObj = BuildExportJson(def);
            var exportJson = JsonSerializer.Serialize(exportObj, new JsonSerializerOptions { WriteIndented = true });
            var fileName = $"{def.StoryId}-v{def.Version}.zip";

            using var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
            {
                // Add manifest JSON
                var manifestEntry = zip.CreateEntry($"manifest/{def.StoryId}/v{def.Version}/story.json", CompressionLevel.Fastest);
                await using (var writer = new StreamWriter(manifestEntry.Open(), new UTF8Encoding(false)))
                {
                    await writer.WriteAsync(exportJson);
                }

                // Collect media paths from definition (already in published layout)
                var mediaPaths = CollectPublishedMediaPaths(def);
                mediaCount = mediaPaths.Count;
                foreach (var path in mediaPaths)
                {
                    // Download from published container and add to ZIP preserving relative path under "media/"
                    var client = ep._sas.GetBlobClient(ep._sas.PublishedContainer, path);
                    var entry = zip.CreateEntry($"media/{path}".Replace('\\', '/'), CompressionLevel.Fastest);
                    await using var entryStream = entry.Open();
                    var download = await client.DownloadStreamingAsync(cancellationToken: ct);
                    await download.Value.Content.CopyToAsync(entryStream, ct);
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
                isDraft: false,
                durationMs: stopwatch.ElapsedMilliseconds,
                outcome: outcome,
                locale: locale,
                storyId: storyId,
                userId: userId,
                userEmail: userEmail,
                version: version,
                mediaCount: mediaCount,
                zipBytes: zipBytes,
                isAdmin: isAdmin,
                isCreator: isCreator,
                isOwner: isOwner);
        }
    }

    private void TrackExportTelemetry(
        bool isDraft,
        long durationMs,
        string outcome,
        string locale,
        string storyId,
        string? userId,
        string? userEmail,
        int version,
        int mediaCount,
        long zipBytes,
        bool isAdmin,
        bool isCreator,
        bool isOwner)
    {
        var zipMb = zipBytes / 1024d / 1024d;

        _logger.LogInformation(
            "Export story telemetry | storyId={StoryId} locale={Locale} outcome={Outcome} durationMs={DurationMs} mediaCount={MediaCount} zipSizeMB={ZipSizeMB:F2} isDraft={IsDraft} isAdmin={IsAdmin} isCreator={IsCreator} isOwner={IsOwner}",
            storyId,
            locale,
            outcome,
            durationMs,
            mediaCount,
            zipMb,
            isDraft,
            isAdmin,
            isCreator,
            isOwner);

        var properties = new Dictionary<string, string?>
        {
            ["IsDraft"] = isDraft.ToString(CultureInfo.InvariantCulture),
            ["Outcome"] = outcome,
            ["Locale"] = locale,
            ["StoryId"] = storyId,
            ["UserId"] = userId,
            ["UserEmail"] = userEmail,
            ["Version"] = version.ToString(CultureInfo.InvariantCulture),
            ["MediaCount"] = mediaCount.ToString(CultureInfo.InvariantCulture),
            ["IsAdmin"] = isAdmin.ToString(CultureInfo.InvariantCulture),
            ["IsCreator"] = isCreator.ToString(CultureInfo.InvariantCulture),
            ["IsOwner"] = isOwner.ToString(CultureInfo.InvariantCulture)
        };

        TrackMetric("ExportStory_Duration", durationMs, properties);
        TrackMetric("ExportStory_MediaCount", mediaCount, properties);
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

    private static object BuildExportJson(StoryDefinition def)
    {
        // Extract topic IDs
        var topicIds = def.Topics?
            .Where(t => t.StoryTopic != null)
            .Select(t => t.StoryTopic!.TopicId)
            .ToList() ?? new List<string>();

        // Extract age group IDs
        var ageGroupIds = def.AgeGroups?
            .Where(ag => ag.StoryAgeGroup != null)
            .Select(ag => ag.StoryAgeGroup!.AgeGroupId)
            .ToList() ?? new List<string>();

        return new
        {
            id = def.StoryId,
            version = def.Version,
            title = def.Title,
            summary = def.Summary,
            storyType = def.StoryType,
            coverImageUrl = def.CoverImageUrl,
            storyTopic = def.StoryTopic,
            topicIds = topicIds,
            ageGroupIds = ageGroupIds,
            authorName = def.AuthorName,
            classicAuthorId = def.ClassicAuthorId,
            priceInCredits = def.PriceInCredits,
            translations = def.Translations.Select(t => new
            {
                lang = t.LanguageCode,
                title = t.Title
            }).ToList(),
            tiles = def.Tiles
                .OrderBy(t => t.SortOrder)
                .Select(t => new
                {
                    id = t.TileId,
                    type = t.Type,
                    sortOrder = t.SortOrder,
                    imageUrl = t.ImageUrl,
                    // Audio and Video are now language-specific (in translations)
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
                        tokens = (a.Tokens ?? new()).Select(tok => new { type = tok.Type, value = tok.Value, quantity = tok.Quantity })
                    }).ToList()
                }).ToList()
        };
    }

    private static List<string> CollectPublishedMediaPaths(StoryDefinition def)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(def.CoverImageUrl)) result.Add(Normalize(def.CoverImageUrl));
        foreach (var t in def.Tiles)
        {
            // Image is common for all languages
            if (!string.IsNullOrWhiteSpace(t.ImageUrl)) result.Add(Normalize(t.ImageUrl));
            
            // Audio and Video are now language-specific (in translations)
            foreach (var tr in t.Translations)
            {
                if (!string.IsNullOrWhiteSpace(tr.AudioUrl)) result.Add(Normalize(tr.AudioUrl));
                if (!string.IsNullOrWhiteSpace(tr.VideoUrl)) result.Add(Normalize(tr.VideoUrl));
            }
        }
        return result.ToList();
    }

    private static string Normalize(string path)
    {
        return path.TrimStart('/').Replace('\\', '/');
    }
}


