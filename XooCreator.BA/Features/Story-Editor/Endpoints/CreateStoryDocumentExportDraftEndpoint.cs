using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class CreateStoryDocumentExportDraftEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IStoryCraftsRepository _crafts;
    private readonly IStoryDocumentExportQueue _queue;
    private readonly IJobEventsHub _jobEvents;
    private readonly IConfiguration _config;

    public CreateStoryDocumentExportDraftEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IStoryCraftsRepository crafts,
        IStoryDocumentExportQueue queue,
        IJobEventsHub jobEvents,
        IConfiguration config)
    {
        _db = db;
        _auth0 = auth0;
        _crafts = crafts;
        _queue = queue;
        _jobEvents = jobEvents;
        _config = config;
    }

    public record CreateStoryDocumentExportRequest(
        string Format,
        string? LanguageCode,
        string? PaperSize,
        bool IncludeCover,
        bool IncludeQuizAnswers,
        bool? UseMobileImageLayout);

    public record CreateStoryDocumentExportResponse(Guid JobId);

    [Route("/api/{locale}/stories/{storyId}/pdf-draft")]
    [Authorize]
    public static async Task<Results<Accepted<CreateStoryDocumentExportResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, BadRequest<string>, ProblemHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromBody] CreateStoryDocumentExportRequest? body,
        [FromServices] CreateStoryDocumentExportDraftEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);
        if (!isAdmin && !isCreator) return TypedResults.Forbid();

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null) return TypedResults.NotFound();

        if (!isAdmin && craft.OwnerUserId != user.Id) return TypedResults.Forbid();

        var request = body ?? new CreateStoryDocumentExportRequest(
            Format: StoryDocumentExportFormat.Pdf,
            LanguageCode: locale,
            PaperSize: "A4",
            IncludeCover: true,
            IncludeQuizAnswers: false,
            UseMobileImageLayout: true);

        var format = (request.Format ?? StoryDocumentExportFormat.Pdf).Trim().ToLowerInvariant();
        if (format is not (StoryDocumentExportFormat.Pdf or StoryDocumentExportFormat.Docx))
            return TypedResults.BadRequest($"Unsupported format: {format}");

        var paperSize = string.IsNullOrWhiteSpace(request.PaperSize) ? "A4" : request.PaperSize.Trim();
        if (!paperSize.Equals("A4", StringComparison.OrdinalIgnoreCase) && !paperSize.Equals("Letter", StringComparison.OrdinalIgnoreCase))
            return TypedResults.BadRequest($"Unsupported paperSize: {paperSize}");

        // Draft: IncludeQuizAnswers is owner/admin only; for draft owner is the requesting creator (or admin).
        if (request.IncludeQuizAnswers && !isAdmin && craft.OwnerUserId != user.Id)
            return TypedResults.Forbid();

        // Print quota (same as published) – supporters (UserSubscriptions) get unlimited
        var freePrintLimit = ep._config.GetValue("Subscription:FreePrintLimit", 1);
        if (freePrintLimit >= 0 && !isAdmin)
        {
            var nowUtc = DateTime.UtcNow;
            var isSupporter = await ep._db.UserSubscriptions
                .AsNoTracking()
                .AnyAsync(u => u.UserId == user.Id && (u.ExpiresAtUtc == null || u.ExpiresAtUtc > nowUtc), ct);
            if (!isSupporter)
            {
                var printedIds = await ep._db.StoryPrintRecords.AsNoTracking().Where(r => r.UserId == user.Id).Select(r => r.StoryId).Distinct().ToListAsync(ct);
                var exportedIds = await ep._db.StoryDocumentExportJobs.AsNoTracking().Where(j => j.RequestedByUserId == user.Id && j.Status == StoryDocumentExportJobStatus.Completed).Select(j => j.StoryId).Distinct().ToListAsync(ct);
                var usedCount = printedIds.Union(exportedIds).Count();
                var alreadyPrinted = printedIds.Contains(storyId) || exportedIds.Contains(storyId);
                if (usedCount >= freePrintLimit && !alreadyPrinted)
                    return TypedResults.Problem(detail: "print_quota_api_limit_reached", statusCode: 402);
            }
        }

        var job = new StoryDocumentExportJob
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            StoryOwnerUserId = craft.OwnerUserId,
            RequestedByUserId = user.Id,
            RequestedByEmail = user.Email ?? string.Empty,
            Locale = (request.LanguageCode ?? locale).Trim().ToLowerInvariant(),
            IsDraft = true,
            Format = format,
            PaperSize = paperSize,
            IncludeCover = request.IncludeCover,
            IncludeQuizAnswers = request.IncludeQuizAnswers,
            UseMobileImageLayout = request.UseMobileImageLayout ?? true,
            Status = StoryDocumentExportJobStatus.Queued,
            QueuedAtUtc = DateTime.UtcNow
        };

        ep._db.StoryDocumentExportJobs.Add(job);
        await ep._db.SaveChangesAsync(ct);

        ep._jobEvents.Publish(JobTypes.StoryDocumentExport, job.Id, new
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
            downloadUrl = (string?)null,
            outputFileName = job.OutputFileName,
            outputSizeBytes = job.OutputSizeBytes
        });

        await ep._queue.EnqueueAsync(job, ct);

        var location = $"/api/stories/{Uri.EscapeDataString(storyId)}/pdf-jobs/{job.Id}";
        return TypedResults.Accepted(location, new CreateStoryDocumentExportResponse(job.Id));
    }
}


