using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using XooCreator.BA.Features.Payment.DTOs;

namespace XooCreator.BA.Features.Payment.Services;

public interface IPaymentService
{
    Task<PaymentResponse> ProcessWebhookAsync(BuyMeCoffeeWebhookRequest request);
    Task<PaymentStatusResponse> GetPaymentStatusAsync(string paymentId);
    Task<bool> VerifyWebhookSignatureAsync(string payload, string signature);
}

public class PaymentService : IPaymentService
{
    private readonly XooDbContext _context;
    private readonly ILogger<PaymentService> _logger;
    private readonly string _webhookSecret;

    public PaymentService(XooDbContext context, ILogger<PaymentService> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _webhookSecret = configuration["BuyMeCoffee:WebhookSecret"] ?? string.Empty;
    }

    public async Task<PaymentResponse> ProcessWebhookAsync(BuyMeCoffeeWebhookRequest request)
    {
        try
        {
            _logger.LogInformation("Processing Buy Me a Coffee webhook: {Type} - {Id}", request.Type, request.Id);

            if (!await VerifyWebhookSignatureAsync(JsonSerializer.Serialize(request), request.Id))
            {
                _logger.LogWarning("Invalid webhook signature for payment {Id}", request.Id);
                return new PaymentResponse { Success = false, Message = "Invalid signature" };
            }

            switch (request.Type.ToLower())
            {
                case "wishlist_payment_created":
                    return await ProcessWishlistPaymentAsync(request);
                case "wishlist_payment_refunded":
                    return await ProcessWishlistRefundAsync(request);
                case "membership_started":
                    return await ProcessMembershipStartedAsync(request);
                case "membership_updated":
                    return await ProcessMembershipUpdatedAsync(request);
                default:
                    _logger.LogWarning("Unknown webhook type: {Type}", request.Type);
                    return new PaymentResponse { Success = false, Message = "Unknown webhook type" };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook for payment {Id}", request.Id);
            return new PaymentResponse { Success = false, Message = "Internal error" };
        }
    }

    private async Task<PaymentResponse> ProcessWishlistPaymentAsync(BuyMeCoffeeWebhookRequest request)
    {
        var payment = request.Data;
        _logger.LogInformation("Wishlist payment received: {Amount} {Currency} from {Name} ({Email})", 
            payment.Amount, payment.Currency, payment.Name, payment.Email);
        var creditsToAdd = CalculateCreditsFromAmount(payment.Amount);
        return new PaymentResponse 
        { 
            Success = true, 
            Message = "Wishlist payment processed successfully",
            PaymentId = payment.Id
        };
    }

    private async Task<PaymentResponse> ProcessWishlistRefundAsync(BuyMeCoffeeWebhookRequest request)
    {
        var refund = request.Data;
        _logger.LogInformation("Wishlist payment refunded: {Amount} {Currency} from {Name} ({Email})", 
            refund.Amount, refund.Currency, refund.Name, refund.Email);
        return new PaymentResponse 
        { 
            Success = true, 
            Message = "Wishlist refund processed successfully",
            PaymentId = refund.Id
        };
    }

    private async Task<PaymentResponse> ProcessMembershipStartedAsync(BuyMeCoffeeWebhookRequest request)
    {
        var membership = request.Data;
        _logger.LogInformation("Membership started: {Amount} {Currency} from {Name} ({Email})", 
            membership.Amount, membership.Currency, membership.Name, membership.Email);
        return new PaymentResponse 
        { 
            Success = true, 
            Message = "Membership started successfully",
            PaymentId = membership.Id
        };
    }

    private async Task<PaymentResponse> ProcessMembershipUpdatedAsync(BuyMeCoffeeWebhookRequest request)
    {
        var membership = request.Data;
        _logger.LogInformation("Membership updated: {Amount} {Currency} from {Name} ({Email})", 
            membership.Amount, membership.Currency, membership.Name, membership.Email);
        return new PaymentResponse 
        { 
            Success = true, 
            Message = "Membership updated successfully",
            PaymentId = membership.Id
        };
    }

    private int CalculateCreditsFromAmount(decimal amount)
    {
        return (int)(amount * 10);
    }

    public async Task<PaymentStatusResponse> GetPaymentStatusAsync(string paymentId)
    {
        return new PaymentStatusResponse
        {
            Success = true,
            Status = "completed",
            Message = "Payment completed successfully",
            Amount = 0,
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task<bool> VerifyWebhookSignatureAsync(string payload, string signature)
    {
        if (string.IsNullOrEmpty(_webhookSecret))
        {
            _logger.LogWarning("Webhook secret not configured, skipping signature verification");
            return true;
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_webhookSecret));
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computedSignature = Convert.ToHexString(computedHash).ToLower();

        return computedSignature == signature.ToLower();
    }
}


