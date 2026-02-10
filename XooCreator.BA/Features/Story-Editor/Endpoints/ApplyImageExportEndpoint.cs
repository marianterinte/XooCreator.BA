using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class ApplyImageExportEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IStoryCraftsRepository _crafts;
    private readonly IBlobSasService _sas;
    private readonly IStoryImageImportProcessor _processor;
    private readonly ILogger<ApplyImageExportEndpoint> _logger;

    public ApplyImageExportEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IStoryCraftsRepository crafts,
        IBlobSasService sas,
        IStoryImageImportProcessor processor,
        ILogger<ApplyImageExportEndpoint> logger)
    {
        _db = db;
        _auth0 = auth0;
        _crafts = crafts;
        _sas = sas;
        _processor = processor;
        _logger = logger;
    }

    public record ApplyImageExportResponse
    {
        public bool Success { get; init; }
        public List<string> Errors { get; init; } = new();
        public List<string> Warnings { get; init; } = new();
        public int ImportedCount { get; init; }
        public int TotalPages { get; init; }
    }

    [Route("/api/stories/{storyId}/image-export-jobs/{jobId}/apply")]
    [Authorize]
    public static async Task<Results<Ok<ApplyImageExportResponse>, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string storyId,
        [FromRoute] Guid jobId,
        [FromServices] ApplyImageExportEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        if (!isAdmin)
        {
            ep._logger.LogWarning("Apply image export forbidden: userId={UserId} jobId={JobId} not admin", user.Id, jobId);
            return TypedResults.Forbid();
        }

        var job = await ep._db.StoryImageExportJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId && j.StoryId == storyId, ct);

        if (job == null)
        {
            return TypedResults.NotFound();
        }

        if (job.Status != StoryImageExportJobStatus.Completed)
        {
            return TypedResults.BadRequest("Image export job is not completed. Only completed jobs can be applied.");
        }

        if (string.IsNullOrWhiteSpace(job.ZipBlobPath))
        {
            return TypedResults.BadRequest("Image export job has no ZIP file.");
        }

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null)
        {
            return TypedResults.NotFound();
        }

        string ownerEmail = job.RequestedByEmail ?? string.Empty;
        if (craft.OwnerUserId != Guid.Empty)
        {
            var ownerEmailFromDb = await ep._db.AlchimaliaUsers
                .AsNoTracking()
                .Where(u => u.Id == craft.OwnerUserId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync(ct);
            if (!string.IsNullOrWhiteSpace(ownerEmailFromDb))
            {
                ownerEmail = ownerEmailFromDb;
            }
        }

        Stream? zipStream = null;
        try
        {
            var blobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, job.ZipBlobPath);
            if (!await blobClient.ExistsAsync(ct))
            {
                return TypedResults.BadRequest("Export ZIP file no longer exists in storage.");
            }

            zipStream = await blobClient.OpenReadAsync(new Azure.Storage.Blobs.Models.BlobOpenReadOptions(false), ct);
            var result = await ep._processor.ProcessAsync(zipStream, storyId, job.Locale, ownerEmail, ct);

            return TypedResults.Ok(new ApplyImageExportResponse
            {
                Success = result.Success,
                Errors = result.Errors,
                Warnings = result.Warnings,
                ImportedCount = result.ImportedCount,
                TotalPages = result.TotalPages
            });
        }
        finally
        {
            await (zipStream?.DisposeAsync() ?? ValueTask.CompletedTask);
        }
    }
}
