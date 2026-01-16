using XooCreator.BA.Data.Enums;
using XooCreator.BA.Data;
using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;
using XooCreator.BA.Features.AlchimaliaUniverse.Repositories;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public class HeroDefinitionService : IHeroDefinitionService
{
    private readonly IHeroDefinitionRepository _repository;
    private readonly ILogger<HeroDefinitionService> _logger;

    public HeroDefinitionService(
        IHeroDefinitionRepository repository,
        ILogger<HeroDefinitionService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<HeroDefinitionDto> GetAsync(string heroId, string? languageCode = null, CancellationToken ct = default)
    {
        var hero = await _repository.GetWithTranslationsAsync(heroId, ct);
        if (hero == null)
        {
            throw new KeyNotFoundException($"HeroDefinition with Id '{heroId}' not found");
        }

        return MapToDto(hero, languageCode);
    }

    public async Task<ListHeroDefinitionsResponse> ListAsync(string? status = null, string? type = null, string? search = null, string? languageCode = null, CancellationToken ct = default)
    {
        var heroes = await _repository.ListAsync(status, type, search, ct);
        var totalCount = await _repository.CountAsync(status, type, ct);

        var items = heroes.Select(h =>
        {
            // Get translation for selected language, or first available
            HeroDefinitionTranslation? selectedTranslation = null;
            if (!string.IsNullOrWhiteSpace(languageCode))
            {
                var normalizedLang = languageCode.ToLowerInvariant();
                selectedTranslation = h.Translations.FirstOrDefault(t => t.LanguageCode.ToLowerInvariant() == normalizedLang);
            }
            
            // Fallback to first translation if no match
            selectedTranslation ??= h.Translations.FirstOrDefault();
            
            // Get all available language codes
            var availableLanguages = h.Translations.Select(t => t.LanguageCode.ToLowerInvariant()).ToList();
            
            return new HeroDefinitionListItemDto
            {
                Id = h.Id,
                Type = h.Type,
                Name = selectedTranslation?.Name ?? h.Id,
                Image = h.Image,
                Status = h.Status,
                UpdatedAt = h.UpdatedAt,
                CreatedByUserId = h.CreatedByUserId,
                AvailableLanguages = availableLanguages
            };
        }).ToList();

        return new ListHeroDefinitionsResponse
        {
            Heroes = items,
            TotalCount = totalCount
        };
    }

    public async Task<HeroDefinitionDto> CreateAsync(Guid userId, CreateHeroDefinitionRequest request, CancellationToken ct = default)
    {
        var heroId = string.IsNullOrWhiteSpace(request.Id) ? Guid.NewGuid().ToString() : request.Id.Trim();
        
        // Check if hero already exists
        var existing = await _repository.GetAsync(heroId, ct);
        if (existing != null)
        {
            throw new InvalidOperationException($"HeroDefinition with Id '{heroId}' already exists");
        }

        var hero = new HeroDefinition
        {
            Id = heroId,
            Type = string.IsNullOrWhiteSpace(request.Type) ? "hero" : request.Type.Trim(),
            CourageCost = request.CourageCost ?? 0,
            CuriosityCost = request.CuriosityCost ?? 0,
            ThinkingCost = request.ThinkingCost ?? 0,
            CreativityCost = request.CreativityCost ?? 0,
            SafetyCost = request.SafetyCost ?? 0,
            PrerequisitesJson = request.PrerequisitesJson ?? "[]",
            RewardsJson = request.RewardsJson ?? "[]",
            PositionX = request.PositionX,
            PositionY = request.PositionY,
            Image = request.Image ?? string.Empty,
            Status = AlchimaliaUniverseStatus.Draft.ToDb(),
            CreatedByUserId = userId,
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Translations = new List<HeroDefinitionTranslation>
            {
                new HeroDefinitionTranslation
                {
                    Id = Guid.NewGuid(),
                    HeroDefinitionId = heroId,
                    LanguageCode = request.LanguageCode.ToLowerInvariant(),
                    Name = request.Name,
                    Description = request.Description,
                    Story = request.Story,
                    AudioUrl = request.AudioUrl
                }
            }
        };

        await _repository.CreateAsync(hero, ct);
        return MapToDto(hero, request.LanguageCode);
    }

    public async Task<HeroDefinitionDto> UpdateAsync(Guid userId, string heroId, UpdateHeroDefinitionRequest request, CancellationToken ct = default)
    {
        var hero = await _repository.GetWithTranslationsAsync(heroId, ct);
        if (hero == null)
        {
            throw new KeyNotFoundException($"HeroDefinition with Id '{heroId}' not found");
        }

        // Check permissions
        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(hero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
        {
            throw new InvalidOperationException($"Cannot update HeroDefinition in status '{currentStatus}'");
        }

        if (hero.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("Only the creator can update this HeroDefinition");
        }

        // Update fields
        if (request.Type != null) hero.Type = request.Type;
        if (request.CourageCost.HasValue) hero.CourageCost = request.CourageCost.Value;
        if (request.CuriosityCost.HasValue) hero.CuriosityCost = request.CuriosityCost.Value;
        if (request.ThinkingCost.HasValue) hero.ThinkingCost = request.ThinkingCost.Value;
        if (request.CreativityCost.HasValue) hero.CreativityCost = request.CreativityCost.Value;
        if (request.SafetyCost.HasValue) hero.SafetyCost = request.SafetyCost.Value;
        if (request.PrerequisitesJson != null) hero.PrerequisitesJson = request.PrerequisitesJson;
        if (request.RewardsJson != null) hero.RewardsJson = request.RewardsJson;
        if (request.IsUnlocked.HasValue) hero.IsUnlocked = request.IsUnlocked.Value;
        if (request.PositionX.HasValue) hero.PositionX = request.PositionX.Value;
        if (request.PositionY.HasValue) hero.PositionY = request.PositionY.Value;
        if (request.Image != null) hero.Image = request.Image;

        // Update translations
        if (request.Translations != null)
        {
            foreach (var (langCode, translationDto) in request.Translations)
            {
                var normalizedLang = langCode.ToLowerInvariant();
                var existingTranslation = hero.Translations.FirstOrDefault(t => t.LanguageCode == normalizedLang);
                
                if (existingTranslation != null)
                {
                    existingTranslation.Name = translationDto.Name;
                    existingTranslation.Description = translationDto.Description;
                    existingTranslation.Story = translationDto.Story;
                    existingTranslation.AudioUrl = translationDto.AudioUrl;
                }
                else
                {
                    hero.Translations.Add(new HeroDefinitionTranslation
                    {
                        Id = Guid.NewGuid(),
                        HeroDefinitionId = heroId,
                        LanguageCode = normalizedLang,
                        Name = translationDto.Name,
                        Description = translationDto.Description,
                        Story = translationDto.Story,
                        AudioUrl = translationDto.AudioUrl
                    });
                }
            }
        }

        await _repository.SaveAsync(hero, ct);
        return MapToDto(hero);
    }

    public async Task SubmitForReviewAsync(Guid userId, string heroId, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
        {
            throw new KeyNotFoundException($"HeroDefinition with Id '{heroId}' not found");
        }

        if (hero.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("Only the creator can submit this HeroDefinition");
        }

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(hero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
        {
            throw new InvalidOperationException($"Cannot submit HeroDefinition in status '{currentStatus}'");
        }

        hero.Status = AlchimaliaUniverseStatus.SentForApproval.ToDb();
        await _repository.SaveAsync(hero, ct);
        _logger.LogInformation("HeroDefinition {HeroId} submitted for review by user {UserId}", heroId, userId);
    }

    public async Task ReviewAsync(Guid reviewerId, string heroId, ReviewHeroDefinitionRequest request, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
        {
            throw new KeyNotFoundException($"HeroDefinition with Id '{heroId}' not found");
        }

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(hero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.SentForApproval && currentStatus != AlchimaliaUniverseStatus.InReview)
        {
            throw new InvalidOperationException($"Cannot review HeroDefinition in status '{currentStatus}'");
        }

        hero.Status = request.Approve 
            ? AlchimaliaUniverseStatus.Approved.ToDb() 
            : AlchimaliaUniverseStatus.ChangesRequested.ToDb();
        hero.ReviewedByUserId = reviewerId;
        hero.ReviewNotes = request.Notes;
        await _repository.SaveAsync(hero, ct);
        _logger.LogInformation("HeroDefinition {HeroId} reviewed by {ReviewerId}: {Decision}", heroId, reviewerId, request.Approve ? "Approved" : "ChangesRequested");
    }

    public async Task PublishAsync(Guid userId, string heroId, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
        {
            throw new KeyNotFoundException($"HeroDefinition with Id '{heroId}' not found");
        }

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(hero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Approved)
        {
            throw new InvalidOperationException($"Cannot publish HeroDefinition in status '{currentStatus}'. Must be Approved.");
        }

        hero.Status = AlchimaliaUniverseStatus.Published.ToDb();
        await _repository.SaveAsync(hero, ct);
        _logger.LogInformation("HeroDefinition {HeroId} published by user {UserId}", heroId, userId);
    }

    public async Task DeleteAsync(Guid userId, string heroId, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
        {
            throw new KeyNotFoundException($"HeroDefinition with Id '{heroId}' not found");
        }

        if (hero.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("Only the creator can delete this HeroDefinition");
        }

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(hero.Status);
        if (currentStatus == AlchimaliaUniverseStatus.Published)
        {
            throw new InvalidOperationException("Cannot delete published HeroDefinition");
        }

        await _repository.DeleteAsync(heroId, ct);
        _logger.LogInformation("HeroDefinition {HeroId} deleted by user {UserId}", heroId, userId);
    }

    private HeroDefinitionDto MapToDto(HeroDefinition hero, string? preferredLanguageCode = null)
    {
        return new HeroDefinitionDto
        {
            Id = hero.Id,
            Type = hero.Type,
            CourageCost = hero.CourageCost,
            CuriosityCost = hero.CuriosityCost,
            ThinkingCost = hero.ThinkingCost,
            CreativityCost = hero.CreativityCost,
            SafetyCost = hero.SafetyCost,
            PrerequisitesJson = hero.PrerequisitesJson,
            RewardsJson = hero.RewardsJson,
            IsUnlocked = hero.IsUnlocked,
            PositionX = hero.PositionX,
            PositionY = hero.PositionY,
            Image = hero.Image,
            Status = hero.Status,
            CreatedByUserId = hero.CreatedByUserId,
            ReviewedByUserId = hero.ReviewedByUserId,
            ReviewNotes = hero.ReviewNotes,
            Version = hero.Version,
            ParentVersionId = hero.ParentVersionId,
            CreatedAt = hero.CreatedAt,
            UpdatedAt = hero.UpdatedAt,
            Translations = hero.Translations.Select(t => new HeroDefinitionTranslationDto
            {
                Id = t.Id,
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Description = t.Description,
                Story = t.Story,
                AudioUrl = t.AudioUrl
            }).ToList()
        };
    }
}
