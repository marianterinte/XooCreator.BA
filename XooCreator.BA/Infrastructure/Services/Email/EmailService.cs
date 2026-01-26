using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Infrastructure.Services.Email;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string htmlBody, string? textBody = null, CancellationToken ct = default);
    Task<bool> SendBulkEmailAsync(List<string> recipients, string subject, string htmlBody, string? textBody = null, CancellationToken ct = default);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly EmailSettings _settings;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _settings = new EmailSettings
        {
            SmtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com",
            SmtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587"),
            SmtpUsername = _configuration["Email:SmtpUsername"] ?? string.Empty,
            SmtpPassword = _configuration["Email:SmtpPassword"] ?? string.Empty,
            FromEmail = _configuration["Email:FromEmail"] ?? "noreply@alchimalia.com",
            FromName = _configuration["Email:FromName"] ?? "Alchimalia"
        };
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody, string? textBody = null, CancellationToken ct = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = textBody ?? StripHtml(htmlBody)
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls, ct);
            
            if (!string.IsNullOrEmpty(_settings.SmtpUsername) && !string.IsNullOrEmpty(_settings.SmtpPassword))
            {
                await client.AuthenticateAsync(_settings.SmtpUsername, _settings.SmtpPassword, ct);
            }

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            _logger.LogInformation("Email sent successfully to {To}", to);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}: {Error}", to, ex.Message);
            return false;
        }
    }

    public async Task<bool> SendBulkEmailAsync(List<string> recipients, string subject, string htmlBody, string? textBody = null, CancellationToken ct = default)
    {
        var successCount = 0;
        var failureCount = 0;

        foreach (var recipient in recipients)
        {
            var success = await SendEmailAsync(recipient, subject, htmlBody, textBody, ct);
            if (success)
                successCount++;
            else
                failureCount++;

            // Small delay to avoid rate limiting
            await Task.Delay(100, ct);
        }

        _logger.LogInformation("Bulk email sent: {SuccessCount} successful, {FailureCount} failed", successCount, failureCount);
        return failureCount == 0;
    }

    private static string StripHtml(string html)
    {
        // Simple HTML stripping - for production, consider using HtmlAgilityPack
        return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
    }
}

public class EmailSettings
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}
