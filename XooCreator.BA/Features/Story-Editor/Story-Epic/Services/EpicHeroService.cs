using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;
using XooCreator.BA.Infrastructure.Services.Blob;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using System.IO;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class EpicHeroService : IEpicHeroService
{
    private readonly IEpicHeroRepository _repository;
    private readonly XooDbContext _context;
    private readonly IBlobSasService _blobSas;
    private readonly IHeroPublishedAssetCleanupService _assetCleanup;
    private readonly ILogger<EpicHeroService> _logger;

    public EpicHeroService(
        IEpicHeroRepository repository,
        XooDbContext context,
        IBlobSasService blobSas,
        IHeroPublishedAssetCleanupService assetCleanup,
        ILogger<EpicHeroService> logger)
    {
        _repository = repository;
        _context = context;
        _blobSas = blobSas;
        _assetCleanup = assetCleanup;
        _logger = logger;
    }

    public async Task<EpicHeroDto?> GetHeroAsync(string heroId, CancellationToken ct = default)
    {
        // Try to get craft first (draft/in-review/approved)
        var craft = await _repository.GetCraftAsync(heroId, ct);
        if (craft != null)
        {
            return new EpicHeroDto
            {
                Id = craft.Id,
                ImageUrl = craft.ImageUrl,
                GreetingAudioUrl = null, // Not stored at craft level
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
                Translations = craft.Translations.Select(t => new EpicHeroTranslationDto
                {
                    LanguageCode = t.LanguageCode,
                    Name = t.Name,
                    Description = t.Description,
                    GreetingText = t.GreetingText,
                    GreetingAudioUrl = t.GreetingAudioUrl
                }).ToList()
            };
        }

        // Try to get definition (published)
        var definition = await _repository.GetDefinitionAsync(heroId, ct);
        if (definition != null)
        {
            return new EpicHeroDto
            {
                Id = definition.Id,
                ImageUrl = definition.ImageUrl,
                GreetingAudioUrl = null, // Not stored at definition level
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
                Translations = definition.Translations.Select(t => new EpicHeroTranslationDto
                {
                    LanguageCode = t.LanguageCode,
                    Name = t.Name,
                    Description = t.Description,
                    GreetingText = t.GreetingText,
                    GreetingAudioUrl = t.GreetingAudioUrl
                }).ToList()
            };
        }

        return null;
    }

    public async Task<EpicHeroDto> CreateHeroAsync(Guid ownerUserId, string heroId, string name, string languageCode, string? description, CancellationToken ct = default)
    {
        var heroCraft = await _repository.CreateCraftAsync(ownerUserId, heroId, name, ct);
        
        // Create initial translation with the provided language code, name, and description
        var defaultTranslation = new EpicHeroCraftTranslation
        {
            EpicHeroCraftId = heroCraft.Id,
            LanguageCode = languageCode,
            Name = name,
            Description = description,
            GreetingText = null,
            GreetingAudioUrl = null
        };
        _context.EpicHeroCraftTranslations.Add(defaultTranslation);
        await _context.SaveChangesAsync(ct);
        
        // Reload hero craft with translations to return complete DTO
        heroCraft = await _repository.GetCraftAsync(heroId, ct);
        if (heroCraft == null)
        {
            throw new InvalidOperationException($"Hero craft '{heroId}' not found after creation");
        }
        
        return new EpicHeroDto
        {
            Id = heroCraft.Id,
            ImageUrl = heroCraft.ImageUrl,
            GreetingAudioUrl = null,
            Status = heroCraft.Status,
            CreatedAt = heroCraft.CreatedAt,
            UpdatedAt = heroCraft.UpdatedAt,
            PublishedAtUtc = null,
            AssignedReviewerUserId = heroCraft.AssignedReviewerUserId,
            ReviewedByUserId = heroCraft.ReviewedByUserId,
            ApprovedByUserId = heroCraft.ApprovedByUserId,
            ReviewNotes = heroCraft.ReviewNotes,
            ReviewStartedAt = heroCraft.ReviewStartedAt,
            ReviewEndedAt = heroCraft.ReviewEndedAt,
            Translations = heroCraft.Translations.Select(t => new EpicHeroTranslationDto
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Description = t.Description,
                GreetingText = t.GreetingText,
                GreetingAudioUrl = t.GreetingAudioUrl
            }).ToList()
        };
    }

    public async Task SaveHeroAsync(Guid ownerUserId, string heroId, EpicHeroDto dto, CancellationToken ct = default)
    {
        var heroCraft = await _repository.GetCraftAsync(heroId, ct);
        if (heroCraft == null)
        {
            throw new InvalidOperationException($"Hero craft '{heroId}' not found");
        }

        if (heroCraft.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own hero '{heroId}'");
        }

        // Update properties (non-translatable)
        heroCraft.ImageUrl = dto.ImageUrl;
        heroCraft.UpdatedAt = DateTime.UtcNow;

        // Update translations
        if (dto.Translations != null && dto.Translations.Count > 0)
        {
            foreach (var translationDto in dto.Translations)
            {
                var lang = translationDto.LanguageCode.ToLowerInvariant();
                var translation = heroCraft.Translations.FirstOrDefault(t => t.LanguageCode == lang);
                
                if (translation == null)
                {
                    translation = new EpicHeroCraftTranslation
                    {
                        EpicHeroCraftId = heroCraft.Id,
                        LanguageCode = lang,
                        Name = translationDto.Name,
                        Description = translationDto.Description,
                        GreetingText = translationDto.GreetingText,
                        GreetingAudioUrl = translationDto.GreetingAudioUrl
                    };
                    _context.EpicHeroCraftTranslations.Add(translation);
                }
                else
                {
                    translation.Name = translationDto.Name;
                    translation.Description = translationDto.Description;
                    translation.GreetingText = translationDto.GreetingText;
                    translation.GreetingAudioUrl = translationDto.GreetingAudioUrl;
                }
            }
        }

        await _repository.SaveCraftAsync(heroCraft, ct);
    }

    public async Task<List<EpicHeroListItemDto>> ListHeroesByOwnerAsync(Guid ownerUserId, string? status = null, Guid? currentUserId = null, CancellationToken ct = default)
    {
        var heroCrafts = await _repository.ListCraftsByOwnerAsync(ownerUserId, status, ct);
        return heroCrafts.Select(h =>
        {
            // Get name and greeting from first available translation
            var firstTranslation = h.Translations.FirstOrDefault();
            var name = firstTranslation?.Name ?? string.Empty;
            var greetingText = firstTranslation?.GreetingText;
            
            // Compute flags for current user
            var isOwnedByCurrentUser = currentUserId.HasValue && h.OwnerUserId == currentUserId.Value;
            var isAssignedToCurrentUser = currentUserId.HasValue && 
                                          h.AssignedReviewerUserId.HasValue && 
                                          h.AssignedReviewerUserId.Value == currentUserId.Value;
            
            return new EpicHeroListItemDto
            {
                Id = h.Id,
                Name = name,
                ImageUrl = h.ImageUrl,
                GreetingText = greetingText,
                GreetingAudioUrl = firstTranslation?.GreetingAudioUrl,
                Status = h.Status,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt,
                PublishedAtUtc = null,
                AssignedReviewerUserId = h.AssignedReviewerUserId,
                IsAssignedToCurrentUser = isAssignedToCurrentUser,
                IsOwnedByCurrentUser = isOwnedByCurrentUser
            };
        }).ToList();
    }

    public async Task<List<EpicHeroListItemDto>> ListHeroesForEditorAsync(Guid currentUserId, string? status = null, CancellationToken ct = default)
    {
        var ownedCrafts = await _repository.ListCraftsByOwnerAsync(currentUserId, status, ct);
        var publishedDefinitions = await _repository.ListPublishedDefinitionsAsync(excludeOwnerId: null, ct); // Include owner's published heroes
        var craftsForReview = await _repository.ListCraftsForReviewAsync(ct);

        var combined = new List<EpicHeroListItemDto>();

        // Add owned crafts
        foreach (var craft in ownedCrafts)
        {
            combined.Add(MapCraftToListItem(craft, currentUserId));
        }

        // Add published definitions - always include, even if draft exists
        // This allows published heroes to remain visible when "new version" creates a draft
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
            .OrderByDescending(h => h.IsOwnedByCurrentUser)
            .ThenBy(h => h.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private EpicHeroListItemDto MapCraftToListItem(EpicHeroCraft craft, Guid? currentUserId)
    {
        var firstTranslation = craft.Translations.FirstOrDefault();
        var name = firstTranslation?.Name ?? string.Empty;
        var greetingText = firstTranslation?.GreetingText;
        
        var isOwnedByCurrentUser = currentUserId.HasValue && craft.OwnerUserId == currentUserId.Value;
        var isAssignedToCurrentUser = currentUserId.HasValue && 
                                      craft.AssignedReviewerUserId.HasValue && 
                                      craft.AssignedReviewerUserId.Value == currentUserId.Value;
        
        return new EpicHeroListItemDto
        {
            Id = craft.Id,
            Name = name,
            ImageUrl = craft.ImageUrl,
            GreetingText = greetingText,
            GreetingAudioUrl = firstTranslation?.GreetingAudioUrl,
            Status = craft.Status,
            CreatedAt = craft.CreatedAt,
            UpdatedAt = craft.UpdatedAt,
            PublishedAtUtc = null,
            AssignedReviewerUserId = craft.AssignedReviewerUserId,
            IsAssignedToCurrentUser = isAssignedToCurrentUser,
            IsOwnedByCurrentUser = isOwnedByCurrentUser
        };
    }

    private EpicHeroListItemDto MapDefinitionToListItem(EpicHeroDefinition definition, Guid? currentUserId)
    {
        var firstTranslation = definition.Translations.FirstOrDefault();
        var name = firstTranslation?.Name ?? string.Empty;
        var greetingText = firstTranslation?.GreetingText;
        
        var isOwnedByCurrentUser = currentUserId.HasValue && definition.OwnerUserId == currentUserId.Value;
        
        return new EpicHeroListItemDto
        {
            Id = definition.Id,
            Name = name,
            ImageUrl = definition.ImageUrl,
            GreetingText = greetingText,
            GreetingAudioUrl = firstTranslation?.GreetingAudioUrl,
            Status = definition.Status,
            CreatedAt = definition.CreatedAt,
            UpdatedAt = definition.UpdatedAt,
            PublishedAtUtc = definition.PublishedAtUtc,
            AssignedReviewerUserId = null,
            IsAssignedToCurrentUser = false,
            IsOwnedByCurrentUser = isOwnedByCurrentUser
        };
    }

    public async Task DeleteHeroAsync(Guid ownerUserId, string heroId, CancellationToken ct = default)
    {
        var heroCraft = await _repository.GetCraftAsync(heroId, ct);
        if (heroCraft == null)
        {
            throw new InvalidOperationException($"Hero craft '{heroId}' not found");
        }

        if (heroCraft.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own hero '{heroId}'");
        }

        // Check if draft can be deleted (only draft and changes_requested)
        if (heroCraft.Status != "draft" && heroCraft.Status != "changes_requested")
        {
            throw new InvalidOperationException($"Cannot delete hero craft in status '{heroCraft.Status}'. Please retract it first.");
        }

        await _repository.DeleteCraftAsync(heroId, ct);
    }

    public async Task SubmitForReviewAsync(Guid ownerUserId, string heroId, CancellationToken ct = default)
    {
        var heroCraft = await _repository.GetCraftAsync(heroId, ct);
        if (heroCraft == null)
        {
            throw new InvalidOperationException($"Hero craft '{heroId}' not found");
        }

        if (heroCraft.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own hero '{heroId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(heroCraft.Status);
        if (currentStatus != StoryStatus.Draft && currentStatus != StoryStatus.ChangesRequested)
        {
            throw new InvalidOperationException($"Invalid state transition. Expected Draft or ChangesRequested, got {currentStatus}");
        }

        heroCraft.Status = StoryStatus.SentForApproval.ToDb();
        heroCraft.AssignedReviewerUserId = null;
        heroCraft.ReviewNotes = null;
        heroCraft.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveCraftAsync(heroCraft, ct);
        _logger.LogInformation("Hero craft submitted for review: heroId={HeroId}", heroId);
    }

    public async Task ReviewAsync(Guid reviewerUserId, string heroId, bool approve, string? notes, CancellationToken ct = default)
    {
        var heroCraft = await _repository.GetCraftAsync(heroId, ct);
        if (heroCraft == null)
        {
            throw new InvalidOperationException($"Hero craft '{heroId}' not found");
        }

        var currentStatus = StoryStatusExtensions.FromDb(heroCraft.Status);
        if (currentStatus != StoryStatus.InReview && currentStatus != StoryStatus.SentForApproval)
        {
            throw new InvalidOperationException($"Invalid state transition. Expected InReview or SentForApproval, got {currentStatus}");
        }

        var newStatus = approve ? StoryStatus.Approved : StoryStatus.ChangesRequested;
        heroCraft.Status = newStatus.ToDb();
        heroCraft.ReviewNotes = string.IsNullOrWhiteSpace(notes) ? heroCraft.ReviewNotes : notes;
        heroCraft.ReviewEndedAt = DateTime.UtcNow;
        heroCraft.ReviewedByUserId = reviewerUserId;
        if (approve)
        {
            heroCraft.ApprovedByUserId = reviewerUserId;
        }
        heroCraft.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveCraftAsync(heroCraft, ct);
        _logger.LogInformation("Hero craft reviewed: heroId={HeroId} approved={Approve}", heroId, approve);
    }

    public async Task PublishAsync(Guid ownerUserId, string heroId, string ownerEmail, CancellationToken ct = default)
    {
        var heroCraft = await _repository.GetCraftAsync(heroId, ct);
        if (heroCraft == null)
        {
            throw new InvalidOperationException($"Hero craft '{heroId}' not found");
        }

        if (heroCraft.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own hero '{heroId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(heroCraft.Status);
        if (currentStatus != StoryStatus.Approved)
        {
            throw new InvalidOperationException($"Cannot publish hero. Expected Approved, got {currentStatus}");
        }

        // Copy image asset synchronously if exists
        string? publishedImageUrl = heroCraft.ImageUrl;
        if (!string.IsNullOrWhiteSpace(heroCraft.ImageUrl))
        {
            publishedImageUrl = await PublishImageAsync(heroId, heroCraft.ImageUrl, ownerEmail, ct);
        }

        // Check if definition already exists
        var existingDefinition = await _repository.GetDefinitionAsync(heroId, ct);
        EpicHeroDefinition definition;
        
        if (existingDefinition == null)
        {
            // Create new definition
            definition = new EpicHeroDefinition
            {
                Id = heroCraft.Id,
                Name = heroCraft.Name,
                ImageUrl = publishedImageUrl,
                OwnerUserId = heroCraft.OwnerUserId,
                Status = "published",
                CreatedAt = heroCraft.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                PublishedAtUtc = DateTime.UtcNow,
                Version = 1
            };
            _context.EpicHeroDefinitions.Add(definition);
        }
        else
        {
            // Update existing definition
            existingDefinition.Name = heroCraft.Name;
            existingDefinition.ImageUrl = publishedImageUrl;
            existingDefinition.UpdatedAt = DateTime.UtcNow;
            existingDefinition.PublishedAtUtc = DateTime.UtcNow;
            existingDefinition.Version += 1;
            definition = existingDefinition;
            
            // Remove old translations
            var oldTranslations = await _context.EpicHeroDefinitionTranslations
                .Where(t => t.EpicHeroDefinitionId == heroId)
                .ToListAsync(ct);
            _context.EpicHeroDefinitionTranslations.RemoveRange(oldTranslations);
        }

        // Copy translations from craft to definition
        foreach (var craftTranslation in heroCraft.Translations)
        {
            // Copy greeting audio from draft to published container if exists
            string? publishedAudioUrl = craftTranslation.GreetingAudioUrl;
            if (!string.IsNullOrWhiteSpace(craftTranslation.GreetingAudioUrl))
            {
                try
                {
                    publishedAudioUrl = await PublishGreetingAudioAsync(heroId, craftTranslation.GreetingAudioUrl, craftTranslation.LanguageCode, ownerEmail, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to publish greeting audio for heroId={HeroId} lang={Lang}, using draft path", heroId, craftTranslation.LanguageCode);
                    // Continue with draft path if publish fails (non-blocking)
                }
            }
            
            var definitionTranslation = new EpicHeroDefinitionTranslation
            {
                EpicHeroDefinitionId = definition.Id,
                LanguageCode = craftTranslation.LanguageCode,
                Name = craftTranslation.Name,
                Description = craftTranslation.Description,
                GreetingText = craftTranslation.GreetingText,
                GreetingAudioUrl = publishedAudioUrl
            };
            _context.EpicHeroDefinitionTranslations.Add(definitionTranslation);
        }

        await _context.SaveChangesAsync(ct);
        
        // Cleanup: Delete EpicHeroCraft after successful publish (similar to Epics)
        try
        {
            // Reload craft in current context to ensure it's tracked properly
            var craftToDelete = await _context.EpicHeroCrafts
                .FirstOrDefaultAsync(c => c.Id == heroId, ct);
            
            if (craftToDelete != null)
            {
                _context.EpicHeroCrafts.Remove(craftToDelete);
                await _context.SaveChangesAsync(ct);
                _logger.LogInformation("Hero published and craft cleaned up: heroId={HeroId} version={Version}", heroId, definition.Version);
            }
            else
            {
                _logger.LogWarning("EpicHeroCraft not found for cleanup: heroId={HeroId}", heroId);
            }
        }
        catch (Exception cleanupEx)
        {
            _logger.LogWarning(cleanupEx, "Failed to cleanup hero craft after publish: heroId={HeroId}, but publish succeeded", heroId);
            // Don't fail if cleanup fails - publish was successful
        }
    }

    public async Task RetractAsync(Guid ownerUserId, string heroId, CancellationToken ct = default)
    {
        var heroCraft = await _repository.GetCraftAsync(heroId, ct);
        if (heroCraft == null)
        {
            throw new InvalidOperationException($"Hero craft '{heroId}' not found");
        }

        if (heroCraft.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own hero '{heroId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(heroCraft.Status);
        if (currentStatus != StoryStatus.SentForApproval && currentStatus != StoryStatus.Approved)
        {
            throw new InvalidOperationException($"Cannot retract hero. Expected SentForApproval or Approved, got {currentStatus}");
        }

        // Clear all review-related fields and revert to Draft
        heroCraft.Status = StoryStatus.Draft.ToDb();
        heroCraft.AssignedReviewerUserId = null;
        heroCraft.ReviewNotes = null;
        heroCraft.ReviewStartedAt = null;
        heroCraft.ReviewEndedAt = null;
        if (currentStatus == StoryStatus.Approved)
        {
            heroCraft.ApprovedByUserId = null;
        }
        heroCraft.ReviewedByUserId = null;
        heroCraft.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveCraftAsync(heroCraft, ct);
        _logger.LogInformation("Hero craft retracted: heroId={HeroId}", heroId);
    }

    private async Task<string> PublishImageAsync(string heroId, string draftPath, string ownerEmail, CancellationToken ct)
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
            fileName = $"{heroId}-image.png";
        }

        // Path format: images/heroes/{heroId}/{fileName}
        var destinationPath = $"images/heroes/{heroId}/{fileName}";
        var destinationClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, destinationPath);

        _logger.LogInformation("Copying hero image from {Source} to {Destination}", normalizedPath, destinationPath);

        var sasUri = sourceClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(10));
        var operation = await destinationClient.StartCopyFromUriAsync(sasUri, cancellationToken: ct);
        await operation.WaitForCompletionAsync(cancellationToken: ct);

        return destinationPath;
    }

    private async Task<string> PublishGreetingAudioAsync(string heroId, string draftPath, string languageCode, string ownerEmail, CancellationToken ct)
    {
        var normalizedPath = NormalizeBlobPath(draftPath);
        
        // Extract the path structure from draft to preserve encoding
        // Draft path format: heroes/{encodedEmail}/{heroId}/greeting/{languageCode}/{fileName}
        // We want to preserve the exact structure, just change container from draft to published
        
        // Check if already published
        if (IsAlreadyPublished(normalizedPath))
        {
            // Path is already in published format, but verify it exists in published container
            var publishedClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, normalizedPath);
            if (await publishedClient.ExistsAsync(ct))
            {
                return normalizedPath;
            }
            _logger.LogWarning("Path marked as published but file doesn't exist in published container, will copy from draft: {Path}", normalizedPath);
        }
        
        // Verify source exists in draft container
        var sourceClient = _blobSas.GetBlobClient(_blobSas.DraftContainer, normalizedPath);
        if (!await sourceClient.ExistsAsync(ct))
        {
            throw new InvalidOperationException($"Draft greeting audio '{normalizedPath}' does not exist in draft container.");
        }
        
        // Determine destination path
        string destinationPath;
        if (normalizedPath.StartsWith("heroes/", StringComparison.OrdinalIgnoreCase))
        {
            // Path already has correct structure - reuse it as-is (preserves email encoding)
            destinationPath = normalizedPath;
        }
        else
        {
            // Fallback: build path from scratch if structure doesn't match
            var fileName = Path.GetFileName(normalizedPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = $"{heroId}-greeting-{languageCode}.mp3";
            }
            var emailEscaped = Uri.EscapeDataString(ownerEmail);
            destinationPath = $"heroes/{emailEscaped}/{heroId}/greeting/{languageCode}/{fileName}";
        }
        
        // Check if file already exists in published container (skip copy if already published)
        var destinationClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, destinationPath);
        if (await destinationClient.ExistsAsync(ct))
        {
            return destinationPath; // Already published, return the path
        }

        _logger.LogInformation("Copying hero greeting audio: {Source} → {Destination}", normalizedPath, destinationPath);

        var sasUri = sourceClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(10));
        var operation = await destinationClient.StartCopyFromUriAsync(sasUri, cancellationToken: ct);
        await operation.WaitForCompletionAsync(cancellationToken: ct);
        
        // Verify copy was successful
        if (!await destinationClient.ExistsAsync(ct))
        {
            throw new InvalidOperationException($"Failed to copy greeting audio to published container. Source: {normalizedPath}, Destination: {destinationPath}");
        }
        
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
        // Check if path is already in published container structure
        // Published paths start with "images/" or "heroes/" (for greeting audio)
        return path.StartsWith("images/", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("heroes/", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("audio/", StringComparison.OrdinalIgnoreCase);
    }

    public async Task CreateVersionFromPublishedAsync(Guid ownerUserId, string heroId, CancellationToken ct = default)
    {
        // Load published EpicHeroDefinition with all related data
        var definition = await _context.EpicHeroDefinitions
            .Include(d => d.Translations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.Id == heroId, ct);

        if (definition == null)
        {
            throw new InvalidOperationException($"Published hero '{heroId}' not found");
        }

        // Verify ownership
        if (definition.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own hero '{heroId}'");
        }

        // Check if hero is published
        if (definition.Status != "published")
        {
            throw new InvalidOperationException($"Hero '{heroId}' is not published (status: {definition.Status})");
        }

        // Check if draft already exists (use AsNoTracking to avoid tracking conflicts)
        var existingCraft = await _context.EpicHeroCrafts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == heroId, ct);
        
        if (existingCraft != null)
        {
            if (existingCraft.Status != "published")
            {
                throw new InvalidOperationException("A draft already exists for this hero. Please edit or publish it first.");
            }
            
            // If status is "published", this is a leftover craft that should have been deleted
            // Delete it now before creating the new version
            _logger.LogWarning("Found leftover published craft for heroId={HeroId}, deleting it before creating new version", heroId);
            var craftToDelete = await _context.EpicHeroCrafts.FirstOrDefaultAsync(c => c.Id == heroId, ct);
            if (craftToDelete != null)
            {
                _context.EpicHeroCrafts.Remove(craftToDelete);
                await _context.SaveChangesAsync(ct);
            }
        }

        // Create new EpicHeroCraft from EpicHeroDefinition
        var craft = new EpicHeroCraft
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
            craft.Translations.Add(new EpicHeroCraftTranslation
            {
                EpicHeroCraftId = craft.Id,
                LanguageCode = defTranslation.LanguageCode,
                Name = defTranslation.Name,
                Description = defTranslation.Description,
                GreetingText = defTranslation.GreetingText,
                GreetingAudioUrl = defTranslation.GreetingAudioUrl
            });
        }

        _context.EpicHeroCrafts.Add(craft);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Created new version from published hero: heroId={HeroId} baseVersion={BaseVersion}", heroId, definition.Version);
    }

    public async Task UnpublishAsync(Guid ownerUserId, string heroId, string reason, CancellationToken ct = default)
    {
        var definition = await _repository.GetDefinitionAsync(heroId, ct);
        if (definition == null)
        {
            throw new InvalidOperationException($"Published hero '{heroId}' not found");
        }

        if (definition.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own hero '{heroId}'");
        }

        if (definition.Status != "published" || !definition.IsActive)
        {
            throw new InvalidOperationException($"Cannot unpublish hero. Expected Published and Active, got status '{definition.Status}' and IsActive '{definition.IsActive}'");
        }

        // Mark as unpublished (destructive - will delete assets)
        definition.Status = "unpublished";
        definition.IsActive = false;
        definition.UpdatedAt = DateTime.UtcNow;

        // TODO: Create separate HeroPublicationAudits table for audit logging
        // For now, we skip audit logging since StoryPublicationAudits has FK to StoryDefinitions

        await _context.SaveChangesAsync(ct);
        
        // Extract owner email from ImageUrl and delete published assets from blob storage
        var ownerEmail = TryExtractOwnerEmail(definition);
        if (!string.IsNullOrWhiteSpace(ownerEmail))
        {
            await _assetCleanup.DeletePublishedAssetsAsync(ownerEmail, heroId, ct);
        }
        else
        {
            _logger.LogWarning("Could not determine owner email for published assets cleanup. heroId={HeroId}", heroId);
        }
        
        _logger.LogInformation("Hero unpublished and assets deleted: heroId={HeroId} reason={Reason}", heroId, reason);
    }

    private string? TryExtractOwnerEmail(EpicHeroDefinition definition)
    {
        if (string.IsNullOrWhiteSpace(definition.ImageUrl))
        {
            return null;
        }

        // ImageUrl format: images/tales-of-alchimalia/heroes/{ownerEmail}/{heroId}/...
        var parts = definition.ImageUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 5 && parts[0] == "images" && parts[1] == "tales-of-alchimalia" && parts[2] == "heroes")
        {
            return parts[3]; // ownerEmail
        }

        return null;
    }
}

