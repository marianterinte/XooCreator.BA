using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GetTranslationJobStatusEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetTranslationJobStatusEndpoint> _logger;

    public GetTranslationJobStatusEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        ILogger<GetTranslationJobStatusEndpoint> logger)
    {
        _db = db;
        _auth0 = auth0;
        _logger = logger;
    }

    public record TranslationJobStatusResponse
    {
        public Guid JobId { get; init; }
        public string StoryId { get; init; } = string.Empty;
        public string Status { get; init; } = StoryTranslationJobStatus.Queued;
        public DateTime QueuedAtUtc { get; init; }
        public DateTime? StartedAtUtc { get; init; }
        public DateTime? CompletedAtUtc { get; init; }
        public string? ErrorMessage { get; init; }
        public int? FieldsTranslated { get; init; }
        public List<string>? UpdatedLanguages { get; init; }
    }

    [Route("/api/stories/{storyId}/translation-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<TranslationJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string storyId,
        [FromRoute] Guid jobId,
        [FromServices] GetTranslationJobStatusEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);
        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        if (!isCreator && !isAdmin)
        {
            ep._logger.LogWarning("Translation job access forbidden: userId={UserId} jobId={JobId}", user.Id, jobId);
            return TypedResults.Forbid();
        }

        var job = await ep._db.StoryTranslationJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId && j.StoryId == storyId, ct);

        if (job == null)
            return TypedResults.NotFound();

        if (job.OwnerUserId != user.Id && !isAdmin)
        {
            ep._logger.LogWarning("Translation job access forbidden: userId={UserId} jobId={JobId} not owner", user.Id, jobId);
            return TypedResults.Forbid();
        }

        List<string>? updatedLanguages = null;
        if (!string.IsNullOrWhiteSpace(job.UpdatedLanguagesJson))
        {
            try
            {
                updatedLanguages = JsonSerializer.Deserialize<List<string>>(job.UpdatedLanguagesJson);
            }
            catch
            {
                // ignore
            }
        }

        var response = new TranslationJobStatusResponse
        {
            JobId = job.Id,
            StoryId = job.StoryId,
            Status = job.Status,
            QueuedAtUtc = job.QueuedAtUtc,
            StartedAtUtc = job.StartedAtUtc,
            CompletedAtUtc = job.CompletedAtUtc,
            ErrorMessage = job.ErrorMessage,
            FieldsTranslated = job.FieldsTranslated,
            UpdatedLanguages = updatedLanguages
        };

        return TypedResults.Ok(response);
    }
}
