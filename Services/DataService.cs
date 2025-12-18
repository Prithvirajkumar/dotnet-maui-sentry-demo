using Sentry;

namespace SentryMauiDemo.Services;

public class DataService : IDataService
{
    public async Task<string> FetchDataAsync(string endpoint)
    {
        // Start a custom transaction for data fetching
        var transaction = SentrySdk.StartTransaction(
            "data-fetch",
            "fetch-operation"
        );
        
        // Set transaction on scope
        SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);

        try
        {
            // Span for API call simulation
            var apiSpan = transaction.StartChild(
                "http.client",
                $"GET {endpoint}"
            );
            
            apiSpan.SetExtra("endpoint", endpoint);
            apiSpan.SetExtra("method", "GET");

            // Simulate API call delay
            await Task.Delay(Random.Shared.Next(500, 1500));
            
            apiSpan.Status = SpanStatus.Ok;
            apiSpan.Finish();

            // Span for data processing
            var processingSpan = transaction.StartChild(
                "data.processing",
                "Processing fetched data"
            );

            // Simulate data processing
            await Task.Delay(Random.Shared.Next(200, 800));
            
            var result = $"Data from {endpoint} at {DateTime.Now:HH:mm:ss}";
            
            processingSpan.SetExtra("data_size", result.Length);
            processingSpan.Status = SpanStatus.Ok;
            processingSpan.Finish();

            transaction.Status = SpanStatus.Ok;
            return result;
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

    public async Task<bool> ValidateDataAsync(string data)
    {
        var span = SentrySdk.GetSpan();
        
        if (span == null)
        {
            span = SentrySdk.StartTransaction("data-validation", "validation");
        }
        else
        {
            span = span.StartChild("data.validation", "Validating data");
        }

        try
        {
            // Simulate validation logic
            await Task.Delay(Random.Shared.Next(100, 500));
            
            var isValid = !string.IsNullOrEmpty(data) && data.Length > 10;
            
            span.SetExtra("data_length", data?.Length ?? 0);
            span.SetExtra("is_valid", isValid);
            span.Status = isValid ? SpanStatus.Ok : SpanStatus.InvalidArgument;
            
            return isValid;
        }
        catch (Exception ex)
        {
            span.Status = SpanStatus.InternalError;
            SentrySdk.CaptureException(ex);
            throw;
        }
        finally
        {
            span.Finish();
        }
    }
}

