using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.Subscription.Endpoints;

[Endpoint]
public class StripeWebhookEndpoint
{
    private readonly XooDbContext _db;
    private readonly IConfiguration _config;

    public StripeWebhookEndpoint(XooDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    /// <summary>
    /// Stripe webhook for checkout.session.completed. No locale, no [Authorize].
    /// Reads raw body, verifies signature, inserts UserSubscription (idempotent by StripeSessionId).
    /// </summary>
    [Route("/api/webhooks/stripe")]
    [AllowAnonymous]
    public static async Task<IResult> HandlePost(
        HttpContext httpContext,
        [FromServices] StripeWebhookEndpoint ep,
        CancellationToken ct)
    {
        var request = httpContext.Request;
        request.EnableBuffering();
        request.Body.Position = 0;

        string body;
        using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
        {
            body = await reader.ReadToEndAsync(ct);
        }

        request.Body.Position = 0;

        var signature = request.Headers["Stripe-Signature"].FirstOrDefault();
        if (string.IsNullOrEmpty(signature))
            return Results.BadRequest("Missing Stripe-Signature header");

        var webhookSecret = ep._config["Stripe:WebhookSecret"];
        if (string.IsNullOrWhiteSpace(webhookSecret))
            return Results.StatusCode(500);

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(body, signature, webhookSecret);
        }
        catch (StripeException)
        {
            return Results.BadRequest("Invalid signature");
        }

        if (stripeEvent.Type != "checkout.session.completed")
            return Results.Ok();

        var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
        if (session == null)
            return Results.Ok();

        var sessionId = session.Id;
        if (string.IsNullOrEmpty(sessionId))
            return Results.Ok();

        if (!session.Metadata.TryGetValue("user_id", out var userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return Results.Ok();

        var planId = session.Metadata.TryGetValue("plan_id", out var p) ? p : "basic";

        var paidAtUtc = session.Created != default
            ? DateTime.SpecifyKind(session.Created, DateTimeKind.Utc)
            : DateTime.UtcNow;

        var exists = await ep._db.UserSubscriptions
            .AsNoTracking()
            .AnyAsync(u => u.StripeSessionId == sessionId, ct);
        if (exists)
            return Results.Ok();

        var sub = new UserSubscription
        {
            UserId = userId,
            PlanId = planId,
            StripeSessionId = sessionId,
            StripeCustomerId = session.CustomerId,
            StripePaymentIntentId = session.PaymentIntentId,
            PaidAtUtc = paidAtUtc,
            ExpiresAtUtc = paidAtUtc.AddDays(30),
            CreatedAtUtc = DateTime.UtcNow
        };
        ep._db.UserSubscriptions.Add(sub);
        await ep._db.SaveChangesAsync(ct);

        return Results.Ok();
    }
}
