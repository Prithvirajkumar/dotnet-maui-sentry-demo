using Sentry;
using SentryMauiDemo.Services;

namespace SentryMauiDemo;

public partial class MainPage : ContentPage
{
    private readonly IDataService _dataService;
    private readonly IPaymentService _paymentService;
    private readonly IDatabaseService _databaseService;
    private readonly IFileProcessingService _fileProcessingService;
    private int _requestCounter = 0;

    public MainPage(
        IDataService dataService, 
        IPaymentService paymentService,
        IDatabaseService databaseService,
        IFileProcessingService fileProcessingService)
    {
        InitializeComponent();
        _dataService = dataService;
        _paymentService = paymentService;
        _databaseService = databaseService;
        _fileProcessingService = fileProcessingService;
    }

    private async void OnFetchDataClicked(object sender, EventArgs e)
    {
        try
        {
            FetchDataBtn.IsEnabled = false;
            StatusLabel.Text = "Fetching data...";
            
            _requestCounter++;
            var endpoint = $"https://api.example.com/data/{_requestCounter}";
            
            var result = await _dataService.FetchDataAsync(endpoint);
            
            StatusLabel.Text = $"✓ Success: {result}";
            StatusLabel.TextColor = Colors.Green;
            
            await DisplayAlertAsync("Success", $"Data fetched successfully!\n{result}", "OK");
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"✗ Error: {ex.Message}";
            StatusLabel.TextColor = Colors.Red;
            SentrySdk.CaptureException(ex);
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            FetchDataBtn.IsEnabled = true;
        }
    }

    private async void OnValidateDataClicked(object sender, EventArgs e)
    {
        try
        {
            ValidateDataBtn.IsEnabled = false;
            StatusLabel.Text = "Validating data...";
            
            var testData = $"Sample data string with timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            var isValid = await _dataService.ValidateDataAsync(testData);
            
            StatusLabel.Text = isValid ? "✓ Data is valid" : "✗ Data is invalid";
            StatusLabel.TextColor = isValid ? Colors.Green : Colors.Orange;
            
            await DisplayAlertAsync("Validation Result", 
                isValid ? "Data is valid!" : "Data validation failed", 
                "OK");
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"✗ Error: {ex.Message}";
            StatusLabel.TextColor = Colors.Red;
            SentrySdk.CaptureException(ex);
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            ValidateDataBtn.IsEnabled = true;
        }
    }

    private async void OnProcessPaymentClicked(object sender, EventArgs e)
    {
        try
        {
            ProcessPaymentBtn.IsEnabled = false;
            StatusLabel.Text = "Processing payment...";
            
            var amount = Random.Shared.Next(10, 500);
            var result = await _paymentService.ProcessPaymentAsync(amount, "USD");
            
            StatusLabel.Text = result ? $"✓ Payment processed: ${amount}" : "✗ Payment failed";
            StatusLabel.TextColor = result ? Colors.Green : Colors.Red;
            
            await DisplayAlertAsync("Payment Result", 
                result ? $"Payment of ${amount} processed successfully!" : "Payment processing failed", 
                "OK");
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"✗ Error: {ex.Message}";
            StatusLabel.TextColor = Colors.Red;
            SentrySdk.CaptureException(ex);
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            ProcessPaymentBtn.IsEnabled = true;
        }
    }

    private async void OnGenerateInvoiceClicked(object sender, EventArgs e)
    {
        try
        {
            GenerateInvoiceBtn.IsEnabled = false;
            StatusLabel.Text = "Generating invoice...";
            
            var paymentId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var invoicePath = await _paymentService.GenerateInvoiceAsync(paymentId);
            
            StatusLabel.Text = $"✓ Invoice generated: {invoicePath}";
            StatusLabel.TextColor = Colors.Green;
            
            await DisplayAlertAsync("Invoice Generated", 
                $"Invoice created successfully!\n{invoicePath}", 
                "OK");
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"✗ Error: {ex.Message}";
            StatusLabel.TextColor = Colors.Red;
            SentrySdk.CaptureException(ex);
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            GenerateInvoiceBtn.IsEnabled = true;
        }
    }

    private async void OnThrowErrorClicked(object sender, EventArgs e)
    {
        try
        {
            ThrowErrorBtn.IsEnabled = false;
            StatusLabel.Text = "Throwing test exception...";
            
            // Intentionally throw an exception to test Sentry error capturing
            throw new InvalidOperationException("This is a test exception to verify Sentry integration!");
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"✗ Exception thrown and captured";
            StatusLabel.TextColor = Colors.Orange;
            SentrySdk.CaptureException(ex);
            await DisplayAlertAsync("Exception Captured", 
                $"Exception has been sent to Sentry:\n{ex.Message}", 
                "OK");
        }
        finally
        {
            ThrowErrorBtn.IsEnabled = true;
        }
    }

