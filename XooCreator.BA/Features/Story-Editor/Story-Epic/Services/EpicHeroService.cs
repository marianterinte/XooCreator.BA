using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
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
    private readonly ILogger<EpicHeroService> _logger;

    public EpicHeroService(
        IEpicHeroRepository repository,
        XooDbContext context,
        IBlobSasService blobSas,
        ILogger<EpicHeroService> logger)
    {
        _repository = repository;
        _context = context;
        _blobSas = blobSas;
        _logger = logger;
    }

    public async Task<EpicHeroDto?> GetHeroAsync(string heroId, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null) return null;

        return new EpicHeroDto
        {
            Id = hero.Id,
            ImageUrl = hero.ImageUrl,
            GreetingAudioUrl = hero.GreetingAudioUrl,
            Status = hero.Status,
            CreatedAt = hero.CreatedAt,
            UpdatedAt = hero.UpdatedAt,
            PublishedAtUtc = hero.PublishedAtUtc,
            AssignedReviewerUserId = hero.AssignedReviewerUserId,
            ReviewedByUserId = hero.ReviewedByUserId,
            ApprovedByUserId = hero.ApprovedByUserId,
            ReviewNotes = hero.ReviewNotes,
            ReviewStartedAt = hero.ReviewStartedAt,
            ReviewEndedAt = hero.ReviewEndedAt,
            Translations = hero.Translations.Select(t => new EpicHeroTranslationDto
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                GreetingText = t.GreetingText
            }).ToList()
        };
    }

    public async Task<EpicHeroDto> CreateHeroAsync(Guid ownerUserId, string heroId, string name, CancellationToken ct = default)
    {
        var hero = await _repository.CreateAsync(ownerUserId, heroId, name, ct);
        
        // Create default translation (ro-ro) with the provided name
        // Add translation directly to context instead of using SaveAsync to avoid concurrency issues
        var defaultTranslation = new EpicHeroTranslation
        {
            Id = Guid.NewGuid(),
            EpicHeroId = hero.Id,
            LanguageCode = "ro-ro",
            Name = name
        };
        _context.EpicHeroTranslations.Add(defaultTranslation);
        await _context.SaveChangesAsync(ct);
        
        // Reload hero with translations to return complete DTO
        hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
        {
            throw new InvalidOperationException($"Hero '{heroId}' not found after creation");
        }
        
        return new EpicHeroDto
        {
            Id = hero.Id,
            ImageUrl = hero.ImageUrl,
            GreetingAudioUrl = hero.GreetingAudioUrl,
            Status = hero.Status,
            CreatedAt = hero.CreatedAt,
            UpdatedAt = hero.UpdatedAt,
            PublishedAtUtc = hero.PublishedAtUtc,
            AssignedReviewerUserId = hero.AssignedReviewerUserId,
            ReviewedByUserId = hero.ReviewedByUserId,
            ApprovedByUserId = hero.ApprovedByUserId,
            ReviewNotes = hero.ReviewNotes,
            ReviewStartedAt = hero.ReviewStartedAt,
            ReviewEndedAt = hero.ReviewEndedAt,
            Translations = hero.Translations.Select(t => new EpicHeroTranslationDto
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                GreetingText = t.GreetingText
            }).ToList()
        };
    }

    public async Task SaveHeroAsync(Guid ownerUserId, string heroId, EpicHeroDto dto, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
        {
            throw new InvalidOperationException($"Hero '{heroId}' not found");
        }

        if (hero.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own hero '{heroId}'");
        }

        // Update properties (non-translatable)
        hero.ImageUrl = dto.ImageUrl;
        hero.GreetingAudioUrl = dto.GreetingAudioUrl;
        hero.UpdatedAt = DateTime.UtcNow;

        // Update translations
        if (dto.Translations != null && dto.Translations.Count > 0)
        {
            foreach (var translationDto in dto.Translations)
            {
                var lang = translationDto.LanguageCode.ToLowerInvariant();
                var translation = hero.Translations.FirstOrDefault(t => t.LanguageCode == lang);
                
                if (translation == null)
                {
                    translation = new EpicHeroTranslation
                    {
                        Id = Guid.NewGuid(),
                        EpicHeroId = hero.Id,
                        LanguageCode = lang,
                        Name = translationDto.Name,
                        GreetingText = translationDto.GreetingText
                    };
                    _context.EpicHeroTranslations.Add(translation);
                }
                else
                {
                    translation.Name = translationDto.Name;
                    translation.GreetingText = translationDto.GreetingText;
                }
            }
        }

        await _repository.SaveAsync(hero, ct);
    }

    public async Task<List<EpicHeroListItemDto>> ListHeroesByOwnerAsync(Guid ownerUserId, string? status = null, Guid? currentUserId = null, CancellationToken ct = default)
    {
        var heroes = await _repository.ListByOwnerAsync(ownerUserId, status, ct);
        return heroes.Select(h =>
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
                GreetingAudioUrl = h.GreetingAudioUrl,
                Status = h.Status,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt,
                PublishedAtUtc = h.PublishedAtUtc,
                AssignedReviewerUserId = h.AssignedReviewerUserId,
                IsAssignedToCurrentUser = isAssignedToCurrentUser,
                IsOwnedByCurrentUser = isOwnedByCurrentUser
            };
        }).ToList();
    }

    public async Task<List<EpicHeroListItemDto>> ListHeroesForEditorAsync(Guid currentUserId, string? status = null, CancellationToken ct = default)
    {
        var ownedHeroes = await _repository.ListByOwnerAsync(currentUserId, status, ct);
        var publishedHeroes = await _repository.ListPublishedAsync(currentUserId, ct);
        var heroesForReview = await _repository.ListForReviewAsync(ct);

        var combined = new Dictionary<string, EpicHero>(StringComparer.OrdinalIgnoreCase);

        foreach (var hero in ownedHeroes)
        {
            combined[hero.Id] = hero;
        }

        foreach (var hero in publishedHeroes)
        {
            combined[hero.Id] = hero;
        }

        // Include heroes sent_for_approval and in_review (for reviewers)
        foreach (var hero in heroesForReview)
        {
            if (!combined.ContainsKey(hero.Id))
            {
                combined[hero.Id] = hero;
            }
        }

        return combined.Values
            .Select(h => MapToListItem(h, currentUserId))
            .OrderByDescending(h => h.IsOwnedByCurrentUser)
            .ThenBy(h => h.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private EpicHeroListItemDto MapToListItem(EpicHero hero, Guid? currentUserId)
    {
        // Get name and greeting from first available translation
        var firstTranslation = hero.Translations.FirstOrDefault();
        var name = firstTranslation?.Name ?? string.Empty;
        var greetingText = firstTranslation?.GreetingText;
        
        // Compute flags for current user
        var isOwnedByCurrentUser = currentUserId.HasValue && hero.OwnerUserId == currentUserId.Value;
        var isAssignedToCurrentUser = currentUserId.HasValue && 
                                      hero.AssignedReviewerUserId.HasValue && 
                                      hero.AssignedReviewerUserId.Value == currentUserId.Value;
        
        return new EpicHeroListItemDto
        {
            Id = hero.Id,
            Name = name,
            ImageUrl = hero.ImageUrl,
            GreetingText = greetingText,
            GreetingAudioUrl = hero.GreetingAudioUrl,
            Status = hero.Status,
            CreatedAt = hero.CreatedAt,
            UpdatedAt = hero.UpdatedAt,
            PublishedAtUtc = hero.PublishedAtUtc,
            AssignedReviewerUserId = hero.AssignedReviewerUserId,
            IsAssignedToCurrentUser = isAssignedToCurrentUser,
            IsOwnedByCurrentUser = isOwnedByCurrentUser
        };
    }

    public async Task DeleteHeroAsync(Guid ownerUserId, string heroId, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
        {
            throw new InvalidOperationException($"Hero '{heroId}' not found");
        }

        if (hero.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own hero '{heroId}'");
        }

        // Check if hero is used in any epic
        var isUsed = await _repository.IsUsedInEpicsAsync(heroId, ct);
        if (isUsed && hero.Status == "published")
        {
            throw new InvalidOperationException($"Cannot delete published hero '{heroId}' that is used in epics");
        }

        await _repository.DeleteAsync(heroId, ct);
    }

    public async Task SubmitForReviewAsync(Guid ownerUserId, string heroId, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
        {
            throw new InvalidOperationException($"Hero '{heroId}' not found");
        }

        if (hero.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own hero '{heroId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(hero.Status);
        if (currentStatus != StoryStatus.Draft && currentStatus != StoryStatus.ChangesRequested)
        {
            throw new InvalidOperationException($"Invalid state transition. Expected Draft or ChangesRequested, got {currentStatus}");
        }

        hero.Status = StoryStatus.SentForApproval.ToDb();
        hero.AssignedReviewerUserId = null;
        hero.ReviewNotes = null;
        hero.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveAsync(hero, ct);
        _logger.LogInformation("Hero submitted for review: heroId={HeroId}", heroId);
    }

    public async Task ReviewAsync(Guid reviewerUserId, string heroId, bool approve, string? notes, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
        {
            throw new InvalidOperationException($"Hero '{heroId}' not found");
        }

        var currentStatus = StoryStatusExtensions.FromDb(hero.Status);
        if (currentStatus != StoryStatus.InReview && currentStatus != StoryStatus.SentForApproval)
        {
            throw new InvalidOperationException($"Invalid state transition. Expected InReview or SentForApproval, got {currentStatus}");
        }

        var newStatus = approve ? StoryStatus.Approved : StoryStatus.ChangesRequested;
        hero.Status = newStatus.ToDb();
        hero.ReviewNotes = string.IsNullOrWhiteSpace(notes) ? hero.ReviewNotes : notes;
        hero.ReviewEndedAt = DateTime.UtcNow;
        hero.ReviewedByUserId = reviewerUserId;
        if (approve)
        {
            hero.ApprovedByUserId = reviewerUserId;
        }
        hero.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveAsync(hero, ct);
        _logger.LogInformation("Hero reviewed: heroId={HeroId} approved={Approve}", heroId, approve);
    }

    public async Task PublishAsync(Guid ownerUserId, string heroId, string ownerEmail, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
        {
            throw new InvalidOperationException($"Hero '{heroId}' not found");
        }

        if (hero.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own hero '{heroId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(hero.Status);
        if (currentStatus != StoryStatus.Approved)
        {
            throw new InvalidOperationException($"Cannot publish hero. Expected Approved, got {currentStatus}");
        }

        // Copy image asset synchronously if exists
        if (!string.IsNullOrWhiteSpace(hero.ImageUrl))
        {
            var publishedImageUrl = await PublishImageAsync(heroId, hero.ImageUrl, ownerEmail, ct);
            hero.ImageUrl = publishedImageUrl;
        }

        // Copy audio asset synchronously if exists
        if (!string.IsNullOrWhiteSpace(hero.GreetingAudioUrl))
        {
            var publishedAudioUrl = await PublishAudioAsync(heroId, hero.GreetingAudioUrl, ownerEmail, ct);
            hero.GreetingAudioUrl = publishedAudioUrl;
        }

        hero.Status = StoryStatus.Published.ToDb();
        hero.PublishedAtUtc = DateTime.UtcNow;
        hero.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveAsync(hero, ct);
        _logger.LogInformation("Hero published: heroId={HeroId}", heroId);
    }

    public async Task RetractAsync(Guid ownerUserId, string heroId, CancellationToken ct = default)
    {
        var hero = await _repository.GetAsync(heroId, ct);
        if (hero == null)
        {
            throw new InvalidOperationException($"Hero '{heroId}' not found");
        }

        if (hero.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own hero '{heroId}'");
        }

        var currentStatus = StoryStatusExtensions.FromDb(hero.Status);
        if (currentStatus != StoryStatus.SentForApproval && currentStatus != StoryStatus.Approved)
        {
            throw new InvalidOperationException($"Cannot retract hero. Expected SentForApproval or Approved, got {currentStatus}");
        }

        // Clear all review-related fields and revert to Draft (similar to RetractStoryEndpoint)
        hero.Status = StoryStatus.Draft.ToDb();
        hero.AssignedReviewerUserId = null;
        hero.ReviewNotes = null;
        hero.ReviewStartedAt = null;
        hero.ReviewEndedAt = null;
        if (currentStatus == StoryStatus.Approved)
        {
            hero.ApprovedByUserId = null;
        }
        hero.ReviewedByUserId = null; // Reset reviewed by as well
        hero.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveAsync(hero, ct);
        _logger.LogInformation("Hero retracted: heroId={HeroId}", heroId);
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

    private async Task<string> PublishAudioAsync(string heroId, string draftPath, string ownerEmail, CancellationToken ct)
    {
        var normalizedPath = NormalizeBlobPath(draftPath);
        if (IsAlreadyPublished(normalizedPath))
        {
            return normalizedPath; // Already published
        }

        var sourceClient = _blobSas.GetBlobClient(_blobSas.DraftContainer, normalizedPath);
        if (!await sourceClient.ExistsAsync(ct))
        {
            throw new InvalidOperationException($"Draft audio '{normalizedPath}' does not exist.");
        }

        var fileName = Path.GetFileName(normalizedPath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = $"{heroId}-greeting.mp3";
        }

        // Path format: audio/heroes/{heroId}/{fileName}
        var destinationPath = $"audio/heroes/{heroId}/{fileName}";
        var destinationClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, destinationPath);

        _logger.LogInformation("Copying hero audio from {Source} to {Destination}", normalizedPath, destinationPath);

        var sasUri = sourceClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(10));
        var operation = await destinationClient.StartCopyFromUriAsync(sasUri, cancellationToken: ct);
        await operation.WaitForCompletionAsync(cancellationToken: ct);

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
        return path.StartsWith("images/", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("audio/", StringComparison.OrdinalIgnoreCase);
    }
}

