using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryCreatorsChallenge.DTOs;
using XooCreator.BA.Features.User.Services;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryCreatorsChallenge.Services;

public class StoryCreatorsChallengeService : IStoryCreatorsChallengeService
{
    private readonly XooDbContext _context;
    private readonly ILogger<StoryCreatorsChallengeService> _logger;
    private readonly ICreatorTokenService _creatorTokenService;
    private readonly IBlobSasService _blobSas;

    public StoryCreatorsChallengeService(
        XooDbContext context,
        ILogger<StoryCreatorsChallengeService> logger,
        ICreatorTokenService creatorTokenService,
        IBlobSasService blobSas)
    {
        _context = context;
        _logger = logger;
        _creatorTokenService = creatorTokenService;
        _blobSas = blobSas;
    }

    public async Task<List<StoryCreatorsChallengeListItemDto>> GetAllChallengesAsync(string? languageCode, CancellationToken ct)
    {
        var query = _context.StoryCreatorsChallenges
            .Include(c => c.Translations)
            .Include(c => c.Items)
            .OrderBy(c => c.SortOrder)
            .ThenByDescending(c => c.CreatedAt)
            .AsQueryable();

        var challenges = await query.ToListAsync(ct);

        var defaultLang = languageCode ?? "ro-ro";
        
        // Get all challenge IDs for batch query
        var challengeIds = challenges.Select(c => c.ChallengeId).ToList();
        
        // Get subscription counts for all challenges in one query
        var subscriptionCounts = await _context.StoryCreatorsChallengeSubscriptions
            .Where(s => challengeIds.Contains(s.ChallengeId))
            .GroupBy(s => s.ChallengeId)
            .Select(g => new { ChallengeId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ChallengeId, x => x.Count, ct);
        
        // Get submission counts for all challenges in one query
        var submissionCounts = await _context.StoryCreatorsChallengeSubmissions
            .Where(s => challengeIds.Contains(s.ChallengeId))
            .GroupBy(s => s.ChallengeId)
            .Select(g => new { ChallengeId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ChallengeId, x => x.Count, ct);
        
        return challenges.Select(c => new StoryCreatorsChallengeListItemDto
        {
            ChallengeId = c.ChallengeId,
            Status = c.Status,
            SortOrder = c.SortOrder,
            EndDate = c.EndDate,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            Topic = c.Translations
                .FirstOrDefault(t => t.LanguageCode.Equals(defaultLang, StringComparison.OrdinalIgnoreCase))?.Topic
                ?? c.Translations.FirstOrDefault()?.Topic
                ?? string.Empty,
            ItemsCount = c.Items.Count,
            IsExpired = c.EndDate.HasValue && c.EndDate.Value < DateTime.UtcNow,
            SubscriptionsCount = subscriptionCounts.GetValueOrDefault(c.ChallengeId, 0),
            SubmissionsCount = submissionCounts.GetValueOrDefault(c.ChallengeId, 0)
        }).ToList();
    }

    public async Task<StoryCreatorsChallengeDto?> GetChallengeByIdAsync(string challengeId, string? languageCode, CancellationToken ct)
    {
        var challenge = await _context.StoryCreatorsChallenges
            .Include(c => c.Translations)
            .Include(c => c.Items)
                .ThenInclude(i => i.Translations)
            .Include(c => c.Items)
                .ThenInclude(i => i.Rewards)
            .FirstOrDefaultAsync(c => c.ChallengeId == challengeId, ct);

        if (challenge == null) return null;

        var coverImageUrl = await ResolveCoverImageUrlAsync(challenge.CoverImageRelPath, challenge.CoverImageUrl, ct);

        return new StoryCreatorsChallengeDto
        {
            ChallengeId = challenge.ChallengeId,
            Status = challenge.Status,
            SortOrder = challenge.SortOrder,
            EndDate = challenge.EndDate,
            CoverImageUrl = coverImageUrl,
            CoverImageRelPath = challenge.CoverImageRelPath,
            CreatedAt = challenge.CreatedAt,
            UpdatedAt = challenge.UpdatedAt,
            Translations = challenge.Translations.Select(t => new StoryCreatorsChallengeTranslationDto
            {
                LanguageCode = t.LanguageCode,
                Topic = t.Topic,
                Description = t.Description
            }).ToList(),
            Items = challenge.Items
                .OrderBy(i => i.SortOrder)
                .Select(i => new ChallengeItemDto
                {
                    ItemId = i.ItemId,
                    SortOrder = i.SortOrder,
                    Translations = i.Translations.Select(t => new ChallengeItemTranslationDto
                    {
                        LanguageCode = t.LanguageCode,
                        Title = t.Title,
                        Description = t.Description
                    }).ToList(),
                    Rewards = i.Rewards
                        .OrderBy(r => r.SortOrder)
                        .Select(r => new ChallengeItemRewardDto
                        {
                            TokenType = r.TokenType,
                            TokenValue = r.TokenValue,
                            Quantity = r.Quantity,
                            SortOrder = r.SortOrder
                        }).ToList()
                }).ToList()
        };
    }

    public async Task<StoryCreatorsChallengeDto> CreateChallengeAsync(StoryCreatorsChallengeDto dto, Guid userId, CancellationToken ct)
    {
        var challenge = new XooCreator.BA.Data.Entities.StoryCreatorsChallenge
        {
            ChallengeId = dto.ChallengeId,
            Status = dto.Status,
            SortOrder = dto.SortOrder,
            EndDate = dto.EndDate,
            CoverImageUrl = dto.CoverImageUrl,
            CoverImageRelPath = dto.CoverImageRelPath,
            CreatedByUserId = userId,
            UpdatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var t in dto.Translations)
        {
            challenge.Translations.Add(new StoryCreatorsChallengeTranslation
            {
                ChallengeId = challenge.ChallengeId,
                LanguageCode = t.LanguageCode,
                Topic = t.Topic,
                Description = t.Description
            });
        }

        foreach (var itemDto in dto.Items)
        {
            var item = new StoryCreatorsChallengeItem
            {
                ChallengeId = challenge.ChallengeId,
                ItemId = itemDto.ItemId,
                SortOrder = itemDto.SortOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            foreach (var t in itemDto.Translations)
            {
                item.Translations.Add(new StoryCreatorsChallengeItemTranslation
                {
                    ItemId = item.ItemId,
                    LanguageCode = t.LanguageCode,
                    Title = t.Title,
                    Description = t.Description
                });
            }

            foreach (var r in itemDto.Rewards)
            {
                item.Rewards.Add(new StoryCreatorsChallengeItemReward
                {
                    ItemId = item.ItemId,
                    TokenType = r.TokenType,
                    TokenValue = r.TokenValue,
                    Quantity = r.Quantity,
                    SortOrder = r.SortOrder
                });
            }

            challenge.Items.Add(item);
        }

        _context.StoryCreatorsChallenges.Add(challenge);
        await _context.SaveChangesAsync(ct);

        return await GetChallengeByIdAsync(challenge.ChallengeId, null, ct) ?? throw new InvalidOperationException("Failed to retrieve created challenge");
    }

    public async Task<StoryCreatorsChallengeDto> UpdateChallengeAsync(string challengeId, StoryCreatorsChallengeDto dto, Guid userId, CancellationToken ct)
    {
        var challenge = await _context.StoryCreatorsChallenges
            .Include(c => c.Translations)
            .Include(c => c.Items)
                .ThenInclude(i => i.Translations)
            .Include(c => c.Items)
                .ThenInclude(i => i.Rewards)
            .FirstOrDefaultAsync(c => c.ChallengeId == challengeId, ct);

        if (challenge == null) throw new KeyNotFoundException($"Challenge {challengeId} not found");

        challenge.Status = dto.Status;
        challenge.SortOrder = dto.SortOrder;
        challenge.EndDate = dto.EndDate;
        challenge.CoverImageUrl = dto.CoverImageUrl;
        challenge.CoverImageRelPath = dto.CoverImageRelPath;
        challenge.UpdatedByUserId = userId;
        challenge.UpdatedAt = DateTime.UtcNow;

        // Update Translations
        UpdateTranslations(challenge.Translations, dto.Translations, challengeId);

        // Update Items
        UpdateItems(challenge.Items, dto.Items, challengeId);

        await _context.SaveChangesAsync(ct);
        
        return await GetChallengeByIdAsync(challengeId, null, ct) ?? throw new InvalidOperationException("Failed to retrieve updated challenge");
    }

    private void UpdateTranslations(ICollection<StoryCreatorsChallengeTranslation> existing, List<StoryCreatorsChallengeTranslationDto> dtos, string challengeId)
    {
        // Remove missing
        var codeSet = new HashSet<string>(dtos.Select(d => d.LanguageCode));
        var toRemove = existing.Where(e => !codeSet.Contains(e.LanguageCode)).ToList();
        foreach (var r in toRemove) existing.Remove(r);

        // Update or Add
        foreach (var d in dtos)
        {
            var e = existing.FirstOrDefault(x => x.LanguageCode == d.LanguageCode);
            if (e != null)
            {
                e.Topic = d.Topic;
                e.Description = d.Description;
            }
            else
            {
                existing.Add(new StoryCreatorsChallengeTranslation
                {
                    ChallengeId = challengeId,
                    LanguageCode = d.LanguageCode,
                    Topic = d.Topic,
                    Description = d.Description
                });
            }
        }
    }

    private void UpdateItems(ICollection<StoryCreatorsChallengeItem> existing, List<ChallengeItemDto> dtos, string challengeId)
    {
        // Remove missing
        var itemIdSet = new HashSet<string>(dtos.Select(d => d.ItemId));
        var toRemove = existing.Where(e => !itemIdSet.Contains(e.ItemId)).ToList();
        foreach (var r in toRemove) existing.Remove(r);

        // Update or Add
        foreach (var d in dtos)
        {
            var item = existing.FirstOrDefault(x => x.ItemId == d.ItemId);
            if (item != null)
            {
                item.SortOrder = d.SortOrder;
                item.UpdatedAt = DateTime.UtcNow;
                
                UpdateItemTranslations(item.Translations, d.Translations, item.ItemId);
                UpdateItemRewards(item.Rewards, d.Rewards, item.ItemId);
            }
            else
            {
                item = new StoryCreatorsChallengeItem
                {
                    ChallengeId = challengeId,
                    ItemId = d.ItemId,
                    SortOrder = d.SortOrder,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                foreach (var t in d.Translations)
                {
                    item.Translations.Add(new StoryCreatorsChallengeItemTranslation
                    {
                        ItemId = item.ItemId,
                        LanguageCode = t.LanguageCode,
                        Title = t.Title,
                        Description = t.Description
                    });
                }
                
                foreach (var r in d.Rewards)
                {
                    item.Rewards.Add(new StoryCreatorsChallengeItemReward
                    {
                        ItemId = item.ItemId,
                        TokenType = r.TokenType,
                        TokenValue = r.TokenValue,
                        Quantity = r.Quantity,
                        SortOrder = r.SortOrder
                    });
                }
                
                existing.Add(item);
            }
        }
    }

    private void UpdateItemTranslations(ICollection<StoryCreatorsChallengeItemTranslation> existing, List<ChallengeItemTranslationDto> dtos, string itemId)
    {
        var codeSet = new HashSet<string>(dtos.Select(d => d.LanguageCode));
        var toRemove = existing.Where(e => !codeSet.Contains(e.LanguageCode)).ToList();
        foreach (var r in toRemove) existing.Remove(r);

        foreach (var d in dtos)
        {
            var e = existing.FirstOrDefault(x => x.LanguageCode == d.LanguageCode);
            if (e != null)
            {
                e.Title = d.Title;
                e.Description = d.Description;
            }
            else
            {
                existing.Add(new StoryCreatorsChallengeItemTranslation
                {
                    ItemId = itemId,
                    LanguageCode = d.LanguageCode,
                    Title = d.Title,
                    Description = d.Description
                });
            }
        }
    }

    private void UpdateItemRewards(ICollection<StoryCreatorsChallengeItemReward> existing, List<ChallengeItemRewardDto> dtos, string itemId)
    {
        // Since rewards don't have unique IDs in DTO (user might just send list), 
        // a simple strategy is to clear and re-add if tracking individual rewards is not critical for history.
        // Or we can try to match by properties but that's duplicate-prone.
        // Given the UI is likely "add/remove" rewards, full replacement per item is safest/simplest.
        
        existing.Clear();
        foreach (var r in dtos)
        {
            existing.Add(new StoryCreatorsChallengeItemReward
            {
                ItemId = itemId,
                TokenType = r.TokenType,
                TokenValue = r.TokenValue,
                Quantity = r.Quantity,
                SortOrder = r.SortOrder
            });
        }
    }

    /// <summary>
    /// Resolves cover image URL using the environment-specific DraftContainer (from config).
    /// When CoverImageRelPath (blob path) is present, generates a fresh read SAS; otherwise returns stored URL.
    /// </summary>
    private async Task<string?> ResolveCoverImageUrlAsync(string? coverImageRelPath, string? storedCoverImageUrl, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(coverImageRelPath))
        {
            try
            {
                var readUri = await _blobSas.GetReadSasAsync(_blobSas.DraftContainer, coverImageRelPath, TimeSpan.FromHours(1), ct);
                return readUri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resolve CCC cover image SAS for blob path {BlobPath}, falling back to stored URL", coverImageRelPath);
            }
        }
        return storedCoverImageUrl;
    }

    public async Task<bool> DeleteChallengeAsync(string challengeId, CancellationToken ct)
    {
        var challenge = await _context.StoryCreatorsChallenges
            .FirstOrDefaultAsync(c => c.ChallengeId == challengeId, ct);

        if (challenge == null) return false;

        _context.StoryCreatorsChallenges.Remove(challenge);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<PublicChallengeDto?> GetActiveChallengeAsync(string languageCode, CancellationToken ct)
    {
        var challenge = await _context.StoryCreatorsChallenges
            .Include(c => c.Translations)
            .Include(c => c.Items)
                .ThenInclude(i => i.Translations)
            .Include(c => c.Items)
                .ThenInclude(i => i.Rewards)
            .Where(c => c.Status == "active")
            .OrderByDescending(c => c.CreatedAt) // Or SortOrder
            .FirstOrDefaultAsync(ct);

        if (challenge == null) return null;
        
        // Filter expired
        if (challenge.EndDate.HasValue && challenge.EndDate.Value < DateTime.UtcNow) return null;

        var lang = languageCode ?? "ro-ro";
        var translation = challenge.Translations.FirstOrDefault(t => t.LanguageCode == lang) 
                          ?? challenge.Translations.FirstOrDefault();
                          
        if (translation == null) return null; // Should have at least one translation

        var coverImageUrl = await ResolveCoverImageUrlAsync(challenge.CoverImageRelPath, challenge.CoverImageUrl, ct);

        return new PublicChallengeDto
        {
            ChallengeId = challenge.ChallengeId,
            Topic = translation.Topic,
            CoverImageUrl = coverImageUrl,
            Description = translation.Description,
            EndDate = challenge.EndDate,
            IsExpired = challenge.EndDate.HasValue && challenge.EndDate.Value < DateTime.UtcNow,
            Items = challenge.Items.OrderBy(i => i.SortOrder).Select(i => 
            {
                var itemTrans = i.Translations.FirstOrDefault(t => t.LanguageCode == lang) 
                                ?? i.Translations.FirstOrDefault();
                return new PublicChallengeItemDto
                {
                    ItemId = i.ItemId,
                    Title = itemTrans?.Title ?? "Untitled",
                    Description = itemTrans?.Description,
                    Rewards = i.Rewards.OrderBy(r => r.SortOrder).Select(r => new ChallengeItemRewardDto
                    {
                        TokenType = r.TokenType,
                        TokenValue = r.TokenValue,
                        Quantity = r.Quantity,
                        SortOrder = r.SortOrder
                    }).ToList()
                };
            }).ToList()
        };
    }

    public async Task<StoryCreatorsChallengeDto> ExtendChallengeEndDateAsync(string challengeId, DateTime newEndDate, Guid userId, CancellationToken ct)
    {
        var challenge = await _context.StoryCreatorsChallenges
             .Include(c => c.Translations) // Include to return full DTO
             .Include(c => c.Items)
             .FirstOrDefaultAsync(c => c.ChallengeId == challengeId, ct);
             
        if (challenge == null) throw new KeyNotFoundException($"Challenge {challengeId} not found");

        challenge.EndDate = newEndDate;
        challenge.UpdatedByUserId = userId;
        challenge.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return await GetChallengeByIdAsync(challengeId, null, ct) ?? throw new InvalidOperationException();
    }

    public async Task<ChallengeSubscriptionDto> SubscribeToChallengeAsync(string challengeId, Guid userId, CancellationToken ct)
    {
        var challenge = await _context.StoryCreatorsChallenges
            .FirstOrDefaultAsync(c => c.ChallengeId == challengeId, ct);
        
        if (challenge == null)
            throw new InvalidOperationException($"Challenge not found: {challengeId}");
        
        if (challenge.Status != "active")
            throw new InvalidOperationException("Can only subscribe to active challenges");
        
        var existing = await _context.StoryCreatorsChallengeSubscriptions
            .FirstOrDefaultAsync(s => s.ChallengeId == challengeId && s.UserId == userId, ct);
        
        if (existing != null)
            return new ChallengeSubscriptionDto
            {
                ChallengeId = challengeId,
                UserId = userId,
                SubscribedAt = existing.SubscribedAt,
                IsSubscribed = true
            };
        
        var subscription = new Data.Entities.StoryCreatorsChallengeSubscription
        {
            ChallengeId = challengeId,
            UserId = userId,
            SubscribedAt = DateTime.UtcNow
        };
        
        _context.StoryCreatorsChallengeSubscriptions.Add(subscription);
        await _context.SaveChangesAsync(ct);
        
        return new ChallengeSubscriptionDto
        {
            ChallengeId = challengeId,
            UserId = userId,
            SubscribedAt = subscription.SubscribedAt,
            IsSubscribed = true
        };
    }

    public async Task<bool> UnsubscribeFromChallengeAsync(string challengeId, Guid userId, CancellationToken ct)
    {
        var subscription = await _context.StoryCreatorsChallengeSubscriptions
            .FirstOrDefaultAsync(s => s.ChallengeId == challengeId && s.UserId == userId, ct);
        
        if (subscription == null) return false;
        
        _context.StoryCreatorsChallengeSubscriptions.Remove(subscription);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> IsSubscribedAsync(string challengeId, Guid userId, CancellationToken ct)
    {
        return await _context.StoryCreatorsChallengeSubscriptions
            .AnyAsync(s => s.ChallengeId == challengeId && s.UserId == userId, ct);
    }

    public async Task<List<ChallengeSubscriptionDto>> GetUserSubscriptionsAsync(Guid userId, CancellationToken ct)
    {
        return await _context.StoryCreatorsChallengeSubscriptions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SubscribedAt)
            .Select(s => new ChallengeSubscriptionDto
            {
                ChallengeId = s.ChallengeId,
                UserId = s.UserId,
                SubscribedAt = s.SubscribedAt,
                IsSubscribed = true
            })
            .ToListAsync(ct);
    }

    public async Task<ChallengeSubmissionDto> SubmitStoryToChallengeAsync(string challengeId, string storyId, Guid userId, CancellationToken ct)
    {
        var isSubscribed = await IsSubscribedAsync(challengeId, userId, ct);
        if (!isSubscribed)
            throw new InvalidOperationException("Must be subscribed to challenge before submitting");
        
        var challenge = await _context.StoryCreatorsChallenges
            .FirstOrDefaultAsync(c => c.ChallengeId == challengeId, ct);
        
        if (challenge == null || challenge.Status != "active")
            throw new InvalidOperationException("Can only submit to active challenges");
        
        var story = await _context.StoryDefinitions
            .FirstOrDefaultAsync(s => s.StoryId == storyId && s.CreatedBy == userId, ct);
        
        if (story == null)
            throw new InvalidOperationException("Story not found or user is not owner");
        
        var existing = await _context.StoryCreatorsChallengeSubmissions
            .FirstOrDefaultAsync(s => s.ChallengeId == challengeId && s.StoryId == storyId, ct);
        
        if (existing != null)
            throw new InvalidOperationException("Story already submitted to this challenge");
        
        var likesCount = await _context.StoryLikes
            .CountAsync(l => l.StoryId == storyId, ct);
        
        var submission = new Data.Entities.StoryCreatorsChallengeSubmission
        {
            ChallengeId = challengeId,
            StoryId = storyId,
            UserId = userId,
            SubmittedAt = DateTime.UtcNow,
            LikesCount = likesCount,
            IsWinner = false
        };
        
        _context.StoryCreatorsChallengeSubmissions.Add(submission);
        await _context.SaveChangesAsync(ct);
        
        var user = await _context.AlchimaliaUsers
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        return new ChallengeSubmissionDto
        {
            ChallengeId = challengeId,
            StoryId = storyId,
            UserId = userId,
            StoryTitle = story.Title,
            StoryCoverImageUrl = story.CoverImageUrl,
            AuthorFirstName = user?.FirstName,
            AuthorLastName = user?.LastName,
            SubmittedAt = submission.SubmittedAt,
            LikesCount = submission.LikesCount,
            IsWinner = false,
            IsCurrentUserSubmission = true
        };
    }

    public async Task<bool> RemoveSubmissionAsync(string challengeId, string storyId, Guid userId, CancellationToken ct)
    {
        var submission = await _context.StoryCreatorsChallengeSubmissions
            .FirstOrDefaultAsync(s => s.ChallengeId == challengeId && s.StoryId == storyId, ct);
        
        if (submission == null) return false;
        
        if (submission.UserId != userId)
             throw new UnauthorizedAccessException("Cannot remove submission owned by another user");
            
        _context.StoryCreatorsChallengeSubmissions.Remove(submission);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<ChallengeSubmissionDto>> GetUserSubmissionsAsync(string challengeId, Guid userId, CancellationToken ct)
    {
        var submissions = await _context.StoryCreatorsChallengeSubmissions
            .Where(s => s.ChallengeId == challengeId && s.UserId == userId)
            .ToListAsync(ct);
            
        var storyIds = submissions.Select(s => s.StoryId).ToList();
        var stories = await _context.StoryDefinitions
            .Where(s => storyIds.Contains(s.StoryId))
            .ToDictionaryAsync(s => s.StoryId, s => s, ct);

        var user = await _context.AlchimaliaUsers
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        return submissions.Select(s => new ChallengeSubmissionDto
        {
            ChallengeId = s.ChallengeId,
            StoryId = s.StoryId,
            UserId = s.UserId,
            StoryTitle = stories.GetValueOrDefault(s.StoryId)?.Title ?? s.StoryId,
            StoryCoverImageUrl = stories.GetValueOrDefault(s.StoryId)?.CoverImageUrl,
            AuthorFirstName = user?.FirstName,
            AuthorLastName = user?.LastName,
            SubmittedAt = s.SubmittedAt,
            LikesCount = s.LikesCount,
            IsWinner = s.IsWinner,
            IsCurrentUserSubmission = true
        }).ToList();
    }

    public async Task<ChallengeLeaderboardDto> GetChallengeLeaderboardAsync(string challengeId, Guid? currentUserId, CancellationToken ct)
    {
        // Get challenge to check if it's expired
        var challenge = await _context.StoryCreatorsChallenges
            .FirstOrDefaultAsync(c => c.ChallengeId == challengeId, ct);
        
        var isExpired = challenge?.EndDate.HasValue == true && challenge.EndDate.Value < DateTime.UtcNow;

        var submissions = await _context.StoryCreatorsChallengeSubmissions
            .Where(s => s.ChallengeId == challengeId)
            .OrderByDescending(s => s.LikesCount)
            .ThenBy(s => s.SubmittedAt)
            .ToListAsync(ct);
        
        var storyIds = submissions.Select(s => s.StoryId).ToList();
        var stories = await _context.StoryDefinitions
            .Where(s => storyIds.Contains(s.StoryId))
            .ToDictionaryAsync(s => s.StoryId, s => s, ct);

        var userIds = submissions.Select(s => s.UserId).Distinct().ToList();
        var users = await _context.AlchimaliaUsers
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u, ct);

        bool changes = false;
        foreach (var sub in submissions)
        {
             // Use exact match instead of ILike pattern matching
             var actualLikes = await _context.StoryLikes.CountAsync(l => l.StoryId == sub.StoryId, ct);
             if (sub.LikesCount != actualLikes) {
                 sub.LikesCount = actualLikes;
                 changes = true;
             }
        }
        
        if (changes) await _context.SaveChangesAsync(ct);
        
        submissions = submissions.OrderByDescending(s => s.LikesCount).ThenBy(s => s.SubmittedAt).ToList();

        var dtos = submissions.Select(s => 
        {
            var user = users.GetValueOrDefault(s.UserId);
            return new ChallengeSubmissionDto
            {
                ChallengeId = s.ChallengeId,
                StoryId = s.StoryId,
                UserId = s.UserId,
                StoryTitle = stories.GetValueOrDefault(s.StoryId)?.Title ?? s.StoryId,
                StoryCoverImageUrl = stories.GetValueOrDefault(s.StoryId)?.CoverImageUrl,
                AuthorFirstName = user?.FirstName,
                AuthorLastName = user?.LastName,
                SubmittedAt = s.SubmittedAt,
                LikesCount = s.LikesCount,
                IsWinner = s.IsWinner,
                IsCurrentUserSubmission = currentUserId.HasValue && s.UserId == currentUserId.Value
            };
        }).ToList();
        
        // Winner logic: Only show winner if challenge is expired AND it's the first submission (highest likes)
        // OR if IsWinner flag is explicitly set (for manually determined winners)
        ChallengeSubmissionDto? winner = null;
        if (isExpired && dtos.Any())
        {
            // If expired, winner is the first one (highest likes) OR one with IsWinner flag
            winner = dtos.FirstOrDefault(d => d.IsWinner) ?? dtos.FirstOrDefault();
        }
        else if (!isExpired)
        {
            // If not expired, only show winner if IsWinner flag is explicitly set
            winner = dtos.FirstOrDefault(d => d.IsWinner);
        }
        
        return new ChallengeLeaderboardDto
        {
            ChallengeId = challengeId,
            Submissions = dtos,
            Winner = winner
        };
    }

    public async Task<PublicChallengeLeaderboardDto> GetPublicLeaderboardAsync(string challengeId, CancellationToken ct)
    {
        var leaderboard = await GetChallengeLeaderboardAsync(challengeId, null, ct);
        
        return new PublicChallengeLeaderboardDto
        {
            ChallengeId = challengeId,
            Submissions = leaderboard.Submissions.Select(s => new PublicSubmissionDto
            {
                StoryId = s.StoryId,
                StoryTitle = s.StoryTitle ?? "",
                StoryCoverImageUrl = s.StoryCoverImageUrl,
                AuthorFirstName = s.AuthorFirstName,
                AuthorLastName = s.AuthorLastName,
                LikesCount = s.LikesCount,
                IsWinner = s.IsWinner
            }).ToList(),
            Winner = leaderboard.Winner == null ? null : new PublicSubmissionDto
            {
                StoryId = leaderboard.Winner.StoryId,
                StoryTitle = leaderboard.Winner.StoryTitle ?? "",
                StoryCoverImageUrl = leaderboard.Winner.StoryCoverImageUrl,
                AuthorFirstName = leaderboard.Winner.AuthorFirstName,
                AuthorLastName = leaderboard.Winner.AuthorLastName,
                LikesCount = leaderboard.Winner.LikesCount,
                IsWinner = leaderboard.Winner.IsWinner
            }
        };
    }

    public async Task<ChallengeSubmissionDto?> DetermineWinnerAsync(string challengeId, CancellationToken ct)
    {
        // Get challenge to check if expired and get rewards
        var challenge = await _context.StoryCreatorsChallenges
            .Include(c => c.Items)
                .ThenInclude(i => i.Rewards)
            .FirstOrDefaultAsync(c => c.ChallengeId == challengeId, ct);
        
        if (challenge == null) return null;
        
        var isExpired = challenge.EndDate.HasValue && challenge.EndDate.Value < DateTime.UtcNow;
        if (!isExpired)
        {
            throw new InvalidOperationException("Cannot determine winner before challenge end date");
        }
        
        var leaderboard = await GetChallengeLeaderboardAsync(challengeId, null, ct);
        if (!leaderboard.Submissions.Any()) return null;
        
        var winnerDto = leaderboard.Submissions.First();
        
        await _context.StoryCreatorsChallengeSubmissions
            .Where(s => s.ChallengeId == challengeId && s.IsWinner)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsWinner, false), ct);
            
        var submission = await _context.StoryCreatorsChallengeSubmissions
            .FirstOrDefaultAsync(s => s.ChallengeId == challengeId && s.StoryId == winnerDto.StoryId, ct);
            
        if (submission != null)
        {
            submission.IsWinner = true;
            await _context.SaveChangesAsync(ct);
            
            // Award tokens to winner from all challenge items
            var allRewards = challenge.Items
                .SelectMany(i => i.Rewards)
                .ToList();
            
            foreach (var reward in allRewards)
            {
                await _creatorTokenService.AwardTokenAsync(
                    winnerDto.UserId,
                    reward.TokenType,
                    reward.TokenValue,
                    reward.Quantity,
                    ct);
            }
            
            return winnerDto with { IsWinner = true };
        }
        
        return winnerDto;
    }

    public async Task<bool> SetWinnerManuallyAsync(string challengeId, string storyId, CancellationToken ct)
    {
        var submission = await _context.StoryCreatorsChallengeSubmissions
            .FirstOrDefaultAsync(s => s.ChallengeId == challengeId && s.StoryId == storyId, ct);
            
        if (submission == null) return false;
        
        await _context.StoryCreatorsChallengeSubmissions
             .Where(s => s.ChallengeId == challengeId && s.IsWinner)
             .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsWinner, false), ct);
             
        submission.IsWinner = true;
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
