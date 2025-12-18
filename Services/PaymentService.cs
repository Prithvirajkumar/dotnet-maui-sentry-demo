using Sentry;

namespace SentryMauiDemo.Services;

public class PaymentService : IPaymentService
{
    public async Task<bool> ProcessPaymentAsync(decimal amount, string currency)
    {
        // Create a transaction for the entire payment process
        var transaction = SentrySdk.StartTransaction(
            "payment-checkout",
            "process-payment"
        );
        
        SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);

        try
        {
            // Set transaction details
            transaction.SetExtra("amount", amount);
            transaction.SetExtra("currency", currency);
            transaction.SetTag("user_id", "demo-user-123");
            transaction.SetTag("username", "demo_user");

            // Span 1: Validate payment details
            var validationSpan = transaction.StartChild(
                "payment.validation",
                "Validating payment details"
            );
            
            await Task.Delay(Random.Shared.Next(300, 700));
            
            if (amount <= 0)
            {
                validationSpan.Status = SpanStatus.InvalidArgument;
                validationSpan.Finish();
                transaction.Status = SpanStatus.InvalidArgument;
                transaction.Finish();
                return false;
            }
            
            validationSpan.Status = SpanStatus.Ok;
            validationSpan.Finish();

            // Span 2: Process payment with payment gateway
            var gatewaySpan = transaction.StartChild(
                "payment.gateway",
                "Processing with payment gateway"
            );
            
            gatewaySpan.SetExtra("gateway", "StripeDemo");
            await Task.Delay(Random.Shared.Next(1000, 2000));
            
            gatewaySpan.Status = SpanStatus.Ok;
            gatewaySpan.Finish();

            // Span 3: Update database
            var dbSpan = transaction.StartChild(
                "db.update",
                "Updating payment records"
            );
            
            await Task.Delay(Random.Shared.Next(200, 500));
            
            dbSpan.Status = SpanStatus.Ok;
            dbSpan.Finish();

            // Span 4: Send confirmation email
            var emailSpan = transaction.StartChild(
                "email.send",
                "Sending confirmation email"
            );
            
            await Task.Delay(Random.Shared.Next(300, 600));
            
            emailSpan.Status = SpanStatus.Ok;
            emailSpan.Finish();

            transaction.Status = SpanStatus.Ok;
            return true;
        }
        catch (Exception ex)
        {
            transaction.Status = SpanStatus.InternalError;
            SentrySdk.CaptureException(ex);
            throw;
        }
        finally
        {
            transaction.Finish();
        }
    }

    public async Task<string> GenerateInvoiceAsync(string paymentId)
    {
        var transaction = SentrySdk.StartTransaction(
            "invoice-generation",
            "generate-invoice"
        );
        
        SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);

        try
        {
            transaction.SetExtra("payment_id", paymentId);

            // Span 1: Fetch payment details
            var fetchSpan = transaction.StartChild(
                "db.query",
                "Fetching payment details"
            );
            
            await Task.Delay(Random.Shared.Next(200, 500));
            fetchSpan.Status = SpanStatus.Ok;
            fetchSpan.Finish();

            // Span 2: Generate PDF
            var pdfSpan = transaction.StartChild(
                "pdf.generate",
                "Generating PDF invoice"
            );
            
            await Task.Delay(Random.Shared.Next(500, 1000));
            pdfSpan.Status = SpanStatus.Ok;
            pdfSpan.Finish();

            // Span 3: Upload to storage
            var uploadSpan = transaction.StartChild(
                "storage.upload",
                "Uploading invoice to cloud storage"
            );
            
            await Task.Delay(Random.Shared.Next(300, 700));
            uploadSpan.Status = SpanStatus.Ok;
            uploadSpan.Finish();

            transaction.Status = SpanStatus.Ok;
            return $"invoice-{paymentId}-{Guid.NewGuid():N}.pdf";
        }
        catch (Exception ex)
        {
            transaction.Status = SpanStatus.InternalError;
            SentrySdk.CaptureException(ex);
            throw;
        }
        finally
        {
            transaction.Finish();
        }
    }
}

