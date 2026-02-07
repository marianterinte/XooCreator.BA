using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Endpoints;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Jobs;

namespace XooCreator.BA.Infrastructure.Endpoints;

[Endpoint]
public sealed class JobEventsSseEndpoint
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private static readonly HashSet<string> KnownJobTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        JobTypes.StoryVersion,
        JobTypes.StoryPublish,
        JobTypes.StoryImport,
        JobTypes.StoryExport,
        JobTypes.StoryFork,
        JobTypes.StoryForkAssets,
        JobTypes.StoryDocumentExport,
        JobTypes.StoryAudioExport,
        JobTypes.StoryAudioImport,
        JobTypes.EpicVersion,
        JobTypes.EpicPublish,
        JobTypes.HeroVersion,
        JobTypes.RegionVersion,
        JobTypes.HeroPublish,
        JobTypes.AnimalPublish,
        JobTypes.HeroDefinitionVersion
    };

    [Route("/api/jobs/{jobType}/{jobId}/events")]
    [Authorize]
    public static async Task HandleGet(
        [FromRoute] string jobType,
        [FromRoute] Guid jobId,
        [FromServices] IJobEventsHub hub,
        [FromServices] XooDbContext db,
        [FromServices] IAuth0UserService auth0,
        HttpContext http,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(jobType) || !KnownJobTypes.Contains(jobType))
        {
            http.Response.StatusCode = StatusCodes.Status400BadRequest;
            await http.Response.WriteAsync($"Unknown jobType '{jobType}'.", ct);
            return;
        }

        var user = await auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            http.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var isAdmin = auth0.HasRole(user, UserRole.Admin);
        var isCreator = auth0.HasRole(user, UserRole.Creator);
        if (!isAdmin && !isCreator)
        {
            http.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        // Build an initial snapshot from DB so a late subscriber doesn't hang if the job is already finished.
        var snapshot = await TryBuildSnapshotAsync(jobType, jobId, db, user, auth0, ct);
        if (snapshot == null)
        {
            http.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        // SSE headers
        http.Response.StatusCode = StatusCodes.Status200OK;
        http.Response.Headers.CacheControl = "no-cache";
        http.Response.Headers.Connection = "keep-alive";
        http.Response.Headers.ContentType = "text/event-stream";
        http.Response.Headers["X-Accel-Buffering"] = "no";

        await WriteDataAsync(http, snapshot.PayloadJson, ct);

        if (IsTerminalStatus(snapshot.Status))
        {
            return;
        }

        await using var stream = hub.Subscribe(jobType, jobId);
        var keepAliveAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(15);

        while (!ct.IsCancellationRequested)
        {
            // Keep-alive to prevent idle timeouts.
            if (DateTimeOffset.UtcNow >= keepAliveAt)
            {
                await http.Response.WriteAsync(": keep-alive\n\n", ct);
                await http.Response.Body.FlushAsync(ct);
                keepAliveAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(15);
            }

            // Drain any available events; if none, wait briefly so we can keep-alive.
            while (stream.Reader.TryRead(out var json))
            {
                await WriteDataAsync(http, json, ct);

                var status = TryExtractStatus(json);
                if (status != null && IsTerminalStatus(status))
                {
                    return;
                }
            }

            await Task.Delay(200, ct);
        }

        return;
    }

    private static async Task WriteDataAsync(HttpContext http, string json, CancellationToken ct)
    {
        await http.Response.WriteAsync($"data: {json}\n\n", ct);
        await http.Response.Body.FlushAsync(ct);
    }

    private static bool IsTerminalStatus(string status) =>
        status.Equals("Completed", StringComparison.OrdinalIgnoreCase) ||
        status.Equals("Failed", StringComparison.OrdinalIgnoreCase) ||
        status.Equals("Superseded", StringComparison.OrdinalIgnoreCase);

    private static string? TryExtractStatus(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("status", out var statusProp) && statusProp.ValueKind == JsonValueKind.String)
            {
                return statusProp.GetString();
            }
        }
        catch
        {
            // ignore parsing failures
        }

        return null;
    }

    private sealed record Snapshot(string Status, string PayloadJson);

    private static async Task<Snapshot?> TryBuildSnapshotAsync(
        string jobType,
        Guid jobId,
        XooDbContext db,
        AlchimaliaUser user,
        IAuth0UserService auth0,
        CancellationToken ct)
    {
        // NOTE: This intentionally mirrors existing GET status endpoints' auth rules as closely as possible.
        switch (jobType.ToLowerInvariant())
        {
            case JobTypes.StoryVersion:
            {
                var job = await db.StoryVersionJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                var payload = new
                {
                    jobId = job.Id,
                    storyId = job.StoryId,
                    status = job.Status,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    dequeueCount = job.DequeueCount,
                    baseVersion = job.BaseVersion
                };

                return new Snapshot(payload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(payload, Json));
            }
            case JobTypes.StoryPublish:
            {
                var job = await db.StoryPublishJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                var payload = new
                {
                    jobId = job.Id,
                    storyId = job.StoryId,
                    status = job.Status.ToString(),
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    dequeueCount = job.DequeueCount
                };

                return new Snapshot(payload.status, JsonSerializer.Serialize(payload, Json));
            }
            case JobTypes.StoryImport:
            {
                var job = await db.StoryImportJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                // Existing status endpoint is creator/admin only; owner is enforced elsewhere in app.
                if (!auth0.HasRole(user, UserRole.Admin) && job.OwnerUserId != user.Id)
                {
                    return null;
                }

                var payload = new
                {
                    jobId = job.Id,
                    storyId = job.StoryId,
                    originalStoryId = job.OriginalStoryId,
                    status = job.Status,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    importedAssets = job.ImportedAssets,
                    totalAssets = job.TotalAssets,
                    importedLanguagesCount = job.ImportedLanguagesCount,
                    errorMessage = job.ErrorMessage,
                    warningSummary = job.WarningSummary,
                    dequeueCount = job.DequeueCount
                };

                return new Snapshot(payload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(payload, Json));
            }
            case JobTypes.StoryExport:
            {
                var job = await db.StoryExportJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                if (!auth0.HasRole(user, UserRole.Admin) && job.OwnerUserId != user.Id)
                {
                    return null;
                }

                var payload = new
                {
                    jobId = job.Id,
                    storyId = job.StoryId,
                    status = job.Status,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    zipDownloadUrl = (string?)null, // fetch via existing GET /api/stories/{storyId}/export-jobs/{jobId}
                    zipFileName = job.ZipFileName,
                    zipSizeBytes = job.ZipSizeBytes,
                    mediaCount = job.MediaCount,
                    languageCount = job.LanguageCount
                };

                return new Snapshot(payload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(payload, Json));
            }
            case JobTypes.StoryFork:
            {
                var job = await db.StoryForkJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                if (!auth0.HasRole(user, UserRole.Admin) && job.TargetOwnerUserId != user.Id)
                {
                    return null;
                }

                ForkStoryEndpoint.ForkStoryAssetJobStatus? assetJob = null;
                if (job.AssetJobId.HasValue)
                {
                    var aj = await db.StoryForkAssetJobs.AsNoTracking().FirstOrDefaultAsync(a => a.Id == job.AssetJobId.Value, ct);
                    if (aj != null)
                    {
                        assetJob = new ForkStoryEndpoint.ForkStoryAssetJobStatus
                        {
                            JobId = aj.Id,
                            Status = aj.Status,
                            AttemptedAssets = aj.AttemptedAssets,
                            CopiedAssets = aj.CopiedAssets,
                            DequeueCount = aj.DequeueCount,
                            QueuedAtUtc = aj.QueuedAtUtc,
                            StartedAtUtc = aj.StartedAtUtc,
                            CompletedAtUtc = aj.CompletedAtUtc,
                            ErrorMessage = aj.ErrorMessage,
                            WarningSummary = aj.WarningSummary
                        };
                    }
                }

                var payload = new
                {
                    jobId = job.Id,
                    storyId = job.TargetStoryId,
                    sourceStoryId = job.SourceStoryId,
                    status = job.Status,
                    copyAssets = job.CopyAssets,
                    queueName = (string?)null,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    warningSummary = job.WarningSummary,
                    sourceTranslations = job.SourceTranslations,
                    sourceTiles = job.SourceTiles,
                    assetJob = assetJob
                };

                return new Snapshot(payload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(payload, Json));
            }
            case JobTypes.StoryForkAssets:
            {
                var job = await db.StoryForkAssetJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                // Avoid leaking job existence unless admin or target owner (derive from parent job if possible).
                if (!auth0.HasRole(user, UserRole.Admin))
                {
                    var parent = await db.StoryForkJobs.AsNoTracking().FirstOrDefaultAsync(j => j.AssetJobId == job.Id, ct);
                    if (parent == null || parent.TargetOwnerUserId != user.Id)
                    {
                        return null;
                    }
                }

                var payload = new
                {
                    jobId = job.Id,
                    status = job.Status,
                    attemptedAssets = job.AttemptedAssets,
                    copiedAssets = job.CopiedAssets,
                    dequeueCount = job.DequeueCount,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    warningSummary = job.WarningSummary
                };

                return new Snapshot(payload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(payload, Json));
            }
            case JobTypes.StoryDocumentExport:
            {
                var job = await db.StoryDocumentExportJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                // Match existing endpoint: admin OR requestor OR story owner.
                if (!auth0.HasRole(user, UserRole.Admin) && job.RequestedByUserId != user.Id && job.StoryOwnerUserId != user.Id)
                {
                    return null;
                }

                var payload = new
                {
                    jobId = job.Id,
                    storyId = job.StoryId,
                    status = job.Status,
                    format = job.Format,
                    locale = job.Locale,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    downloadUrl = (string?)null, // fetch via existing GET /api/stories/{storyId}/pdf-jobs/{jobId}
                    outputFileName = job.OutputFileName,
                    outputSizeBytes = job.OutputSizeBytes
                };

                return new Snapshot(payload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(payload, Json));
            }
            case JobTypes.StoryAudioExport:
            {
                var job = await db.StoryAudioExportJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                if (!auth0.HasRole(user, UserRole.Admin) && job.OwnerUserId != user.Id)
                {
                    return null;
                }

                var payload = new
                {
                    jobId = job.Id,
                    storyId = job.StoryId,
                    status = job.Status,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    zipDownloadUrl = (string?)null, // fetch via GET /api/stories/{storyId}/audio-export-jobs/{jobId}
                    zipFileName = job.ZipFileName,
                    zipSizeBytes = job.ZipSizeBytes,
                    audioCount = job.AudioCount
                };

                return new Snapshot(payload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(payload, Json));
            }
            case JobTypes.StoryAudioImport:
            {
                var job = await db.StoryAudioImportJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                if (!auth0.HasRole(user, UserRole.Admin) && job.OwnerUserId != user.Id)
                {
                    return null;
                }

                var importPayload = new
                {
                    jobId = job.Id,
                    storyId = job.StoryId,
                    status = job.Status,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    success = job.Success,
                    importedCount = job.ImportedCount,
                    totalPages = job.TotalPages,
                    errorsJson = job.ErrorsJson,
                    warningsJson = job.WarningsJson
                };

                return new Snapshot(importPayload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(importPayload, Json));
            }
            case JobTypes.EpicVersion:
            {
                var job = await db.EpicVersionJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                // Existing epic job status endpoint enforces ownership unless admin.
                if (!auth0.HasRole(user, UserRole.Admin) && job.OwnerUserId != user.Id)
                {
                    return null; // treat as NotFound to avoid leaking job existence
                }

                var payload = new
                {
                    jobId = job.Id,
                    epicId = job.EpicId,
                    status = job.Status,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    dequeueCount = job.DequeueCount,
                    baseVersion = job.BaseVersion
                };

                return new Snapshot(payload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(payload, Json));
            }
            case JobTypes.EpicPublish:
            {
                var job = await db.EpicPublishJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                if (!auth0.HasRole(user, UserRole.Admin) && job.OwnerUserId != user.Id)
                {
                    return null;
                }

                var payload = new
                {
                    jobId = job.Id,
                    epicId = job.EpicId,
                    status = job.Status,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    dequeueCount = job.DequeueCount,
                    draftVersion = job.DraftVersion
                };

                return new Snapshot(payload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(payload, Json));
            }
            case JobTypes.HeroVersion:
            {
                var job = await db.HeroVersionJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                var payload = new
                {
                    jobId = job.Id,
                    heroId = job.HeroId,
                    status = job.Status,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    dequeueCount = job.DequeueCount,
                    baseVersion = job.BaseVersion
                };

                return new Snapshot(payload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(payload, Json));
            }
            case JobTypes.RegionVersion:
            {
                var job = await db.RegionVersionJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                var payload = new
                {
                    jobId = job.Id,
                    regionId = job.RegionId,
                    status = job.Status,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    dequeueCount = job.DequeueCount,
                    baseVersion = job.BaseVersion
                };

                return new Snapshot(payload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(payload, Json));
            }
            case JobTypes.HeroPublish:
            {
                var job = await db.HeroPublishJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                if (!auth0.HasRole(user, UserRole.Admin) && job.OwnerUserId != user.Id)
                {
                    return null;
                }

                var payload = new
                {
                    jobId = job.Id,
                    heroId = job.HeroId,
                    status = job.Status,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    dequeueCount = job.DequeueCount
                };

                return new Snapshot(payload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(payload, Json));
            }
            case JobTypes.AnimalPublish:
            {
                var job = await db.AnimalPublishJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                if (!auth0.HasRole(user, UserRole.Admin) && job.OwnerUserId != user.Id)
                {
                    return null;
                }

                var payload = new
                {
                    jobId = job.Id,
                    animalId = job.AnimalId,
                    status = job.Status,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    dequeueCount = job.DequeueCount
                };

                return new Snapshot(payload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(payload, Json));
            }
            case JobTypes.HeroDefinitionVersion:
            {
                var job = await db.HeroDefinitionVersionJobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId, ct);
                if (job == null) return null;

                if (!auth0.HasRole(user, UserRole.Admin) && job.OwnerUserId != user.Id)
                {
                    return null;
                }

                var payload = new
                {
                    jobId = job.Id,
                    heroId = job.HeroId,
                    status = job.Status,
                    queuedAtUtc = job.QueuedAtUtc,
                    startedAtUtc = job.StartedAtUtc,
                    completedAtUtc = job.CompletedAtUtc,
                    errorMessage = job.ErrorMessage,
                    dequeueCount = job.DequeueCount,
                    baseVersion = job.BaseVersion
                };

                return new Snapshot(payload.status?.ToString() ?? string.Empty, JsonSerializer.Serialize(payload, Json));
            }
        }

        return null;
    }
}


