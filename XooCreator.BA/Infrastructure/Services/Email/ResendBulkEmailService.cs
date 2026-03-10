using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;

namespace XooCreator.BA.Infrastructure.Services.Email;

/// <summary>
/// Sends emails via Resend, implementing IEmailService so newsletter flow can send real emails
/// without relying on SMTP credentials.
/// </summary>
public class ResendBulkEmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly ResendOptions _options;
    private readonly ILogger<ResendBulkEmailService> _logger;

    public ResendBulkEmailService(IResend resend, IOptions<ResendOptions> options, ILogger<ResendBulkEmailService> logger)
    {
        _resend = resend;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody, string? textBody = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _logger.LogDebug("Resend send skipped (ApiKey not configured): To={To}, Subject={Subject}", to, subject);
            return false;
        }

        var from = string.IsNullOrWhiteSpace(_options.FromEmail)
            ? "Alchimalia <onboarding@resend.dev>"
            : string.IsNullOrWhiteSpace(_options.FromName)
                ? _options.FromEmail
                : $"{_options.FromName} <{_options.FromEmail}>";

        try
        {
            var message = new EmailMessage
            {
                From = from,
                Subject = subject,
                HtmlBody = htmlBody,
                TextBody = textBody
            };
            message.To.Add(to);
            await _resend.EmailSendAsync(message, ct);
            _logger.LogInformation("Resend email sent to {To}, Subject={Subject}", to, subject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Resend failed to send email to {To}: {Error}", to, ex.Message);
            return false;
        }
    }

    public async Task<bool> SendBulkEmailAsync(List<string> recipients, string subject, string htmlBody, string? textBody = null, CancellationToken ct = default)
    {
        if (recipients.Count == 0) return true;

        var successCount = 0;
        var failureCount = 0;

        // Simple implementation: one email per recipient via Resend.
        // This is reliable and keeps behavior consistent with existing newsletter flow.
        foreach (var recipient in recipients)
        {
            ct.ThrowIfCancellationRequested();
            var ok = await SendEmailAsync(recipient, subject, htmlBody, textBody, ct);
            if (ok) successCount++; else failureCount++;

            // Small delay to reduce provider throttling risk
            await Task.Delay(50, ct);
        }

        _logger.LogInformation("Resend bulk email completed: {SuccessCount} successful, {FailureCount} failed", successCount, failureCount);
        return failureCount == 0;
    }
}

