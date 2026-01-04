using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class GetEpicImportJobStatusEndpoint
{
    private readonly IEpicImportQueueService _queueService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetEpicImportJobStatusEndpoint> _logger;

    public GetEpicImportJobStatusEndpoint(
        IEpicImportQueueService queueService,
        IAuth0UserService auth0,
        ILogger<GetEpicImportJobStatusEndpoint> logger)
    {
        _queueService = queueService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/epics/import-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<EpicImportJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] Guid jobId,
        [FromServices] GetEpicImportJobStatusEndpoint ep,
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
                ep._logger.LogWarning("Import job access forbidden: userId={UserId} jobId={JobId} ownerId={OwnerId}",
                    user.Id, jobId, job.OwnerUserId);
                return TypedResults.Forbid();
            }
        }

        // Deserialize phases and ID mappings
        EpicImportPhasesDto? phases = null;
        if (!string.IsNullOrWhiteSpace(job.PhasesJson))
        {
            try
            {
                var phasesData = JsonSerializer.Deserialize<EpicImportPhases>(job.PhasesJson);
                if (phasesData != null)
                {
                    phases = new EpicImportPhasesDto
                    {
                        Validation = new PhaseStatusDto
                        {
                            Status = phasesData.Validation.Status,
                            Errors = phasesData.Validation.Errors
                        },
                        Regions = new ImportPhaseProgressDto
                        {
                            Status = phasesData.Regions.Status,
                            Imported = phasesData.Regions.Imported,
                            Total = phasesData.Regions.Total,
                            Errors = phasesData.Regions.Errors
                        },
                        Heroes = new ImportPhaseProgressDto
                        {
                            Status = phasesData.Heroes.Status,
                            Imported = phasesData.Heroes.Imported,
                            Total = phasesData.Heroes.Total,
                            Errors = phasesData.Heroes.Errors
                        },
                        Stories = new ImportPhaseProgressDto
                        {
                            Status = phasesData.Stories.Status,
                            Imported = phasesData.Stories.Imported,
                            Total = phasesData.Stories.Total,
                            Errors = phasesData.Stories.Errors
                        },
                        Assets = new ImportPhaseProgressDto
                        {
                            Status = phasesData.Assets.Status,
                            Imported = phasesData.Assets.Imported,
                            Total = phasesData.Assets.Total,
                            Errors = phasesData.Assets.Errors
                        },
                        Relationships = new ImportPhaseProgressDto
                        {
                            Status = phasesData.Relationships.Status,
                            Imported = phasesData.Relationships.Imported,
                            Total = phasesData.Relationships.Total,
                            Errors = phasesData.Relationships.Errors
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                ep._logger.LogWarning(ex, "Failed to deserialize phases JSON for job: jobId={JobId}", jobId);
            }
        }

        EpicIdMappingsDto? idMappings = null;
        if (!string.IsNullOrWhiteSpace(job.IdMappingsJson))
        {
            try
            {
                var mappingsData = JsonSerializer.Deserialize<EpicIdMappings>(job.IdMappingsJson);
                if (mappingsData != null)
                {
                    idMappings = new EpicIdMappingsDto
                    {
                        Regions = mappingsData.Regions,
                        Heroes = mappingsData.Heroes,
                        Stories = mappingsData.Stories
                    };
                }
            }
            catch (Exception ex)
            {
                ep._logger.LogWarning(ex, "Failed to deserialize ID mappings JSON for job: jobId={JobId}", jobId);
            }
        }

        var warnings = new List<string>();
        if (!string.IsNullOrWhiteSpace(job.WarningsJson))
        {
            try
            {
                warnings = JsonSerializer.Deserialize<List<string>>(job.WarningsJson) ?? new List<string>();
            }
            catch (Exception ex)
            {
                ep._logger.LogWarning(ex, "Failed to deserialize warnings JSON for job: jobId={JobId}", jobId);
            }
        }

        var errors = new List<string>();
        if (!string.IsNullOrWhiteSpace(job.ErrorMessage))
        {
            errors.Add(job.ErrorMessage);
        }

        var response = new EpicImportJobStatusResponse
        {
            JobId = job.Id,
            EpicId = job.EpicId,
            OriginalEpicId = job.OriginalEpicId,
            Status = job.Status,
            Phases = phases ?? new EpicImportPhasesDto(),
            IdMappings = idMappings,
            Warnings = warnings,
            Errors = errors,
            QueuedAtUtc = job.QueuedAtUtc,
            StartedAtUtc = job.StartedAtUtc,
            CompletedAtUtc = job.CompletedAtUtc
        };

        return TypedResults.Ok(response);
    }
}

