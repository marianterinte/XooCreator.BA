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
using System.IO;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class EpicHeroService : IEpicHeroService
{
    private readonly IEpicHeroRepository _repository;
    private readonly XooDbContext _context;
    private readonly IBlobSasService _blobSas;
    private readonly IHeroPublishedAssetCleanupService _assetCleanup;
    private readonly IImageCompressionService _imageCompression;
    private readonly IHeroPublishChangeLogService _changeLogService;
    private readonly IHeroAssetLinkService _assetLinkService;
    private readonly ILogger<EpicHeroService> _logger;

    public EpicHeroService(
        IEpicHeroRepository repository,
        XooDbContext context,
        IBlobSasService blobSas,
        IHeroPublishedAssetCleanupService assetCleanup,
        IImageCompressionService imageCompression,
        IHeroPublishChangeLogService changeLogService,
        IHeroAssetLinkService assetLinkService,
        ILogger<EpicHeroService> logger)
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

        // Determine language code for change tracking (use first translation or default)
        const string defaultLang = "ro-ro";
        var langForTracking = dto.Translations?.FirstOrDefault()?.LanguageCode ?? defaultLang;
        
        // Capture snapshot before changes
        var snapshotBeforeChanges = _changeLogService.CaptureSnapshot(heroCraft, langForTracking);

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
        
        // Append changes to change log for delta publish
        await _changeLogService.AppendChangesAsync(heroCraft, snapshotBeforeChanges, langForTracking, ownerUserId, ct);
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

        // Get unique owner IDs for email lookup
        var uniqueOwnerIds = ownedCrafts.Select(c => c.OwnerUserId)
            .Concat(publishedDefinitions.Select(d => d.OwnerUserId))
            .Concat(craftsForReview.Select(c => c.OwnerUserId))
            .Distinct()
            .ToList();
        
        var ownerEmailMap = await _context.AlchimaliaUsers
            .AsNoTracking()
            .Where(u => uniqueOwnerIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Email })
            .ToDictionaryAsync(u => u.Id, u => u.Email ?? "", ct);

        var combined = new List<EpicHeroListItemDto>();

        _logger.LogInformation(
            "ListHeroesForEditor: userId={UserId} status={Status} ownedCrafts={OwnedCrafts} publishedDefinitions={Published} craftsForReview={ForReview}",
            currentUserId,
            status ?? "(null)",
            ownedCrafts.Count,
            publishedDefinitions.Count,
            craftsForReview.Count);

        // Add owned crafts
        foreach (var craft in ownedCrafts)
        {
            var ownerEmail = ownerEmailMap.TryGetValue(craft.OwnerUserId, out var email) ? email : "";
            combined.Add(MapCraftToListItem(craft, currentUserId, ownerEmail));
        }

        // Add published definitions - always include, even if draft exists
        // This allows published heroes to remain visible when "new version" creates a draft
        foreach (var definition in publishedDefinitions)
        {
            var ownerEmail = ownerEmailMap.TryGetValue(definition.OwnerUserId, out var email) ? email : "";
            combined.Add(MapDefinitionToListItem(definition, currentUserId, ownerEmail));
        }

        // Add crafts for review (avoid duplicates)
        var ownedIds = new HashSet<string>(ownedCrafts.Select(c => c.Id), StringComparer.OrdinalIgnoreCase);
        foreach (var craft in craftsForReview)
        {
            if (!ownedIds.Contains(craft.Id))
            {
                var ownerEmail = ownerEmailMap.TryGetValue(craft.OwnerUserId, out var email) ? email : "";
                combined.Add(MapCraftToListItem(craft, currentUserId, ownerEmail));
            }
        }

        return combined
            .OrderByDescending(h => h.IsOwnedByCurrentUser)
            .ThenBy(h => h.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<List<EpicHeroListItemDto>> ListAllHeroesAsync(Guid currentUserId, string? status = null, CancellationToken ct = default)
    {
        // Get all crafts - no owner filter
        var craftQuery = _context.EpicHeroCrafts.AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
        {
            craftQuery = craftQuery.Where(x => x.Status == status);
        }
        var allCrafts = await craftQuery
            .Include(x => x.Translations)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);

        // Get all published definitions - no owner filter
        var allDefinitions = await _context.EpicHeroDefinitions
            .Where(x => x.Status == "published")
            .Include(x => x.Translations)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);

        // Get unique owner IDs for email lookup
        var uniqueOwnerIds = allCrafts.Select(c => c.OwnerUserId)
            .Concat(allDefinitions.Select(d => d.OwnerUserId))
            .Distinct()
            .ToList();
        
        var ownerEmailMap = await _context.AlchimaliaUsers
            .AsNoTracking()
            .Where(u => uniqueOwnerIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Email })
            .ToDictionaryAsync(u => u.Id, u => u.Email ?? "", ct);

        var combined = new List<EpicHeroListItemDto>();

        // Add all crafts
        foreach (var craft in allCrafts)
        {
            var ownerEmail = ownerEmailMap.TryGetValue(craft.OwnerUserId, out var email) ? email : "";
            combined.Add(MapCraftToListItem(craft, currentUserId, ownerEmail));
        }

        // Add all published definitions
        foreach (var definition in allDefinitions)
        {
            var ownerEmail = ownerEmailMap.TryGetValue(definition.OwnerUserId, out var email) ? email : "";
            combined.Add(MapDefinitionToListItem(definition, currentUserId, ownerEmail));
        }

        return combined
            .OrderByDescending(h => h.IsOwnedByCurrentUser)
            .ThenBy(h => h.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private EpicHeroListItemDto MapCraftToListItem(EpicHeroCraft craft, Guid? currentUserId, string? ownerEmail = null)
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
            IsOwnedByCurrentUser = isOwnedByCurrentUser,
            OwnerEmail = ownerEmail
        };
    }

    private EpicHeroListItemDto MapDefinitionToListItem(EpicHeroDefinition definition, Guid? currentUserId, string? ownerEmail = null)
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
            IsOwnedByCurrentUser = isOwnedByCurrentUser,
            OwnerEmail = ownerEmail
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

    public async Task PublishAsync(Guid ownerUserId, string heroId, string ownerEmail, bool isAdmin = false, CancellationToken ct = default)
    {
        var heroCraft = await _repository.GetCraftAsync(heroId, ct);
        if (heroCraft == null)
        {
            throw new InvalidOperationException($"Hero craft '{heroId}' not found");
        }

        if (!isAdmin && heroCraft.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own hero '{heroId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(heroCraft.Status);
        // Admin can publish from draft, changes_requested, or approved
        if (!isAdmin && currentStatus != StoryStatus.Approved)
        {
            throw new InvalidOperationException($"Cannot publish hero. Expected Approved, got {currentStatus}");
        }
        if (isAdmin && !(currentStatus == StoryStatus.Approved || currentStatus == StoryStatus.Draft || currentStatus == StoryStatus.ChangesRequested))
        {
            throw new InvalidOperationException($"Admin cannot publish hero in status '{currentStatus}'. Expected Draft, ChangesRequested, or Approved.");
        }

        // Load craft with translations
        heroCraft = await _context.EpicHeroCrafts
            .Include(c => c.Translations)
            .FirstOrDefaultAsync(c => c.Id == heroId, ct) ?? heroCraft;

        // Check if definition already exists
        var existingDefinition = await _repository.GetDefinitionAsync(heroId, ct);
        
        var requiresFullPublish = existingDefinition == null;
        List<HeroPublishChangeLog>? pendingLogs = null;
        const string defaultLangTag = "ro-ro"; // Default language for delta publish

        if (!requiresFullPublish && existingDefinition != null)
        {
            pendingLogs = await _context.HeroPublishChangeLogs
                .Where(x => x.HeroId == heroId && x.DraftVersion > existingDefinition.LastPublishedVersion)
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
            var deltaApplied = await TryApplyDeltaPublishAsync(existingDefinition, heroCraft, pendingLogs, ownerEmail, defaultLangTag, ct);
            if (deltaApplied)
            {
                await _context.SaveChangesAsync(ct);
                await CleanupChangeLogsAsync(heroId, heroCraft.LastDraftVersion, ct);
                
                // Cleanup craft after successful publish
                await CleanupCraftAsync(heroId, ct);
                return;
            }

            requiresFullPublish = true;
        }

        // Full publish
        await ApplyFullPublishAsync(existingDefinition, heroCraft, ownerEmail, ct);
        await CleanupChangeLogsAsync(heroId, heroCraft.LastDraftVersion, ct);
        
        // Cleanup craft after successful publish
        await CleanupCraftAsync(heroId, ct);
    }

    private async Task ApplyFullPublishAsync(EpicHeroDefinition? existingDefinition, EpicHeroCraft heroCraft, string ownerEmail, CancellationToken ct)
    {
        // Sync image asset first
        await _assetLinkService.SyncImageAsync(heroCraft, ownerEmail, ct);

        // Get published image URL from asset link
        string? publishedImageUrl = null;
        if (!string.IsNullOrWhiteSpace(heroCraft.ImageUrl))
        {
            var normalizedPath = NormalizeBlobPath(heroCraft.ImageUrl);
            var assetLink = await _context.HeroAssetLinks
                .FirstOrDefaultAsync(x => x.HeroId == heroCraft.Id && x.DraftPath == normalizedPath, ct);
            
            if (assetLink != null && !string.IsNullOrWhiteSpace(assetLink.PublishedPath))
            {
                publishedImageUrl = assetLink.PublishedPath;
            }
            else if (IsAlreadyPublished(normalizedPath))
            {
                publishedImageUrl = normalizedPath;
            }
        }

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
                Version = 1,
                LastPublishedVersion = heroCraft.LastDraftVersion
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
            existingDefinition.LastPublishedVersion = heroCraft.LastDraftVersion;
            definition = existingDefinition;
            
            // Remove old translations
            var oldTranslations = await _context.EpicHeroDefinitionTranslations
                .Where(t => t.EpicHeroDefinitionId == heroCraft.Id)
                .ToListAsync(ct);
            _context.EpicHeroDefinitionTranslations.RemoveRange(oldTranslations);
        }

        // Copy translations from craft to definition
        if (heroCraft.Translations == null || heroCraft.Translations.Count == 0)
        {
            throw new InvalidOperationException("Cannot publish hero. No translations found.");
        }

        // Normalize language codes and prevent intermittent publish failures caused by duplicates (case/whitespace)
        var normalizedTranslations = heroCraft.Translations
            .Select(t => new
            {
                Translation = t,
                Lang = (t.LanguageCode ?? string.Empty).Trim().ToLowerInvariant()
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.Lang))
            .GroupBy(x => x.Lang, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        if (normalizedTranslations.Count == 0)
        {
            throw new InvalidOperationException("Cannot publish hero. All translations have empty language code.");
        }

        foreach (var item in normalizedTranslations)
        {
            var craftTranslation = item.Translation;
            var lang = item.Lang;

            // Sync greeting audio asset
            await _assetLinkService.SyncGreetingAudioAsync(heroCraft, lang, ownerEmail, ct);

            // Get published audio URL from asset link
            string? publishedAudioUrl = craftTranslation.GreetingAudioUrl;
            if (!string.IsNullOrWhiteSpace(craftTranslation.GreetingAudioUrl))
            {
                var normalizedAudioPath = NormalizeBlobPath(craftTranslation.GreetingAudioUrl);
                var audioAssetLink = await _context.HeroAssetLinks
                    .FirstOrDefaultAsync(x => x.HeroId == heroCraft.Id && 
                                             x.LanguageCode == lang && 
                                             x.EntityId == $"__greeting_{lang}__" &&
                                             x.DraftPath == normalizedAudioPath, ct);
                
                if (audioAssetLink != null && !string.IsNullOrWhiteSpace(audioAssetLink.PublishedPath))
                {
                    publishedAudioUrl = audioAssetLink.PublishedPath;
                }
                else if (IsAlreadyPublished(normalizedAudioPath))
                {
                    publishedAudioUrl = normalizedAudioPath;
                }
            }
            
            var definitionTranslation = new EpicHeroDefinitionTranslation
            {
                EpicHeroDefinitionId = definition.Id,
                LanguageCode = lang,
                Name = craftTranslation.Name,
                Description = craftTranslation.Description,
                GreetingText = craftTranslation.GreetingText,
                GreetingAudioUrl = publishedAudioUrl
            };
            _context.EpicHeroDefinitionTranslations.Add(definitionTranslation);
        }

        try
        {
            definition.IsActive = true;
            definition.Status = "published";
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to publish hero (DB update). heroId={HeroId}", heroCraft.Id);
            throw new InvalidOperationException("Failed to publish hero due to invalid/duplicate translation data. Please review the hero translations and try again.");
        }
    }

    private async Task<bool> TryApplyDeltaPublishAsync(
        EpicHeroDefinition definition,
        EpicHeroCraft craft,
        IReadOnlyCollection<HeroPublishChangeLog> changeLogs,
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
            var applied = await ApplyTranslationChangeAsync(definition, craft, change, ownerEmail, ct);
            if (!applied)
            {
                return false;
            }
        }

        definition.LastPublishedVersion = craft.LastDraftVersion;
        definition.Version = definition.Version <= 0 ? 1 : definition.Version + 1;
        definition.Status = "published";
        definition.IsActive = true;
        definition.UpdatedAt = DateTime.UtcNow;
        definition.PublishedAtUtc = DateTime.UtcNow;

        return true;
    }

    private async Task ApplyDefinitionMetadataDeltaAsync(EpicHeroDefinition definition, EpicHeroCraft craft, string ownerEmail, CancellationToken ct)
    {
        definition.Name = craft.Name;
        definition.UpdatedAt = DateTime.UtcNow;

        // Sync image asset
        await _assetLinkService.SyncImageAsync(craft, ownerEmail, ct);

        // Get published path from asset link
        if (!string.IsNullOrWhiteSpace(craft.ImageUrl))
        {
            var normalizedPath = NormalizeBlobPath(craft.ImageUrl);
            var assetLink = await _context.HeroAssetLinks
                .FirstOrDefaultAsync(x => x.HeroId == craft.Id && x.DraftPath == normalizedPath, ct);
            
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
        EpicHeroDefinition definition,
        EpicHeroCraft craft,
        HeroPublishChangeLog change,
        string ownerEmail,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(change.EntityId))
        {
            return true;
        }

        var languageCode = change.EntityId;

        if (string.Equals(change.ChangeType, "Removed", StringComparison.OrdinalIgnoreCase))
        {
            var existingTranslation = await _context.EpicHeroDefinitionTranslations
                .FirstOrDefaultAsync(t => t.EpicHeroDefinitionId == definition.Id && t.LanguageCode == languageCode, ct);
            
            if (existingTranslation != null)
            {
                _context.EpicHeroDefinitionTranslations.Remove(existingTranslation);
            }

            // Remove greeting audio asset
            await _assetLinkService.RemoveGreetingAudioAsync(definition.Id, languageCode, ct);
            
            return true;
        }

        var craftTranslation = craft.Translations.FirstOrDefault(t => 
            string.Equals(t.LanguageCode, languageCode, StringComparison.OrdinalIgnoreCase));
        
        if (craftTranslation == null)
        {
            _logger.LogWarning("Delta publish failed: heroId={HeroId} languageCode={LanguageCode} missing in craft.", 
                definition.Id, languageCode);
            return false;
        }

        // Sync greeting audio asset
        await _assetLinkService.SyncGreetingAudioAsync(craft, languageCode, ownerEmail, ct);

        // Remove existing translation if exists
        var existingTranslation2 = await _context.EpicHeroDefinitionTranslations
            .FirstOrDefaultAsync(t => t.EpicHeroDefinitionId == definition.Id && t.LanguageCode == languageCode, ct);
        
        if (existingTranslation2 != null)
        {
            _context.EpicHeroDefinitionTranslations.Remove(existingTranslation2);
        }

        // Get published audio URL from asset link
        string? publishedAudioUrl = craftTranslation.GreetingAudioUrl;
        if (!string.IsNullOrWhiteSpace(craftTranslation.GreetingAudioUrl))
        {
            var normalizedAudioPath = NormalizeBlobPath(craftTranslation.GreetingAudioUrl);
            var audioAssetLink = await _context.HeroAssetLinks
                .FirstOrDefaultAsync(x => x.HeroId == craft.Id && 
                                         x.LanguageCode == languageCode && 
                                         x.EntityId == $"__greeting_{languageCode}__" &&
                                         x.DraftPath == normalizedAudioPath, ct);
            
            if (audioAssetLink != null && !string.IsNullOrWhiteSpace(audioAssetLink.PublishedPath))
            {
                publishedAudioUrl = audioAssetLink.PublishedPath;
            }
            else if (IsAlreadyPublished(normalizedAudioPath))
            {
                publishedAudioUrl = normalizedAudioPath;
            }
        }

        // Add new translation
        _context.EpicHeroDefinitionTranslations.Add(new EpicHeroDefinitionTranslation
        {
            EpicHeroDefinitionId = definition.Id,
            LanguageCode = craftTranslation.LanguageCode,
            Name = craftTranslation.Name,
            Description = craftTranslation.Description,
            GreetingText = craftTranslation.GreetingText,
            GreetingAudioUrl = publishedAudioUrl
        });

        return true;
    }

    private async Task CleanupChangeLogsAsync(string heroId, int lastDraftVersion, CancellationToken ct)
    {
        if (lastDraftVersion <= 0)
        {
            return;
        }

        await _context.HeroPublishChangeLogs
            .Where(x => x.HeroId == heroId && x.DraftVersion <= lastDraftVersion)
            .ExecuteDeleteAsync(ct);
    }

    private async Task CleanupCraftAsync(string heroId, CancellationToken ct)
    {
        try
        {
            var craftToDelete = await _context.EpicHeroCrafts
                .FirstOrDefaultAsync(c => c.Id == heroId, ct);
            
            if (craftToDelete != null)
            {
                _context.EpicHeroCrafts.Remove(craftToDelete);
                await _context.SaveChangesAsync(ct);
                _logger.LogInformation("Hero published and craft cleaned up: heroId={HeroId}", heroId);
            }
        }
        catch (Exception cleanupEx)
        {
            _logger.LogWarning(cleanupEx, "Failed to cleanup hero craft after publish: heroId={HeroId}, but publish succeeded", heroId);
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

        // Generate s/m variants for hero image (non-blocking).
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
            _logger.LogWarning(ex, "Failed to generate hero image variants: path={Path}", destinationPath);
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

    private async Task<string> PublishGreetingAudioAsync(string heroId, string draftPath, string languageCode, string ownerEmail, CancellationToken ct)
    {
        var normalizedPath = NormalizeBlobPath(draftPath);
        
        // Draft path format: heroes/{encodedEmail}/{heroId}/greeting/{languageCode}/{fileName}
        // Published path format (NEW): audio/heroes/{heroId}/{languageCode}/{fileName}
        // We do NOT publish into "heroes/..." in the published container anymore.
        
        // If already published under the new structure, keep it
        if (normalizedPath.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
        {
            var publishedClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, normalizedPath);
            if (await publishedClient.ExistsAsync(ct))
            {
                return normalizedPath;
            }
            _logger.LogWarning("Audio path starts with audio/ but doesn't exist in published container, will copy from draft: {Path}", normalizedPath);
        }
        
        // Verify source exists in draft container
        var sourceClient = _blobSas.GetBlobClient(_blobSas.DraftContainer, normalizedPath);
        if (!await sourceClient.ExistsAsync(ct))
        {
            throw new InvalidOperationException($"Draft greeting audio '{normalizedPath}' does not exist in draft container.");
        }
        
        // Build destination path under NEW published structure
        var fileName = Path.GetFileName(normalizedPath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = $"{heroId}-greeting-{languageCode}.mp3";
        }
        var lang = string.IsNullOrWhiteSpace(languageCode) ? "en-us" : languageCode.Trim().ToLowerInvariant();
        var destinationPath = $"audio/heroes/{heroId}/{lang}/{fileName}";
        
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

    private string NormalizeBlobPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return string.Empty;

        var trimmed = path.Trim();
        if (trimmed.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(trimmed);
            trimmed = uri.AbsolutePath.TrimStart('/');
        }

        trimmed = trimmed.TrimStart('/');

        // If a full URL was provided, AbsolutePath includes the container as the first segment:
        //   {containerName}/{blobName...}
        // Our blob clients already know the container, so strip it if present.
        // This makes publishing resilient when FE stores full blob URLs.
        var draftPrefix = _blobSas.DraftContainer.Trim('/') + "/";
        var publishedPrefix = _blobSas.PublishedContainer.Trim('/') + "/";
        if (trimmed.StartsWith(draftPrefix, StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed.Substring(draftPrefix.Length);
        }
        else if (trimmed.StartsWith(publishedPrefix, StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed.Substring(publishedPrefix.Length);
        }

        return trimmed;
    }

    private static bool IsAlreadyPublished(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        // Check if path is already in published container structure
        // Published paths start with "images/" or "audio/" (or "video/").
        return path.StartsWith("images/", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("audio/", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("video/", StringComparison.OrdinalIgnoreCase);
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
        
        // Delete published assets from blob storage (new structure doesn't require owner email)
        await _assetCleanup.DeletePublishedAssetsAsync(heroId, ct);
        
        _logger.LogInformation("Hero unpublished and assets deleted: heroId={HeroId} reason={Reason}", heroId, reason);
    }

    // NOTE: We no longer encode ownerEmail in published hero asset paths.
}


