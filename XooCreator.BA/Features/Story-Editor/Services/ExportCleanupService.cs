using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Background service that periodically cleans up old export ZIP files from blob storage.
/// Deletes exports older than 1 day (configurable) after completion.
/// </summary>
public class ExportCleanupService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<ExportCleanupService> _logger;
    private readonly TimeSpan _cleanupInterval;
    private readonly TimeSpan _retentionPeriod;

    public ExportCleanupService(
        IServiceProvider services,
        ILogger<ExportCleanupService> logger,
        IConfiguration configuration)
    {
        _services = services;
        _logger = logger;

        // Default: run cleanup every hour
        var intervalHours = configuration.GetValue<int>("ExportCleanup:IntervalHours", 1);
        _cleanupInterval = TimeSpan.FromHours(intervalHours);

        // Default: delete exports older than 1 day
        var retentionDays = configuration.GetValue<int>("ExportCleanup:RetentionDays", 1);
        _retentionPeriod = TimeSpan.FromDays(retentionDays);

        _logger.LogInformation(
            "ExportCleanupService initialized: Interval={IntervalHours}h Retention={RetentionDays}d",
            intervalHours,
            retentionDays);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExportCleanupService started. Will run cleanup every {IntervalHours} hours, deleting exports older than {RetentionDays} days.",
            _cleanupInterval.TotalHours,
            _retentionPeriod.TotalDays);

        // Wait a bit before first run to allow app to fully start
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

        using var timer = new PeriodicTimer(_cleanupInterval);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupOldExportsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during export cleanup cycle. Will retry on next interval.");
                }

                // Wait for next interval or cancellation
                await timer.WaitForNextTickAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("ExportCleanupService stopping due to cancellation request.");
        }
        finally
        {
            timer.Dispose();
        }

        _logger.LogInformation("ExportCleanupService has stopped.");
    }

    private async Task CleanupOldExportsAsync(CancellationToken ct)
    {
        var cutoffDate = DateTime.UtcNow - _retentionPeriod;
        _logger.LogInformation("Starting export cleanup. Deleting exports completed before {CutoffDate}", cutoffDate);

        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<XooDbContext>();
        var sas = scope.ServiceProvider.GetRequiredService<IBlobSasService>();
        var containerClient = sas.GetContainerClient(sas.DraftContainer);

        var deletedCount = 0;
        var errorCount = 0;

        // Cleanup StoryExportJob exports
        var oldExportJobs = await db.StoryExportJobs
            .Where(j => j.Status == StoryExportJobStatus.Completed
                     && j.CompletedAtUtc.HasValue
                     && j.CompletedAtUtc.Value < cutoffDate
                     && !string.IsNullOrEmpty(j.ZipBlobPath))
            .Select(j => new { j.Id, j.ZipBlobPath, j.StoryId })
            .ToListAsync(ct);

        foreach (var job in oldExportJobs)
        {
            try
            {
                if (!string.IsNullOrEmpty(job.ZipBlobPath))
                {
                    var blobClient = containerClient.GetBlobClient(job.ZipBlobPath);
                    var deleted = await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
                    if (deleted.Value)
                    {
                        deletedCount++;
                        _logger.LogDebug("Deleted export ZIP: jobId={JobId} storyId={StoryId} path={Path}",
                            job.Id, job.StoryId, job.ZipBlobPath);
                    }
                }
            }
            catch (Exception ex)
            {
                errorCount++;
                _logger.LogWarning(ex, "Failed to delete export ZIP: jobId={JobId} path={Path}",
                    job.Id, job.ZipBlobPath);
            }
        }

        // Cleanup StoryAudioExportJob exports
        var oldAudioExportJobs = await db.StoryAudioExportJobs
            .Where(j => j.Status == StoryAudioExportJobStatus.Completed
                     && j.CompletedAtUtc.HasValue
                     && j.CompletedAtUtc.Value < cutoffDate
                     && !string.IsNullOrEmpty(j.ZipBlobPath))
            .Select(j => new { j.Id, j.ZipBlobPath, j.StoryId })
            .ToListAsync(ct);

        foreach (var job in oldAudioExportJobs)
        {
            try
            {
                if (!string.IsNullOrEmpty(job.ZipBlobPath))
                {
                    var blobClient = containerClient.GetBlobClient(job.ZipBlobPath);
                    var deleted = await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
                    if (deleted.Value)
                    {
                        deletedCount++;
                        _logger.LogDebug("Deleted audio export ZIP: jobId={JobId} storyId={StoryId} path={Path}",
                            job.Id, job.StoryId, job.ZipBlobPath);
                    }
                }
            }
            catch (Exception ex)
            {
                errorCount++;
                _logger.LogWarning(ex, "Failed to delete audio export ZIP: jobId={JobId} path={Path}",
                    job.Id, job.ZipBlobPath);
            }
        }

        // Cleanup StoryImageExportJob exports
        var oldImageExportJobs = await db.StoryImageExportJobs
            .Where(j => j.Status == StoryImageExportJobStatus.Completed
                     && j.CompletedAtUtc.HasValue
                     && j.CompletedAtUtc.Value < cutoffDate
                     && !string.IsNullOrEmpty(j.ZipBlobPath))
            .Select(j => new { j.Id, j.ZipBlobPath, j.StoryId })
            .ToListAsync(ct);

        foreach (var job in oldImageExportJobs)
        {
            try
            {
                if (!string.IsNullOrEmpty(job.ZipBlobPath))
                {
                    var blobClient = containerClient.GetBlobClient(job.ZipBlobPath);
                    var deleted = await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
                    if (deleted.Value)
                    {
                        deletedCount++;
                        _logger.LogDebug("Deleted image export ZIP: jobId={JobId} storyId={StoryId} path={Path}",
                            job.Id, job.StoryId, job.ZipBlobPath);
                    }
                }
            }
            catch (Exception ex)
            {
                errorCount++;
                _logger.LogWarning(ex, "Failed to delete image export ZIP: jobId={JobId} path={Path}",
                    job.Id, job.ZipBlobPath);
            }
        }

        // Cleanup StoryDocumentExportJob exports
        var oldDocumentExportJobs = await db.StoryDocumentExportJobs
            .Where(j => j.Status == StoryDocumentExportJobStatus.Completed
                     && j.CompletedAtUtc.HasValue
                     && j.CompletedAtUtc.Value < cutoffDate
                     && !string.IsNullOrEmpty(j.OutputBlobPath))
            .Select(j => new { j.Id, j.OutputBlobPath, j.StoryId })
            .ToListAsync(ct);

        foreach (var job in oldDocumentExportJobs)
        {
            try
            {
                if (!string.IsNullOrEmpty(job.OutputBlobPath))
                {
                    var blobClient = containerClient.GetBlobClient(job.OutputBlobPath);
                    var deleted = await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
                    if (deleted.Value)
                    {
                        deletedCount++;
                        _logger.LogDebug("Deleted document export: jobId={JobId} storyId={StoryId} path={Path}",
                            job.Id, job.StoryId, job.OutputBlobPath);
                    }
                }
            }
            catch (Exception ex)
            {
                errorCount++;
                _logger.LogWarning(ex, "Failed to delete document export: jobId={JobId} path={Path}",
                    job.Id, job.OutputBlobPath);
            }
        }

        _logger.LogInformation(
            "Export cleanup completed: deleted={DeletedCount} errors={ErrorCount} cutoffDate={CutoffDate}",
            deletedCount,
            errorCount,
            cutoffDate);
    }
}
