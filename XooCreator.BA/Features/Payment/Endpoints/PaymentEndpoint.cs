using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Features.Payment.DTOs;
using XooCreator.BA.Features.Payment.Services;

namespace XooCreator.BA.Features.Payment.Endpoints;

[Endpoint]
public class PaymentEndpoint
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentEndpoint> _logger;

    public PaymentEndpoint(IPaymentService paymentService, ILogger<PaymentEndpoint> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [Route("/api/payment/buymeacoffee/webhook")]
    public static async Task<Results<Ok<PaymentResponse>, BadRequest<PaymentResponse>>> HandleWebhook(
        [FromBody] BuyMeCoffeeWebhookRequest request,
        [FromServices] PaymentEndpoint ep)
    {
        try
        {
            ep._logger.LogInformation("Received Buy Me a Coffee webhook: {Type}", request.Type);
            var result = await ep._paymentService.ProcessWebhookAsync(request);
            if (result.Success) return TypedResults.Ok(result);
            return TypedResults.BadRequest(result);
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "Error processing Buy Me a Coffee webhook");
            return TypedResults.BadRequest(new PaymentResponse { Success = false, Message = "Internal server error" });
        }
    }

    [Route("/api/payment/status/{paymentId}")]
    public static async Task<Results<Ok<PaymentStatusResponse>, NotFound>> HandleGetPaymentStatus(
        [FromRoute] string paymentId,
        [FromServices] PaymentEndpoint ep)
    {
        try
        {
            var result = await ep._paymentService.GetPaymentStatusAsync(paymentId);
            return TypedResults.Ok(result);
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "Error getting payment status for {PaymentId}", paymentId);
            return TypedResults.NotFound();
        }
    }

    [Route("/api/payment/create")]
    public static async Task<Results<Ok<PaymentResponse>, BadRequest<PaymentResponse>>> HandleCreatePayment(
        [FromBody] CreatePaymentRequest request,
        [FromServices] PaymentEndpoint ep)
    {
        try
        {
            var response = new PaymentResponse
            {
                Success = true,
                Message = $"Please visit https://buymeacoffee.com/marian.terinte?amount={request.Amount} to complete your payment of {request.Amount} {request.Currency}",
                PaymentId = Guid.NewGuid().ToString()
            };
            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "Error creating payment");
            return TypedResults.BadRequest(new PaymentResponse { Success = false, Message = "Failed to create payment" });
        }
    }
}


