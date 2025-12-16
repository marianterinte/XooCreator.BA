using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;
using XooCreator.BA.Infrastructure.Services.Blob;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using System.IO;
using System.Linq;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class StoryRegionService : IStoryRegionService
{
    private readonly IStoryRegionRepository _repository;
    private readonly XooDbContext _context;
    private readonly IBlobSasService _blobSas;
    private readonly ILogger<StoryRegionService> _logger;

    public StoryRegionService(
        IStoryRegionRepository repository,
        XooDbContext context,
        IBlobSasService blobSas,
        ILogger<StoryRegionService> logger)
    {
        _repository = repository;
        _context = context;
        _blobSas = blobSas;
        _logger = logger;
    }

    public async Task<StoryRegionDto?> GetRegionAsync(string regionId, CancellationToken ct = default)
    {
        var region = await _repository.GetAsync(regionId, ct);
        if (region == null) return null;

        return new StoryRegionDto
        {
            Id = region.Id,
            ImageUrl = region.ImageUrl,
            Status = region.Status,
            CreatedAt = region.CreatedAt,
            UpdatedAt = region.UpdatedAt,
            PublishedAtUtc = region.PublishedAtUtc,
            AssignedReviewerUserId = region.AssignedReviewerUserId,
            ReviewedByUserId = region.ReviewedByUserId,
            ApprovedByUserId = region.ApprovedByUserId,
            ReviewNotes = region.ReviewNotes,
            ReviewStartedAt = region.ReviewStartedAt,
            ReviewEndedAt = region.ReviewEndedAt,
            Translations = region.Translations.Select(t => new StoryRegionTranslationDto
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Description = t.Description
            }).ToList()
        };
    }

    public async Task<StoryRegionDto> CreateRegionAsync(Guid ownerUserId, string regionId, string name, CancellationToken ct = default)
    {
        var region = await _repository.CreateAsync(ownerUserId, regionId, name, ct);
        
        // Create default translation (ro-ro) with the provided name
        // Add translation directly to context instead of using SaveAsync to avoid concurrency issues
        var defaultTranslation = new StoryRegionTranslation
        {
            Id = Guid.NewGuid(),
            StoryRegionId = region.Id,
            LanguageCode = "ro-ro",
            Name = name
        };
        _context.StoryRegionTranslations.Add(defaultTranslation);
        await _context.SaveChangesAsync(ct);
        
        // Reload region with translations to return complete DTO
        region = await _repository.GetAsync(regionId, ct);
        if (region == null)
        {
            throw new InvalidOperationException($"Region '{regionId}' not found after creation");
        }
        
        return new StoryRegionDto
        {
            Id = region.Id,
            ImageUrl = region.ImageUrl,
            Status = region.Status,
            CreatedAt = region.CreatedAt,
            UpdatedAt = region.UpdatedAt,
            PublishedAtUtc = region.PublishedAtUtc,
            AssignedReviewerUserId = region.AssignedReviewerUserId,
            ReviewedByUserId = region.ReviewedByUserId,
            ApprovedByUserId = region.ApprovedByUserId,
            ReviewNotes = region.ReviewNotes,
            ReviewStartedAt = region.ReviewStartedAt,
            ReviewEndedAt = region.ReviewEndedAt,
            Translations = region.Translations.Select(t => new StoryRegionTranslationDto
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Description = t.Description
            }).ToList()
        };
    }

    public async Task SaveRegionAsync(Guid ownerUserId, string regionId, StoryRegionDto dto, CancellationToken ct = default)
    {
        var region = await _repository.GetAsync(regionId, ct);
        if (region == null)
        {
            throw new InvalidOperationException($"Region '{regionId}' not found");
        }

        if (region.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own region '{regionId}'");
        }

        // Update properties (non-translatable)
        region.ImageUrl = dto.ImageUrl;
        region.UpdatedAt = DateTime.UtcNow;

        // Update translations
        if (dto.Translations != null && dto.Translations.Count > 0)
        {
            foreach (var translationDto in dto.Translations)
            {
                var lang = translationDto.LanguageCode.ToLowerInvariant();
                var translation = region.Translations.FirstOrDefault(t => t.LanguageCode == lang);
                
                if (translation == null)
                {
                    translation = new StoryRegionTranslation
                    {
                        Id = Guid.NewGuid(),
                        StoryRegionId = region.Id,
                        LanguageCode = lang,
                        Name = translationDto.Name,
                        Description = translationDto.Description
                    };
                    _context.StoryRegionTranslations.Add(translation);
                }
                else
                {
                    translation.Name = translationDto.Name;
                    translation.Description = translationDto.Description;
                }
            }
        }

        await _repository.SaveAsync(region, ct);
    }

    public async Task<List<StoryRegionListItemDto>> ListRegionsByOwnerAsync(Guid ownerUserId, string? status = null, Guid? currentUserId = null, CancellationToken ct = default)
    {
        var regions = await _repository.ListByOwnerAsync(ownerUserId, status, ct);
        return regions.Select(r => MapToListItem(r, currentUserId)).ToList();
    }

    public async Task<List<StoryRegionListItemDto>> ListRegionsForEditorAsync(Guid currentUserId, string? status = null, CancellationToken ct = default)
    {
        var ownedRegions = await _repository.ListByOwnerAsync(currentUserId, status, ct);
        var publishedRegions = await _repository.ListPublishedAsync(currentUserId, ct);
        var regionsForReview = await _repository.ListForReviewAsync(ct);

        var combined = new Dictionary<string, StoryRegion>(StringComparer.OrdinalIgnoreCase);

        foreach (var region in ownedRegions)
        {
            combined[region.Id] = region;
        }

        foreach (var region in publishedRegions)
        {
            combined[region.Id] = region;
        }

        // Include regions sent_for_approval and in_review (for reviewers)
        foreach (var region in regionsForReview)
        {
            if (!combined.ContainsKey(region.Id))
            {
                combined[region.Id] = region;
            }
        }

        return combined.Values
            .Select(r => MapToListItem(r, currentUserId))
            .OrderByDescending(r => r.IsOwnedByCurrentUser)
            .ThenBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task DeleteRegionAsync(Guid ownerUserId, string regionId, CancellationToken ct = default)
    {
        var region = await _repository.GetAsync(regionId, ct);
        if (region == null)
        {
            throw new InvalidOperationException($"Region '{regionId}' not found");
        }

        if (region.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own region '{regionId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(region.Status);

        // Only allow deletion of draft or changes_requested regions
        if (currentStatus != StoryStatus.Draft && currentStatus != StoryStatus.ChangesRequested)
        {
            var statusName = currentStatus.ToString();
            if (currentStatus == StoryStatus.SentForApproval || currentStatus == StoryStatus.InReview || currentStatus == StoryStatus.Approved)
            {
                throw new InvalidOperationException($"Cannot delete region '{regionId}' while it is in '{statusName}' status. Please retract it first to move it back to Draft.");
            }
            throw new InvalidOperationException($"Cannot delete region '{regionId}' in '{statusName}' status.");
        }

        // Check if region is used in any epic (for extra safety)
        var isUsed = await _repository.IsUsedInEpicsAsync(regionId, ct);
        if (isUsed && currentStatus == StoryStatus.Published)
        {
            throw new InvalidOperationException($"Cannot delete published region '{regionId}' that is used in epics");
        }

        await _repository.DeleteAsync(regionId, ct);
    }

    public async Task SubmitForReviewAsync(Guid ownerUserId, string regionId, CancellationToken ct = default)
    {
        var region = await _repository.GetAsync(regionId, ct);
        if (region == null)
        {
            throw new InvalidOperationException($"Region '{regionId}' not found");
        }

        if (region.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own region '{regionId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(region.Status);
        if (currentStatus != StoryStatus.Draft && currentStatus != StoryStatus.ChangesRequested)
        {
            throw new InvalidOperationException($"Invalid state transition. Expected Draft or ChangesRequested, got {currentStatus}");
        }

        region.Status = StoryStatus.SentForApproval.ToDb();
        region.AssignedReviewerUserId = null;
        region.ReviewNotes = null;
        region.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveAsync(region, ct);
        _logger.LogInformation("Region submitted for review: regionId={RegionId}", regionId);
    }

    public async Task ReviewAsync(Guid reviewerUserId, string regionId, bool approve, string? notes, CancellationToken ct = default)
    {
        var region = await _repository.GetAsync(regionId, ct);
        if (region == null)
        {
            throw new InvalidOperationException($"Region '{regionId}' not found");
        }

        var currentStatus = StoryStatusExtensions.FromDb(region.Status);
        if (currentStatus != StoryStatus.InReview && currentStatus != StoryStatus.SentForApproval)
        {
            throw new InvalidOperationException($"Invalid state transition. Expected InReview or SentForApproval, got {currentStatus}");
        }

        var newStatus = approve ? StoryStatus.Approved : StoryStatus.ChangesRequested;
        region.Status = newStatus.ToDb();
        region.ReviewNotes = string.IsNullOrWhiteSpace(notes) ? region.ReviewNotes : notes;
        region.ReviewEndedAt = DateTime.UtcNow;
        region.ReviewedByUserId = reviewerUserId;
        if (approve)
        {
            region.ApprovedByUserId = reviewerUserId;
        }
        region.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveAsync(region, ct);
        _logger.LogInformation("Region reviewed: regionId={RegionId} approved={Approve}", regionId, approve);
    }

    public async Task PublishAsync(Guid ownerUserId, string regionId, string ownerEmail, CancellationToken ct = default)
    {
        var region = await _repository.GetAsync(regionId, ct);
        if (region == null)
        {
            throw new InvalidOperationException($"Region '{regionId}' not found");
        }

        if (region.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own region '{regionId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(region.Status);
        if (currentStatus != StoryStatus.Approved)
        {
            throw new InvalidOperationException($"Cannot publish region. Expected Approved, got {currentStatus}");
        }

        // Copy image asset synchronously if exists
        if (!string.IsNullOrWhiteSpace(region.ImageUrl))
        {
            var publishedImageUrl = await PublishImageAsync(regionId, region.ImageUrl, ownerEmail, ct);
            region.ImageUrl = publishedImageUrl;
        }

        region.Status = StoryStatus.Published.ToDb();
        region.PublishedAtUtc = DateTime.UtcNow;
        region.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveAsync(region, ct);
        _logger.LogInformation("Region published: regionId={RegionId}", regionId);
    }

    public async Task RetractAsync(Guid ownerUserId, string regionId, CancellationToken ct = default)
    {
        var region = await _repository.GetAsync(regionId, ct);
        if (region == null)
        {
            throw new InvalidOperationException($"Region '{regionId}' not found");
        }

        if (region.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own region '{regionId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(region.Status);
        if (currentStatus != StoryStatus.SentForApproval && currentStatus != StoryStatus.Approved)
        {
            throw new InvalidOperationException($"Cannot retract region. Expected SentForApproval or Approved, got {currentStatus}");
        }

        // Clear all review-related fields and revert to Draft (similar to RetractStoryEndpoint)
        region.Status = StoryStatus.Draft.ToDb();
        region.AssignedReviewerUserId = null;
        region.ReviewNotes = null;
        region.ReviewStartedAt = null;
        region.ReviewEndedAt = null;
        if (currentStatus == StoryStatus.Approved)
        {
            region.ApprovedByUserId = null;
        }
        region.ReviewedByUserId = null; // Reset reviewed by as well
        region.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveAsync(region, ct);
        _logger.LogInformation("Region retracted: regionId={RegionId}", regionId);
    }

    private StoryRegionListItemDto MapToListItem(StoryRegion region, Guid? currentUserId)
    {
        var firstTranslation = region.Translations.FirstOrDefault();
        var name = firstTranslation?.Name ?? region.Name ?? string.Empty;

        var isOwnedByCurrentUser = currentUserId.HasValue && region.OwnerUserId == currentUserId.Value;
        var isAssignedToCurrentUser = currentUserId.HasValue &&
                                      region.AssignedReviewerUserId.HasValue &&
                                      region.AssignedReviewerUserId.Value == currentUserId.Value;

        return new StoryRegionListItemDto
        {
            Id = region.Id,
            Name = name,
            ImageUrl = region.ImageUrl,
            Status = region.Status,
            CreatedAt = region.CreatedAt,
            UpdatedAt = region.UpdatedAt,
            PublishedAtUtc = region.PublishedAtUtc,
            AssignedReviewerUserId = region.AssignedReviewerUserId,
            IsAssignedToCurrentUser = isAssignedToCurrentUser,
            IsOwnedByCurrentUser = isOwnedByCurrentUser
        };
    }

    private async Task<string> PublishImageAsync(string regionId, string draftPath, string ownerEmail, CancellationToken ct)
    {
        var normalizedPath = NormalizeBlobPath(draftPath);
        if (IsAlreadyPublished(normalizedPath))
        {
            return normalizedPath; // Already published
        }

        var sourceClient = _blobSas.GetBlobClient(_blobSas.DraftContainer, normalizedPath);
        if (!await sourceClient.ExistsAsync(ct))
        {
            throw new InvalidOperationException($"Draft image '{normalizedPath}' does not exist.");
        }

        var fileName = Path.GetFileName(normalizedPath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = $"{regionId}-image.png";
        }

        // Path format: images/regions/{regionId}/{fileName}
        var destinationPath = $"images/regions/{regionId}/{fileName}";
        var destinationClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, destinationPath);

        _logger.LogInformation("Copying region image from {Source} to {Destination}", normalizedPath, destinationPath);

        var sasUri = sourceClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(10));
        var operation = await destinationClient.StartCopyFromUriAsync(sasUri, cancellationToken: ct);
        await operation.WaitForCompletionAsync(cancellationToken: ct);

        return destinationPath;
    }

    private static string NormalizeBlobPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return string.Empty;

        var trimmed = path.Trim();
        if (trimmed.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(trimmed);
            trimmed = uri.AbsolutePath.TrimStart('/');
        }

        return trimmed.TrimStart('/');
    }

    private static bool IsAlreadyPublished(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        return path.StartsWith("images/", StringComparison.OrdinalIgnoreCase);
    }
}

