using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class GetEpicExportJobStatusEndpoint
{
    private readonly IEpicExportQueueService _queueService;
    private readonly IAuth0UserService _auth0;
    private readonly IBlobSasService _sas;
    private readonly ILogger<GetEpicExportJobStatusEndpoint> _logger;

    public GetEpicExportJobStatusEndpoint(
        IEpicExportQueueService queueService,
        IAuth0UserService auth0,
        IBlobSasService sas,
        ILogger<GetEpicExportJobStatusEndpoint> logger)
    {
        _queueService = queueService;
        _auth0 = auth0;
        _sas = sas;
        _logger = logger;
    }

    [Route("/api/story-editor/epics/export-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<EpicExportJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] Guid jobId,
        [FromServices] GetEpicExportJobStatusEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);

        if (!isAdmin && !isCreator)
        {
            return TypedResults.Forbid();
        }

        var job = await ep._queueService.GetJobStatusAsync(jobId, ct);
        if (job == null)
        {
            return TypedResults.NotFound();
        }

        // Verify ownership (unless admin)
        if (!isAdmin)
        {
            if (job.OwnerUserId != user.Id)
            {
                ep._logger.LogWarning("Export job access forbidden: userId={UserId} jobId={JobId} ownerId={OwnerId}",
                    user.Id, jobId, job.OwnerUserId);
                return TypedResults.Forbid();
            }
        }

        // Generate download URL if job is completed
        string? zipDownloadUrl = null;
        if (job.Status == EpicExportJobStatus.Completed && !string.IsNullOrWhiteSpace(job.ZipBlobPath))
        {
            try
            {
                zipDownloadUrl = await ep._queueService.GetDownloadUrlAsync(job, ct);
            }
            catch (Exception ex)
            {
                ep._logger.LogWarning(ex, "Failed to generate download URL for export job: jobId={JobId} blobPath={BlobPath}",
                    jobId, job.ZipBlobPath);
            }
        }

        var response = new EpicExportJobStatusResponse
        {
            JobId = job.Id,
            EpicId = job.EpicId,
            Status = job.Status,
            Progress = new EpicExportProgressDto
            {
                CurrentPhase = job.CurrentPhase,
                StoriesExported = job.StoriesExported,
                TotalStories = job.TotalStories,
                HeroesExported = job.HeroesExported,
                TotalHeroes = job.TotalHeroes,
                RegionsExported = job.RegionsExported,
                TotalRegions = job.TotalRegions,
                AssetsExported = job.AssetsExported,
                TotalAssets = job.TotalAssets
            },
            QueuedAtUtc = job.QueuedAtUtc,
            StartedAtUtc = job.StartedAtUtc,
            CompletedAtUtc = job.CompletedAtUtc,
            ErrorMessage = job.ErrorMessage,
            ZipDownloadUrl = zipDownloadUrl,
            ZipFileName = job.ZipFileName,
            ZipSizeBytes = job.ZipSizeBytes
        };

        return TypedResults.Ok(response);
    }
}

