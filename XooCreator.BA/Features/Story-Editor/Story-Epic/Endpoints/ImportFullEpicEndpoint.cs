using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class ImportFullEpicEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IEpicImportQueueService _queueService;
    private readonly IBlobSasService _sas;
    private readonly ILogger<ImportFullEpicEndpoint> _logger;
    private const long MaxZipSizeBytes = 500 * 1024 * 1024; // 500MB

    public ImportFullEpicEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IEpicImportQueueService queueService,
        IBlobSasService sas,
        ILogger<ImportFullEpicEndpoint> logger)
    {
        _db = db;
        _auth0 = auth0;
        _queueService = queueService;
        _sas = sas;
        _logger = logger;
    }

    [Route("/api/story-editor/epics/import-full")]
    [Authorize]
    [RequestSizeLimit(MaxZipSizeBytes)]
    public static async Task<Results<Accepted<EpicImportResponse>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromServices] ImportFullEpicEndpoint ep,
        HttpRequest request,
        CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        var outcome = "Unknown";

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            outcome = "Unauthorized";
            return TypedResults.Unauthorized();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);

        if (!isAdmin && !isCreator)
        {
            outcome = "Forbidden";
            return TypedResults.Forbid();
        }

        if (!request.HasFormContentType)
        {
            return TypedResults.BadRequest("Request must be multipart/form-data");
        }

        var form = await request.ReadFormAsync(ct);
        var file = form.Files.GetFile("file");
        if (file == null || file.Length == 0)
        {
            return TypedResults.BadRequest("No file provided");
        }

        if (file.Length > MaxZipSizeBytes)
        {
            return TypedResults.BadRequest($"File size exceeds maximum allowed size of {MaxZipSizeBytes / (1024 * 1024)}MB");
        }

        if (!file.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("File must be a ZIP archive");
        }

        try
        {
            // Parse options from form
            var options = new EpicImportRequest();
            if (form.TryGetValue("conflictStrategy", out var conflictStrategy))
            {
                options = options with { ConflictStrategy = conflictStrategy.ToString() };
            }
            if (form.TryGetValue("includeStories", out var includeStories) && bool.TryParse(includeStories, out var storiesBool))
            {
                options = options with { IncludeStories = storiesBool };
            }
            if (form.TryGetValue("includeHeroes", out var includeHeroes) && bool.TryParse(includeHeroes, out var heroesBool))
            {
                options = options with { IncludeHeroes = heroesBool };
            }
            if (form.TryGetValue("includeRegions", out var includeRegions) && bool.TryParse(includeRegions, out var regionsBool))
            {
                options = options with { IncludeRegions = regionsBool };
            }
            if (form.TryGetValue("includeImages", out var includeImages) && bool.TryParse(includeImages, out var imagesBool))
            {
                options = options with { IncludeImages = imagesBool };
            }
            if (form.TryGetValue("includeAudio", out var includeAudio) && bool.TryParse(includeAudio, out var audioBool))
            {
                options = options with { IncludeAudio = audioBool };
            }
            if (form.TryGetValue("includeVideo", out var includeVideo) && bool.TryParse(includeVideo, out var videoBool))
            {
                options = options with { IncludeVideo = videoBool };
            }
            if (form.TryGetValue("generateNewIds", out var generateNewIds) && bool.TryParse(generateNewIds, out var newIdsBool))
            {
                options = options with { GenerateNewIds = newIdsBool };
            }
            if (form.TryGetValue("idPrefix", out var idPrefix))
            {
                options = options with { IdPrefix = idPrefix.ToString() };
            }

            await using var zipStream = file.OpenReadStream();

            // Upload ZIP to blob storage
            var jobId = Guid.NewGuid();
            var sanitizedFileName = file.FileName.Replace(" ", "_").Replace("..", "");
            var blobPath = $"epic-imports/{user.Id}/{jobId}/{sanitizedFileName}";

            var containerClient = ep._sas.GetContainerClient(ep._sas.DraftContainer);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var blobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, blobPath);
            await blobClient.UploadAsync(zipStream, overwrite: true, cancellationToken: ct);

            // Enqueue import job
            var epicJobId = await ep._queueService.EnqueueImportAsync(
                originalEpicId: "unknown", // Will be determined from manifest during processing
                zipBlobPath: blobPath,
                zipFileName: sanitizedFileName,
                zipSizeBytes: file.Length,
                options: options,
                ownerUserId: user.Id,
                requestedByEmail: user.Email ?? string.Empty,
                locale: "ro-ro", // TODO: Get from request context
                ct);

            outcome = "Queued";
            ep._logger.LogInformation("Epic import job queued: jobId={JobId} fileName={FileName}",
                epicJobId, sanitizedFileName);

            return TypedResults.Accepted($"/api/story-editor/epics/import-jobs/{epicJobId}", 
                new EpicImportResponse { JobId = epicJobId });
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "Failed to queue epic import: fileName={FileName}", file.FileName);
            outcome = "Error";
            return TypedResults.BadRequest($"Failed to queue import: {ex.Message}");
        }
        finally
        {
            stopwatch.Stop();
            ep._logger.LogInformation(
                "Import epic telemetry | fileName={FileName} outcome={Outcome} durationMs={DurationMs}",
                file.FileName, outcome, stopwatch.ElapsedMilliseconds);
        }
    }
}

