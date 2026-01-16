using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;
using XooCreator.BA.Features.AlchimaliaUniverse.Repositories;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public class HeroDefinitionCraftService : IHeroDefinitionCraftService
{
    private readonly IHeroDefinitionCraftRepository _repository;
    private readonly XooDbContext _db;
    private readonly ILogger<HeroDefinitionCraftService> _logger;

    public HeroDefinitionCraftService(
        IHeroDefinitionCraftRepository repository,
        XooDbContext db,
        ILogger<HeroDefinitionCraftService> logger)
    {
        _repository = repository;
        _db = db;
        _logger = logger;
    }

    public async Task<HeroDefinitionCraftDto> GetAsync(string heroId, string? languageCode = null, CancellationToken ct = default)
    {
        var hero = await _repository.GetWithTranslationsAsync(heroId, ct);
        if (hero == null)
            throw new KeyNotFoundException($"HeroDefinitionCraft with Id '{heroId}' not found");

        return MapToDto(hero, languageCode);
    }

    public async Task<ListHeroDefinitionCraftsResponse> ListAsync(string? status = null, string? type = null, string? search = null, string? languageCode = null, CancellationToken ct = default)
    {
        var heroes = await _repository.ListAsync(status, type, search, ct);
        var totalCount = await _repository.CountAsync(status, type, ct);

        var items = heroes.Select(h =>
        {
            HeroDefinitionCraftTranslation? selectedTranslation = null;
            if (!string.IsNullOrWhiteSpace(languageCode))
            {
                var normalizedLang = languageCode.ToLowerInvariant();
                selectedTranslation = h.Translations.FirstOrDefault(t => t.LanguageCode.ToLowerInvariant() == normalizedLang);
            }
            selectedTranslation ??= h.Translations.FirstOrDefault();

            var availableLanguages = h.Translations.Select(t => t.LanguageCode.ToLowerInvariant()).ToList();

            return new HeroDefinitionCraftListItemDto
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

        return new ListHeroDefinitionCraftsResponse
        {
            Heroes = items,
            TotalCount = totalCount
        };
    }

    public async Task<HeroDefinitionCraftDto> CreateAsync(Guid userId, CreateHeroDefinitionCraftRequest request, CancellationToken ct = default)
    {
        var heroId = string.IsNullOrWhiteSpace(request.Id) ? Guid.NewGuid().ToString() : request.Id.Trim();

        var existing = await _repository.GetAsync(heroId, ct);
        if (existing != null)
            throw new InvalidOperationException($"HeroDefinitionCraft with Id '{heroId}' already exists");

        var hero = new HeroDefinitionCraft
        {
            Id = heroId,
            Type = request.Type,
            CourageCost = request.CourageCost,
            CuriosityCost = request.CuriosityCost,
            ThinkingCost = request.ThinkingCost,
            CreativityCost = request.CreativityCost,
            SafetyCost = request.SafetyCost,
            PrerequisitesJson = request.PrerequisitesJson ?? "[]",
            RewardsJson = request.RewardsJson ?? "[]",
            PositionX = request.PositionX,
            PositionY = request.PositionY,
            Image = request.Image ?? string.Empty,
            Status = AlchimaliaUniverseStatus.Draft.ToDb(),
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Translations = new List<HeroDefinitionCraftTranslation>
            {
                new HeroDefinitionCraftTranslation
                {
                    Id = Guid.NewGuid(),
                    HeroDefinitionCraftId = heroId,
                    LanguageCode = request.LanguageCode.ToLowerInvariant(),
                    Name = request.Name,
                    Description = request.Description,
                    Story = request.Story
                }
            }
        };

        await _repository.CreateAsync(hero, ct);
        return MapToDto(hero, request.LanguageCode);
    }

    public async Task<HeroDefinitionCraftDto> UpdateAsync(Guid userId, string heroId, UpdateHeroDefinitionCraftRequest request, CancellationToken ct = default)
    {
        var hero = await _repository.GetWithTranslationsAsync(heroId, ct);
        if (hero == null)
            throw new KeyNotFoundException($"HeroDefinitionCraft with Id '{heroId}' not found");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(hero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
            throw new InvalidOperationException($"Cannot update HeroDefinitionCraft in status '{currentStatus}'");

        if (hero.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("Only the creator can update this HeroDefinitionCraft");

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
                }
                else
                {
                    hero.Translations.Add(new HeroDefinitionCraftTranslation
                    {
                        Id = Guid.NewGuid(),
                        HeroDefinitionCraftId = heroId,
                        LanguageCode = normalizedLang,
                        Name = translationDto.Name,
                        Description = translationDto.Description,
                        Story = translationDto.Story
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
            throw new KeyNotFoundException($"HeroDefinitionCraft with Id '{heroId}' not found");

        if (hero.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("Only the creator can submit this HeroDefinitionCraft");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(hero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
            throw new InvalidOperationException($"Cannot submit HeroDefinitionCraft in status '{currentStatus}'");

        hero.Status = AlchimaliaUniverseStatus.SentForApproval.ToDb();
        await _repository.SaveAsync(hero, ct);
        _logger.LogInformation("HeroDefinitionCraft {HeroId} submitted for review by user {UserId}", heroId, userId);
    }

    public async Task ReviewAsync(Guid reviewerId, string heroId, ReviewHeroDefinitionCraftRequest request, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
            throw new KeyNotFoundException($"HeroDefinitionCraft with Id '{heroId}' not found");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(hero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.SentForApproval && currentStatus != AlchimaliaUniverseStatus.InReview)
            throw new InvalidOperationException($"Cannot review HeroDefinitionCraft in status '{currentStatus}'");

        hero.Status = request.Approve
            ? AlchimaliaUniverseStatus.Approved.ToDb()
            : AlchimaliaUniverseStatus.ChangesRequested.ToDb();
        hero.ReviewedByUserId = reviewerId;
        hero.ReviewNotes = request.Notes;
        await _repository.SaveAsync(hero, ct);
        _logger.LogInformation("HeroDefinitionCraft {HeroId} reviewed by {ReviewerId}: {Decision}", heroId, reviewerId, request.Approve ? "Approved" : "ChangesRequested");
    }

    public async Task PublishAsync(Guid publisherId, string heroId, CancellationToken ct = default)
    {
        var hero = await _repository.GetWithTranslationsAsync(heroId, ct);
        if (hero == null)
            throw new KeyNotFoundException($"HeroDefinitionCraft with Id '{heroId}' not found");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(hero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Approved)
            throw new InvalidOperationException($"Cannot publish HeroDefinitionCraft in status '{currentStatus}'. Must be Approved.");

        var definitionId = hero.PublishedDefinitionId ?? hero.Id;
        var definition = await _db.HeroDefinitionDefinitions
            .Include(d => d.Translations)
            .FirstOrDefaultAsync(d => d.Id == definitionId, ct);

        if (definition == null)
        {
            definition = new HeroDefinitionDefinition
            {
                Id = definitionId,
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
                Status = AlchimaliaUniverseStatus.Published.ToDb(),
                PublishedByUserId = publisherId,
                PublishedAtUtc = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Translations = hero.Translations.Select(t => new HeroDefinitionDefinitionTranslation
                {
                    Id = Guid.NewGuid(),
                    HeroDefinitionDefinitionId = definitionId,
                    LanguageCode = t.LanguageCode,
                    Name = t.Name,
                    Description = t.Description,
                    Story = t.Story
                }).ToList()
            };
            _db.HeroDefinitionDefinitions.Add(definition);
        }
        else
        {
            definition.Type = hero.Type;
            definition.CourageCost = hero.CourageCost;
            definition.CuriosityCost = hero.CuriosityCost;
            definition.ThinkingCost = hero.ThinkingCost;
            definition.CreativityCost = hero.CreativityCost;
            definition.SafetyCost = hero.SafetyCost;
            definition.PrerequisitesJson = hero.PrerequisitesJson;
            definition.RewardsJson = hero.RewardsJson;
            definition.IsUnlocked = hero.IsUnlocked;
            definition.PositionX = hero.PositionX;
            definition.PositionY = hero.PositionY;
            definition.Image = hero.Image;
            definition.Status = AlchimaliaUniverseStatus.Published.ToDb();
            definition.PublishedByUserId = publisherId;
            definition.PublishedAtUtc = DateTime.UtcNow;
            definition.UpdatedAt = DateTime.UtcNow;

            // Replace translations
            _db.HeroDefinitionDefinitionTranslations.RemoveRange(definition.Translations);
            definition.Translations = hero.Translations.Select(t => new HeroDefinitionDefinitionTranslation
            {
                Id = Guid.NewGuid(),
                HeroDefinitionDefinitionId = definitionId,
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Description = t.Description,
                Story = t.Story
            }).ToList();
        }

        hero.PublishedDefinitionId = definitionId;
        hero.Status = AlchimaliaUniverseStatus.Published.ToDb();
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("HeroDefinitionCraft {HeroId} published by user {UserId}", heroId, publisherId);
    }

    private HeroDefinitionCraftDto MapToDto(HeroDefinitionCraft hero, string? preferredLanguageCode = null)
    {
        return new HeroDefinitionCraftDto
        {
            Id = hero.Id,
            PublishedDefinitionId = hero.PublishedDefinitionId,
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
            CreatedAt = hero.CreatedAt,
            UpdatedAt = hero.UpdatedAt,
            Translations = hero.Translations.Select(t => new HeroDefinitionTranslationDto
            {
                Id = t.Id,
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Description = t.Description,
                Story = t.Story
            }).ToList()
        };
    }
}
