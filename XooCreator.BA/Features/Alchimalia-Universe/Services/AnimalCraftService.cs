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
    private static readonly Dictionary<string, string> LanguageAliasMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ro"] = "ro-ro",
        ["en"] = "en-us",
        ["hu"] = "hu-hu",
        ["de"] = "de-de",
        ["es"] = "es-es",
        ["it"] = "it-it"
    };

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

        var normalizedLanguage = NormalizeLanguageCode(languageCode);
        var languageBase = GetLanguageBase(normalizedLanguage);

        var items = animals.Select(a =>
        {
            var selectedTranslation = FindBestTranslation(
                a.Translations,
                normalizedLanguage,
                languageBase,
                t => t.LanguageCode);

            var availableLanguages = a.Translations
                .Select(t => NormalizeLanguageCode(t.LanguageCode))
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return new AnimalCraftListItemDto
            {
                Id = a.Id,
                PublishedDefinitionId = a.PublishedDefinitionId,
                Label = selectedTranslation?.Label ?? a.Label,
                Src = a.Src,
                IsHybrid = a.IsHybrid,
                RegionId = a.RegionId,
                RegionName = a.Region?.Name,
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

        // Verify region exists if provided
        if (request.RegionId.HasValue)
        {
            var region = await _db.Regions.FirstOrDefaultAsync(r => r.Id == request.RegionId.Value, ct);
            if (region == null)
            {
                throw new KeyNotFoundException($"Region with Id '{request.RegionId.Value}' not found");
            }
        }

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
                    LanguageCode = NormalizeLanguageCode(request.LanguageCode),
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
        return MapToDto(animal, NormalizeLanguageCode(request.LanguageCode));
    }

    public async Task<AnimalCraftDto> CreateCraftFromDefinitionAsync(Guid userId, Guid definitionId, bool allowAdminOverride = false, CancellationToken ct = default)
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

        if (existingCraft != null && !allowAdminOverride)
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
                LanguageCode = NormalizeLanguageCode(t.LanguageCode),
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

    public async Task<AnimalCraftDto> UpdateAsync(Guid userId, Guid animalId, UpdateAnimalCraftRequest request, bool allowAdminOverride = false, CancellationToken ct = default)
    {
        var animal = await _repository.GetWithTranslationsAsync(animalId, ct);
        if (animal == null)
            throw new KeyNotFoundException($"AnimalCraft with Id '{animalId}' not found");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(animal.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
            throw new InvalidOperationException($"Cannot update AnimalCraft in status '{currentStatus}'");

        if (animal.CreatedByUserId != userId && !allowAdminOverride)
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

            var newSupportedParts = request.SupportedParts.Select(partKey => new AnimalCraftPartSupport
            {
                AnimalCraftId = animal.Id,
                BodyPartKey = partKey
            }).ToList();

            animal.SupportedParts = newSupportedParts;
            // Ensure EF treats new graph nodes as Added (avoid Update() on detached graph issues)
            _db.AnimalCraftPartSupports.AddRange(newSupportedParts);
        }

        if (request.HybridParts != null)
        {
            await ValidateHybridPartsAsync(request.HybridParts, ct);
            var existingHybrid = animal.HybridParts.ToList();
            foreach (var part in existingHybrid)
                _db.AnimalHybridCraftParts.Remove(part);

            var newHybridParts = request.HybridParts.Select(part => new AnimalHybridCraftPart
            {
                Id = Guid.NewGuid(),
                AnimalCraftId = animal.Id,
                SourceAnimalId = part.SourceAnimalId,
                BodyPartKey = part.BodyPartKey,
                OrderIndex = part.OrderIndex
            }).ToList();

            animal.HybridParts = newHybridParts;
            // Ensure EF treats new graph nodes as Added (avoid Update() on detached graph issues)
            _db.AnimalHybridCraftParts.AddRange(newHybridParts);
        }

        if (request.Translations != null)
        {
            foreach (var (langCode, translationDto) in request.Translations)
            {
                var normalizedLang = NormalizeLanguageCode(langCode);
                var existingTranslation = animal.Translations.FirstOrDefault(t =>
                    NormalizeLanguageCode(t.LanguageCode) == normalizedLang);
                if (existingTranslation != null)
                {
                    existingTranslation.Label = translationDto.Label;
                    existingTranslation.AudioUrl = translationDto.AudioUrl;
                }
                else
                {
                    var newTranslation = new AnimalCraftTranslation
                    {
                        Id = Guid.NewGuid(),
                        AnimalCraftId = animal.Id,
                        LanguageCode = normalizedLang,
                        Label = translationDto.Label,
                        AudioUrl = translationDto.AudioUrl
                    };
                    animal.Translations.Add(newTranslation);
                    // Explicitly add to ensure it's tracked as Added
                    _db.AnimalCraftTranslations.Add(newTranslation);
                }
            }
        }

        // Avoid _repository.SaveAsync (Update on graph) to prevent concurrency errors on inserts
        animal.UpdatedAt = DateTime.UtcNow;
        await SaveChangesWithClientWinsAsync(ct);

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
        if (currentStatus != AlchimaliaUniverseStatus.SentForApproval && 
            currentStatus != AlchimaliaUniverseStatus.InReview &&
            currentStatus != AlchimaliaUniverseStatus.Approved)
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
        if (currentStatus != AlchimaliaUniverseStatus.Approved && currentStatus != AlchimaliaUniverseStatus.Published)
            throw new InvalidOperationException($"Cannot publish AnimalCraft in status '{currentStatus}'. Must be Approved or Published.");

        var definitionId = animal.PublishedDefinitionId ?? animal.Id;
        var definition = await _db.AnimalDefinitions
            .Include(d => d.Translations)
            .Include(d => d.SupportedParts)
            .Include(d => d.HybridParts)
            .FirstOrDefaultAsync(d => d.Id == definitionId, ct);

        if (definition == null)
        {
            // ... (keep existing creation logic) ...
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
            // ... (keep existing update logic) ...
            definition.Label = animal.Label;
            definition.Src = animal.Src;
            definition.IsHybrid = animal.IsHybrid;
            definition.RegionId = animal.RegionId;
            definition.Status = AlchimaliaUniverseStatus.Published.ToDb();
            definition.PublishedByUserId = publisherId;
            definition.PublishedAtUtc = DateTime.UtcNow;
            definition.UpdatedAt = DateTime.UtcNow;

            _db.AnimalDefinitionTranslations.RemoveRange(definition.Translations);
            definition.Translations.Clear();
            foreach (var t in animal.Translations)
            {
                definition.Translations.Add(new AnimalDefinitionTranslation
                {
                    Id = Guid.NewGuid(),
                    AnimalDefinitionId = definitionId,
                    LanguageCode = t.LanguageCode,
                    Label = t.Label,
                    AudioUrl = t.AudioUrl
                });
            }

            _db.AnimalDefinitionPartSupports.RemoveRange(definition.SupportedParts);
            definition.SupportedParts.Clear();
            foreach (var p in animal.SupportedParts)
            {
                definition.SupportedParts.Add(new AnimalDefinitionPartSupport
                {
                    AnimalDefinitionId = definitionId,
                    BodyPartKey = p.BodyPartKey
                });
            }

            _db.AnimalHybridDefinitionParts.RemoveRange(definition.HybridParts);
            definition.HybridParts.Clear();
            foreach (var p in animal.HybridParts)
            {
                definition.HybridParts.Add(new AnimalHybridDefinitionPart
                {
                    Id = Guid.NewGuid(),
                    AnimalDefinitionId = definitionId,
                    SourceAnimalId = p.SourceAnimalId,
                    BodyPartKey = p.BodyPartKey,
                    OrderIndex = p.OrderIndex
                });
            }
        }

        // Delete Craft after successful copy to Definition
        // Note: To align with Story Editor and support "New Version" flow, we delete the draft.
        if (animal.Translations != null) _db.AnimalCraftTranslations.RemoveRange(animal.Translations);
        if (animal.SupportedParts != null) _db.AnimalCraftPartSupports.RemoveRange(animal.SupportedParts);
        if (animal.HybridParts != null) _db.AnimalHybridCraftParts.RemoveRange(animal.HybridParts);
        
        _db.AnimalCrafts.Remove(animal);

        // Copy assets and update definition paths
        var creatorUser = await _db.AlchimaliaUsers.FirstOrDefaultAsync(u => u.Id == (animal.CreatedByUserId ?? Guid.Empty), ct);
        var creatorEmail = creatorUser?.Email;

        if (!string.IsNullOrWhiteSpace(creatorEmail))
        {
            try 
            {
                // ... (keep existing asset copy logic) ...
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

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency error publishing animal {AnimalId}. Attempting to resolve (Client Wins).", animalId);

            // Client Wins strategy: if an entry was already deleted, detach it; otherwise refresh OriginalValues.
            foreach (var entry in ex.Entries)
            {
                var databaseValues = await entry.GetDatabaseValuesAsync(ct);
                if (databaseValues == null)
                {
                    // In publish we intentionally delete the craft as "cleanup". If it's already deleted elsewhere,
                    // treat it as already done and continue the publish transaction.
                    if (entry.State == EntityState.Deleted)
                    {
                        entry.State = EntityState.Detached;
                        continue;
                    }

                    throw new InvalidOperationException("The entity being updated was deleted by another process.");
                }

                entry.OriginalValues.SetValues(databaseValues);
            }

            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("AnimalCraft {AnimalId} published (concurrency resolved)", animalId);
        }
        catch (DbUpdateException ex)
        {
             _logger.LogError(ex, "Database update failed during publish for animal {AnimalId}", animalId);
             // Wrap in InvalidOperationException to return 409 to client with details
             throw new InvalidOperationException($"Database update failed: {ex.InnerException?.Message ?? ex.Message}");
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Unexpected error publishing animal {AnimalId}", animalId);
             throw new InvalidOperationException($"Unexpected error: {ex.Message}");
        }

        _logger.LogInformation("AnimalCraft {AnimalId} published by user {UserId}", animalId, publisherId);
    }

    private async Task SaveChangesWithClientWinsAsync(CancellationToken ct)
    {
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency error saving AnimalCraft. Attempting to resolve (Client Wins).");

            foreach (var entry in ex.Entries)
            {
                var databaseValues = await entry.GetDatabaseValuesAsync(ct);
                if (databaseValues == null)
                {
                    // Entity deleted while we were updating it; treat as non-recoverable here.
                    throw new InvalidOperationException("The entity being updated was deleted by another process.");
                }

                entry.OriginalValues.SetValues(databaseValues);
            }

            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteAsync(Guid userId, Guid animalId, CancellationToken ct = default)
    {
        var animal = await _repository.GetAsync(animalId, ct);
        if (animal == null)
            throw new KeyNotFoundException($"AnimalCraft with Id '{animalId}' not found");

        if (animal.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("Only the creator can delete this AnimalCraft");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(animal.Status);
        
        // Allow deletion regardless of status (as requested)
        // if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested) ...

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
            RegionName = animal.Region?.Name,
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

    private static string NormalizeLanguageCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code)) return string.Empty;
        var trimmed = code.Trim().ToLowerInvariant();
        if (!trimmed.Contains('-') && LanguageAliasMap.TryGetValue(trimmed, out var normalized))
        {
            return normalized;
        }
        return trimmed;
    }

    private static string GetLanguageBase(string? code)
    {
        if (string.IsNullOrWhiteSpace(code)) return string.Empty;
        var normalized = code.Trim().ToLowerInvariant();
        var dashIndex = normalized.IndexOf('-');
        return dashIndex > 0 ? normalized[..dashIndex] : normalized;
    }

    private static T? FindBestTranslation<T>(
        IEnumerable<T>? translations,
        string normalizedLanguage,
        string languageBase,
        Func<T, string?> getLanguage)
    {
        if (translations == null) return default;
        var list = translations.ToList();
        if (list.Count == 0) return default;
        if (string.IsNullOrWhiteSpace(normalizedLanguage))
        {
            return list.FirstOrDefault();
        }

        var exact = list
            .Where(t => string.Equals(getLanguage(t)?.Trim(), normalizedLanguage, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(t => (getLanguage(t) ?? string.Empty).Contains('-'))
            .FirstOrDefault();
        if (exact != null) return exact;

        var normalized = list
            .Where(t => NormalizeLanguageCode(getLanguage(t)) == normalizedLanguage)
            .OrderByDescending(t => (getLanguage(t) ?? string.Empty).Contains('-'))
            .FirstOrDefault();
        if (normalized != null) return normalized;

        if (!string.IsNullOrWhiteSpace(languageBase))
        {
            var baseMatch = list
                .Where(t => GetLanguageBase(NormalizeLanguageCode(getLanguage(t))) == languageBase)
                .OrderByDescending(t => (getLanguage(t) ?? string.Empty).Contains('-'))
                .FirstOrDefault();
            if (baseMatch != null) return baseMatch;
        }

        return list.FirstOrDefault();
    }
}
