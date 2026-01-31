using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.User.DTOs;

namespace XooCreator.BA.Features.User.Services;

public interface INewsletterSubscriptionService
{
    Task<UpdateNewsletterSubscriptionResponse> UpdateSubscriptionAsync(Guid userId, bool isSubscribed, CancellationToken ct = default);
    Task<GetNewsletterSubscriptionResponse> GetSubscriptionStatusAsync(Guid userId, CancellationToken ct = default);
}

public class NewsletterSubscriptionService : INewsletterSubscriptionService
{
    private readonly XooDbContext _db;

    public NewsletterSubscriptionService(XooDbContext db)
    {
        _db = db;
    }

    public async Task<UpdateNewsletterSubscriptionResponse> UpdateSubscriptionAsync(Guid userId, bool isSubscribed, CancellationToken ct = default)
    {
        try
        {
            var user = await _db.AlchimaliaUsers.FirstOrDefaultAsync(u => u.Id == userId, ct);
            if (user == null)
            {
                return new UpdateNewsletterSubscriptionResponse
                {
                    Success = false,
                    ErrorMessage = "User not found",
                    IsSubscribed = false
                };
            }

            var now = DateTime.UtcNow;
            user.IsNewsletterSubscribed = isSubscribed;
            user.UpdatedAt = now;

            if (isSubscribed)
            {
                // If subscribing and wasn't subscribed before, set subscribed date
                if (user.NewsletterSubscribedAt == null)
                {
                    user.NewsletterSubscribedAt = now;
                }
                user.NewsletterUnsubscribedAt = null;
            }
            else
            {
                // If unsubscribing, set unsubscribed date
                user.NewsletterUnsubscribedAt = now;
            }

            await _db.SaveChangesAsync(ct);

            return new UpdateNewsletterSubscriptionResponse
            {
                Success = true,
                IsSubscribed = user.IsNewsletterSubscribed
            };
        }
        catch (Exception ex)
        {
            return new UpdateNewsletterSubscriptionResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                IsSubscribed = false
            };
        }
    }

    public async Task<GetNewsletterSubscriptionResponse> GetSubscriptionStatusAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var user = await _db.AlchimaliaUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user == null)
            {
                return new GetNewsletterSubscriptionResponse
                {
                    Success = false,
                    ErrorMessage = "User not found",
                    IsSubscribed = false
                };
            }

            return new GetNewsletterSubscriptionResponse
            {
                Success = true,
                IsSubscribed = user.IsNewsletterSubscribed,
                SubscribedAt = user.NewsletterSubscribedAt,
                UnsubscribedAt = user.NewsletterUnsubscribedAt
            };
        }
        catch (Exception ex)
        {
            return new GetNewsletterSubscriptionResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                IsSubscribed = false
            };
        }
    }
}
