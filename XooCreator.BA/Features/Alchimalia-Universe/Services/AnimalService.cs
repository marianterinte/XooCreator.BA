using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;
using XooCreator.BA.Features.AlchimaliaUniverse.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public class AnimalService : IAnimalService
{
    private readonly IAnimalRepository _repository;
    private readonly XooDbContext _context;
    private readonly ILogger<AnimalService> _logger;

    public AnimalService(
        IAnimalRepository repository,
        XooDbContext context,
        ILogger<AnimalService> logger)
    {
        _repository = repository;
        _context = context;
        _logger = logger;
    }

    public async Task<AnimalDto> GetAsync(Guid animalId, string? languageCode = null, CancellationToken ct = default)
    {
        var animal = await _repository.GetWithTranslationsAsync(animalId, ct);
        if (animal == null)
        {
            throw new KeyNotFoundException($"Animal with Id '{animalId}' not found");
        }

        return MapToDto(animal, languageCode);
    }

    public async Task<ListAnimalsResponse> ListAsync(string? status = null, Guid? regionId = null, bool? isHybrid = null, string? search = null, string? languageCode = null, CancellationToken ct = default)
    {
        var animals = await _repository.ListAsync(status, regionId, isHybrid, search, ct);
        var totalCount = await _repository.CountAsync(status, regionId, isHybrid, ct);

        var items = animals.Select(a =>
        {
            // Get translation for selected language, or first available
            AnimalTranslation? selectedTranslation = null;
            if (!string.IsNullOrWhiteSpace(languageCode))
            {
                var normalizedLang = languageCode.ToLowerInvariant();
                selectedTranslation = a.Translations.FirstOrDefault(t => t.LanguageCode.ToLowerInvariant() == normalizedLang);
            }
            
            // Fallback to first translation if no match
            selectedTranslation ??= a.Translations.FirstOrDefault();
            
            // Get all available language codes
            var availableLanguages = a.Translations.Select(t => t.LanguageCode.ToLowerInvariant()).ToList();
            
            return new AnimalListItemDto
            {
                Id = a.Id,
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

        return new ListAnimalsResponse
        {
            Animals = items,
            TotalCount = totalCount
        };
    }

    public async Task<AnimalDto> CreateAsync(Guid userId, CreateAnimalRequest request, CancellationToken ct = default)
    {
        // Verify region exists
        var region = await _context.Regions.FirstOrDefaultAsync(r => r.Id == request.RegionId, ct);
        if (region == null)
        {
            throw new KeyNotFoundException($"Region with Id '{request.RegionId}' not found");
        }

        var animal = new Animal
        {
            Id = Guid.NewGuid(),
            Label = request.Label,
            Src = request.Src,
            IsHybrid = request.IsHybrid,
            RegionId = request.RegionId,
            Status = AlchimaliaUniverseStatus.Draft.ToDb(),
            CreatedByUserId = userId,
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Translations = new List<AnimalTranslation>
            {
                new AnimalTranslation
                {
                    Id = Guid.NewGuid(),
                    AnimalId = Guid.Empty, // Will be set after save
                    LanguageCode = request.LanguageCode.ToLowerInvariant(),
                    Label = request.TranslatedLabel
                }
            },
            SupportedParts = request.SupportedParts.Select(partKey => new AnimalPartSupport
            {
                AnimalId = Guid.Empty, // Will be set after save
                PartKey = partKey
            }).ToList()
        };

        await _repository.CreateAsync(animal, ct);
        
        // Update FK references
        foreach (var translation in animal.Translations)
        {
            translation.AnimalId = animal.Id;
        }
        foreach (var part in animal.SupportedParts)
        {
            part.AnimalId = animal.Id;
        }
        await _repository.SaveAsync(animal, ct);

        return MapToDto(animal, request.LanguageCode);
    }

    public async Task<AnimalDto> UpdateAsync(Guid userId, Guid animalId, UpdateAnimalRequest request, CancellationToken ct = default)
    {
        var animal = await _repository.GetWithTranslationsAsync(animalId, ct);
        if (animal == null)
        {
            throw new KeyNotFoundException($"Animal with Id '{animalId}' not found");
        }

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(animal.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
        {
            throw new InvalidOperationException($"Cannot update Animal in status '{currentStatus}'");
        }

        if (animal.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("Only the creator can update this Animal");
        }

        if (request.Label != null) animal.Label = request.Label;
        if (request.Src != null) animal.Src = request.Src;
        if (request.IsHybrid.HasValue) animal.IsHybrid = request.IsHybrid.Value;
        if (request.RegionId.HasValue)
        {
            var region = await _context.Regions.FirstOrDefaultAsync(r => r.Id == request.RegionId.Value, ct);
            if (region == null)
            {
                throw new KeyNotFoundException($"Region with Id '{request.RegionId}' not found");
            }
            animal.RegionId = request.RegionId.Value;
        }

        // Update supported parts
        if (request.SupportedParts != null)
        {
            // Remove existing parts
            var existingParts = animal.SupportedParts.ToList();
            foreach (var part in existingParts)
            {
                if (!request.SupportedParts.Contains(part.PartKey))
                {
                    _context.AnimalPartSupports.Remove(part);
                }
            }
            // Add new parts
            foreach (var partKey in request.SupportedParts)
            {
                if (!existingParts.Any(p => p.PartKey == partKey))
                {
                    animal.SupportedParts.Add(new AnimalPartSupport
                    {
                        AnimalId = animal.Id,
                        PartKey = partKey
                    });
                }
            }
        }

        // Update translations
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
                    animal.Translations.Add(new AnimalTranslation
                    {
                        Id = Guid.NewGuid(),
                        AnimalId = animal.Id,
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
        {
            throw new KeyNotFoundException($"Animal with Id '{animalId}' not found");
        }

        if (animal.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("Only the creator can submit this Animal");
        }

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(animal.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
        {
            throw new InvalidOperationException($"Cannot submit Animal in status '{currentStatus}'");
        }

        animal.Status = AlchimaliaUniverseStatus.SentForApproval.ToDb();
        await _repository.SaveAsync(animal, ct);
        _logger.LogInformation("Animal {AnimalId} submitted for review by user {UserId}", animalId, userId);
    }

    public async Task ReviewAsync(Guid reviewerId, Guid animalId, ReviewAnimalRequest request, CancellationToken ct = default)
    {
        var animal = await _repository.GetAsync(animalId, ct);
        if (animal == null)
        {
            throw new KeyNotFoundException($"Animal with Id '{animalId}' not found");
        }

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(animal.Status);
        if (currentStatus != AlchimaliaUniverseStatus.SentForApproval && currentStatus != AlchimaliaUniverseStatus.InReview)
        {
            throw new InvalidOperationException($"Cannot review Animal in status '{currentStatus}'");
        }

        animal.Status = request.Approve 
            ? AlchimaliaUniverseStatus.Approved.ToDb() 
            : AlchimaliaUniverseStatus.ChangesRequested.ToDb();
        animal.ReviewedByUserId = reviewerId;
        animal.ReviewNotes = request.Notes;
        await _repository.SaveAsync(animal, ct);
        _logger.LogInformation("Animal {AnimalId} reviewed by {ReviewerId}: {Decision}", animalId, reviewerId, request.Approve ? "Approved" : "ChangesRequested");
    }

    public async Task PublishAsync(Guid userId, Guid animalId, CancellationToken ct = default)
    {
        var animal = await _repository.GetAsync(animalId, ct);
        if (animal == null)
        {
            throw new KeyNotFoundException($"Animal with Id '{animalId}' not found");
        }

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(animal.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Approved)
        {
            throw new InvalidOperationException($"Cannot publish Animal in status '{currentStatus}'. Must be Approved.");
        }

        animal.Status = AlchimaliaUniverseStatus.Published.ToDb();
        await _repository.SaveAsync(animal, ct);
        _logger.LogInformation("Animal {AnimalId} published by user {UserId}", animalId, userId);
    }

    public async Task DeleteAsync(Guid userId, Guid animalId, CancellationToken ct = default)
    {
        var animal = await _repository.GetAsync(animalId, ct);
        if (animal == null)
        {
            throw new KeyNotFoundException($"Animal with Id '{animalId}' not found");
        }

        if (animal.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("Only the creator can delete this Animal");
        }

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(animal.Status);
        if (currentStatus == AlchimaliaUniverseStatus.Published)
        {
            throw new InvalidOperationException("Cannot delete published Animal");
        }

        await _repository.DeleteAsync(animalId, ct);
        _logger.LogInformation("Animal {AnimalId} deleted by user {UserId}", animalId, userId);
    }

    private AnimalDto MapToDto(Animal animal, string? preferredLanguageCode = null)
    {
        return new AnimalDto
        {
            Id = animal.Id,
            Label = animal.Label,
            Src = animal.Src,
            IsHybrid = animal.IsHybrid,
            RegionId = animal.RegionId,
            RegionName = animal.Region?.Name,
            Status = animal.Status,
            CreatedByUserId = animal.CreatedByUserId,
            ReviewedByUserId = animal.ReviewedByUserId,
            ReviewNotes = animal.ReviewNotes,
            Version = animal.Version,
            ParentVersionId = animal.ParentVersionId,
            CreatedAt = animal.CreatedAt,
            UpdatedAt = animal.UpdatedAt,
            SupportedParts = animal.SupportedParts.Select(p => p.PartKey).ToList(),
            HybridParts = new List<AnimalHybridPartDto>(),
            Translations = animal.Translations.Select(t => new AnimalTranslationDto
            {
                Id = t.Id,
                LanguageCode = t.LanguageCode,
                Label = t.Label,
                AudioUrl = null
            }).ToList()
        };
    }
}
