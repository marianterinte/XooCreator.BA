using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;
using XooCreator.BA.Features.AlchimaliaUniverse.Repositories;
using XooCreator.BA.Features.AlchimaliaUniverse.Mappers;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public class HeroDefinitionCraftService : IHeroDefinitionCraftService
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

    private readonly IHeroDefinitionCraftRepository _repository;
    private readonly XooDbContext _db;
    private readonly IHeroDefinitionPublishChangeLogService _changeLogService;
    private readonly IAlchimaliaUniverseAssetCopyService _assetCopyService;
    private readonly ILogger<HeroDefinitionCraftService> _logger;

    public HeroDefinitionCraftService(
        IHeroDefinitionCraftRepository repository,
        XooDbContext db,
        IHeroDefinitionPublishChangeLogService changeLogService,
        IAlchimaliaUniverseAssetCopyService assetCopyService,
        ILogger<HeroDefinitionCraftService> logger)
    {
        _repository = repository;
        _db = db;
        _changeLogService = changeLogService;
        _assetCopyService = assetCopyService;
        _logger = logger;
    }

    public async Task<HeroDefinitionCraftDto> GetAsync(string heroId, string? languageCode = null, CancellationToken ct = default)
    {
        var hero = await _repository.GetWithTranslationsAsync(heroId, ct);
        if (hero == null)
            throw new KeyNotFoundException($"HeroDefinitionCraft with Id '{heroId}' not found");

        return MapToDto(hero, languageCode);
    }

    public async Task<ListHeroDefinitionCraftsResponse> ListAsync(Guid currentUserId, string? status = null, string? type = null, string? search = null, string? languageCode = null, CancellationToken ct = default)
    {
        // Crafts only (definitions are listed via hero-definitions endpoint)
        var craftQuery = _db.HeroDefinitionCrafts.Include(x => x.Translations).AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
        {
            craftQuery = craftQuery.Where(x => x.Status == status);
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            craftQuery = craftQuery.Where(x => 
                x.Id.Contains(search) ||
                x.Translations.Any(t => t.Name.Contains(search)));
        }
        var allCrafts = await craftQuery.ToListAsync(ct);

        var normalizedLanguage = NormalizeLanguageCode(languageCode);
        var languageBase = GetLanguageBase(normalizedLanguage);

        var ownerIds = allCrafts
            .Where(c => c.CreatedByUserId.HasValue)
            .Select(c => c.CreatedByUserId!.Value)
            .Distinct()
            .ToList();

        var ownerEmailMap = await _db.AlchimaliaUsers
            .AsNoTracking()
            .Where(u => ownerIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Email })
            .ToDictionaryAsync(u => u.Id, u => u.Email ?? string.Empty, ct);

        var items = new List<HeroDefinitionCraftListItemDto>();
        foreach (var craft in allCrafts)
        {
            // Select translation - use requested language if provided, otherwise first available (exactly like story heroes)
            var selectedTranslation = FindBestTranslation(
                craft.Translations,
                normalizedLanguage,
                languageBase,
                t => t.LanguageCode);

            var availableLanguages = craft.Translations
                .Select(t => NormalizeLanguageCode(t.LanguageCode))
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var ownerEmail = craft.CreatedByUserId.HasValue && ownerEmailMap.TryGetValue(craft.CreatedByUserId.Value, out var email)
                ? email
                : string.Empty;

            var isOwnedByCurrentUser = craft.CreatedByUserId.HasValue && craft.CreatedByUserId.Value == currentUserId;
            var isAssignedToCurrentUser = craft.AssignedReviewerUserId.HasValue && craft.AssignedReviewerUserId.Value == currentUserId;

            items.Add(new HeroDefinitionCraftListItemDto
            {
                Id = craft.Id,
                PublishedDefinitionId = craft.PublishedDefinitionId,
                Name = selectedTranslation?.Name ?? craft.Id,
                Image = craft.Image,
                Status = craft.Status,
                UpdatedAt = craft.UpdatedAt,
                CreatedByUserId = craft.CreatedByUserId,
                AvailableLanguages = availableLanguages,
                AssignedReviewerUserId = craft.AssignedReviewerUserId,
                IsAssignedToCurrentUser = isAssignedToCurrentUser,
                IsOwnedByCurrentUser = isOwnedByCurrentUser,
                OwnerEmail = ownerEmail
            });
        }

        return new ListHeroDefinitionCraftsResponse
        {
            Heroes = items,
            TotalCount = items.Count
        };
    }

    public async Task<HeroDefinitionCraftDto> CreateAsync(Guid userId, CreateHeroDefinitionCraftRequest request, CancellationToken ct = default)
    {
        var heroId = string.IsNullOrWhiteSpace(request.Id)
            ? $"hero_{DateTime.UtcNow:yyyyMMddHHmmssfff}"
            : request.Id.Trim();

        var existing = await _repository.GetAsync(heroId, ct);
        if (existing != null)
            throw new InvalidOperationException($"HeroDefinitionCraft with Id '{heroId}' already exists");

        var hero = new HeroDefinitionCraft
        {
            Id = heroId,
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Translations = new List<HeroDefinitionCraftTranslation>
            {
                new HeroDefinitionCraftTranslation
                {
                    Id = Guid.NewGuid(),
                    HeroDefinitionCraftId = heroId,
                    LanguageCode = NormalizeLanguageCode(request.LanguageCode),
                    Name = request.Name,
                    Description = request.Description ?? string.Empty,
                    Story = request.Story ?? string.Empty,
                    AudioUrl = request.AudioUrl
                }
            }
        };

        await _repository.CreateAsync(hero, ct);
        return MapToDto(hero, request.LanguageCode);
    }

    public async Task<HeroDefinitionCraftDto> CreateCraftFromDefinitionAsync(Guid userId, string definitionId, bool allowAdminOverride = false, CancellationToken ct = default)
    {
        var definition = await _db.HeroDefinitionDefinitions
            .Include(d => d.Translations)
            .FirstOrDefaultAsync(d => d.Id == definitionId, ct);

        if (definition == null)
            throw new KeyNotFoundException($"HeroDefinitionDefinition with Id '{definitionId}' not found");

        // Check if there's already a draft craft for this definition
        var existingCraft = await _db.HeroDefinitionCrafts
            .FirstOrDefaultAsync(c => c.PublishedDefinitionId == definitionId && 
                (c.Status == AlchimaliaUniverseStatus.Draft.ToDb() || 
                 c.Status == AlchimaliaUniverseStatus.ChangesRequested.ToDb()), ct);

        if (existingCraft != null && !allowAdminOverride)
        {
            // Return existing draft instead of creating a new one
            return await GetAsync(existingCraft.Id, null, ct);
        }

        var currentUser = await _db.AlchimaliaUsers.FirstOrDefaultAsync(u => u.Id == userId, ct);
        var currentUserEmail = currentUser?.Email;
        string? publishedOwnerEmail = null;
        if (definition.PublishedByUserId.HasValue)
        {
            var publishedOwner = await _db.AlchimaliaUsers
                .FirstOrDefaultAsync(u => u.Id == definition.PublishedByUserId.Value, ct);
            publishedOwnerEmail = publishedOwner?.Email;
        }

        var hero = new HeroDefinitionCraft
        {
            Id = $"hero_{DateTime.UtcNow:yyyyMMddHHmmssfff}",
            PublishedDefinitionId = definitionId,
            CourageCost = definition.CourageCost,
            CuriosityCost = definition.CuriosityCost,
            ThinkingCost = definition.ThinkingCost,
            CreativityCost = definition.CreativityCost,
            SafetyCost = definition.SafetyCost,
            PrerequisitesJson = definition.PrerequisitesJson,
            RewardsJson = definition.RewardsJson,
            IsUnlocked = definition.IsUnlocked,
            PositionX = definition.PositionX,
            PositionY = definition.PositionY,
            Image = definition.Image,
            Status = AlchimaliaUniverseStatus.Draft.ToDb(),
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Translations = definition.Translations.Select(t => new HeroDefinitionCraftTranslation
            {
                Id = Guid.NewGuid(),
                HeroDefinitionCraftId = string.Empty, // set after save
                LanguageCode = NormalizeLanguageCode(t.LanguageCode),
                Name = t.Name,
                Description = t.Description ?? string.Empty,
                Story = t.Story ?? string.Empty,
                AudioUrl = t.AudioUrl
            }).ToList()
        };

        await _repository.CreateAsync(hero, ct);

        foreach (var translation in hero.Translations)
            translation.HeroDefinitionCraftId = hero.Id;

        // If assets are published, copy them to draft and update draft paths
        if (!string.IsNullOrWhiteSpace(currentUserEmail) && !string.IsNullOrWhiteSpace(publishedOwnerEmail))
        {
            var assets = new List<AlchimaliaUniverseAssetPathMapper.AssetInfo>();
            if (!string.IsNullOrWhiteSpace(definition.Image))
            {
                assets.Add(new AlchimaliaUniverseAssetPathMapper.AssetInfo(
                    Path.GetFileName(definition.Image),
                    AlchimaliaUniverseAssetPathMapper.AssetType.Image,
                    null));
            }

            foreach (var translation in definition.Translations)
            {
                if (!string.IsNullOrWhiteSpace(translation.AudioUrl))
                {
                    assets.Add(new AlchimaliaUniverseAssetPathMapper.AssetInfo(
                        Path.GetFileName(translation.AudioUrl),
                        AlchimaliaUniverseAssetPathMapper.AssetType.Audio,
                        NormalizeLanguageCode(translation.LanguageCode)));
                }
            }

            if (assets.Count > 0)
            {
                await _assetCopyService.CopyPublishedToDraftAsync(
                    assets,
                    publishedOwnerEmail,
                    definition.Id,
                    currentUserEmail,
                    hero.Id,
                    AlchimaliaUniverseAssetPathMapper.EntityType.Hero,
                    ct);
            }

            // Update craft paths to point to draft locations
            if (!string.IsNullOrWhiteSpace(definition.Image))
            {
                var imageAsset = new AlchimaliaUniverseAssetPathMapper.AssetInfo(
                    Path.GetFileName(definition.Image),
                    AlchimaliaUniverseAssetPathMapper.AssetType.Image,
                    null);
                hero.Image = AlchimaliaUniverseAssetPathMapper.BuildDraftPath(
                    imageAsset,
                    currentUserEmail,
                    hero.Id,
                    AlchimaliaUniverseAssetPathMapper.EntityType.Hero);
            }

            foreach (var translation in hero.Translations)
            {
                if (!string.IsNullOrWhiteSpace(translation.AudioUrl))
                {
                    var audioAsset = new AlchimaliaUniverseAssetPathMapper.AssetInfo(
                        Path.GetFileName(translation.AudioUrl),
                        AlchimaliaUniverseAssetPathMapper.AssetType.Audio,
                        NormalizeLanguageCode(translation.LanguageCode));
                    translation.AudioUrl = AlchimaliaUniverseAssetPathMapper.BuildDraftPath(
                        audioAsset,
                        currentUserEmail,
                        hero.Id,
                        AlchimaliaUniverseAssetPathMapper.EntityType.Hero);
                }
            }
        }

        hero.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("HeroDefinitionCraft {HeroId} created from definition {DefinitionId} by user {UserId}", hero.Id, definitionId, userId);
        return MapToDto(hero);
    }

    public async Task<HeroDefinitionCraftDto> UpdateAsync(Guid userId, string heroId, UpdateHeroDefinitionCraftRequest request, bool allowAdminOverride = false, CancellationToken ct = default)
    {
        var hero = await _repository.GetWithTranslationsAsync(heroId, ct);
        if (hero == null)
            throw new KeyNotFoundException($"HeroDefinitionCraft with Id '{heroId}' not found");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(hero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested)
            throw new InvalidOperationException($"Cannot update HeroDefinitionCraft in status '{currentStatus}'");

        if (hero.CreatedByUserId != userId && !allowAdminOverride)
            throw new UnauthorizedAccessException("Only the creator can update this HeroDefinitionCraft");

        // Change Log Tracking
        var langForTracking = request.Translations?.FirstOrDefault()?.LanguageCode ?? "ro-ro";
        var snapshotBeforeChanges = _changeLogService.CaptureSnapshot(hero, langForTracking);

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
            foreach (var translationDto in request.Translations)
            {
                var normalizedLang = NormalizeLanguageCode(translationDto.LanguageCode);
                var existingTranslation = hero.Translations.FirstOrDefault(t =>
                    NormalizeLanguageCode(t.LanguageCode) == normalizedLang);
                if (existingTranslation != null)
                {
                    existingTranslation.Name = translationDto.Name;
                    existingTranslation.Description = translationDto.Description ?? string.Empty;
                    existingTranslation.Story = translationDto.Story ?? string.Empty;
                    existingTranslation.AudioUrl = translationDto.AudioUrl;
                }
                else
                {
                    var newTranslation = new HeroDefinitionCraftTranslation
                    {
                        Id = Guid.NewGuid(),
                        HeroDefinitionCraftId = heroId,
                        LanguageCode = normalizedLang,
                        Name = translationDto.Name,
                        Description = translationDto.Description ?? string.Empty,
                        Story = translationDto.Story ?? string.Empty,
                        AudioUrl = translationDto.AudioUrl
                    };

                    // Add explicitly to DbSet to guarantee state = Added
                    _db.HeroDefinitionCraftTranslations.Add(newTranslation);
                    hero.Translations.Add(newTranslation);
                }
            }
        }

        hero.UpdatedAt = DateTime.UtcNow;
        await SaveChangesWithClientWinsAsync(ct);
        
        // Append changes after save
        await _changeLogService.AppendChangesAsync(hero, snapshotBeforeChanges, langForTracking, userId, ct);

        return MapToDto(hero);
    }

    private async Task SaveChangesWithClientWinsAsync(CancellationToken ct)
    {
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency error saving HeroDefinitionCraft. Attempting to resolve (Client Wins).");

            foreach (var entry in ex.Entries)
            {
                var databaseValues = await entry.GetDatabaseValuesAsync(ct);
                if (databaseValues == null)
                {
                    throw new InvalidOperationException("The entity being updated was deleted by another process.");
                }
                entry.OriginalValues.SetValues(databaseValues);
            }

            await _db.SaveChangesAsync(ct);
        }
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

    public async Task ClaimAsync(Guid reviewerId, string heroId, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
            throw new KeyNotFoundException($"HeroDefinitionCraft with Id '{heroId}' not found");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(hero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.SentForApproval)
            throw new InvalidOperationException($"Cannot claim HeroDefinitionCraft in status '{currentStatus}'. Must be SentForApproval.");

        hero.Status = AlchimaliaUniverseStatus.InReview.ToDb();
        hero.AssignedReviewerUserId = reviewerId;
        hero.ReviewStartedAt = DateTime.UtcNow;
        
        await _repository.SaveAsync(hero, ct);
        _logger.LogInformation("HeroDefinitionCraft {HeroId} claimed for review by {ReviewerId}", heroId, reviewerId);
    }

    public async Task RetractAsync(Guid userId, string heroId, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
            throw new KeyNotFoundException($"HeroDefinitionCraft with Id '{heroId}' not found");

        if (hero.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("Only the creator can retract this HeroDefinitionCraft");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(hero.Status);
        if (currentStatus != AlchimaliaUniverseStatus.SentForApproval && 
            currentStatus != AlchimaliaUniverseStatus.InReview &&
            currentStatus != AlchimaliaUniverseStatus.Approved)
            throw new InvalidOperationException($"Cannot retract HeroDefinitionCraft in status '{currentStatus}'");

        hero.Status = AlchimaliaUniverseStatus.Draft.ToDb();
        hero.AssignedReviewerUserId = null;
        hero.ReviewStartedAt = null;
        hero.ReviewEndedAt = null;
        hero.ReviewedByUserId = null;
        hero.ApprovedByUserId = null;

        await _repository.SaveAsync(hero, ct);
        _logger.LogInformation("HeroDefinitionCraft {HeroId} retracted by user {UserId}", heroId, userId);
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

    public async Task PublishAsync(Guid publisherId, string heroId, bool allowAdminOverride = false, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
            throw new KeyNotFoundException($"HeroDefinitionCraft with Id '{heroId}' not found");

        if (hero.CreatedByUserId != publisherId && !allowAdminOverride)
            throw new UnauthorizedAccessException($"User does not own hero '{heroId}'");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(hero.Status);
        
        // Relax status check if admin override is allowed
        if (currentStatus != AlchimaliaUniverseStatus.Approved && !allowAdminOverride)
        {
            throw new InvalidOperationException($"Cannot publish HeroDefinitionCraft in status '{currentStatus}'. Must be Approved.");
        }
        else if (currentStatus == AlchimaliaUniverseStatus.Published)
        {
            // Even admins shouldn't publish what's already published to avoid duplicates/confusion, 
            // though tecnically harmless if logic handles it. Let's prevent it or ensure idempotency.
            // Existing logic creates a new version if definition exists.
            // For now, let's allow it if it falls through, but the check above was preventing non-approved.
        }

        // Load craft with translations (exactly like story heroes)
        hero = await _db.HeroDefinitionCrafts
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.Id == heroId, ct) ?? hero;

        var definitionId = hero.PublishedDefinitionId;
        if (string.IsNullOrWhiteSpace(definitionId))
        {
             definitionId = hero.Id;
        }

        // Get creator email for asset operations
        var creatorUser = await _db.AlchimaliaUsers.FirstOrDefaultAsync(u => u.Id == (hero.CreatedByUserId ?? Guid.Empty), ct);
        var creatorEmail = creatorUser?.Email;

        if (string.IsNullOrWhiteSpace(creatorEmail))
        {
            throw new InvalidOperationException($"Cannot publish hero. Creator email not found for user {hero.CreatedByUserId}");
        }

        // Load or create definition
        var definition = await _db.HeroDefinitionDefinitions
            .Include(d => d.Translations)
            .FirstOrDefaultAsync(d => d.Id == definitionId, ct);

        // Sync assets FIRST (before creating/updating definition) - exactly like story heroes/regions
        var assets = AlchimaliaUniverseAssetPathMapper.CollectFromHeroCraft(hero);
        await _assetCopyService.CopyDraftToPublishedAsync(
            assets,
            creatorEmail,
            hero.Id,
            AlchimaliaUniverseAssetPathMapper.EntityType.Hero,
            ct);

        // Get published image URL
        string? publishedImageUrl = null;
        if (!string.IsNullOrWhiteSpace(hero.Image))
        {
            // If the craft image is a draft path, publish it.
            if (hero.Image.StartsWith("draft/", StringComparison.OrdinalIgnoreCase))
            {
                var filename = Path.GetFileName(hero.Image);
                var assetInfo = new AlchimaliaUniverseAssetPathMapper.AssetInfo(filename, AlchimaliaUniverseAssetPathMapper.AssetType.Image, null);
                var pubPath = AlchimaliaUniverseAssetPathMapper.BuildPublishedPath(assetInfo, creatorEmail, hero.Id, AlchimaliaUniverseAssetPathMapper.EntityType.Hero);
                publishedImageUrl = _assetCopyService.GetPublishedUrl(pubPath);
            }
            else if (definition != null && !string.IsNullOrWhiteSpace(definition.Image))
            {
                // If image wasn't changed (still published), keep the old published path.
                publishedImageUrl = definition.Image;
            }
            else if (Uri.TryCreate(hero.Image, UriKind.Absolute, out _))
            {
                // Already a full URL
                publishedImageUrl = hero.Image;
            }
            else if (hero.Image.StartsWith("images/", StringComparison.OrdinalIgnoreCase))
            {
                // Already a published relative path
                publishedImageUrl = hero.Image;
            }
            else
            {
                // Fallback: treat as filename or legacy path
                var filename = Path.GetFileName(hero.Image);
                var assetInfo = new AlchimaliaUniverseAssetPathMapper.AssetInfo(filename, AlchimaliaUniverseAssetPathMapper.AssetType.Image, null);
                var pubPath = AlchimaliaUniverseAssetPathMapper.BuildPublishedPath(assetInfo, creatorEmail, hero.Id, AlchimaliaUniverseAssetPathMapper.EntityType.Hero);
                publishedImageUrl = _assetCopyService.GetPublishedUrl(pubPath);
            }
        }

        if (definition == null)
        {
            // Create new definition
            definition = new HeroDefinitionDefinition
            {
                Id = definitionId,
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
                Image = publishedImageUrl ?? string.Empty,
                Status = AlchimaliaUniverseStatus.Published.ToDb(),
                PublishedByUserId = publisherId,
                PublishedAtUtc = DateTime.UtcNow,
                CreatedAt = hero.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                Version = 1,
                LastPublishedVersion = hero.LastDraftVersion
            };
            _db.HeroDefinitionDefinitions.Add(definition);
        }
        else
        {
            // Update existing definition (new version)
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
            definition.Image = publishedImageUrl ?? string.Empty;
            definition.PublishedByUserId = publisherId;
            definition.PublishedAtUtc = DateTime.UtcNow;
            definition.UpdatedAt = DateTime.UtcNow;
            definition.Version += 1;
            definition.LastPublishedVersion = hero.LastDraftVersion;

            // Remove old translations BEFORE adding new ones
            var oldTranslations = await _db.HeroDefinitionDefinitionTranslations
                .Where(t => t.HeroDefinitionDefinitionId == definitionId)
                .ToListAsync(ct);
            _db.HeroDefinitionDefinitionTranslations.RemoveRange(oldTranslations);
        }

        // Copy translations from craft to definition
        if (hero.Translations == null || hero.Translations.Count == 0)
        {
            throw new InvalidOperationException("Cannot publish hero. No translations found.");
        }

        // Normalize language codes and prevent duplicate translations (exactly like story heroes)
        var normalizedTranslations = hero.Translations
            .Select(t => new
            {
                Translation = t,
                Lang = NormalizeLanguageCode(t.LanguageCode)
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.Lang))
            .GroupBy(x => x.Lang, StringComparer.OrdinalIgnoreCase)
            .Select(g => g
                .OrderByDescending(x => (x.Translation.LanguageCode ?? string.Empty).Contains('-'))
                .First())
            .ToList();

        if (normalizedTranslations.Count == 0)
        {
            throw new InvalidOperationException("Cannot publish hero. All translations have empty language code.");
        }

        // Add new translations with published audio URLs
        foreach (var item in normalizedTranslations)
        {
            var craftTranslation = item.Translation;
            var lang = item.Lang;

            // Get published audio URL
            string? publishedAudioUrl = craftTranslation.AudioUrl;
            if (!string.IsNullOrWhiteSpace(craftTranslation.AudioUrl))
            {
                var filename = Path.GetFileName(craftTranslation.AudioUrl);
                var assetInfo = new AlchimaliaUniverseAssetPathMapper.AssetInfo(filename, AlchimaliaUniverseAssetPathMapper.AssetType.Audio, lang);
                var pubPath = AlchimaliaUniverseAssetPathMapper.BuildPublishedPath(assetInfo, creatorEmail, hero.Id, AlchimaliaUniverseAssetPathMapper.EntityType.Hero);
                publishedAudioUrl = _assetCopyService.GetPublishedUrl(pubPath);
            }

            var definitionTranslation = new HeroDefinitionDefinitionTranslation
            {
                Id = Guid.NewGuid(),
                HeroDefinitionDefinitionId = definitionId,
                LanguageCode = lang,
                Name = craftTranslation.Name,
                Description = craftTranslation.Description ?? string.Empty,
                Story = craftTranslation.Story ?? string.Empty,
                AudioUrl = publishedAudioUrl
            };
            _db.HeroDefinitionDefinitionTranslations.Add(definitionTranslation);
        }

        // Save definition first (before deleting craft)
        try
        {
            definition.Status = AlchimaliaUniverseStatus.Published.ToDb();
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("HeroDefinitionCraft {HeroId} definition published by user {UserId}. DefinitionId: {DefId}, Version: {Version}", heroId, publisherId, definitionId, definition.Version);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update failed during publish for hero {HeroId}", heroId);
            throw new InvalidOperationException($"Database update failed: {ex.InnerException?.Message ?? ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error publishing hero {HeroId}", heroId);
            throw new InvalidOperationException($"Unexpected error: {ex.Message}");
        }

        // Cleanup craft after successful publish
        await CleanupCraftAsync(heroId, ct);
    }

    private async Task CleanupCraftAsync(string heroId, CancellationToken ct)
    {
        try
        {
            var craftToDelete = await _db.HeroDefinitionCrafts
                .FirstOrDefaultAsync(c => c.Id == heroId, ct);
            
            if (craftToDelete != null)
            {
                _db.HeroDefinitionCrafts.Remove(craftToDelete);
                await _db.SaveChangesAsync(ct);
                _logger.LogInformation("Hero published and craft cleaned up: heroId={HeroId}", heroId);
            }
        }
        catch (Exception cleanupEx)
        {
            _logger.LogWarning(cleanupEx, "Failed to cleanup hero craft after publish: heroId={HeroId}, but publish succeeded", heroId);
        }
    }

    public async Task DeleteAsync(Guid userId, string heroId, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
            throw new KeyNotFoundException($"HeroDefinitionCraft with Id '{heroId}' not found");

        if (hero.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("Only the creator can delete this HeroDefinitionCraft");

        var currentStatus = AlchimaliaUniverseStatusExtensions.FromDb(hero.Status);
        
        // Allow deletion regardless of status (as requested)
        // if (currentStatus != AlchimaliaUniverseStatus.Draft && currentStatus != AlchimaliaUniverseStatus.ChangesRequested) ...

        // Delete draft assets from Azure Storage before deleting from database
        var creatorUser = await _db.AlchimaliaUsers.FirstOrDefaultAsync(u => u.Id == userId, ct);
        var creatorEmail = creatorUser?.Email;
        
        if (!string.IsNullOrWhiteSpace(creatorEmail))
        {
            await _assetCopyService.DeleteDraftAssetsAsync(creatorEmail, heroId, AlchimaliaUniverseAssetPathMapper.EntityType.Hero, ct);
        }

        // Delete draft from database
        await _repository.DeleteAsync(heroId, ct);
        
        _logger.LogInformation("HeroDefinitionCraft {HeroId} deleted by user {UserId}", heroId, userId);
    }

    private HeroDefinitionCraftDto MapToDto(HeroDefinitionCraft hero, string? preferredLanguageCode = null)
    {
        return new HeroDefinitionCraftDto
        {
            Id = hero.Id,
            PublishedDefinitionId = hero.PublishedDefinitionId,
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
                Story = t.Story,
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
