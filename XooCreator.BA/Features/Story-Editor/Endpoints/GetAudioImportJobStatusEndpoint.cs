using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GetAudioImportJobStatusEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetAudioImportJobStatusEndpoint> _logger;

    public GetAudioImportJobStatusEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        ILogger<GetAudioImportJobStatusEndpoint> logger)
    {
        _db = db;
        _auth0 = auth0;
        _logger = logger;
    }

    public record AudioImportJobStatusResponse
    {
        public Guid JobId { get; init; }
        public string StoryId { get; init; } = string.Empty;
        public string Status { get; init; } = StoryAudioImportJobStatus.Queued;
        public DateTime QueuedAtUtc { get; init; }
        public DateTime? StartedAtUtc { get; init; }
        public DateTime? CompletedAtUtc { get; init; }
        public string? ErrorMessage { get; init; }
        public bool Success { get; init; }
        public int ImportedCount { get; init; }
        public int TotalPages { get; init; }
        public List<string> Errors { get; init; } = new();
        public List<string> Warnings { get; init; } = new();
    }

    [Route("/api/stories/{storyId}/audio-import-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<AudioImportJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string storyId,
        [FromRoute] Guid jobId,
        [FromServices] GetAudioImportJobStatusEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        var job = await ep._db.StoryAudioImportJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId && j.StoryId == storyId, ct);

        if (job == null)
            return TypedResults.NotFound();

        if (!ep._auth0.HasRole(user, UserRole.Admin) && job.OwnerUserId != user.Id)
        {
            ep._logger.LogWarning("Audio import job access forbidden: userId={UserId} jobId={JobId}", user.Id, jobId);
            return TypedResults.Forbid();
        }

        var errors = ParseJsonArray(job.ErrorsJson);
        var warnings = ParseJsonArray(job.WarningsJson);

        var response = new AudioImportJobStatusResponse
        {
            JobId = job.Id,
            StoryId = job.StoryId,
            Status = job.Status,
            QueuedAtUtc = job.QueuedAtUtc,
            StartedAtUtc = job.StartedAtUtc,
            CompletedAtUtc = job.CompletedAtUtc,
            ErrorMessage = job.ErrorMessage,
            Success = job.Success,
            ImportedCount = job.ImportedCount,
            TotalPages = job.TotalPages,
            Errors = errors,
            Warnings = warnings
        };

        return TypedResults.Ok(response);
    }

    private static List<string> ParseJsonArray(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new List<string>();
        try
        {
            var list = JsonSerializer.Deserialize<List<string>>(json);
            return list ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}
