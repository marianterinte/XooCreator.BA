using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;
using XooCreator.BA.Features.AlchimaliaUniverse.Repositories;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public class AnimalCraftService : IAnimalCraftService
{
    private readonly IAnimalCraftRepository _repository;
    private readonly XooDbContext _db;
    private readonly ILogger<AnimalCraftService> _logger;

    public AnimalCraftService(
        IAnimalCraftRepository repository,
        XooDbContext db,
        ILogger<AnimalCraftService> logger)
    {
        _repository = repository;
        _db = db;
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
                    Label = request.TranslatedLabel
                }
            },
            SupportedParts = request.SupportedParts.Select(partKey => new AnimalCraftPartSupport
            {
                AnimalCraftId = Guid.Empty,
                BodyPartKey = partKey
            }).ToList()
        };

        await _repository.CreateAsync(animal, ct);

        foreach (var translation in animal.Translations)
            translation.AnimalCraftId = animal.Id;
        foreach (var part in animal.SupportedParts)
            part.AnimalCraftId = animal.Id;

        await _repository.SaveAsync(animal, ct);
        return MapToDto(animal, request.LanguageCode);
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

        if (request.Translations != null)
        {
            foreach (var (langCode, translationDto) in request.Translations)
            {
                var normalizedLang = langCode.ToLowerInvariant();
                var existingTranslation = animal.Translations.FirstOrDefault(t => t.LanguageCode == normalizedLang);
                if (existingTranslation != null)
                {
                    existingTranslation.Label = translationDto.Label;
                }
                else
                {
                    animal.Translations.Add(new AnimalCraftTranslation
                    {
                        Id = Guid.NewGuid(),
                        AnimalCraftId = animal.Id,
                        LanguageCode = normalizedLang,
                        Label = translationDto.Label
                    });
                }
            }
        }

        await _repository.SaveAsync(animal, ct);
        return MapToDto(animal);
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
                    Label = t.Label
                }).ToList(),
                SupportedParts = animal.SupportedParts.Select(p => new AnimalDefinitionPartSupport
                {
                    AnimalDefinitionId = definitionId,
                    BodyPartKey = p.BodyPartKey
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
                Label = t.Label
            }).ToList();

            _db.AnimalDefinitionPartSupports.RemoveRange(definition.SupportedParts);
            definition.SupportedParts = animal.SupportedParts.Select(p => new AnimalDefinitionPartSupport
            {
                AnimalDefinitionId = definitionId,
                BodyPartKey = p.BodyPartKey
            }).ToList();
        }

        animal.PublishedDefinitionId = definitionId;
        animal.Status = AlchimaliaUniverseStatus.Published.ToDb();
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("AnimalCraft {AnimalId} published by user {UserId}", animalId, publisherId);
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
            Translations = animal.Translations.Select(t => new AnimalTranslationDto
            {
                Id = t.Id,
                LanguageCode = t.LanguageCode,
                Label = t.Label
            }).ToList()
        };
    }
}
