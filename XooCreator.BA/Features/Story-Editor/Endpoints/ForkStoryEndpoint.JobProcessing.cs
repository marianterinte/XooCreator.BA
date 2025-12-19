using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

public partial class ForkStoryEndpoint
{
    private async Task<StoryForkAssetJob?> CreateForkAssetJobAsync(
        AlchimaliaUser currentUser,
        StoryCraft newCraft,
        StoryCraft? sourceCraft,
        StoryDefinition? sourceDefinition,
        string targetStoryId,
        string sourceType,
        CancellationToken ct)
    {
        var hasActiveJob = await _db.StoryForkAssetJobs
            .AnyAsync(j => j.TargetStoryId == targetStoryId &&
                           (j.Status == StoryForkAssetJobStatus.Queued || j.Status == StoryForkAssetJobStatus.Running), ct);

        if (hasActiveJob)
        {
            _logger.LogWarning("Fork asset job already queued for target storyId={StoryId}", targetStoryId);
            return null;
        }

        string sourceStoryId;
        Guid? sourceOwnerId = null;
        string? sourceOwnerEmail = null;

        if (sourceType == StoryForkAssetJobSourceTypes.Draft && sourceCraft != null)
        {
            sourceStoryId = sourceCraft.StoryId;
            sourceOwnerId = sourceCraft.OwnerUserId;
            sourceOwnerEmail = await ResolveOwnerEmailAsync(sourceCraft.OwnerUserId, currentUser, ct);
        }
        else if (sourceType == StoryForkAssetJobSourceTypes.Published && sourceDefinition != null)
        {
            sourceStoryId = sourceDefinition.StoryId;
            if (sourceDefinition.CreatedBy.HasValue)
            {
                sourceOwnerId = sourceDefinition.CreatedBy.Value;
                sourceOwnerEmail = await ResolveUserEmailAsync(sourceDefinition.CreatedBy.Value, ct);
            }
            
            // Fallback: If sourceOwnerEmail is still null, try to detect if this is a seeded story
            // Seeded stories have assets in "images/tol/stories/seed@alchimalia.com/..." path
            if (string.IsNullOrWhiteSpace(sourceOwnerEmail))
            {
                _logger.LogWarning("Source owner email not found for story {StoryId}. Checking if this is a seeded story...", sourceStoryId);
                
                // Check if cover image path contains "seed@alchimalia.com" or "tol/stories"
                if (!string.IsNullOrWhiteSpace(sourceDefinition.CoverImageUrl))
                {
                    var coverPath = sourceDefinition.CoverImageUrl.ToLowerInvariant();
                    if (coverPath.Contains("seed@alchimalia.com") || coverPath.Contains("/tol/stories/"))
                    {
                        sourceOwnerEmail = "seed@alchimalia.com";
                        _logger.LogInformation("Detected seeded story {StoryId}. Using seed@alchimalia.com as source owner email.", sourceStoryId);
                    }
                }
            }
        }
        else
        {
            _logger.LogWarning("Unable to determine source metadata for fork job. sourceType={SourceType} targetStoryId={TargetStoryId}", sourceType, targetStoryId);
            return null;
        }

        var job = new StoryForkAssetJob
        {
            Id = Guid.NewGuid(),
            SourceStoryId = sourceStoryId,
            SourceType = sourceType,
            SourceOwnerUserId = sourceOwnerId,
            SourceOwnerEmail = sourceOwnerEmail,
            TargetStoryId = targetStoryId,
            TargetOwnerUserId = currentUser.Id,
            TargetOwnerEmail = currentUser.Email,
            RequestedByEmail = currentUser.Email,
            Status = StoryForkAssetJobStatus.Queued,
            QueuedAtUtc = DateTime.UtcNow
        };

        _db.StoryForkAssetJobs.Add(job);
        await _db.SaveChangesAsync(ct);

        return job;
    }

