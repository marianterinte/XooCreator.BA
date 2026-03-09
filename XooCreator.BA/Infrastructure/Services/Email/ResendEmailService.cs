using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;

namespace XooCreator.BA.Infrastructure.Services.Email;

/// <summary>
/// Sends transactional emails via Resend API (bank-transfer confirmations, order fulfilled, etc.).
/// Configure Resend:ApiKey (and optionally Resend:FromEmail, Resend:FromName) in appsettings or User Secrets.
/// </summary>
public interface IResendEmailService
{
    /// <summary>
    /// Sends an email. Returns true if sent successfully, false if Resend is not configured or send fails.
    /// </summary>
    Task<bool> SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
}

public class ResendEmailService : IResendEmailService
{
    private readonly IResend _resend;
    private readonly ResendOptions _options;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(IResend resend, IOptions<ResendOptions> options, ILogger<ResendEmailService> logger)
    {
        _resend = resend;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
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
                HtmlBody = htmlBody
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
}

public class ResendOptions
{
    public const string SectionName = "Resend";
    /// <summary>API key from Resend dashboard. Use User Secrets or env in production.</summary>
    public string ApiKey { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "Alchimalia";
}
