using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class EpicExportQueueService : IEpicExportQueueService
{
    private readonly XooDbContext _context;
    private readonly QueueClient _queueClient;
    private readonly IBlobSasService _sas;
    private readonly ILogger<EpicExportQueueService> _logger;

    public EpicExportQueueService(
        XooDbContext context,
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        IBlobSasService sas,
        ILogger<EpicExportQueueService> logger)
    {
        _context = context;
        _sas = sas;
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["EpicExport"];
        _queueClient = queueClientFactory.CreateClient(queueName, "epic-export-queue");
    }

    public async Task<Guid> EnqueueExportAsync(
        string epicId,
        EpicExportRequest options,
        bool isDraft,
        Guid ownerUserId,
        string requestedByEmail,
        string locale,
        CancellationToken ct)
    {
        var job = new EpicExportJob
        {
            Id = Guid.NewGuid(),
            EpicId = epicId,
            OwnerUserId = ownerUserId,
            RequestedByEmail = requestedByEmail,
            Locale = locale,
            IsDraft = isDraft,
            Status = EpicExportJobStatus.Queued,
            IncludeStories = options.IncludeStories,
            IncludeHeroes = options.IncludeHeroes,
            IncludeRegions = options.IncludeRegions,
            IncludeImages = options.IncludeImages,
            IncludeAudio = options.IncludeAudio,
            IncludeVideo = options.IncludeVideo,
            IncludeProgress = options.IncludeProgress,
            LanguageFilter = options.LanguageFilter,
            QueuedAtUtc = DateTime.UtcNow
        };

        _context.EpicExportJobs.Add(job);
        await _context.SaveChangesAsync(ct);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new
            {
                JobId = job.Id,
                EpicId = epicId,
                IsDraft = isDraft
            };

            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing EpicExportJob: jobId={JobId} epicId={EpicId} isDraft={IsDraft} queueName={QueueName}",
                job.Id, epicId, isDraft, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);

            _logger.LogInformation("Successfully enqueued EpicExportJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);

            return job.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue EpicExportJob: jobId={JobId} epicId={EpicId} queueName={QueueName}",
                job.Id, epicId, _queueClient.Name);
            throw;
        }
    }

    public async Task<EpicExportJob?> GetJobStatusAsync(Guid jobId, CancellationToken ct)
    {
        return await _context.EpicExportJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId, ct);
    }

    public async Task UpdateJobProgressAsync(
        Guid jobId,
        string status,
        string? currentPhase = null,
        Action<EpicExportJob>? updateAction = null,
        CancellationToken ct = default)
    {
        var job = await _context.EpicExportJobs.FindAsync(new object[] { jobId }, ct);
        if (job == null)
        {
            _logger.LogWarning("EpicExportJob not found for update: jobId={JobId}", jobId);
            return;
        }

        job.Status = status;
        if (currentPhase != null)
        {
            job.CurrentPhase = currentPhase;
        }

        updateAction?.Invoke(job);

        await _context.SaveChangesAsync(ct);
    }

    public async Task CompleteJobAsync(
        Guid jobId,
        string zipBlobPath,
        string zipFileName,
        long zipSizeBytes,
        CancellationToken ct)
    {
        var job = await _context.EpicExportJobs.FindAsync(new object[] { jobId }, ct);
        if (job == null)
        {
            _logger.LogWarning("EpicExportJob not found for completion: jobId={JobId}", jobId);
            return;
        }

        job.Status = EpicExportJobStatus.Completed;
        job.CompletedAtUtc = DateTime.UtcNow;
        job.ZipBlobPath = zipBlobPath;
        job.ZipFileName = zipFileName;
        job.ZipSizeBytes = zipSizeBytes;
        job.ErrorMessage = null;

        await _context.SaveChangesAsync(ct);
    }

    public async Task FailJobAsync(Guid jobId, string errorMessage, CancellationToken ct)
    {
        var job = await _context.EpicExportJobs.FindAsync(new object[] { jobId }, ct);
        if (job == null)
        {
            _logger.LogWarning("EpicExportJob not found for failure: jobId={JobId}", jobId);
            return;
        }

        job.Status = EpicExportJobStatus.Failed;
        job.CompletedAtUtc = DateTime.UtcNow;
        job.ErrorMessage = errorMessage;

        await _context.SaveChangesAsync(ct);
    }

    public async Task<EpicExportJob?> DequeueJobAsync(CancellationToken ct)
    {
        var jobs = await _context.EpicExportJobs
            .Where(j => j.Status == EpicExportJobStatus.Queued)
            .OrderBy(j => j.QueuedAtUtc)
            .Take(1)
            .ToListAsync(ct);

        return jobs.FirstOrDefault();
    }

    public async Task<string?> GetDownloadUrlAsync(EpicExportJob job, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(job.ZipBlobPath))
        {
            return null;
        }

        var blobClient = _sas.GetBlobClient(_sas.DraftContainer, job.ZipBlobPath);
        if (!await blobClient.ExistsAsync(ct))
        {
            _logger.LogWarning("Export ZIP not found in blob storage: path={Path}", job.ZipBlobPath);
            return null;
        }

        var sasUri = await _sas.GetReadSasAsync(_sas.DraftContainer, job.ZipBlobPath, TimeSpan.FromHours(24), ct);
        return sasUri.ToString();
    }
}

