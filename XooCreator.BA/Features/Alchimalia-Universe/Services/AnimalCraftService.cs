using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;
using XooCreator.BA.Features.AlchimaliaUniverse.Repositories;
using XooCreator.BA.Features.AlchimaliaUniverse.Mappers;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public class AnimalCraftService : IAnimalCraftService
{
    private readonly IAnimalCraftRepository _repository;
    private readonly XooDbContext _db;
    private readonly IAnimalPublishChangeLogService _changeLogService;
    private readonly IAlchimaliaUniverseAssetCopyService _assetCopyService;
    private readonly ILogger<AnimalCraftService> _logger;

    public AnimalCraftService(
        IAnimalCraftRepository repository,
        XooDbContext db,
        IAnimalPublishChangeLogService changeLogService,
        IAlchimaliaUniverseAssetCopyService assetCopyService,
        ILogger<AnimalCraftService> logger)
    {
        _repository = repository;
        _db = db;
        _changeLogService = changeLogService;
        _assetCopyService = assetCopyService;
        _logger = logger;
    }

    public async Task<AnimalCraftDto> GetAsync(Guid animalId, string? languageCode = null, CancellationToken ct = default)
    {
        var animal = await _repository.GetWithTranslationsAsync(animalId, ct);
        if (animal == null)
            throw new KeyNotFoundException($"AnimalCraft with Id '{animalId}' not found");

        return MapToDto(animal, languageCode);
    }

    public async Task<ListAnimalCraftsResponse> ListAsync(string? status = null, Guid? regionId = null, bool? isHybrid = null, string? search = null, string? languageCode = null, CancellationToken ct = default)
    {
        var animals = await _repository.ListAsync(status, regionId, isHybrid, search, ct);
        var totalCount = await _repository.CountAsync(status, regionId, ct);

        var items = animals.Select(a =>
        {
            AnimalCraftTranslation? selectedTranslation = null;
            if (!string.IsNullOrWhiteSpace(languageCode))
            {
                var normalizedLang = languageCode.ToLowerInvariant();
                selectedTranslation = a.Translations.FirstOrDefault(t => t.LanguageCode.ToLowerInvariant() == normalizedLang);
            }
            selectedTranslation ??= a.Translations.FirstOrDefault();

            var availableLanguages = a.Translations.Select(t => t.LanguageCode.ToLowerInvariant()).ToList();

            return new AnimalCraftListItemDto
            {
                Id = a.Id,
                PublishedDefinitionId = a.PublishedDefinitionId,
                Label = selectedTranslation?.Label ?? a.Label,
                Src = a.Src,
                IsHybrid = a.IsHybrid,
                RegionId = a.RegionId,
                RegionName = null,
                Status = a.Status,
                UpdatedAt = a.UpdatedAt,
                CreatedByUserId = a.CreatedByUserId,
                AvailableLanguages = availableLanguages
            };
        }).ToList();

        return new ListAnimalCraftsResponse
        {
            Animals = items,
            TotalCount = totalCount
        };
    }

    public async Task<AnimalCraftDto> CreateAsync(Guid userId, CreateAnimalCraftRequest request, CancellationToken ct = default)
    {
        await ValidateHybridPartsAsync(request.HybridParts, ct);

        var animal = new AnimalCraft
        {
            Id = Guid.NewGuid(),
            Label = request.Label,
            Src = request.Src,
            IsHybrid = request.IsHybrid,
            RegionId = request.RegionId,
            Status = AlchimaliaUniverseStatus.Draft.ToDb(),
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Translations = new List<AnimalCraftTranslation>
            {
                new AnimalCraftTranslation
                {
                    Id = Guid.NewGuid(),
                    AnimalCraftId = Guid.Empty, // set after save
                    LanguageCode = request.LanguageCode.ToLowerInvariant(),
                    Label = request.TranslatedLabel,
                    AudioUrl = null
                }
            },
            SupportedParts = request.SupportedParts.Select(partKey => new AnimalCraftPartSupport
            {
                AnimalCraftId = Guid.Empty,
                BodyPartKey = partKey
            }).ToList(),
            HybridParts = request.HybridParts.Select(part => new AnimalHybridCraftPart
            {
                Id = Guid.NewGuid(),
                AnimalCraftId = Guid.Empty,
                SourceAnimalId = part.SourceAnimalId,
                BodyPartKey = part.BodyPartKey,
                OrderIndex = part.OrderIndex
            }).ToList()
        };

        await _repository.CreateAsync(animal, ct);

        foreach (var translation in animal.Translations)
            translation.AnimalCraftId = animal.Id;
        foreach (var part in animal.SupportedParts)
            part.AnimalCraftId = animal.Id;
        foreach (var hybrid in animal.HybridParts)
            hybrid.AnimalCraftId = animal.Id;

        await _repository.SaveAsync(animal, ct);
        return MapToDto(animal, request.LanguageCode);
    }

    public async Task<AnimalCraftDto> CreateCraftFromDefinitionAsync(Guid userId, Guid definitionId, CancellationToken ct = default)
    {
        var definition = await _db.AnimalDefinitions
            .Include(d => d.Translations)
            .Include(d => d.SupportedParts)
            .Include(d => d.HybridParts)
            .FirstOrDefaultAsync(d => d.Id == definitionId, ct);

        if (definition == null)
            throw new KeyNotFoundException($"AnimalDefinition with Id '{definitionId}' not found");

        // Check if there's already a draft craft for this definition
        var existingCraft = await _db.AnimalCrafts
            .FirstOrDefaultAsync(c => c.PublishedDefinitionId == definitionId && 
                (c.Status == AlchimaliaUniverseStatus.Draft.ToDb() || 
                 c.Status == AlchimaliaUniverseStatus.ChangesRequested.ToDb()), ct);

        if (existingCraft != null)
        {
            // Return existing draft instead of creating a new one
            return await GetAsync(existingCraft.Id, null, ct);
        }

        // Validate hybrid parts if this is a hybrid
        if (definition.IsHybrid && definition.HybridParts.Any())
        {
            var hybridParts = definition.HybridParts.Select(p => new AnimalHybridPartDto
            {
                SourceAnimalId = p.SourceAnimalId,
                BodyPartKey = p.BodyPartKey,
                OrderIndex = p.OrderIndex
            }).ToList();
            await ValidateHybridPartsAsync(hybridParts, ct);
        }

        var animal = new AnimalCraft
        {
            Id = Guid.NewGuid(),
            PublishedDefinitionId = definitionId,
            Label = definition.Label,
            Src = definition.Src,
            IsHybrid = definition.IsHybrid,
            RegionId = definition.RegionId,
            Status = AlchimaliaUniverseStatus.Draft.ToDb(),
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Translations = definition.Translations.Select(t => new AnimalCraftTranslation
            {
                Id = Guid.NewGuid(),
                AnimalCraftId = Guid.Empty, // set after save
                LanguageCode = t.LanguageCode,
                Label = t.Label,
                AudioUrl = t.AudioUrl
            }).ToList(),
            SupportedParts = definition.SupportedParts.Select(p => new AnimalCraftPartSupport
            {
                AnimalCraftId = Guid.Empty, // set after save
                BodyPartKey = p.BodyPartKey
            }).ToList(),
            HybridParts = definition.HybridParts.Select(p => new AnimalHybridCraftPart
            {
                Id = Guid.NewGuid(),
                AnimalCraftId = Guid.Empty, // set after save
                SourceAnimalId = p.SourceAnimalId,
                BodyPartKey = p.BodyPartKey,
                OrderIndex = p.OrderIndex
            }).ToList()
        };

        await _repository.CreateAsync(animal, ct);

        foreach (var translation in animal.Translations)
            translation.AnimalCraftId = animal.Id;
        foreach (var part in animal.SupportedParts)
            part.AnimalCraftId = animal.Id;
        foreach (var hybrid in animal.HybridParts)
            hybrid.AnimalCraftId = animal.Id;

        await _repository.SaveAsync(animal, ct);
        _logger.LogInformation("AnimalCraft {AnimalId} created from definition {DefinitionId} by user {UserId}", animal.Id, definitionId, userId);
        return MapToDto(animal);
    }

    public async Task<AnimalCraftDto> UpdateAsync(Guid userId, Guid animalId, UpdateAnimalCraftRequest request, CancellationToken ct = default)
    {
        var animal = await _repository.GetWithTranslationsAsync(animalId, ct);
        if (animal == null)
            throw new KeyNotFoundException($"AnimalCraft with Id '{animalId}' not found");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(animal.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
            throw new InvalidOperationException($"Cannot update AnimalCraft in status '{currentStatus}'");

        if (animal.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("Only the creator can update this AnimalCraft");

        // Change Log Tracking
        var langForTracking = request.Translations?.Keys.FirstOrDefault() ?? "ro-ro";
        var snapshotBeforeChanges = _changeLogService.CaptureSnapshot(animal, langForTracking);

        if (request.Label != null) animal.Label = request.Label;
        if (request.Src != null) animal.Src = request.Src;
        if (request.IsHybrid.HasValue) animal.IsHybrid = request.IsHybrid.Value;
        if (request.RegionId.HasValue) animal.RegionId = request.RegionId.Value;

        if (request.SupportedParts != null)
        {
            var existingParts = animal.SupportedParts.ToList();
            foreach (var part in existingParts)
                _db.AnimalCraftPartSupports.Remove(part);

            animal.SupportedParts = request.SupportedParts.Select(partKey => new AnimalCraftPartSupport
            {
                AnimalCraftId = animal.Id,
                BodyPartKey = partKey
            }).ToList();
        }

        if (request.HybridParts != null)
        {
            await ValidateHybridPartsAsync(request.HybridParts, ct);
            var existingHybrid = animal.HybridParts.ToList();
            foreach (var part in existingHybrid)
                _db.AnimalHybridCraftParts.Remove(part);

            animal.HybridParts = request.HybridParts.Select(part => new AnimalHybridCraftPart
            {
                Id = Guid.NewGuid(),
                AnimalCraftId = animal.Id,
                SourceAnimalId = part.SourceAnimalId,
                BodyPartKey = part.BodyPartKey,
                OrderIndex = part.OrderIndex
            }).ToList();
        }

        if (request.Translations != null)
        {
            foreach (var (langCode, translationDto) in request.Translations)
            {
                var normalizedLang = langCode.ToLowerInvariant();
                var existingTranslation = animal.Translations.FirstOrDefault(t => t.LanguageCode == normalizedLang);
                if (existingTranslation != null)
                {
                    existingTranslation.Label = translationDto.Label;
                    existingTranslation.AudioUrl = translationDto.AudioUrl;
                }
                else
                {
                    animal.Translations.Add(new AnimalCraftTranslation
                    {
                        Id = Guid.NewGuid(),
                        AnimalCraftId = animal.Id,
                        LanguageCode = normalizedLang,
                        Label = translationDto.Label,
                        AudioUrl = translationDto.AudioUrl
                    });
                }
            }
        }

        await _repository.SaveAsync(animal, ct);

        // Append changes after save
        await _changeLogService.AppendChangesAsync(animal, snapshotBeforeChanges, langForTracking, userId, ct);

        return MapToDto(animal);
    }

    private async Task ValidateHybridPartsAsync(List<AnimalHybridPartDto> parts, CancellationToken ct)
    {
        if (parts.Count == 0) return;

        var sourceIds = parts.Select(p => p.SourceAnimalId).Distinct().ToList();
        var validSources = await _db.AnimalDefinitions
            .Where(a => sourceIds.Contains(a.Id) && !a.IsHybrid && a.Status == AlchimaliaUniverseStatus.Published.ToDb())
            .Select(a => a.Id)
            .ToListAsync(ct);

        var missing = sourceIds.Except(validSources).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("Hybrid parts must reference published base animals only.");
    }

    public async Task SubmitForReviewAsync(Guid userId, Guid animalId, CancellationToken ct = default)
    {
        var animal = await _repository.GetAsync(animalId, ct);
        if (animal == null)
            throw new KeyNotFoundException($"AnimalCraft with Id '{animalId}' not found");

        if (animal.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("Only the creator can submit this AnimalCraft");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(animal.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
            throw new InvalidOperationException($"Cannot submit AnimalCraft in status '{currentStatus}'");

        animal.Status = AlchimaliaUniverseStatus.SentForApproval.ToDb();
        await _repository.SaveAsync(animal, ct);
        _logger.LogInformation("AnimalCraft {AnimalId} submitted for review by user {UserId}", animalId, userId);
    }

    public async Task ClaimAsync(Guid reviewerId, Guid animalId, CancellationToken ct = default)
    {
        var animal = await _repository.GetAsync(animalId, ct);
        if (animal == null)
            throw new KeyNotFoundException($"AnimalCraft with Id '{animalId}' not found");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(animal.Status);
        if (currentStatus != AlchimaliaUniverseStatus.SentForApproval)
            throw new InvalidOperationException($"Cannot claim AnimalCraft in status '{currentStatus}'. Must be SentForApproval.");

        animal.Status = AlchimaliaUniverseStatus.InReview.ToDb();
        animal.AssignedReviewerUserId = reviewerId;
        animal.ReviewStartedAt = DateTime.UtcNow;

        await _repository.SaveAsync(animal, ct);
        _logger.LogInformation("AnimalCraft {AnimalId} claimed for review by {ReviewerId}", animalId, reviewerId);
    }

    public async Task RetractAsync(Guid userId, Guid animalId, CancellationToken ct = default)
    {
        var animal = await _repository.GetAsync(animalId, ct);
        if (animal == null)
            throw new KeyNotFoundException($"AnimalCraft with Id '{animalId}' not found");

        if (animal.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("Only the creator can retract this AnimalCraft");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(animal.Status);
        if (currentStatus != AlchimaliaUniverseStatus.SentForApproval && currentStatus != AlchimaliaUniverseStatus.InReview)
            throw new InvalidOperationException($"Cannot retract AnimalCraft in status '{currentStatus}'");

        animal.Status = AlchimaliaUniverseStatus.Draft.ToDb();
        animal.AssignedReviewerUserId = null;
        animal.ReviewStartedAt = null;
        animal.ReviewEndedAt = null;
        animal.ReviewedByUserId = null;
        animal.ApprovedByUserId = null;

        await _repository.SaveAsync(animal, ct);
        _logger.LogInformation("AnimalCraft {AnimalId} retracted by user {UserId}", animalId, userId);
    }

    public async Task ReviewAsync(Guid reviewerId, Guid animalId, ReviewAnimalCraftRequest request, CancellationToken ct = default)
    {
        var animal = await _repository.GetAsync(animalId, ct);
        if (animal == null)
            throw new KeyNotFoundException($"AnimalCraft with Id '{animalId}' not found");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(animal.Status);
        if (currentStatus != AlchimaliaUniverseStatus.SentForApproval && currentStatus != AlchimaliaUniverseStatus.InReview)
            throw new InvalidOperationException($"Cannot review AnimalCraft in status '{currentStatus}'");

        animal.Status = request.Approve
            ? AlchimaliaUniverseStatus.Approved.ToDb()
            : AlchimaliaUniverseStatus.ChangesRequested.ToDb();
        animal.ReviewedByUserId = reviewerId;
        animal.ReviewNotes = request.Notes;
        await _repository.SaveAsync(animal, ct);
        _logger.LogInformation("AnimalCraft {AnimalId} reviewed by {ReviewerId}: {Decision}", animalId, reviewerId, request.Approve ? "Approved" : "ChangesRequested");
    }

    public async Task PublishAsync(Guid publisherId, Guid animalId, CancellationToken ct = default)
    {
        var animal = await _repository.GetWithTranslationsAsync(animalId, ct);
        if (animal == null)
            throw new KeyNotFoundException($"AnimalCraft with Id '{animalId}' not found");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(animal.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Approved)
            throw new InvalidOperationException($"Cannot publish AnimalCraft in status '{currentStatus}'. Must be Approved.");

        var definitionId = animal.PublishedDefinitionId ?? animal.Id;
        var definition = await _db.AnimalDefinitions
            .Include(d => d.Translations)
            .Include(d => d.SupportedParts)
            .Include(d => d.HybridParts)
            .FirstOrDefaultAsync(d => d.Id == definitionId, ct);

        if (definition == null)
        {
            definition = new AnimalDefinition
            {
                Id = definitionId,
                Label = animal.Label,
                Src = animal.Src,
                IsHybrid = animal.IsHybrid,
                RegionId = animal.RegionId,
                Status = AlchimaliaUniverseStatus.Published.ToDb(),
                PublishedByUserId = publisherId,
                PublishedAtUtc = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Translations = animal.Translations.Select(t => new AnimalDefinitionTranslation
                {
                    Id = Guid.NewGuid(),
                    AnimalDefinitionId = definitionId,
                    LanguageCode = t.LanguageCode,
                    Label = t.Label,
                    AudioUrl = t.AudioUrl
                }).ToList(),
                SupportedParts = animal.SupportedParts.Select(p => new AnimalDefinitionPartSupport
                {
                    AnimalDefinitionId = definitionId,
                    BodyPartKey = p.BodyPartKey
                }).ToList(),
                HybridParts = animal.HybridParts.Select(p => new AnimalHybridDefinitionPart
                {
                    Id = Guid.NewGuid(),
                    AnimalDefinitionId = definitionId,
                    SourceAnimalId = p.SourceAnimalId,
                    BodyPartKey = p.BodyPartKey,
                    OrderIndex = p.OrderIndex
                }).ToList()
            };
            _db.AnimalDefinitions.Add(definition);
        }
        else
        {
            definition.Label = animal.Label;
            definition.Src = animal.Src;
            definition.IsHybrid = animal.IsHybrid;
            definition.RegionId = animal.RegionId;
            definition.Status = AlchimaliaUniverseStatus.Published.ToDb();
            definition.PublishedByUserId = publisherId;
            definition.PublishedAtUtc = DateTime.UtcNow;
            definition.UpdatedAt = DateTime.UtcNow;

            _db.AnimalDefinitionTranslations.RemoveRange(definition.Translations);
            definition.Translations = animal.Translations.Select(t => new AnimalDefinitionTranslation
            {
                Id = Guid.NewGuid(),
                AnimalDefinitionId = definitionId,
                LanguageCode = t.LanguageCode,
                Label = t.Label,
                AudioUrl = t.AudioUrl
            }).ToList();

            _db.AnimalDefinitionPartSupports.RemoveRange(definition.SupportedParts);
            definition.SupportedParts = animal.SupportedParts.Select(p => new AnimalDefinitionPartSupport
            {
                AnimalDefinitionId = definitionId,
                BodyPartKey = p.BodyPartKey
            }).ToList();

            _db.AnimalHybridDefinitionParts.RemoveRange(definition.HybridParts);
            definition.HybridParts = animal.HybridParts.Select(p => new AnimalHybridDefinitionPart
            {
                Id = Guid.NewGuid(),
                AnimalDefinitionId = definitionId,
                SourceAnimalId = p.SourceAnimalId,
                BodyPartKey = p.BodyPartKey,
                OrderIndex = p.OrderIndex
            }).ToList();
        }

        animal.PublishedDefinitionId = definitionId;
        animal.Status = AlchimaliaUniverseStatus.Published.ToDb();

        // Copy assets and update definition paths
        var creatorUser = await _db.AlchimaliaUsers.FirstOrDefaultAsync(u => u.Id == (animal.CreatedByUserId ?? Guid.Empty), ct);
        var creatorEmail = creatorUser?.Email;

        if (!string.IsNullOrWhiteSpace(creatorEmail))
        {
            try 
            {
                var assets = AlchimaliaUniverseAssetPathMapper.CollectFromAnimalCraft(animal);
                await _assetCopyService.CopyDraftToPublishedAsync(
                    assets,
                    creatorEmail,
                    animal.Id.ToString(), 
                    AlchimaliaUniverseAssetPathMapper.EntityType.Animal,
                    ct);

                // Update definition with published URLs
                if (!string.IsNullOrWhiteSpace(animal.Src))
                {
                    var filename = Path.GetFileName(animal.Src);
                    var assetInfo = new AlchimaliaUniverseAssetPathMapper.AssetInfo(filename, AlchimaliaUniverseAssetPathMapper.AssetType.Image, null);
                    var pubPath = AlchimaliaUniverseAssetPathMapper.BuildPublishedPath(assetInfo, creatorEmail, animal.Id.ToString(), AlchimaliaUniverseAssetPathMapper.EntityType.Animal);
                    definition.Src = _assetCopyService.GetPublishedUrl(pubPath);
                }

                foreach (var t in animal.Translations)
                {
                    if (!string.IsNullOrWhiteSpace(t.AudioUrl))
                    {
                        var filename = Path.GetFileName(t.AudioUrl);
                        var assetInfo = new AlchimaliaUniverseAssetPathMapper.AssetInfo(filename, AlchimaliaUniverseAssetPathMapper.AssetType.Audio, t.LanguageCode);
                        var pubPath = AlchimaliaUniverseAssetPathMapper.BuildPublishedPath(assetInfo, creatorEmail, animal.Id.ToString(), AlchimaliaUniverseAssetPathMapper.EntityType.Animal);
                        var pubUrl = _assetCopyService.GetPublishedUrl(pubPath);
                        
                        var defTrans = definition.Translations.FirstOrDefault(dt => dt.LanguageCode == t.LanguageCode);
                        if (defTrans != null)
                        {
                            defTrans.AudioUrl = pubUrl;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Asset copy failed during publish for Animal {AnimalId}", animal.Id);
            }
        }

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("AnimalCraft {AnimalId} published by user {UserId}", animalId, publisherId);
    }

    public async Task DeleteAsync(Guid userId, Guid animalId, CancellationToken ct = default)
    {
        var animal = await _repository.GetAsync(animalId, ct);
        if (animal == null)
            throw new KeyNotFoundException($"AnimalCraft with Id '{animalId}' not found");

        if (animal.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("Only the creator can delete this AnimalCraft");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(animal.Status);
        
        // Only allow deletion of draft or changes_requested animals
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
        {
            var statusName = currentStatus.ToString();
            if (currentStatus == AlchimaliaUniverseStatus.SentForApproval || 
                currentStatus == AlchimaliaUniverseStatus.InReview || 
                currentStatus == AlchimaliaUniverseStatus.Approved)
            {
                throw new InvalidOperationException($"Cannot delete AnimalCraft '{animalId}' while it is in '{statusName}' status. Please retract it first to move it back to Draft.");
            }
            throw new InvalidOperationException($"Cannot delete AnimalCraft '{animalId}' in '{statusName}' status.");
        }

        // Delete draft assets from Azure Storage before deleting from database
        var creatorUser = await _db.AlchimaliaUsers.FirstOrDefaultAsync(u => u.Id == userId, ct);
        var creatorEmail = creatorUser?.Email;
        
        if (!string.IsNullOrWhiteSpace(creatorEmail))
        {
            await _assetCopyService.DeleteDraftAssetsAsync(creatorEmail, animalId.ToString(), AlchimaliaUniverseAssetPathMapper.EntityType.Animal, ct);
        }

        // Delete draft from database
        await _repository.DeleteAsync(animalId, ct);
        
        _logger.LogInformation("AnimalCraft {AnimalId} deleted by user {UserId}", animalId, userId);
    }

    private AnimalCraftDto MapToDto(AnimalCraft animal, string? preferredLanguageCode = null)
    {
        return new AnimalCraftDto
        {
            Id = animal.Id,
            PublishedDefinitionId = animal.PublishedDefinitionId,
            Label = animal.Label,
            Src = animal.Src,
            IsHybrid = animal.IsHybrid,
            RegionId = animal.RegionId,
            RegionName = null,
            Status = animal.Status,
            CreatedByUserId = animal.CreatedByUserId,
            ReviewedByUserId = animal.ReviewedByUserId,
            ReviewNotes = animal.ReviewNotes,
            CreatedAt = animal.CreatedAt,
            UpdatedAt = animal.UpdatedAt,
            SupportedParts = animal.SupportedParts.Select(p => p.BodyPartKey).ToList(),
            HybridParts = animal.HybridParts.Select(p => new AnimalHybridPartDto
            {
                SourceAnimalId = p.SourceAnimalId,
                BodyPartKey = p.BodyPartKey,
                OrderIndex = p.OrderIndex
            }).ToList(),
            Translations = animal.Translations.Select(t => new AnimalTranslationDto
            {
                Id = t.Id,
                LanguageCode = t.LanguageCode,
                Label = t.Label,
                AudioUrl = t.AudioUrl
            }).ToList()
        };
    }
}
