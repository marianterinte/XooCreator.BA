using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Images;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class StoryRegionService : IStoryRegionService
{
    private readonly IStoryRegionRepository _repository;
    private readonly XooDbContext _context;
    private readonly IBlobSasService _blobSas;
    private readonly IRegionPublishedAssetCleanupService _assetCleanup;
    private readonly IImageCompressionService _imageCompression;
    private readonly IRegionPublishChangeLogService _changeLogService;
    private readonly IRegionAssetLinkService _assetLinkService;
    private readonly ILogger<StoryRegionService> _logger;

    public StoryRegionService(
        IStoryRegionRepository repository,
        XooDbContext context,
        IBlobSasService blobSas,
        IRegionPublishedAssetCleanupService assetCleanup,
        IImageCompressionService imageCompression,
        IRegionPublishChangeLogService changeLogService,
        IRegionAssetLinkService assetLinkService,
        ILogger<StoryRegionService> logger)
    {
        _repository = repository;
        _context = context;
        _blobSas = blobSas;
        _assetCleanup = assetCleanup;
        _imageCompression = imageCompression;
        _changeLogService = changeLogService;
        _assetLinkService = assetLinkService;
        _logger = logger;
    }

    public async Task<StoryRegionDto?> GetRegionAsync(string regionId, CancellationToken ct = default)
    {
        // Try to get craft first (draft/in-review/approved)
        var craft = await _repository.GetCraftAsync(regionId, ct);
        if (craft != null)
        {
            return new StoryRegionDto
            {
                Id = craft.Id,
                ImageUrl = craft.ImageUrl,
                Status = craft.Status,
                CreatedAt = craft.CreatedAt,
                UpdatedAt = craft.UpdatedAt,
                PublishedAtUtc = null,
                AssignedReviewerUserId = craft.AssignedReviewerUserId,
                ReviewedByUserId = craft.ReviewedByUserId,
                ApprovedByUserId = craft.ApprovedByUserId,
                ReviewNotes = craft.ReviewNotes,
                ReviewStartedAt = craft.ReviewStartedAt,
                ReviewEndedAt = craft.ReviewEndedAt,
                Translations = craft.Translations.Select(t => new StoryRegionTranslationDto
                {
                    LanguageCode = t.LanguageCode,
                    Name = t.Name,
                    Description = t.Description
                }).ToList()
            };
        }

        // Try to get definition (published)
        var definition = await _repository.GetDefinitionAsync(regionId, ct);
        if (definition != null)
        {
            return new StoryRegionDto
            {
                Id = definition.Id,
                ImageUrl = definition.ImageUrl,
                Status = definition.Status,
                CreatedAt = definition.CreatedAt,
                UpdatedAt = definition.UpdatedAt,
                PublishedAtUtc = definition.PublishedAtUtc,
                AssignedReviewerUserId = null,
                ReviewedByUserId = null,
                ApprovedByUserId = null,
                ReviewNotes = null,
                ReviewStartedAt = null,
                ReviewEndedAt = null,
                Translations = definition.Translations.Select(t => new StoryRegionTranslationDto
                {
                    LanguageCode = t.LanguageCode,
                    Name = t.Name,
                    Description = t.Description
                }).ToList()
            };
        }

        return null;
    }

    public async Task<StoryRegionDto> CreateRegionAsync(Guid ownerUserId, string regionId, string name, string? description, string languageCode, CancellationToken ct = default)
    {
        var regionCraft = await _repository.CreateCraftAsync(ownerUserId, regionId, name, ct);
        
        // Create default translation with the provided name, description, and language code
        var defaultTranslation = new StoryRegionCraftTranslation
        {
            StoryRegionCraftId = regionCraft.Id,
            LanguageCode = languageCode.ToLowerInvariant(),
            Name = name,
            Description = description
        };
        _context.StoryRegionCraftTranslations.Add(defaultTranslation);
        await _context.SaveChangesAsync(ct);
        
        // Reload region craft with translations to return complete DTO
        regionCraft = await _repository.GetCraftAsync(regionId, ct);
        if (regionCraft == null)
        {
            throw new InvalidOperationException($"Region craft '{regionId}' not found after creation");
        }
        
        return new StoryRegionDto
        {
            Id = regionCraft.Id,
            ImageUrl = regionCraft.ImageUrl,
            Status = regionCraft.Status,
            CreatedAt = regionCraft.CreatedAt,
            UpdatedAt = regionCraft.UpdatedAt,
            PublishedAtUtc = null,
            AssignedReviewerUserId = regionCraft.AssignedReviewerUserId,
            ReviewedByUserId = regionCraft.ReviewedByUserId,
            ApprovedByUserId = regionCraft.ApprovedByUserId,
            ReviewNotes = regionCraft.ReviewNotes,
            ReviewStartedAt = regionCraft.ReviewStartedAt,
            ReviewEndedAt = regionCraft.ReviewEndedAt,
            Translations = regionCraft.Translations.Select(t => new StoryRegionTranslationDto
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Description = t.Description
            }).ToList()
        };
    }

    public async Task SaveRegionAsync(Guid ownerUserId, string regionId, StoryRegionDto dto, CancellationToken ct = default)
    {
        var regionCraft = await _repository.GetCraftAsync(regionId, ct);
        if (regionCraft == null)
        {
            throw new InvalidOperationException($"Region craft '{regionId}' not found");
        }

        if (regionCraft.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own region '{regionId}'");
        }

        // Determine language code for change tracking (use first translation or default)
        const string defaultLang = "ro-ro";
        var langForTracking = dto.Translations?.FirstOrDefault()?.LanguageCode ?? defaultLang;
        
        // Capture snapshot before changes
        var snapshotBeforeChanges = _changeLogService.CaptureSnapshot(regionCraft, langForTracking);

        // Update properties (non-translatable)
        regionCraft.ImageUrl = dto.ImageUrl;
        regionCraft.UpdatedAt = DateTime.UtcNow;

        // Update translations
        if (dto.Translations != null && dto.Translations.Count > 0)
        {
            foreach (var translationDto in dto.Translations)
            {
                var lang = translationDto.LanguageCode.ToLowerInvariant();
                var translation = regionCraft.Translations.FirstOrDefault(t => t.LanguageCode == lang);
                
                if (translation == null)
                {
                    translation = new StoryRegionCraftTranslation
                    {
                        StoryRegionCraftId = regionCraft.Id,
                        LanguageCode = lang,
                        Name = translationDto.Name,
                        Description = translationDto.Description
                    };
                    _context.StoryRegionCraftTranslations.Add(translation);
                }
                else
                {
                    translation.Name = translationDto.Name;
                    translation.Description = translationDto.Description;
                }
            }
        }

        await _repository.SaveCraftAsync(regionCraft, ct);
        
        // Append changes to change log for delta publish
        await _changeLogService.AppendChangesAsync(regionCraft, snapshotBeforeChanges, langForTracking, ownerUserId, ct);
    }

    public async Task<List<StoryRegionListItemDto>> ListRegionsByOwnerAsync(Guid ownerUserId, string? status = null, Guid? currentUserId = null, CancellationToken ct = default)
    {
        var regionCrafts = await _repository.ListCraftsByOwnerAsync(ownerUserId, status, ct);
        return regionCrafts.Select(r => MapCraftToListItem(r, currentUserId)).ToList();
    }

    public async Task<List<StoryRegionListItemDto>> ListRegionsForEditorAsync(Guid currentUserId, string? status = null, CancellationToken ct = default)
    {
        var ownedCrafts = await _repository.ListCraftsByOwnerAsync(currentUserId, status, ct);
        var publishedDefinitions = await _repository.ListPublishedDefinitionsAsync(excludeOwnerId: null, ct); // Include owner's published regions
        var craftsForReview = await _repository.ListCraftsForReviewAsync(ct);

        var combined = new List<StoryRegionListItemDto>();

        // Add owned crafts
        foreach (var craft in ownedCrafts)
        {
            combined.Add(MapCraftToListItem(craft, currentUserId));
        }

        // Add published definitions - always include, even if draft exists
        // This allows published regions to remain visible when "new version" creates a draft
        foreach (var definition in publishedDefinitions)
        {
            combined.Add(MapDefinitionToListItem(definition, currentUserId));
        }

        // Add crafts for review (avoid duplicates)
        var ownedIds = new HashSet<string>(ownedCrafts.Select(c => c.Id), StringComparer.OrdinalIgnoreCase);
        foreach (var craft in craftsForReview)
        {
            if (!ownedIds.Contains(craft.Id))
            {
                combined.Add(MapCraftToListItem(craft, currentUserId));
            }
        }

        return combined
            .OrderByDescending(r => r.IsOwnedByCurrentUser)
            .ThenBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task DeleteRegionAsync(Guid ownerUserId, string regionId, CancellationToken ct = default)
    {
        var regionCraft = await _repository.GetCraftAsync(regionId, ct);
        if (regionCraft == null)
        {
            throw new InvalidOperationException($"Region craft '{regionId}' not found");
        }

        if (regionCraft.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own region '{regionId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(regionCraft.Status);

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

        await _repository.DeleteCraftAsync(regionId, ct);
    }

    public async Task SubmitForReviewAsync(Guid ownerUserId, string regionId, CancellationToken ct = default)
    {
        var regionCraft = await _repository.GetCraftAsync(regionId, ct);
        if (regionCraft == null)
        {
            throw new InvalidOperationException($"Region craft '{regionId}' not found");
        }

        if (regionCraft.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own region '{regionId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(regionCraft.Status);
        if (currentStatus != StoryStatus.Draft && currentStatus != StoryStatus.ChangesRequested)
        {
            throw new InvalidOperationException($"Invalid state transition. Expected Draft or ChangesRequested, got {currentStatus}");
        }

        regionCraft.Status = StoryStatus.SentForApproval.ToDb();
        regionCraft.AssignedReviewerUserId = null;
        regionCraft.ReviewNotes = null;
        regionCraft.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveCraftAsync(regionCraft, ct);
        _logger.LogInformation("Region craft submitted for review: regionId={RegionId}", regionId);
    }

    public async Task ReviewAsync(Guid reviewerUserId, string regionId, bool approve, string? notes, CancellationToken ct = default)
    {
        var regionCraft = await _repository.GetCraftAsync(regionId, ct);
        if (regionCraft == null)
        {
            throw new InvalidOperationException($"Region craft '{regionId}' not found");
        }

        var currentStatus = StoryStatusExtensions.FromDb(regionCraft.Status);
        if (currentStatus != StoryStatus.InReview && currentStatus != StoryStatus.SentForApproval)
        {
            throw new InvalidOperationException($"Invalid state transition. Expected InReview or SentForApproval, got {currentStatus}");
        }

        var newStatus = approve ? StoryStatus.Approved : StoryStatus.ChangesRequested;
        regionCraft.Status = newStatus.ToDb();
        regionCraft.ReviewNotes = string.IsNullOrWhiteSpace(notes) ? regionCraft.ReviewNotes : notes;
        regionCraft.ReviewEndedAt = DateTime.UtcNow;
        regionCraft.ReviewedByUserId = reviewerUserId;
        if (approve)
        {
            regionCraft.ApprovedByUserId = reviewerUserId;
        }
        regionCraft.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveCraftAsync(regionCraft, ct);
        _logger.LogInformation("Region craft reviewed: regionId={RegionId} approved={Approve}", regionId, approve);
    }

    public async Task PublishAsync(Guid ownerUserId, string regionId, string ownerEmail, CancellationToken ct = default)
    {
        var regionCraft = await _repository.GetCraftAsync(regionId, ct);
        if (regionCraft == null)
        {
            throw new InvalidOperationException($"Region craft '{regionId}' not found");
        }

        if (regionCraft.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own region '{regionId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(regionCraft.Status);
        if (currentStatus != StoryStatus.Approved)
        {
            throw new InvalidOperationException($"Cannot publish region. Expected Approved, got {currentStatus}");
        }

        // Load craft with translations
        regionCraft = await _context.StoryRegionCrafts
            .Include(c => c.Translations)
            .FirstOrDefaultAsync(c => c.Id == regionId, ct) ?? regionCraft;

        // Check if definition already exists
        var existingDefinition = await _repository.GetDefinitionAsync(regionId, ct);
        
        var requiresFullPublish = existingDefinition == null;
        List<RegionPublishChangeLog>? pendingLogs = null;
        const string defaultLangTag = "ro-ro"; // Default language for delta publish

        if (!requiresFullPublish && existingDefinition != null)
        {
            pendingLogs = await _context.RegionPublishChangeLogs
                .Where(x => x.RegionId == regionId && x.DraftVersion > existingDefinition.LastPublishedVersion)
                .OrderBy(x => x.DraftVersion)
                .ThenBy(x => x.CreatedAt)
                .ToListAsync(ct);

            if (pendingLogs.Count == 0)
            {
                requiresFullPublish = true;
            }
        }

        if (!requiresFullPublish && existingDefinition != null && pendingLogs != null)
        {
            var deltaApplied = await TryApplyDeltaPublishAsync(existingDefinition, regionCraft, pendingLogs, ownerEmail, defaultLangTag, ct);
            if (deltaApplied)
            {
                await _context.SaveChangesAsync(ct);
                await CleanupChangeLogsAsync(regionId, regionCraft.LastDraftVersion, ct);
                
                // Cleanup craft after successful publish
                await CleanupCraftAsync(regionId, ct);
                return;
            }

            requiresFullPublish = true;
        }

        // Full publish
        await ApplyFullPublishAsync(existingDefinition, regionCraft, ownerEmail, ct);
        await CleanupChangeLogsAsync(regionId, regionCraft.LastDraftVersion, ct);
        
        // Cleanup craft after successful publish
        await CleanupCraftAsync(regionId, ct);
    }

    private async Task ApplyFullPublishAsync(StoryRegionDefinition? existingDefinition, StoryRegionCraft regionCraft, string ownerEmail, CancellationToken ct)
    {
        // Sync assets first
        await _assetLinkService.SyncImageAsync(regionCraft, ownerEmail, ct);

        // Get published image URL from asset link
        string? publishedImageUrl = null;
        if (!string.IsNullOrWhiteSpace(regionCraft.ImageUrl))
        {
            var normalizedPath = NormalizeBlobPath(regionCraft.ImageUrl);
            var assetLink = await _context.RegionAssetLinks
                .FirstOrDefaultAsync(x => x.RegionId == regionCraft.Id && x.DraftPath == normalizedPath, ct);
            
            if (assetLink != null && !string.IsNullOrWhiteSpace(assetLink.PublishedPath))
            {
                publishedImageUrl = assetLink.PublishedPath;
            }
            else if (IsAlreadyPublished(normalizedPath))
            {
                publishedImageUrl = normalizedPath;
            }
        }

        StoryRegionDefinition definition;
        
        if (existingDefinition == null)
        {
            // Create new definition
            definition = new StoryRegionDefinition
            {
                Id = regionCraft.Id,
                Name = regionCraft.Name,
                ImageUrl = publishedImageUrl,
                OwnerUserId = regionCraft.OwnerUserId,
                Status = "published",
                CreatedAt = regionCraft.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                PublishedAtUtc = DateTime.UtcNow,
                Version = 1,
                LastPublishedVersion = regionCraft.LastDraftVersion
            };
            _context.StoryRegionDefinitions.Add(definition);
        }
        else
        {
            // Update existing definition
            existingDefinition.Name = regionCraft.Name;
            existingDefinition.ImageUrl = publishedImageUrl;
            existingDefinition.UpdatedAt = DateTime.UtcNow;
            existingDefinition.PublishedAtUtc = DateTime.UtcNow;
            existingDefinition.Version += 1;
            existingDefinition.LastPublishedVersion = regionCraft.LastDraftVersion;
            definition = existingDefinition;
            
            // Remove old translations
            var oldTranslations = await _context.StoryRegionDefinitionTranslations
                .Where(t => t.StoryRegionDefinitionId == regionCraft.Id)
                .ToListAsync(ct);
            _context.StoryRegionDefinitionTranslations.RemoveRange(oldTranslations);
        }

        // Copy translations from craft to definition
        foreach (var craftTranslation in regionCraft.Translations)
        {
            var definitionTranslation = new StoryRegionDefinitionTranslation
            {
                StoryRegionDefinitionId = definition.Id,
                LanguageCode = craftTranslation.LanguageCode,
                Name = craftTranslation.Name,
                Description = craftTranslation.Description
            };
            _context.StoryRegionDefinitionTranslations.Add(definitionTranslation);
        }

        await _context.SaveChangesAsync(ct);
    }

    private async Task<bool> TryApplyDeltaPublishAsync(
        StoryRegionDefinition definition,
        StoryRegionCraft craft,
        IReadOnlyCollection<RegionPublishChangeLog> changeLogs,
        string ownerEmail,
        string langTag,
        CancellationToken ct)
    {
        if (changeLogs.Count == 0)
        {
            return false;
        }

        var headerChanged = changeLogs.Any(l =>
            string.Equals(l.EntityType, "Header", StringComparison.OrdinalIgnoreCase));

        var translationChanges = changeLogs
            .Where(l => string.Equals(l.EntityType, "Translation", StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(l.EntityId))
            .GroupBy(l => l.EntityId!)
            .Select(g => g.OrderBy(l => l.DraftVersion).ThenBy(l => l.CreatedAt).Last())
            .ToList();

        if (!headerChanged && translationChanges.Count == 0)
        {
            return false;
        }

        if (headerChanged)
        {
            await ApplyDefinitionMetadataDeltaAsync(definition, craft, ownerEmail, ct);
        }

        foreach (var change in translationChanges)
        {
            var applied = await ApplyTranslationChangeAsync(definition, craft, change, ct);
            if (!applied)
            {
                return false;
            }
        }

        definition.LastPublishedVersion = craft.LastDraftVersion;
        definition.Version = definition.Version <= 0 ? 1 : definition.Version + 1;
        definition.Status = "published";
        definition.UpdatedAt = DateTime.UtcNow;
        definition.PublishedAtUtc = DateTime.UtcNow;

        return true;
    }

    private async Task ApplyDefinitionMetadataDeltaAsync(StoryRegionDefinition definition, StoryRegionCraft craft, string ownerEmail, CancellationToken ct)
    {
        definition.Name = craft.Name;
        definition.UpdatedAt = DateTime.UtcNow;

        // Sync image asset
        await _assetLinkService.SyncImageAsync(craft, ownerEmail, ct);

        // Get published path from asset link
        if (!string.IsNullOrWhiteSpace(craft.ImageUrl))
        {
            var normalizedPath = NormalizeBlobPath(craft.ImageUrl);
            var assetLink = await _context.RegionAssetLinks
                .FirstOrDefaultAsync(x => x.RegionId == craft.Id && x.DraftPath == normalizedPath, ct);
            
            if (assetLink != null && !string.IsNullOrWhiteSpace(assetLink.PublishedPath))
            {
                definition.ImageUrl = assetLink.PublishedPath;
            }
            else if (IsAlreadyPublished(normalizedPath))
            {
                definition.ImageUrl = normalizedPath;
            }
        }
        else
        {
            definition.ImageUrl = null;
            await _assetLinkService.RemoveImageAsync(craft.Id, ct);
        }
    }

    private async Task<bool> ApplyTranslationChangeAsync(
        StoryRegionDefinition definition,
        StoryRegionCraft craft,
        RegionPublishChangeLog change,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(change.EntityId))
        {
            return true;
        }

        var languageCode = change.EntityId;

        if (string.Equals(change.ChangeType, "Removed", StringComparison.OrdinalIgnoreCase))
        {
            var existingTranslation = await _context.StoryRegionDefinitionTranslations
                .FirstOrDefaultAsync(t => t.StoryRegionDefinitionId == definition.Id && t.LanguageCode == languageCode, ct);
            
            if (existingTranslation != null)
            {
                _context.StoryRegionDefinitionTranslations.Remove(existingTranslation);
            }
            return true;
        }

        var craftTranslation = craft.Translations.FirstOrDefault(t => 
            string.Equals(t.LanguageCode, languageCode, StringComparison.OrdinalIgnoreCase));
        
        if (craftTranslation == null)
        {
            _logger.LogWarning("Delta publish failed: regionId={RegionId} languageCode={LanguageCode} missing in craft.", 
                definition.Id, languageCode);
            return false;
        }

        // Remove existing translation if exists
        var existingTranslation2 = await _context.StoryRegionDefinitionTranslations
            .FirstOrDefaultAsync(t => t.StoryRegionDefinitionId == definition.Id && t.LanguageCode == languageCode, ct);
        
        if (existingTranslation2 != null)
        {
            _context.StoryRegionDefinitionTranslations.Remove(existingTranslation2);
        }

        // Add new translation
        _context.StoryRegionDefinitionTranslations.Add(new StoryRegionDefinitionTranslation
        {
            StoryRegionDefinitionId = definition.Id,
            LanguageCode = craftTranslation.LanguageCode,
            Name = craftTranslation.Name,
            Description = craftTranslation.Description
        });

        return true;
    }

    private async Task CleanupChangeLogsAsync(string regionId, int lastDraftVersion, CancellationToken ct)
    {
        if (lastDraftVersion <= 0)
        {
            return;
        }

        await _context.RegionPublishChangeLogs
            .Where(x => x.RegionId == regionId && x.DraftVersion <= lastDraftVersion)
            .ExecuteDeleteAsync(ct);
    }

    private async Task CleanupCraftAsync(string regionId, CancellationToken ct)
    {
        try
        {
            var craftToDelete = await _context.StoryRegionCrafts
                .FirstOrDefaultAsync(c => c.Id == regionId, ct);
            
            if (craftToDelete != null)
            {
                _context.StoryRegionCrafts.Remove(craftToDelete);
                await _context.SaveChangesAsync(ct);
                _logger.LogInformation("Region published and craft cleaned up: regionId={RegionId}", regionId);
            }
        }
        catch (Exception cleanupEx)
        {
            _logger.LogWarning(cleanupEx, "Failed to cleanup region craft after publish: regionId={RegionId}, but publish succeeded", regionId);
        }
    }

    public async Task RetractAsync(Guid ownerUserId, string regionId, CancellationToken ct = default)
    {
        var regionCraft = await _repository.GetCraftAsync(regionId, ct);
        if (regionCraft == null)
        {
            throw new InvalidOperationException($"Region craft '{regionId}' not found");
        }

        if (regionCraft.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own region '{regionId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(regionCraft.Status);
        if (currentStatus != StoryStatus.SentForApproval && currentStatus != StoryStatus.Approved)
        {
            throw new InvalidOperationException($"Cannot retract region. Expected SentForApproval or Approved, got {currentStatus}");
        }

        // Clear all review-related fields and revert to Draft
        regionCraft.Status = StoryStatus.Draft.ToDb();
        regionCraft.AssignedReviewerUserId = null;
        regionCraft.ReviewNotes = null;
        regionCraft.ReviewStartedAt = null;
        regionCraft.ReviewEndedAt = null;
        if (currentStatus == StoryStatus.Approved)
        {
            regionCraft.ApprovedByUserId = null;
        }
        regionCraft.ReviewedByUserId = null;
        regionCraft.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveCraftAsync(regionCraft, ct);
        _logger.LogInformation("Region craft retracted: regionId={RegionId}", regionId);
    }

    private StoryRegionListItemDto MapCraftToListItem(StoryRegionCraft craft, Guid? currentUserId)
    {
        var firstTranslation = craft.Translations.FirstOrDefault();
        var name = firstTranslation?.Name ?? string.Empty;

        var isOwnedByCurrentUser = currentUserId.HasValue && craft.OwnerUserId == currentUserId.Value;
        var isAssignedToCurrentUser = currentUserId.HasValue &&
                                      craft.AssignedReviewerUserId.HasValue &&
                                      craft.AssignedReviewerUserId.Value == currentUserId.Value;

        return new StoryRegionListItemDto
        {
            Id = craft.Id,
            Name = name,
            ImageUrl = craft.ImageUrl,
            Status = craft.Status,
            CreatedAt = craft.CreatedAt,
            UpdatedAt = craft.UpdatedAt,
            PublishedAtUtc = null,
            AssignedReviewerUserId = craft.AssignedReviewerUserId,
            IsAssignedToCurrentUser = isAssignedToCurrentUser,
            IsOwnedByCurrentUser = isOwnedByCurrentUser
        };
    }

    private StoryRegionListItemDto MapDefinitionToListItem(StoryRegionDefinition definition, Guid? currentUserId)
    {
        var firstTranslation = definition.Translations.FirstOrDefault();
        var name = firstTranslation?.Name ?? string.Empty;

        var isOwnedByCurrentUser = currentUserId.HasValue && definition.OwnerUserId == currentUserId.Value;

        return new StoryRegionListItemDto
        {
            Id = definition.Id,
            Name = name,
            ImageUrl = definition.ImageUrl,
            Status = definition.Status,
            CreatedAt = definition.CreatedAt,
            UpdatedAt = definition.UpdatedAt,
            PublishedAtUtc = definition.PublishedAtUtc,
            AssignedReviewerUserId = null,
            IsAssignedToCurrentUser = false,
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

        // Generate s/m variants for region image (non-blocking).
        try
        {
            var (basePath, fileName2) = SplitPath(destinationPath);
            await _imageCompression.EnsureStorySizeVariantsAsync(
                sourceBlobPath: destinationPath,
                targetBasePath: basePath,
                filename: fileName2,
                overwriteExisting: true,
                ct: ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to generate region image variants: path={Path}", destinationPath);
        }

        return destinationPath;
    }

    private static (string BasePath, string FileName) SplitPath(string blobPath)
    {
        var trimmed = (blobPath ?? string.Empty).Trim().TrimStart('/');
        var idx = trimmed.LastIndexOf('/');
        if (idx < 0)
        {
            return (string.Empty, trimmed);
        }
        var basePath = trimmed.Substring(0, idx);
        var fileName = trimmed.Substring(idx + 1);
        return (basePath, fileName);
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

    public async Task CreateVersionFromPublishedAsync(Guid ownerUserId, string regionId, CancellationToken ct = default)
    {
        // Load published StoryRegionDefinition with all related data
        var definition = await _context.StoryRegionDefinitions
            .Include(d => d.Translations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.Id == regionId, ct);

        if (definition == null)
        {
            throw new InvalidOperationException($"Published region '{regionId}' not found");
        }

        // Verify ownership
        if (definition.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own region '{regionId}'");
        }

        // Check if region is published
        if (definition.Status != "published")
        {
            throw new InvalidOperationException($"Region '{regionId}' is not published (status: {definition.Status})");
        }

        // Check if draft already exists (use AsNoTracking to avoid tracking conflicts)
        var existingCraft = await _context.StoryRegionCrafts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == regionId, ct);
        
        if (existingCraft != null)
        {
            if (existingCraft.Status != "published")
            {
                throw new InvalidOperationException("A draft already exists for this region. Please edit or publish it first.");
            }
            
            // If status is "published", this is a leftover craft that should have been deleted
            // Delete it now before creating the new version
            _logger.LogWarning("Found leftover published craft for regionId={RegionId}, deleting it before creating new version", regionId);
            var craftToDelete = await _context.StoryRegionCrafts.FirstOrDefaultAsync(c => c.Id == regionId, ct);
            if (craftToDelete != null)
            {
                _context.StoryRegionCrafts.Remove(craftToDelete);
                await _context.SaveChangesAsync(ct);
            }
        }

        // Create new StoryRegionCraft from StoryRegionDefinition
        var craft = new StoryRegionCraft
        {
            Id = definition.Id,
            Name = definition.Name,
            ImageUrl = definition.ImageUrl,
            OwnerUserId = definition.OwnerUserId,
            Status = "draft",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BaseVersion = definition.Version,
            LastDraftVersion = 0
        };

        // Copy Translations
        foreach (var defTranslation in definition.Translations)
        {
            craft.Translations.Add(new StoryRegionCraftTranslation
            {
                StoryRegionCraftId = craft.Id,
                LanguageCode = defTranslation.LanguageCode,
                Name = defTranslation.Name,
                Description = defTranslation.Description
            });
        }

        _context.StoryRegionCrafts.Add(craft);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Created new version from published region: regionId={RegionId} baseVersion={BaseVersion}", regionId, definition.Version);
    }

    public async Task UnpublishAsync(Guid ownerUserId, string regionId, string reason, CancellationToken ct = default)
    {
        var definition = await _repository.GetDefinitionAsync(regionId, ct);
        if (definition == null)
        {
            throw new InvalidOperationException($"Published region '{regionId}' not found");
        }

        if (definition.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own region '{regionId}'");
        }

        if (definition.Status != "published" || !definition.IsActive)
        {
            throw new InvalidOperationException($"Cannot unpublish region. Expected Published and Active, got status '{definition.Status}' and IsActive '{definition.IsActive}'");
        }

        // Mark as unpublished (destructive - will delete assets)
        definition.Status = "unpublished";
        definition.IsActive = false;
        definition.UpdatedAt = DateTime.UtcNow;

        // TODO: Create separate RegionPublicationAudits table for audit logging
        // For now, we skip audit logging since StoryPublicationAudits has FK to StoryDefinitions

        await _context.SaveChangesAsync(ct);
        
        // Extract owner email from ImageUrl and delete published assets from blob storage
        var ownerEmail = TryExtractOwnerEmail(definition);
        if (!string.IsNullOrWhiteSpace(ownerEmail))
        {
            await _assetCleanup.DeletePublishedAssetsAsync(ownerEmail, regionId, ct);
        }
        else
        {
            _logger.LogWarning("Could not determine owner email for published assets cleanup. regionId={RegionId}", regionId);
        }
        
        _logger.LogInformation("Region unpublished and assets deleted: regionId={RegionId} reason={Reason}", regionId, reason);
    }

    private string? TryExtractOwnerEmail(StoryRegionDefinition definition)
    {
        if (string.IsNullOrWhiteSpace(definition.ImageUrl))
        {
            return null;
        }

        // ImageUrl format: images/tales-of-alchimalia/regions/{ownerEmail}/{regionId}/...
        var parts = definition.ImageUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 5 && parts[0] == "images" && parts[1] == "tales-of-alchimalia" && parts[2] == "regions")
        {
            return parts[3]; // ownerEmail
        }

        return null;
    }
}

