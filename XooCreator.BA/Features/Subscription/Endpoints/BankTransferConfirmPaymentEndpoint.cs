using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services.Email;

namespace XooCreator.BA.Features.Subscription.Endpoints;

/// <summary>
/// Called when user clicks "Confirmă plata" on /bank-transfer. Sends email to internal team and to user.
/// No auth required – order is identified by orderId; form data (name, email, amount, message) is used for the emails.
/// </summary>
[Endpoint]
public class BankTransferConfirmPaymentEndpoint
{
    private const string InternalEmail = "gamecraftmastersbv@gmail.com";

    private readonly XooDbContext _db;
    private readonly IResendEmailService _resend;

    public BankTransferConfirmPaymentEndpoint(XooDbContext db, IResendEmailService resend)
    {
        _db = db;
        _resend = resend;
    }

    public record ConfirmPaymentRequest(Guid OrderId, string? Name, string? Email, string? Amount, string? Message);

    [Route("/api/bank-transfer/confirm-payment")]
    public static async Task<Results<Ok<object>, BadRequest<string>, NotFound<string>>> HandlePost(
        [FromBody] ConfirmPaymentRequest? request,
        [FromServices] BankTransferConfirmPaymentEndpoint ep,
        CancellationToken ct)
    {
        if (request == null || request.OrderId == Guid.Empty)
            return TypedResults.BadRequest("OrderId is required.");

        var order = await ep._db.SupporterPackOrders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);
        if (order == null)
            return TypedResults.NotFound("Order not found.");
        if (order.Status != SupporterPackOrderStatus.PendingPayment)
            return TypedResults.BadRequest("Order is not in PendingPayment status.");

        var name = (request.Name ?? "").Trim();
        var email = (request.Email ?? "").Trim();
        var amount = (request.Amount ?? "").Trim();
        var message = (request.Message ?? "").Trim();

        // Email to internal team
        var internalSubject = $"Virament bancar – Supporter Pack – Order #{request.OrderId:N}";
        var internalBody = new StringBuilder();
        internalBody.AppendLine($"<p>Cerere confirmare plată pentru comanda <strong>{request.OrderId}</strong>.</p>");
        internalBody.AppendLine("<ul>");
        internalBody.AppendLine($"<li><strong>Plan:</strong> {WebUtility.HtmlEncode(order.PlanId)}</li>");
        internalBody.AppendLine($"<li><strong>Sumă comandă:</strong> {order.Amount} RON</li>");
        if (!string.IsNullOrEmpty(name)) internalBody.AppendLine($"<li><strong>Nume:</strong> {WebUtility.HtmlEncode(name)}</li>");
        if (!string.IsNullOrEmpty(email)) internalBody.AppendLine($"<li><strong>Email:</strong> {WebUtility.HtmlEncode(email)}</li>");
        if (!string.IsNullOrEmpty(amount)) internalBody.AppendLine($"<li><strong>Sumă declarată:</strong> {WebUtility.HtmlEncode(amount)}</li>");
        if (!string.IsNullOrEmpty(message)) internalBody.AppendLine($"<li><strong>Mesaj:</strong> {WebUtility.HtmlEncode(message)}</li>");
        internalBody.AppendLine("</ul>");
        await ep._resend.SendAsync(InternalEmail, internalSubject, internalBody.ToString(), ct);

        // Email to user (if provided) – they already made the transfer and clicked "Am făcut plata"
        if (!string.IsNullOrEmpty(email))
        {
            var userSubject = "Am primit cererea ta de plată – Supporter Pack Alchimalia";
            var userBody = $@"
<p>Bună{(string.IsNullOrEmpty(name) ? "" : " " + WebUtility.HtmlEncode(name))},</p>
<p>Am înregistrat cererea ta pentru <strong>Supporter Pack {WebUtility.HtmlEncode(order.PlanId)}</strong> (Order ID: <strong>{request.OrderId:N}</strong>).</p>
<p>Vom verifica plata făcută de tine; dacă toate sunt în ordine, în scurt timp îți vom activa pack-ul.</p>
<p>Mulțumim!</p>
<p>Echipa Alchimalia</p>";
            await ep._resend.SendAsync(email, userSubject, userBody, ct);
        }

        return TypedResults.Ok((object)new { Success = true, OrderId = request.OrderId });
    }
}
