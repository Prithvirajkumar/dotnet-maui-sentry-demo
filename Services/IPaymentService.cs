namespace SentryMauiDemo.Services;

public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(decimal amount, string currency);
    Task<string> GenerateInvoiceAsync(string paymentId);
}

