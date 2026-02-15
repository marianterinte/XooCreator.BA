using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.Subscription.Endpoints;

[Endpoint]
public class CreateStripeCheckoutEndpoint
{
    private readonly IAuth0UserService _auth0;
    private readonly IConfiguration _config;

    public CreateStripeCheckoutEndpoint(IAuth0UserService auth0, IConfiguration config)
    {
        _auth0 = auth0;
        _config = config;
    }

    public record CreateCheckoutRequest(string PlanId);
    public record CreateCheckoutResponse(string CheckoutUrl);

    /// <summary>
    /// Creates a Stripe Checkout Session for the Basic plan (1 EUR test).
    /// Requires Stripe:SecretKey and Stripe:SuccessUrl, Stripe:CancelUrl in config.
    /// </summary>
    [Route("/api/{locale}/subscription/checkout")]
    [Authorize]
    public static async Task<Results<Ok<CreateCheckoutResponse>, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromBody] CreateCheckoutRequest? body,
        [FromServices] CreateStripeCheckoutEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var planId = body?.PlanId?.Trim() ?? "";
        if (planId != "basic")
            return TypedResults.BadRequest("Only 'basic' plan is supported for checkout.");

        var secretKey = ep._config["Stripe:SecretKey"];
        if (string.IsNullOrWhiteSpace(secretKey))
            return TypedResults.BadRequest("Stripe is not configured. Add Stripe:SecretKey in appsettings.");

        var baseUrl = ep._config["Stripe:FrontendBaseUrl"] ?? ep._config["Frontend:BaseUrl"] ?? "https://localhost:4200";
        baseUrl = baseUrl.TrimEnd('/');
        var successUrl = ep._config["Stripe:SuccessUrl"] ?? $"{baseUrl}/subscription?success=1";
        var cancelUrl = ep._config["Stripe:CancelUrl"] ?? $"{baseUrl}/subscription?canceled=1";

        StripeConfiguration.ApiKey = secretKey;

        var options = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            CustomerEmail = user.Email ?? null,
            Metadata = new Dictionary<string, string>
            {
                ["user_id"] = user.Id.ToString(),
                ["plan_id"] = "basic"
            },
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "ron",
                        UnitAmount = 1000, // 10 RON
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Supporter Pack - 10 RON/lună",
                            Description = "Abonament Basic – test de plată"
                        }
                    },
                    Quantity = 1
                }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options, null, ct);

        return TypedResults.Ok(new CreateCheckoutResponse(session.Url!));
    }
}