    private async void OnComplexTransactionClicked(object sender, EventArgs e)
    {
        try
        {
            ComplexTransactionBtn.IsEnabled = false;
            StatusLabel.Text = "Running complex transaction...";
            
            // Create a complex transaction with multiple nested spans
            var transaction = SentrySdk.StartTransaction(
                "complex-operation",
                "user-checkout-flow"
            );
            
            SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);

            try
            {
                transaction.SetTag("user_id", "demo-123");
                transaction.SetTag("username", "demo_user");
                transaction.SetTag("email", "demo@example.com");

                // Phase 1: User Authentication
                var authSpan = transaction.StartChild("auth", "Authenticating user");
                await Task.Delay(Random.Shared.Next(200, 500));
                authSpan.SetExtra("auth_method", "oauth2");
                authSpan.Status = SpanStatus.Ok;
                authSpan.Finish();

                // Phase 2: Cart validation
                var cartSpan = transaction.StartChild("cart.validate", "Validating shopping cart");
                await Task.Delay(Random.Shared.Next(300, 700));
                cartSpan.SetExtra("items_count", 3);
                cartSpan.SetExtra("total_amount", 99.99);
                cartSpan.Status = SpanStatus.Ok;
                cartSpan.Finish();

                // Phase 3: Address verification
                var addressSpan = transaction.StartChild("address.verify", "Verifying shipping address");
                await Task.Delay(Random.Shared.Next(400, 800));
                addressSpan.SetExtra("country", "US");
                addressSpan.Status = SpanStatus.Ok;
                addressSpan.Finish();

                // Phase 4: Payment processing (with nested spans)
                var paymentSpan = transaction.StartChild("payment.process", "Processing payment");
                
                var cardValidation = paymentSpan.StartChild("payment.validate", "Validating card");
                await Task.Delay(Random.Shared.Next(200, 400));
                cardValidation.Status = SpanStatus.Ok;
                cardValidation.Finish();

                var paymentGateway = paymentSpan.StartChild("payment.gateway", "Contacting payment gateway");
                await Task.Delay(Random.Shared.Next(500, 1000));
                paymentGateway.SetExtra("gateway", "stripe");
                paymentGateway.Status = SpanStatus.Ok;
                paymentGateway.Finish();

                paymentSpan.Status = SpanStatus.Ok;
                paymentSpan.Finish();

                // Phase 5: Order creation
                var orderSpan = transaction.StartChild("order.create", "Creating order");
                await Task.Delay(Random.Shared.Next(300, 600));
                orderSpan.SetExtra("order_id", Guid.NewGuid().ToString());
                orderSpan.Status = SpanStatus.Ok;
                orderSpan.Finish();

                // Phase 6: Notifications
                var notifySpan = transaction.StartChild("notifications", "Sending notifications");
                
                var emailSpan = notifySpan.StartChild("email.send", "Sending confirmation email");
                await Task.Delay(Random.Shared.Next(200, 400));
                emailSpan.Status = SpanStatus.Ok;
                emailSpan.Finish();

                var smsSpan = notifySpan.StartChild("sms.send", "Sending SMS notification");
                await Task.Delay(Random.Shared.Next(150, 300));
                smsSpan.Status = SpanStatus.Ok;
                smsSpan.Finish();

                notifySpan.Status = SpanStatus.Ok;
                notifySpan.Finish();

                transaction.Status = SpanStatus.Ok;
                
                StatusLabel.Text = "✓ Complex transaction completed successfully";
                StatusLabel.TextColor = Colors.Green;
                
                await DisplayAlertAsync("Success", 
                    "Complex transaction with multiple nested spans completed!\nCheck Sentry for the detailed trace.", 
                    "OK");
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
        catch (Exception ex)
        {
            StatusLabel.Text = $"✗ Error: {ex.Message}";
            StatusLabel.TextColor = Colors.Red;
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            ComplexTransactionBtn.IsEnabled = true;
        }
    }

    private async void OnComplexQueryClicked(object sender, EventArgs e)
    {
        try
        {
            ComplexQueryBtn.IsEnabled = false;
            StatusLabel.Text = "Executing complex database query...";
            
            var query = "SELECT u.*, o.*, p.* FROM users u JOIN orders o ON u.id = o.user_id JOIN products p ON o.product_id = p.id WHERE u.created_at > NOW() - INTERVAL '30 days'";
            var result = await _databaseService.PerformComplexQueryAsync(query);
            
            StatusLabel.Text = result ? "✓ Complex query executed successfully" : "✗ Query failed";
            StatusLabel.TextColor = result ? Colors.Green : Colors.Red;
            
            await DisplayAlertAsync("Query Complete", 
                "Complex database query with 4 levels of nested spans executed!\nCheck Sentry for connection pool → parsing → optimization → index scan → data fetch.", 
                "OK");
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"✗ Error: {ex.Message}";
            StatusLabel.TextColor = Colors.Red;
            SentrySdk.CaptureException(ex);
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            ComplexQueryBtn.IsEnabled = true;
        }
    }

    private async void OnBatchInsertClicked(object sender, EventArgs e)
    {
        try
        {
            BatchInsertBtn.IsEnabled = false;
            StatusLabel.Text = "Performing batch insert...";
            
            var recordCount = Random.Shared.Next(200, 400);
            var inserted = await _databaseService.BatchInsertAsync(recordCount);
            
            StatusLabel.Text = $"✓ Inserted {inserted} records in batches";
            StatusLabel.TextColor = Colors.Green;
            
            await DisplayAlertAsync("Batch Insert Complete", 
                $"Successfully inserted {inserted} records!\nCheck Sentry to see dynamic span creation for each batch with nested prepare → transaction → insert → commit spans.", 
                "OK");
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"✗ Error: {ex.Message}";
            StatusLabel.TextColor = Colors.Red;
            SentrySdk.CaptureException(ex);
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            BatchInsertBtn.IsEnabled = true;
        }
    }

    private async void OnOptimizeDatabaseClicked(object sender, EventArgs e)
    {
        try
        {
            OptimizeDatabaseBtn.IsEnabled = false;
            StatusLabel.Text = "Optimizing database...";
            
            await _databaseService.OptimizeDatabaseAsync();
            
            StatusLabel.Text = "✓ Database optimization complete";
            StatusLabel.TextColor = Colors.Green;
            
            await DisplayAlertAsync("Optimization Complete", 
                "Database optimized successfully!\nCheck Sentry for the maintenance workflow: analyze (tables + indexes) → vacuum → reindex (drop + build).", 
                "OK");
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"✗ Error: {ex.Message}";
            StatusLabel.TextColor = Colors.Red;
            SentrySdk.CaptureException(ex);
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            OptimizeDatabaseBtn.IsEnabled = true;
        }
    }

    private async void OnProcessFileClicked(object sender, EventArgs e)
    {
        try
        {
            ProcessFileBtn.IsEnabled = false;
            StatusLabel.Text = "Processing large file...";
            
            var filePath = "/data/large_dataset.csv";
            var outputFile = await _fileProcessingService.ProcessLargeFileAsync(filePath);
            
            StatusLabel.Text = $"✓ File processed: {outputFile}";
            StatusLabel.TextColor = Colors.Green;
            
            await DisplayAlertAsync("File Processing Complete", 
                $"File processed successfully!\nOutput: {outputFile}\n\nCheck Sentry for the 5-level deep pipeline:\nRead (open + buffer) → Parse (csv + validate) → Transform (normalize + enrich[lookup + merge]) → Write (serialize + compress + save)", 
                "OK");
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"✗ Error: {ex.Message}";
            StatusLabel.TextColor = Colors.Red;
            SentrySdk.CaptureException(ex);
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            ProcessFileBtn.IsEnabled = true;
        }
    }

    private async void OnCloudSyncClicked(object sender, EventArgs e)
    {
        try
        {
            CloudSyncBtn.IsEnabled = false;
            StatusLabel.Text = "Syncing files to cloud...";
            
            var fileCount = Random.Shared.Next(3, 6);
            var result = await _fileProcessingService.SyncFilesToCloudAsync(fileCount);
            
            StatusLabel.Text = result ? $"✓ Synced {fileCount} files to cloud" : "✗ Sync failed";
            StatusLabel.TextColor = result ? Colors.Green : Colors.Red;
            
            await DisplayAlertAsync("Cloud Sync Complete", 
                $"Successfully synced {fileCount} files to AWS S3!\n\nCheck Sentry to see parallel spans for each file with nested operations:\ncheck → upload (hash + transfer + verify)", 
                "OK");
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"✗ Error: {ex.Message}";
            StatusLabel.TextColor = Colors.Red;
            SentrySdk.CaptureException(ex);
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            CloudSyncBtn.IsEnabled = true;
        }
    }
}

