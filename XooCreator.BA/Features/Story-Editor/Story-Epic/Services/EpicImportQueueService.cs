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

public class EpicImportQueueService : IEpicImportQueueService
{
    private readonly XooDbContext _context;
    private readonly QueueClient _queueClient;
    private readonly ILogger<EpicImportQueueService> _logger;

    public EpicImportQueueService(
        XooDbContext context,
        IConfiguration configuration,
        IAzureQueueClientFactory queueClientFactory,
        ILogger<EpicImportQueueService> logger)
    {
        _context = context;
        _logger = logger;

        var queueName = configuration.GetSection("AzureStorage:Queues")?["EpicImport"];
        _queueClient = queueClientFactory.CreateClient(queueName, "epic-import-queue");
    }

    public async Task<Guid> EnqueueImportAsync(
        string originalEpicId,
        string zipBlobPath,
        string zipFileName,
        long zipSizeBytes,
        EpicImportRequest options,
        Guid ownerUserId,
        string requestedByEmail,
        string locale,
        CancellationToken ct)
    {
        var job = new EpicImportJob
        {
            Id = Guid.NewGuid(),
            OriginalEpicId = originalEpicId,
            OwnerUserId = ownerUserId,
            RequestedByEmail = requestedByEmail,
            Locale = locale,
            ZipBlobPath = zipBlobPath,
            ZipFileName = zipFileName,
            ZipSizeBytes = zipSizeBytes,
            Status = EpicImportJobStatus.Queued,
            ConflictStrategy = options.ConflictStrategy,
            IncludeStories = options.IncludeStories,
            IncludeHeroes = options.IncludeHeroes,
            IncludeRegions = options.IncludeRegions,
            IncludeImages = options.IncludeImages,
            IncludeAudio = options.IncludeAudio,
            IncludeVideo = options.IncludeVideo,
            GenerateNewIds = options.GenerateNewIds,
            IdPrefix = options.IdPrefix,
            QueuedAtUtc = DateTime.UtcNow
        };

        // Initialize phases JSON
        var phases = new EpicImportPhases();
        job.PhasesJson = JsonSerializer.Serialize(phases);

        _context.EpicImportJobs.Add(job);
        await _context.SaveChangesAsync(ct);

        try
        {
            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);

            var payload = new
            {
                JobId = job.Id,
                OriginalEpicId = originalEpicId
            };

            var messageText = JsonSerializer.Serialize(payload);

            _logger.LogInformation("Enqueuing EpicImportJob: jobId={JobId} originalEpicId={OriginalEpicId} queueName={QueueName}",
                job.Id, originalEpicId, _queueClient.Name);

            var response = await _queueClient.SendMessageAsync(messageText, cancellationToken: ct);

            _logger.LogInformation("Successfully enqueued EpicImportJob: jobId={JobId} messageId={MessageId} queueName={QueueName}",
                job.Id, response.Value.MessageId, _queueClient.Name);

            return job.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue EpicImportJob: jobId={JobId} originalEpicId={OriginalEpicId} queueName={QueueName}",
                job.Id, originalEpicId, _queueClient.Name);
            throw;
        }
    }

    public async Task<EpicImportJob?> GetJobStatusAsync(Guid jobId, CancellationToken ct)
    {
        return await _context.EpicImportJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId, ct);
    }

    public async Task UpdateJobStatusAsync(Guid jobId, string status, CancellationToken ct)
    {
        var job = await _context.EpicImportJobs.FindAsync(new object[] { jobId }, ct);
        if (job == null)
        {
            _logger.LogWarning("EpicImportJob not found for status update: jobId={JobId}", jobId);
            return;
        }

        job.Status = status;
        if (status != EpicImportJobStatus.Queued && job.StartedAtUtc == null)
        {
            job.StartedAtUtc = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdatePhaseProgressAsync(Guid jobId, string phaseName, ImportPhaseProgress progress, CancellationToken ct)
    {
        var job = await _context.EpicImportJobs.FindAsync(new object[] { jobId }, ct);
        if (job == null)
        {
            _logger.LogWarning("EpicImportJob not found for phase update: jobId={JobId}", jobId);
            return;
        }

        var phases = DeserializePhases(job.PhasesJson);
        
        switch (phaseName.ToLowerInvariant())
        {
            case "validation":
                phases.Validation.Status = progress.Status;
                phases.Validation.Errors = progress.Errors;
                break;
            case "regions":
                phases.Regions.Status = progress.Status;
                phases.Regions.Imported = progress.Imported;
                phases.Regions.Total = progress.Total;
                phases.Regions.Errors = progress.Errors;
                break;
            case "heroes":
                phases.Heroes.Status = progress.Status;
                phases.Heroes.Imported = progress.Imported;
                phases.Heroes.Total = progress.Total;
                phases.Heroes.Errors = progress.Errors;
                break;
            case "stories":
                phases.Stories.Status = progress.Status;
                phases.Stories.Imported = progress.Imported;
                phases.Stories.Total = progress.Total;
                phases.Stories.Errors = progress.Errors;
                break;
            case "assets":
                phases.Assets.Status = progress.Status;
                phases.Assets.Imported = progress.Imported;
                phases.Assets.Total = progress.Total;
                phases.Assets.Errors = progress.Errors;
                break;
            case "relationships":
                phases.Relationships.Status = progress.Status;
                phases.Relationships.Imported = progress.Imported;
                phases.Relationships.Total = progress.Total;
                phases.Relationships.Errors = progress.Errors;
                break;
        }

        job.PhasesJson = JsonSerializer.Serialize(phases);
        await _context.SaveChangesAsync(ct);
    }

    public async Task CompleteJobAsync(
        Guid jobId,
        string importedEpicId,
        EpicIdMappings idMappings,
        List<string> warnings,
        CancellationToken ct)
    {
        var job = await _context.EpicImportJobs.FindAsync(new object[] { jobId }, ct);
        if (job == null)
        {
            _logger.LogWarning("EpicImportJob not found for completion: jobId={JobId}", jobId);
            return;
        }

        job.Status = EpicImportJobStatus.Completed;
        job.CompletedAtUtc = DateTime.UtcNow;
        job.EpicId = importedEpicId;
        job.IdMappingsJson = JsonSerializer.Serialize(idMappings);
        job.WarningsJson = JsonSerializer.Serialize(warnings);
        job.ErrorMessage = null;

        await _context.SaveChangesAsync(ct);
    }

    public async Task FailJobAsync(Guid jobId, string errorMessage, CancellationToken ct)
    {
        var job = await _context.EpicImportJobs.FindAsync(new object[] { jobId }, ct);
        if (job == null)
        {
            _logger.LogWarning("EpicImportJob not found for failure: jobId={JobId}", jobId);
            return;
        }

        job.Status = EpicImportJobStatus.Failed;
        job.CompletedAtUtc = DateTime.UtcNow;
        job.ErrorMessage = errorMessage;

        await _context.SaveChangesAsync(ct);
    }

    public async Task<EpicImportJob?> DequeueJobAsync(CancellationToken ct)
    {
        var jobs = await _context.EpicImportJobs
            .Where(j => j.Status == EpicImportJobStatus.Queued)
            .OrderBy(j => j.QueuedAtUtc)
            .Take(1)
            .ToListAsync(ct);

        return jobs.FirstOrDefault();
    }

    public async Task AddWarningAsync(Guid jobId, string warning, CancellationToken ct)
    {
        var job = await _context.EpicImportJobs.FindAsync(new object[] { jobId }, ct);
        if (job == null)
        {
            _logger.LogWarning("EpicImportJob not found for warning: jobId={JobId}", jobId);
            return;
        }

        var warnings = DeserializeWarnings(job.WarningsJson);
        warnings.Add(warning);
        job.WarningsJson = JsonSerializer.Serialize(warnings);

        await _context.SaveChangesAsync(ct);
    }

    private static EpicImportPhases DeserializePhases(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new EpicImportPhases();
        }

        try
        {
            return JsonSerializer.Deserialize<EpicImportPhases>(json) ?? new EpicImportPhases();
        }
        catch
        {
            return new EpicImportPhases();
        }
    }

    private static List<string> DeserializeWarnings(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<string>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}

