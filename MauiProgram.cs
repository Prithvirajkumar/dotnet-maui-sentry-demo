using Microsoft.Extensions.Logging;
using SentryMauiDemo.Services;

namespace SentryMauiDemo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            
            // Configure Sentry
            .UseSentry(options =>
            {
                // The DSN is the only required setting.
                options.Dsn = "https://4892d51476eb1216b7951d9eadbb8464@o1161257.ingest.us.sentry.io/4510557120954368";

                // Use debug mode if you want to see what the SDK is doing.
                options.Debug = true;

                // Adds request URL and headers, IP and name for users, etc.
                options.SendDefaultPii = true;

                // Set TracesSampleRate to 1.0 to capture 100% of transactions for tracing.
                options.TracesSampleRate = 1.0;

                // Disable automatic instrumentation (we'll use custom instrumentation)
                options.DisableDiagnosticSourceIntegration();

                // Don't include PII in breadcrumbs
                options.IncludeTextInBreadcrumbs = false;
                options.IncludeTitleInBreadcrumbs = false;
                options.IncludeBackgroundingStateInBreadcrumbs = false;
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Register services
        builder.Services.AddSingleton<IDataService, DataService>();
        builder.Services.AddSingleton<IPaymentService, PaymentService>();
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddSingleton<IFileProcessingService, FileProcessingService>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}