    internal async Task<ForkJobResult> ProcessForkJobAsync(StoryForkJob job, CancellationToken ct)
    {
        var warnings = new List<string>();
        var errors = new List<string>();
        StoryForkAssetJob? assetJob = null;
        var sourceTranslations = 0;
        var sourceTiles = 0;

        try
        {
            StoryCraft? newCraft = null;
            StoryCraft? sourceDraft = null;
            StoryDefinition? sourceDefinition = null;

            if (job.SourceType == StoryForkAssetJobSourceTypes.Draft)
            {
                sourceDraft = await _db.StoryCrafts
                    .Include(c => c.Translations)
                    .Include(c => c.Tiles).ThenInclude(t => t.Translations)
                    .Include(c => c.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Translations)
                    .Include(c => c.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
                    .FirstOrDefaultAsync(c => c.StoryId == job.SourceStoryId, ct);

                if (sourceDraft == null)
                {
                    errors.Add($"Source draft not found for storyId={job.SourceStoryId}");
                    return ForkJobResult.Failed(sourceTranslations, sourceTiles, warnings, errors);
                }

                sourceTranslations = sourceDraft.Translations.Count;
                sourceTiles = sourceDraft.Tiles.Count;

                var existingTarget = await _db.StoryCrafts
                    .FirstOrDefaultAsync(c => c.StoryId == job.TargetStoryId, ct);

                if (existingTarget != null)
                {
                    warnings.Add("Target craft already exists; reusing existing draft.");
                    newCraft = existingTarget;
                }
                else
                {
                    newCraft = await _storyCopyService.CreateCopyFromCraftAsync(
                        sourceDraft,
                        job.TargetOwnerUserId,
                        job.TargetStoryId,
                        ct);
                }
            }
            else if (job.SourceType == StoryForkAssetJobSourceTypes.Published)
            {
                sourceDefinition = await _db.StoryDefinitions
                    .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
                    .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Translations)
                    .Include(d => d.Tiles).ThenInclude(t => t.Translations)
                    .Include(d => d.Translations)
                    .Include(d => d.Topics).ThenInclude(t => t.StoryTopic)
                    .Include(d => d.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
                    .FirstOrDefaultAsync(d => d.StoryId == job.SourceStoryId, ct);

                if (sourceDefinition == null)
                {
                    errors.Add($"Source published story not found for storyId={job.SourceStoryId}");
                    return ForkJobResult.Failed(sourceTranslations, sourceTiles, warnings, errors);
                }

                sourceTranslations = sourceDefinition.Translations.Count;
                sourceTiles = sourceDefinition.Tiles.Count;

                var existingTarget = await _db.StoryCrafts
                    .FirstOrDefaultAsync(c => c.StoryId == job.TargetStoryId, ct);

                if (existingTarget != null)
                {
                    warnings.Add("Target craft already exists; reusing existing draft.");
                    newCraft = existingTarget;
                }
                else
                {
                    newCraft = await _storyCopyService.CreateCopyFromDefinitionAsync(
                        sourceDefinition,
                        job.TargetOwnerUserId,
                        job.TargetStoryId,
                        ct);
                }
            }
            else
            {
                errors.Add($"Unsupported source type '{job.SourceType}' for fork job {job.Id}.");
                return ForkJobResult.Failed(sourceTranslations, sourceTiles, warnings, errors);
            }

            if (newCraft == null)
            {
                errors.Add("Unable to create or load target draft for fork job.");
                return ForkJobResult.Failed(sourceTranslations, sourceTiles, warnings, errors);
            }

            if (job.CopyAssets)
            {
                var currentUser = new AlchimaliaUser
                {
                    Id = job.TargetOwnerUserId,
                    Email = job.TargetOwnerEmail
                };

                assetJob = await CreateForkAssetJobAsync(
                    currentUser,
                    newCraft,
                    sourceDraft,
                    sourceDefinition,
                    job.TargetStoryId,
                    job.SourceType,
                    ct);

                if (assetJob != null)
                {
                    await _forkAssetsQueue.EnqueueAsync(assetJob, ct);
                }
                else
                {
                    warnings.Add("Fork asset job already queued for this story.");
                }
            }

            return ForkJobResult.Completed(
                sourceTranslations,
                sourceTiles,
                warnings,
                assetJob?.Id,
                assetJob?.Status ?? (job.CopyAssets ? StoryForkAssetJobStatus.Queued : null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing fork job: jobId={JobId}", job.Id);
            errors.Add(ex.Message);
            return ForkJobResult.Failed(sourceTranslations, sourceTiles, warnings, errors);
        }
    }

    internal async Task<ForkAssetJobResult> ProcessForkAssetJobAsync(StoryForkAssetJob job, CancellationToken ct)
    {
        var warnings = new List<string>();
        var errors = new List<string>();
        var attemptedAssets = 0;
        var copiedAssets = 0;

        try
        {
            var targetCraft = await _db.StoryCrafts
                .Include(c => c.Translations)
                .FirstOrDefaultAsync(c => c.StoryId == job.TargetStoryId, ct);

            if (targetCraft == null)
            {
                errors.Add($"Target draft not found for storyId={job.TargetStoryId}");
                return ForkAssetJobResult.Failed(attemptedAssets, copiedAssets, warnings, errors);
            }

            var targetEmail = job.TargetOwnerEmail;
            if (string.IsNullOrWhiteSpace(targetEmail))
            {
                var targetUserEmail = await ResolveUserEmailAsync(job.TargetOwnerUserId, ct);
                targetEmail = targetUserEmail ?? targetEmail;
            }

            if (job.SourceType == StoryForkAssetJobSourceTypes.Draft)
            {
                var sourceCraft = await _db.StoryCrafts
                    .Include(c => c.Tiles).ThenInclude(t => t.Translations)
                    .Include(c => c.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Translations)
                    .Include(c => c.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
                    .FirstOrDefaultAsync(c => c.StoryId == job.SourceStoryId, ct);

                if (sourceCraft == null)
                {
                    errors.Add($"Source draft not found for storyId={job.SourceStoryId}");
                    return ForkAssetJobResult.Failed(attemptedAssets, copiedAssets, warnings, errors);
                }

                var sourceEmail = job.SourceOwnerEmail;
                if (string.IsNullOrWhiteSpace(sourceEmail) && job.SourceOwnerUserId.HasValue)
                {
                    var placeholderUser = new AlchimaliaUser
                    {
                        Id = job.TargetOwnerUserId,
                        Email = targetEmail ?? string.Empty
                    };

                    sourceEmail = await ResolveOwnerEmailAsync(job.SourceOwnerUserId.Value, placeholderUser, ct);
                }

                if (string.IsNullOrWhiteSpace(sourceEmail))
                {
                    errors.Add("Unable to resolve source draft owner email.");
                    return ForkAssetJobResult.Failed(attemptedAssets, copiedAssets, warnings, errors);
                }

                var assets = _storyAssetCopyService.CollectFromCraft(sourceCraft);
                attemptedAssets = assets.Count;

                if (attemptedAssets == 0)
                {
                    return ForkAssetJobResult.Successful(attemptedAssets, copiedAssets, warnings, errors);
                }

                var result = await _storyAssetCopyService.CopyDraftToDraftAsync(
                    assets,
                    sourceEmail,
                    sourceCraft.StoryId,
                    targetEmail ?? string.Empty,
                    targetCraft.StoryId,
                    ct);

                if (result.HasError)
                {
                    errors.Add(result.ErrorMessage ?? "Asset copy failed.");
                    return ForkAssetJobResult.Failed(attemptedAssets, copiedAssets, warnings, errors, result.AssetFilename);
                }

                copiedAssets = attemptedAssets;
                return ForkAssetJobResult.Successful(attemptedAssets, copiedAssets, warnings, errors);
            }

            if (job.SourceType == StoryForkAssetJobSourceTypes.Published)
            {
                var definition = await _db.StoryDefinitions
                    .Include(d => d.Tiles).ThenInclude(t => t.Translations)
                    .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
                    .Include(d => d.Translations)
                    .FirstOrDefaultAsync(d => d.StoryId == job.SourceStoryId, ct);

                if (definition == null)
                {
                    errors.Add($"Source published story not found for storyId={job.SourceStoryId}");
                    return ForkAssetJobResult.Failed(attemptedAssets, copiedAssets, warnings, errors);
                }

                var sourceEmail = job.SourceOwnerEmail;
                if (string.IsNullOrWhiteSpace(sourceEmail) && definition.CreatedBy.HasValue)
                {
                    sourceEmail = await ResolveUserEmailAsync(definition.CreatedBy.Value, ct);
                }

                // Fallback: If sourceEmail is still null, try to detect if this is a seeded story
                if (string.IsNullOrWhiteSpace(sourceEmail))
                {
                    _logger.LogWarning("Source owner email not found for published story {StoryId}. Checking if this is a seeded story...", definition.StoryId);
                    
                    // Check if cover image path contains "seed@alchimalia.com" or "tol/stories"
                    if (!string.IsNullOrWhiteSpace(definition.CoverImageUrl))
                    {
                        var coverPath = definition.CoverImageUrl.ToLowerInvariant();
                        if (coverPath.Contains("seed@alchimalia.com") || coverPath.Contains("/tol/stories/"))
                        {
                            sourceEmail = "seed@alchimalia.com";
                            _logger.LogInformation("Detected seeded story {StoryId}. Using seed@alchimalia.com as source owner email for asset copy.", definition.StoryId);
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(sourceEmail))
                {
                    errors.Add("Unable to resolve source published owner email.");
                    return ForkAssetJobResult.Failed(attemptedAssets, copiedAssets, warnings, errors);
                }

                var assets = _storyAssetCopyService.CollectFromDefinition(definition);
                attemptedAssets = assets.Count;

                if (attemptedAssets == 0)
                {
                    return ForkAssetJobResult.Successful(attemptedAssets, copiedAssets, warnings, errors);
                }

                var result = await _storyAssetCopyService.CopyPublishedToDraftAsync(
                    assets,
                    sourceEmail,
                    definition.StoryId,
                    targetEmail ?? string.Empty,
                    targetCraft.StoryId,
                    ct);

                if (result.HasError)
                {
                    errors.Add(result.ErrorMessage ?? "Asset copy failed.");
                    return ForkAssetJobResult.Failed(attemptedAssets, copiedAssets, warnings, errors, result.AssetFilename);
                }

                copiedAssets = attemptedAssets;
                return ForkAssetJobResult.Successful(attemptedAssets, copiedAssets, warnings, errors);
            }

            errors.Add($"Unknown sourceType '{job.SourceType}' encountered.");
            return ForkAssetJobResult.Failed(attemptedAssets, copiedAssets, warnings, errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fork asset job failed: jobId={JobId}", job.Id);
            errors.Add(ex.Message);
            return ForkAssetJobResult.Failed(attemptedAssets, copiedAssets, warnings, errors);
        }
    }

    private async Task<string?> ResolveUserEmailAsync(Guid userId, CancellationToken ct)
    {
        return await _db.AlchimaliaUsers
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync(ct);
    }

    internal readonly record struct ForkJobResult(
        bool Success,
        int SourceTranslations,
        int SourceTiles,
        IReadOnlyList<string> Warnings,
        IReadOnlyList<string> Errors,
        Guid? AssetJobId,
        string? AssetJobStatus)
    {
        public static ForkJobResult Completed(
            int sourceTranslations,
            int sourceTiles,
            IReadOnlyList<string> warnings,
            Guid? assetJobId,
            string? assetJobStatus) =>
            new(true, sourceTranslations, sourceTiles, warnings, Array.Empty<string>(), assetJobId, assetJobStatus);

        public static ForkJobResult Failed(
            int sourceTranslations,
            int sourceTiles,
            IReadOnlyList<string> warnings,
            IReadOnlyList<string> errors) =>
            new(false, sourceTranslations, sourceTiles, warnings, errors, null, null);
    }

    internal readonly record struct ForkAssetJobResult(
        bool Success,
        int AttemptedAssets,
        int CopiedAssets,
        IReadOnlyList<string> Warnings,
        IReadOnlyList<string> Errors,
        string? FailureAsset = null)
    {
        public static ForkAssetJobResult Successful(
            int attempted,
            int copied,
            IReadOnlyList<string> warnings,
            IReadOnlyList<string> errors)
            => new(true, attempted, copied, warnings, errors);

        public static ForkAssetJobResult Failed(
            int attempted,
            int copied,
            IReadOnlyList<string> warnings,
            IReadOnlyList<string> errors,
            string? failureAsset = null)
            => new(false, attempted, copied, warnings, errors, failureAsset);
    }
}

