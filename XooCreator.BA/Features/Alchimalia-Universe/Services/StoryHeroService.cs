using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;
using XooCreator.BA.Features.AlchimaliaUniverse.Repositories;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public class StoryHeroService : IStoryHeroService
{
    private readonly IStoryHeroRepository _repository;
    private readonly ILogger<StoryHeroService> _logger;

    public StoryHeroService(
        IStoryHeroRepository repository,
        ILogger<StoryHeroService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<StoryHeroDto> GetAsync(Guid storyHeroId, string? languageCode = null, CancellationToken ct = default)
    {
        var storyHero = await _repository.GetWithTranslationsAsync(storyHeroId, ct);
        if (storyHero == null)
        {
            throw new KeyNotFoundException($"StoryHero with Id '{storyHeroId}' not found");
        }

        return MapToDto(storyHero, languageCode);
    }

    public async Task<StoryHeroDto> GetByHeroIdAsync(string heroId, string? languageCode = null, CancellationToken ct = default)
    {
        var storyHero = await _repository.GetByHeroIdAsync(heroId, ct);
        if (storyHero == null)
        {
            throw new KeyNotFoundException($"StoryHero with HeroId '{heroId}' not found");
        }

        var fullHero = await _repository.GetWithTranslationsAsync(storyHero.Id, ct);
        return MapToDto(fullHero!, languageCode);
    }

    public async Task<ListStoryHeroesResponse> ListAsync(string? status = null, string? search = null, string? languageCode = null, CancellationToken ct = default)
    {
        var storyHeroes = await _repository.ListAsync(status, search, ct);
        var totalCount = await _repository.CountAsync(status, ct);

        var items = storyHeroes.Select(sh =>
        {
            // Get translation for selected language, or first available
            StoryHeroTranslation? selectedTranslation = null;
            if (!string.IsNullOrWhiteSpace(languageCode))
            {
                var normalizedLang = languageCode.ToLowerInvariant();
                selectedTranslation = sh.Translations.FirstOrDefault(t => t.LanguageCode.ToLowerInvariant() == normalizedLang);
            }
            
            // Fallback to first translation if no match
            selectedTranslation ??= sh.Translations.FirstOrDefault();
            
            // Get all available language codes
            var availableLanguages = sh.Translations.Select(t => t.LanguageCode.ToLowerInvariant()).ToList();
            
            return new StoryHeroListItemDto
            {
                Id = sh.Id,
                HeroId = sh.HeroId,
                Name = selectedTranslation?.Name ?? sh.HeroId,
                ImageUrl = sh.ImageUrl,
                Status = sh.Status,
                UpdatedAt = sh.UpdatedAt,
                CreatedByUserId = sh.CreatedByUserId,
                AvailableLanguages = availableLanguages
            };
        }).ToList();

        return new ListStoryHeroesResponse
        {
            Heroes = items,
            TotalCount = totalCount
        };
    }

    public async Task<StoryHeroDto> CreateAsync(Guid userId, CreateStoryHeroRequest request, CancellationToken ct = default)
    {
        // Check if heroId already exists
        var existing = await _repository.GetByHeroIdAsync(request.HeroId, ct);
        if (existing != null)
        {
            throw new InvalidOperationException($"StoryHero with HeroId '{request.HeroId}' already exists");
        }

        var storyHero = new StoryHero
        {
            Id = Guid.NewGuid(),
            HeroId = request.HeroId,
            ImageUrl = request.ImageUrl,
            UnlockConditionsJson = request.UnlockConditionsJson ?? "{}",
            Status = AlchimaliaUniverseStatus.Draft.ToDb(),
            CreatedByUserId = userId,
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Translations = new List<StoryHeroTranslation>
            {
                new StoryHeroTranslation
                {
                    Id = Guid.NewGuid(),
                    StoryHeroId = Guid.Empty, // Will be set after save
                    LanguageCode = request.LanguageCode.ToLowerInvariant(),
                    Name = request.Name,
                    Description = request.Description,
                    GreetingText = request.GreetingText,
                    GreetingAudioUrl = request.GreetingAudioUrl
                }
            }
        };

        await _repository.CreateAsync(storyHero, ct);
        
        // Update FK references
        foreach (var translation in storyHero.Translations)
        {
            translation.StoryHeroId = storyHero.Id;
        }
        await _repository.SaveAsync(storyHero, ct);

        return MapToDto(storyHero, request.LanguageCode);
    }

    public async Task<StoryHeroDto> UpdateAsync(Guid userId, Guid storyHeroId, UpdateStoryHeroRequest request, CancellationToken ct = default)
    {
        var storyHero = await _repository.GetWithTranslationsAsync(storyHeroId, ct);
        if (storyHero == null)
        {
            throw new KeyNotFoundException($"StoryHero with Id '{storyHeroId}' not found");
        }

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(storyHero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
        {
            throw new InvalidOperationException($"Cannot update StoryHero in status '{currentStatus}'");
        }

        if (storyHero.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("Only the creator can update this StoryHero");
        }

        if (request.ImageUrl != null) storyHero.ImageUrl = request.ImageUrl;
        if (request.UnlockConditionsJson != null) storyHero.UnlockConditionsJson = request.UnlockConditionsJson;

        // Update translations
        if (request.Translations != null)
        {
            foreach (var (langCode, translationDto) in request.Translations)
            {
                var normalizedLang = langCode.ToLowerInvariant();
                var existingTranslation = storyHero.Translations.FirstOrDefault(t => t.LanguageCode == normalizedLang);
                
                if (existingTranslation != null)
                {
                    existingTranslation.Name = translationDto.Name;
                    existingTranslation.Description = translationDto.Description;
                    existingTranslation.GreetingText = translationDto.GreetingText;
                    existingTranslation.GreetingAudioUrl = translationDto.GreetingAudioUrl;
                }
                else
                {
                    storyHero.Translations.Add(new StoryHeroTranslation
                    {
                        Id = Guid.NewGuid(),
                        StoryHeroId = storyHero.Id,
                        LanguageCode = normalizedLang,
                        Name = translationDto.Name,
                        Description = translationDto.Description,
                        GreetingText = translationDto.GreetingText,
                        GreetingAudioUrl = translationDto.GreetingAudioUrl
                    });
                }
            }
        }

        await _repository.SaveAsync(storyHero, ct);
        return MapToDto(storyHero);
    }

    public async Task SubmitForReviewAsync(Guid userId, Guid storyHeroId, CancellationToken ct = default)
    {
        var storyHero = await _repository.GetAsync(storyHeroId, ct);
        if (storyHero == null)
        {
            throw new KeyNotFoundException($"StoryHero with Id '{storyHeroId}' not found");
        }

        if (storyHero.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("Only the creator can submit this StoryHero");
        }

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(storyHero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
        {
            throw new InvalidOperationException($"Cannot submit StoryHero in status '{currentStatus}'");
        }

        storyHero.Status = AlchimaliaUniverseStatus.SentForApproval.ToDb();
        await _repository.SaveAsync(storyHero, ct);
        _logger.LogInformation("StoryHero {StoryHeroId} submitted for review by user {UserId}", storyHeroId, userId);
    }

    public async Task ReviewAsync(Guid reviewerId, Guid storyHeroId, ReviewStoryHeroRequest request, CancellationToken ct = default)
    {
        var storyHero = await _repository.GetAsync(storyHeroId, ct);
        if (storyHero == null)
        {
            throw new KeyNotFoundException($"StoryHero with Id '{storyHeroId}' not found");
        }

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(storyHero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.SentForApproval && currentStatus != AlchimaliaUniverseStatus.InReview)
        {
            throw new InvalidOperationException($"Cannot review StoryHero in status '{currentStatus}'");
        }

        storyHero.Status = request.Approve 
            ? AlchimaliaUniverseStatus.Approved.ToDb() 
            : AlchimaliaUniverseStatus.ChangesRequested.ToDb();
        storyHero.ReviewedByUserId = reviewerId;
        storyHero.ReviewNotes = request.Notes;
        await _repository.SaveAsync(storyHero, ct);
        _logger.LogInformation("StoryHero {StoryHeroId} reviewed by {ReviewerId}: {Decision}", storyHeroId, reviewerId, request.Approve ? "Approved" : "ChangesRequested");
    }

    public async Task PublishAsync(Guid userId, Guid storyHeroId, CancellationToken ct = default)
    {
        var storyHero = await _repository.GetAsync(storyHeroId, ct);
        if (storyHero == null)
        {
            throw new KeyNotFoundException($"StoryHero with Id '{storyHeroId}' not found");
        }

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(storyHero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Approved)
        {
            throw new InvalidOperationException($"Cannot publish StoryHero in status '{currentStatus}'. Must be Approved.");
        }

        storyHero.Status = AlchimaliaUniverseStatus.Published.ToDb();
        await _repository.SaveAsync(storyHero, ct);
        _logger.LogInformation("StoryHero {StoryHeroId} published by user {UserId}", storyHeroId, userId);
    }

    public async Task DeleteAsync(Guid userId, Guid storyHeroId, CancellationToken ct = default)
    {
        var storyHero = await _repository.GetAsync(storyHeroId, ct);
        if (storyHero == null)
        {
            throw new KeyNotFoundException($"StoryHero with Id '{storyHeroId}' not found");
        }

        if (storyHero.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("Only the creator can delete this StoryHero");
        }

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(storyHero.Status);
        if (currentStatus == AlchimaliaUniverseStatus.Published)
        {
            throw new InvalidOperationException("Cannot delete published StoryHero");
        }

        await _repository.DeleteAsync(storyHeroId, ct);
        _logger.LogInformation("StoryHero {StoryHeroId} deleted by user {UserId}", storyHeroId, userId);
    }

    private StoryHeroDto MapToDto(StoryHero storyHero, string? preferredLanguageCode = null)
    {
        return new StoryHeroDto
        {
            Id = storyHero.Id,
            HeroId = storyHero.HeroId,
            ImageUrl = storyHero.ImageUrl,
            UnlockConditionsJson = storyHero.UnlockConditionsJson,
            Status = storyHero.Status,
            CreatedByUserId = storyHero.CreatedByUserId,
            ReviewedByUserId = storyHero.ReviewedByUserId,
            ReviewNotes = storyHero.ReviewNotes,
            Version = storyHero.Version,
            CreatedAt = storyHero.CreatedAt,
            UpdatedAt = storyHero.UpdatedAt,
            Translations = storyHero.Translations.Select(t => new StoryHeroTranslationDto
            {
                Id = t.Id,
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Description = t.Description,
                GreetingText = t.GreetingText,
                GreetingAudioUrl = t.GreetingAudioUrl
            }).ToList()
        };
    }
}
