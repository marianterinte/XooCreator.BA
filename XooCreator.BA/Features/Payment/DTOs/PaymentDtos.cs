namespace XooCreator.BA.Features.Payment.DTOs;

public class BuyMeCoffeeWebhookRequest
{
    public string Type { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public BuyMeCoffeeData Data { get; set; } = new();
}

public class BuyMeCoffeeData
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public BuyMeCoffeeSupporter Supporter { get; set; } = new();
}

public class BuyMeCoffeeSupporter
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ProfileUrl { get; set; } = string.Empty;
}

public class PaymentResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? PaymentId { get; set; }
}

public class CreatePaymentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public string Message { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class PaymentStatusResponse
{
    public bool Success { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}


