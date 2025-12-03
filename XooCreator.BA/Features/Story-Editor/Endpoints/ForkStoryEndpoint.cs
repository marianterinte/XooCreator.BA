using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class ForkStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly IStoryIdGenerator _storyIdGenerator;
    private readonly IStoryCopyService _storyCopyService;
    private readonly IStoryAssetCopyService _storyAssetCopyService;
    private readonly ILogger<ForkStoryEndpoint> _logger;
    private readonly TelemetryClient? _telemetryClient;

    public ForkStoryEndpoint(
        IStoryCraftsRepository crafts,
        IAuth0UserService auth0,
        XooDbContext db,
        IStoryIdGenerator storyIdGenerator,
        IStoryCopyService storyCopyService,
        IStoryAssetCopyService storyAssetCopyService,
        ILogger<ForkStoryEndpoint> logger,
        TelemetryClient? telemetryClient = null)
    {
        _crafts = crafts;
        _auth0 = auth0;
        _db = db;
        _storyIdGenerator = storyIdGenerator;
        _storyCopyService = storyCopyService;
        _storyAssetCopyService = storyAssetCopyService;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    public record ForkStoryRequest
    {
        public bool CopyAssets { get; init; } = true;
    }

    public record ForkStoryResponse
    {
        public required string StoryId { get; init; }
        public required string OriginalStoryId { get; init; }
    }

    [Route("/api/stories/{storyId}/fork")]
    [Authorize]
    public static async Task<
        Results<
            Ok<ForkStoryResponse>,
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
        var copyStats = ForkAssetCopyStats.CreateSkipped(sourceType);

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

            if (craft != null)
            {
                sourceType = "draft";
                sourceTranslations = craft.Translations.Count;
                sourceTiles = craft.Tiles.Count;
            }
            else if (definition != null)
            {
                sourceType = "published";
                sourceTranslations = definition.Translations.Count;
                sourceTiles = definition.Tiles.Count;
            }

            newStoryId = await ep._storyIdGenerator.GenerateNextAsync(currentUser.Id, currentUser.FirstName, currentUser.LastName, ct);
            StoryCraft newCraft;

            if (craft != null)
            {
                newCraft = await ep._storyCopyService.CreateCopyFromCraftAsync(craft, currentUser.Id, newStoryId, ct);
            }
            else
            {
                newCraft = await ep._storyCopyService.CreateCopyFromDefinitionAsync(definition!, currentUser.Id, newStoryId, ct);
            }

            ep._logger.LogInformation(
                "Fork story completed: userId={UserId} source={SourceStoryId} newStoryId={NewStoryId} copyAssets={CopyAssets}",
                currentUser.Id,
                storyId,
                newStoryId,
                request.CopyAssets);

            if (request.CopyAssets)
            {
                copyStats = await ep.TryCopyAssetsAsync(currentUser, craft, definition, newStoryId, sourceType, ct);
            }
            else
            {
                copyStats = ForkAssetCopyStats.CreateSkipped(sourceType);
            }

            outcome = !request.CopyAssets || copyStats.Success ? "Success" : "PartialSuccess";

            return TypedResults.Ok(new ForkStoryResponse
            {
                StoryId = newStoryId,
                OriginalStoryId = storyId
            });
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
                copyStats);
        }
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

    private async Task<ForkAssetCopyStats> TryCopyAssetsAsync(
        AlchimaliaUser currentUser,
        StoryCraft? craft,
        StoryDefinition? definition,
        string newStoryId,
        string sourceType,
        CancellationToken ct)
    {
        var attemptedAssets = 0;
        try
        {
            if (craft != null)
            {
                var sourceEmail = await ResolveOwnerEmailAsync(craft.OwnerUserId, currentUser, ct);
                if (sourceEmail == null)
                {
                    _logger.LogWarning("Skipping fork asset copy for storyId={StoryId}: cannot resolve source email", craft.StoryId);
                    return ForkAssetCopyStats.CreateFailure(sourceType, attemptedAssets, null, "SourceEmailMissing");
                }

                var assets = _storyAssetCopyService.CollectFromCraft(craft);
                attemptedAssets = assets.Count;
                if (attemptedAssets == 0)
                {
                    return ForkAssetCopyStats.CreateSkipped(sourceType);
                }

                var copyResult = await _storyAssetCopyService.CopyDraftToDraftAsync(
                    assets,
                    sourceEmail,
                    craft.StoryId,
                    currentUser.Email,
                    newStoryId,
                    ct);

                if (copyResult.HasError)
                {
                    _logger.LogWarning("Fork asset copy failed: storyId={StoryId} asset={Asset} reason={Reason}", newStoryId, copyResult.AssetFilename, copyResult.ErrorMessage);
                    return ForkAssetCopyStats.CreateFailure(sourceType, attemptedAssets, copyResult.AssetFilename, copyResult.ErrorMessage);
                }

                return ForkAssetCopyStats.CreateSuccess(sourceType, attemptedAssets, attemptedAssets);
            }

            if (definition != null)
            {
                if (!definition.CreatedBy.HasValue)
                {
                    _logger.LogWarning("Skipping fork asset copy for published storyId={StoryId}: CreatedBy missing", definition.StoryId);
                    return ForkAssetCopyStats.CreateFailure(sourceType, attemptedAssets, null, "CreatedByMissing");
                }

                var sourceEmail = await ResolveOwnerEmailAsync(definition.CreatedBy.Value, currentUser, ct);
                if (string.IsNullOrWhiteSpace(sourceEmail))
                {
                    _logger.LogWarning("Skipping fork asset copy for published storyId={StoryId}: cannot resolve owner email", definition.StoryId);
                    return ForkAssetCopyStats.CreateFailure(sourceType, attemptedAssets, null, "SourceEmailMissing");
                }

                var assets = _storyAssetCopyService.CollectFromDefinition(definition);
                attemptedAssets = assets.Count;
                if (attemptedAssets == 0)
                {
                    return ForkAssetCopyStats.CreateSkipped(sourceType);
                }

                var copyResult = await _storyAssetCopyService.CopyPublishedToDraftAsync(
                    assets,
                    sourceEmail,
                    definition.StoryId,
                    currentUser.Email,
                    newStoryId,
                    ct);

                if (copyResult.HasError)
                {
                    _logger.LogWarning("Fork asset copy failed (published): storyId={StoryId} asset={Asset} reason={Reason}", newStoryId, copyResult.AssetFilename, copyResult.ErrorMessage);
                    return ForkAssetCopyStats.CreateFailure(sourceType, attemptedAssets, copyResult.AssetFilename, copyResult.ErrorMessage);
                }

                return ForkAssetCopyStats.CreateSuccess(sourceType, attemptedAssets, attemptedAssets);
            }

            return ForkAssetCopyStats.CreateSkipped(sourceType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Asset copy failed during fork for storyId={StoryId}", newStoryId);
            return ForkAssetCopyStats.CreateFailure(sourceType, attemptedAssets, null, ex.Message);
        }
    }

    private async Task<string?> ResolveOwnerEmailAsync(Guid ownerId, AlchimaliaUser currentUser, CancellationToken ct)
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
        ForkAssetCopyStats copyStats)
    {
        _logger.LogInformation(
            "Fork story telemetry | originalStoryId={OriginalStoryId} newStoryId={NewStoryId} outcome={Outcome} durationMs={DurationMs} copyRequested={CopyRequested} copySuccess={CopySuccess} attemptedAssets={Attempted} copiedAssets={Copied} sourceType={SourceType} failureReason={FailureReason}",
            originalStoryId,
            newStoryId ?? "(not generated)",
            outcome,
            durationMs,
            copyRequested,
            copyStats.Success,
            copyStats.AttemptedAssets,
            copyStats.CopiedAssets,
            copyStats.SourceType,
            copyStats.FailureReason ?? "(none)");

        var properties = new Dictionary<string, string?>
        {
            ["Outcome"] = outcome,
            ["OriginalStoryId"] = originalStoryId,
            ["NewStoryId"] = newStoryId,
            ["UserId"] = userId,
            ["UserEmail"] = userEmail,
            ["CopyRequested"] = copyRequested.ToString(CultureInfo.InvariantCulture),
            ["CopySourceType"] = sourceType,
            ["CopySuccess"] = copyStats.Success.ToString(CultureInfo.InvariantCulture),
            ["CopyAttemptedAssets"] = copyStats.AttemptedAssets.ToString(CultureInfo.InvariantCulture),
            ["CopyCopiedAssets"] = copyStats.CopiedAssets.ToString(CultureInfo.InvariantCulture),
            ["CopyFailureAsset"] = copyStats.FailureAsset,
            ["CopyFailureReason"] = copyStats.FailureReason,
            ["SourceTranslations"] = sourceTranslations.ToString(CultureInfo.InvariantCulture),
            ["SourceTiles"] = sourceTiles.ToString(CultureInfo.InvariantCulture)
        };

        TrackMetric("ForkStory_Duration", durationMs, properties);
        TrackMetric("ForkStory_AssetsRequested", copyStats.AttemptedAssets, properties);
        TrackMetric("ForkStory_AssetsCopied", copyStats.CopiedAssets, properties);
        TrackMetric("ForkStory_CopySuccess", copyStats.Success ? 1 : 0, properties);
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

    private readonly record struct ForkAssetCopyStats(
        int AttemptedAssets,
        int CopiedAssets,
        bool Success,
        string SourceType,
        string? FailureAsset,
        string? FailureReason)
    {
        public static ForkAssetCopyStats CreateSkipped(string sourceType) => new(0, 0, true, sourceType, null, null);
        public static ForkAssetCopyStats CreateSuccess(string sourceType, int attempted, int copied) => new(attempted, copied, true, sourceType, null, null);
        public static ForkAssetCopyStats CreateFailure(string sourceType, int attempted, string? failureAsset, string? failureReason) => new(attempted, 0, false, sourceType, failureAsset, failureReason);
    }
}

