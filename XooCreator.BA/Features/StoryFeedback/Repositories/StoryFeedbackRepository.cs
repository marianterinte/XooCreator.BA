using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using StoryFeedbackEntity = XooCreator.BA.Data.StoryFeedback;
using StoryFeedbackPreferenceEntity = XooCreator.BA.Data.StoryFeedbackPreference;

namespace XooCreator.BA.Features.StoryFeedback.Repositories;

public class StoryFeedbackRepository : IStoryFeedbackRepository
{
    private readonly XooDbContext _db;

    public StoryFeedbackRepository(XooDbContext db)
    {
        _db = db;
    }

    public async Task<bool> HasUserSubmittedFeedbackAsync(Guid userId, string storyId, CancellationToken ct = default)
    {
        return await _db.StoryFeedbacks
            .AnyAsync(f => f.UserId == userId && f.StoryId == storyId, ct);
    }

    public async Task<bool> HasUserDeclinedFeedbackAsync(Guid userId, string storyId, CancellationToken ct = default)
    {
        return await _db.StoryFeedbackPreferences
            .AnyAsync(p => p.UserId == userId && p.StoryId == storyId && p.PreferenceType == FeedbackPreferenceType.Never, ct);
    }

    public async Task<bool> HasUserPostponedFeedbackAsync(Guid userId, string storyId, CancellationToken ct = default)
    {
        return await _db.StoryFeedbackPreferences
            .AnyAsync(p => p.UserId == userId && p.StoryId == storyId && p.PreferenceType == FeedbackPreferenceType.Later, ct);
    }

    public async Task<StoryFeedbackEntity?> CreateFeedbackAsync(Guid userId, string storyId, string email, string feedbackText, List<string> whatLiked, List<string> whatDisliked, List<string> whatCouldBeBetter, CancellationToken ct = default)
    {
        // Check if feedback already exists
        var existing = await _db.StoryFeedbacks
            .FirstOrDefaultAsync(f => f.UserId == userId && f.StoryId == storyId, ct);
        
        if (existing != null)
            return null; // Already submitted

        var feedback = new StoryFeedbackEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            StoryId = storyId,
            Email = email,
            FeedbackText = feedbackText,
            WhatLiked = whatLiked ?? new List<string>(),
            WhatDisliked = whatDisliked ?? new List<string>(),
            WhatCouldBeBetter = whatCouldBeBetter ?? new List<string>(),
            CreatedAt = DateTime.UtcNow
        };

        _db.StoryFeedbacks.Add(feedback);

        // Also create preference as "submitted"
        var preference = await _db.StoryFeedbackPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId && p.StoryId == storyId, ct);
        
        if (preference != null)
        {
            preference.PreferenceType = FeedbackPreferenceType.Submitted;
            preference.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _db.StoryFeedbackPreferences.Add(new StoryFeedbackPreferenceEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StoryId = storyId,
                PreferenceType = FeedbackPreferenceType.Submitted,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync(ct);
        return feedback;
    }

    public async Task<StoryFeedbackPreferenceEntity?> CreateOrUpdatePreferenceAsync(Guid userId, string storyId, FeedbackPreferenceType preferenceType, CancellationToken ct = default)
    {
        var preference = await _db.StoryFeedbackPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId && p.StoryId == storyId, ct);

        if (preference != null)
        {
            preference.PreferenceType = preferenceType;
            preference.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            preference = new StoryFeedbackPreferenceEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StoryId = storyId,
                PreferenceType = preferenceType,
                CreatedAt = DateTime.UtcNow
            };
            _db.StoryFeedbackPreferences.Add(preference);
        }

        await _db.SaveChangesAsync(ct);
        return preference;
    }

    public async Task<List<StoryFeedbackEntity>> GetAllFeedbacksAsync(CancellationToken ct = default)
    {
        return await _db.StoryFeedbacks
            .AsNoTracking()
            .Include(f => f.User)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(ct);
    }
}

