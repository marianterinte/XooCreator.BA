using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Services.Email;

namespace XooCreator.BA.Features.User.Services;

public interface INewsletterSendingService
{
    Task<NewsletterSendingResult> SendNewsletterToSubscribersAsync(
        string subject,
        string htmlContent,
        string? textContent = null,
        CancellationToken ct = default);
    
    Task<List<string>> GetSubscribedUserEmailsAsync(CancellationToken ct = default);
}

public class NewsletterSendingService : INewsletterSendingService
{
    private readonly XooDbContext _db;
    private readonly IEmailService _emailService;
    private readonly ILogger<NewsletterSendingService> _logger;

    public NewsletterSendingService(
        XooDbContext db,
        IEmailService emailService,
        ILogger<NewsletterSendingService> logger)
    {
        _db = db;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<List<string>> GetSubscribedUserEmailsAsync(CancellationToken ct = default)
    {
        var emails = await _db.AlchimaliaUsers
            .Where(u => u.IsNewsletterSubscribed && !string.IsNullOrEmpty(u.Email))
            .Select(u => u.Email)
            .Distinct()
            .ToListAsync(ct);

        _logger.LogInformation("Found {Count} subscribed users for newsletter", emails.Count);
        return emails;
    }

    public async Task<NewsletterSendingResult> SendNewsletterToSubscribersAsync(
        string subject,
        string htmlContent,
        string? textContent = null,
        CancellationToken ct = default)
    {
        try
        {
            var recipients = await GetSubscribedUserEmailsAsync(ct);

            if (recipients.Count == 0)
            {
                _logger.LogWarning("No subscribed users found for newsletter");
                return new NewsletterSendingResult
                {
                    Success = true,
                    TotalRecipients = 0,
                    SentCount = 0,
                    FailedCount = 0
                };
            }

            _logger.LogInformation("Sending newsletter to {Count} subscribers", recipients.Count);

            var success = await _emailService.SendBulkEmailAsync(recipients, subject, htmlContent, textContent, ct);

            // Note: SendBulkEmailAsync doesn't return detailed per-recipient results
            // For production, consider implementing a more detailed tracking system
            var result = new NewsletterSendingResult
            {
                Success = success,
                TotalRecipients = recipients.Count,
                SentCount = success ? recipients.Count : 0,
                FailedCount = success ? 0 : recipients.Count
            };

            _logger.LogInformation(
                "Newsletter sending completed: Success={Success}, Total={Total}, Sent={Sent}, Failed={Failed}",
                result.Success, result.TotalRecipients, result.SentCount, result.FailedCount);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending newsletter: {Error}", ex.Message);
            return new NewsletterSendingResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                TotalRecipients = 0,
                SentCount = 0,
                FailedCount = 0
            };
        }
    }
}

public class NewsletterSendingResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int TotalRecipients { get; set; }
    public int SentCount { get; set; }
    public int FailedCount { get; set; }
}
