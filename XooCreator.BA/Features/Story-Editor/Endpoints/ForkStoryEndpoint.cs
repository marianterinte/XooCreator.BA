using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public partial class ForkStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly IStoryIdGenerator _storyIdGenerator;
    private readonly IStoryCopyService _storyCopyService;
    private readonly IStoryAssetCopyService _storyAssetCopyService;
    private readonly ILogger<ForkStoryEndpoint> _logger;
    private readonly TelemetryClient? _telemetryClient;
    private readonly IStoryForkQueue _forkQueue;
    private readonly string _forkQueueName;
    private readonly IStoryForkAssetsQueue _forkAssetsQueue;

    public ForkStoryEndpoint(
        IStoryCraftsRepository crafts,
        IAuth0UserService auth0,
        XooDbContext db,
        IStoryIdGenerator storyIdGenerator,
        IStoryCopyService storyCopyService,
        IStoryAssetCopyService storyAssetCopyService,
        ILogger<ForkStoryEndpoint> logger,
        IConfiguration configuration,
        IStoryForkQueue forkQueue,
        IStoryForkAssetsQueue forkAssetsQueue,
        TelemetryClient? telemetryClient = null)
    {
        _crafts = crafts;
        _auth0 = auth0;
        _db = db;
        _storyIdGenerator = storyIdGenerator;
        _storyCopyService = storyCopyService;
        _storyAssetCopyService = storyAssetCopyService;
        _logger = logger;
        _forkQueue = forkQueue;
        _forkQueueName = configuration.GetSection("AzureStorage:Queues")?["Fork"] ?? "story-fork-queue";
        _forkAssetsQueue = forkAssetsQueue;
        _telemetryClient = telemetryClient;
    }

    public record ForkStoryRequest
    {
        public bool CopyAssets { get; init; } = true;
    }

    public record ForkStoryResponse
    {
        public Guid JobId { get; init; }
        public required string StoryId { get; init; }
        public required string OriginalStoryId { get; init; }
        public bool CopyAssets { get; init; }
        public string Status { get; init; } = StoryForkJobStatus.Queued;
        public string QueueName { get; init; } = string.Empty;
        public Guid? AssetJobId { get; init; }
        public string? AssetJobStatus { get; init; }
    }

    public record ForkStoryJobStatusResponse
    {
        public Guid JobId { get; init; }
        public string StoryId { get; init; } = string.Empty;
        public string SourceStoryId { get; init; } = string.Empty;
        public string Status { get; init; } = StoryForkJobStatus.Queued;
        public bool CopyAssets { get; init; }
        public string QueueName { get; init; } = string.Empty;
        public DateTime QueuedAtUtc { get; init; }
        public DateTime? StartedAtUtc { get; init; }
        public DateTime? CompletedAtUtc { get; init; }
        public string? ErrorMessage { get; init; }
        public string? WarningSummary { get; init; }
        public int SourceTranslations { get; init; }
        public int SourceTiles { get; init; }
        public ForkStoryAssetJobStatus? AssetJob { get; init; }
    }

    public record ForkStoryAssetJobStatus
    {
        public Guid JobId { get; init; }
        public string Status { get; init; } = StoryForkAssetJobStatus.Queued;
        public int AttemptedAssets { get; init; }
        public int CopiedAssets { get; init; }
        public int DequeueCount { get; init; }
        public DateTime QueuedAtUtc { get; init; }
        public DateTime? StartedAtUtc { get; init; }
        public DateTime? CompletedAtUtc { get; init; }
        public string? ErrorMessage { get; init; }
        public string? WarningSummary { get; init; }
    }

    [Route("/api/stories/{storyId}/fork")]
    [Authorize]
    public static async Task<
        Results<
            Accepted<ForkStoryResponse>,
            BadRequest<string>,
            NotFound,
            UnauthorizedHttpResult,
            ForbidHttpResult,
            Conflict<string>>> HandlePost(
        [FromRoute] string storyId,
        [FromBody] ForkStoryRequest request,
        [FromServices] ForkStoryEndpoint ep,
        CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        var outcome = "Unknown";
        string? userId = null;
        string? userEmail = null;
        string sourceType = "unknown";
        int sourceTranslations = 0;
        int sourceTiles = 0;
        string? newStoryId = null;
        Guid? jobId = null;
        string? jobStatus = null;
        var jobQueued = false;

        try
        {
            var (user, authOutcome) = await ep.AuthorizeCreatorAsync(ct);
            if (authOutcome == AuthorizationOutcome.Unauthorized)
            {
                outcome = "Unauthorized";
                return TypedResults.Unauthorized();
            }

            if (authOutcome == AuthorizationOutcome.Forbidden)
            {
                outcome = "Forbidden";
                return TypedResults.Forbid();
            }

            var currentUser = user!;
            userId = currentUser.Id.ToString();
            userEmail = currentUser.Email;

            if (!ep.IsValidStoryId(storyId))
            {
                outcome = "BadRequest";
                return TypedResults.BadRequest("storyId is required and cannot be 'new'");
            }

            var (craft, definition) = await ep.LoadSourceStoryAsync(storyId, ct);
            if (craft == null && definition == null)
            {
                outcome = "NotFound";
                return TypedResults.NotFound();
            }

            // Fork is only allowed for published stories (definitions), not drafts (crafts)
            // For drafts, users should use Copy instead
            if (craft != null)
            {
                outcome = "BadRequest";
                return TypedResults.BadRequest("Fork is only available for published stories. Use Copy for draft stories.");
            }

            if (definition != null)
            {
                sourceType = StoryForkAssetJobSourceTypes.Published;
                sourceTranslations = definition.Translations.Count;
                sourceTiles = definition.Tiles.Count;
            }
            else
            {
                outcome = "BadRequest";
                return TypedResults.BadRequest("Fork is only available for published stories.");
            }

            newStoryId = await ep._storyIdGenerator.GenerateNextAsync(
                currentUser.Id,
                currentUser.FirstName,
                currentUser.LastName,
                ct);

            var requesterEmail = string.IsNullOrWhiteSpace(currentUser.Email)
                ? $"{currentUser.Id}@unknown"
                : currentUser.Email;

            var job = new StoryForkJob
            {
                Id = Guid.NewGuid(),
                SourceStoryId = storyId,
                SourceType = sourceType,
                CopyAssets = request.CopyAssets,
                RequestedByUserId = currentUser.Id,
                RequestedByEmail = requesterEmail,
                TargetOwnerUserId = currentUser.Id,
                TargetOwnerEmail = requesterEmail,
                TargetStoryId = newStoryId,
                Status = StoryForkJobStatus.Queued,
                QueuedAtUtc = DateTime.UtcNow,
                SourceTranslations = sourceTranslations,
                SourceTiles = sourceTiles
            };

            ep._db.StoryForkJobs.Add(job);
            await ep._db.SaveChangesAsync(ct);

            await ep._forkQueue.EnqueueAsync(job, ct);

            outcome = "Queued";
            jobQueued = true;
            jobId = job.Id;
            jobStatus = job.Status;

            ep._logger.LogInformation(
                "Fork story job queued: userId={UserId} sourceStoryId={SourceStoryId} targetStoryId={TargetStoryId} copyAssets={CopyAssets} jobId={JobId}",
                currentUser.Id,
                storyId,
                newStoryId,
                request.CopyAssets,
                job.Id);

            var response = new ForkStoryResponse
            {
                JobId = job.Id,
                StoryId = newStoryId,
                OriginalStoryId = storyId,
                CopyAssets = request.CopyAssets,
                Status = job.Status,
                QueueName = ep._forkQueueName,
                AssetJobId = job.AssetJobId,
                AssetJobStatus = job.AssetJobStatus
            };

            return TypedResults.Accepted(
                $"/api/stories/{storyId}/fork/jobs/{job.Id}",
                response);
        }
        finally
        {
            stopwatch.Stop();
            ep.TrackForkTelemetry(
                stopwatch.ElapsedMilliseconds,
                outcome,
                storyId,
                newStoryId,
                userId,
                userEmail,
                request.CopyAssets,
                sourceType,
                sourceTranslations,
                sourceTiles,
                jobQueued,
                jobStatus);
        }
    }

    [HttpGet]
    [Route("/api/stories/fork/jobs/{jobId:guid}")]
    [Authorize]
    public static async Task<
        Results<
            Ok<ForkStoryJobStatusResponse>,
            NotFound,
            UnauthorizedHttpResult,
            ForbidHttpResult>> HandleGet(
        [FromRoute] Guid jobId,
        [FromServices] ForkStoryEndpoint ep,
        CancellationToken ct)
    {
        var (user, authOutcome) = await ep.AuthorizeCreatorAsync(ct);
        if (authOutcome == AuthorizationOutcome.Unauthorized)
        {
            return TypedResults.Unauthorized();
        }

        if (authOutcome == AuthorizationOutcome.Forbidden)
        {
            return TypedResults.Forbid();
        }

        var currentUser = user!;

        var job = await ep._db.StoryForkJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId, ct);

        if (job == null)
        {
            return TypedResults.NotFound();
        }

        if (!ep.CanAccessForkJob(currentUser, job))
        {
            return TypedResults.Forbid();
        }

        StoryForkAssetJob? assetJob = null;
        if (job.AssetJobId.HasValue)
        {
            assetJob = await ep._db.StoryForkAssetJobs
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == job.AssetJobId.Value, ct);
        }

        ForkStoryAssetJobStatus? assetJobStatus = null;
        if (assetJob != null)
        {
            assetJobStatus = new ForkStoryAssetJobStatus
            {
                JobId = assetJob.Id,
                Status = assetJob.Status,
                AttemptedAssets = assetJob.AttemptedAssets,
                CopiedAssets = assetJob.CopiedAssets,
                DequeueCount = assetJob.DequeueCount,
                QueuedAtUtc = assetJob.QueuedAtUtc,
                StartedAtUtc = assetJob.StartedAtUtc,
                CompletedAtUtc = assetJob.CompletedAtUtc,
                ErrorMessage = assetJob.ErrorMessage,
                WarningSummary = assetJob.WarningSummary
            };
        }

        var response = new ForkStoryJobStatusResponse
        {
            JobId = job.Id,
            StoryId = job.TargetStoryId,
            SourceStoryId = job.SourceStoryId,
            Status = job.Status,
            CopyAssets = job.CopyAssets,
            QueueName = ep._forkQueueName,
            QueuedAtUtc = job.QueuedAtUtc,
            StartedAtUtc = job.StartedAtUtc,
            CompletedAtUtc = job.CompletedAtUtc,
            ErrorMessage = job.ErrorMessage,
            WarningSummary = job.WarningSummary,
            SourceTranslations = job.SourceTranslations,
            SourceTiles = job.SourceTiles,
            AssetJob = assetJobStatus
        };

        return TypedResults.Ok(response);
    }

    private enum AuthorizationOutcome
    {
        Ok,
        Unauthorized,
        Forbidden
    }

    private async Task<(AlchimaliaUser? User, AuthorizationOutcome Outcome)> AuthorizeCreatorAsync(CancellationToken ct)
    {
        var user = await _auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return (null, AuthorizationOutcome.Unauthorized);
        }

        if (!_auth0.HasRole(user, UserRole.Creator) && !_auth0.HasRole(user, UserRole.Admin))
        {
            _logger.LogWarning("Fork story forbidden: userId={UserId}", user.Id);
            return (user, AuthorizationOutcome.Forbidden);
        }

        return (user, AuthorizationOutcome.Ok);
    }

    private bool IsValidStoryId(string storyId)
    {
        return !(string.IsNullOrWhiteSpace(storyId) || storyId.Equals("new", StringComparison.OrdinalIgnoreCase));
    }

    private bool CanAccessForkJob(AlchimaliaUser user, StoryForkJob job)
    {
        if (_auth0.HasRole(user, UserRole.Admin))
        {
            return true;
        }

        return job.TargetOwnerUserId == user.Id || job.RequestedByUserId == user.Id;
    }

    private async Task<(StoryCraft? Craft, StoryDefinition? Definition)> LoadSourceStoryAsync(string storyId, CancellationToken ct)
    {
        var craft = await _crafts.GetAsync(storyId, ct);
        if (craft != null)
        {
            return (craft, null);
        }

        var definition = await _db.StoryDefinitions
            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(d => d.Tiles).ThenInclude(t => t.Translations)
            .Include(d => d.Translations)
            .Include(d => d.Topics).ThenInclude(t => t.StoryTopic)
            .Include(d => d.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .FirstOrDefaultAsync(d => d.StoryId == storyId, ct);

        return (null, definition);
    }


    internal async Task<string?> ResolveOwnerEmailAsync(Guid ownerId, AlchimaliaUser currentUser, CancellationToken ct)
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

    private void TrackForkTelemetry(
        long durationMs,
        string outcome,
        string originalStoryId,
        string? newStoryId,
        string? userId,
        string? userEmail,
        bool copyRequested,
        string sourceType,
        int sourceTranslations,
        int sourceTiles,
        bool jobQueued,
        string? jobStatus)
    {
        _logger.LogInformation(
            "Fork story telemetry | originalStoryId={OriginalStoryId} newStoryId={NewStoryId} outcome={Outcome} durationMs={DurationMs} copyRequested={CopyRequested} jobQueued={JobQueued} jobStatus={JobStatus} sourceType={SourceType}",
            originalStoryId,
            newStoryId ?? "(not generated)",
            outcome,
            durationMs,
            copyRequested,
            jobQueued,
            jobStatus ?? "(none)",
            sourceType);

        var properties = new Dictionary<string, string?>
        {
            ["Outcome"] = outcome,
            ["OriginalStoryId"] = originalStoryId,
            ["NewStoryId"] = newStoryId,
            ["UserId"] = userId,
            ["UserEmail"] = userEmail,
            ["CopyRequested"] = copyRequested.ToString(CultureInfo.InvariantCulture),
            ["CopySourceType"] = sourceType,
            ["JobQueued"] = jobQueued.ToString(CultureInfo.InvariantCulture),
            ["JobStatus"] = jobStatus,
            ["SourceTranslations"] = sourceTranslations.ToString(CultureInfo.InvariantCulture),
            ["SourceTiles"] = sourceTiles.ToString(CultureInfo.InvariantCulture)
        };

        TrackMetric("ForkStory_Duration", durationMs, properties);
        TrackMetric("ForkStory_CopyJobQueued", jobQueued ? 1 : 0, properties);
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

}

