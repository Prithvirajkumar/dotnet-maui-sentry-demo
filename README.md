# Sentry MAUI Demo Application

A comprehensive .NET MAUI demo application demonstrating **custom instrumentation** with the Sentry SDK for performance monitoring and error tracking across Windows, Mac, iOS, and Android platforms.

## 🎯 Overview

This application showcases:
- ✅ Custom transaction creation and management
- ✅ Nested span instrumentation
- ✅ Complex multi-span transactions
- ✅ Error capture and reporting
- ✅ Performance monitoring across different operations
- ✅ User context and metadata tracking

## 🚀 Features

### Custom Instrumentation Examples

1. **Data Operations**
   - Fetch data with HTTP client simulation
   - Nested spans for API calls and data processing
   - Custom metadata and status tracking

2. **Payment Processing**
   - Multi-step payment flow with detailed tracking
   - Spans for validation, gateway processing, database updates, and notifications
   - User context association

3. **Invoice Generation**
   - Document generation workflow
   - Storage upload tracking
   - Error handling and status management

4. **Complex Transactions**
   - Deeply nested span hierarchies
   - Complete checkout flow simulation
   - Multiple concurrent operations

5. **Error Testing**
   - Intentional exception throwing
   - Error capture verification
   - Sentry integration testing

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 17.8+ or Visual Studio for Mac 17.6+
  - With .NET MAUI workload installed
- For Windows: Windows 10 version 1809 or higher
- For Mac: macOS 11 or higher
- For iOS: Xcode 14.3 or higher
- For Android: Android SDK 21 or higher

## 🛠️ Installation

### 1. Clone or Download the Project

```bash
cd "/Users/prithvi/Development/2025/dotNET FE demo"
```

### 2. Install .NET MAUI Workload

If you haven't installed the .NET MAUI workload:

```bash
dotnet workload install maui
```

### 3. Restore NuGet Packages

```bash
dotnet restore
```

### 4. Add Required Assets

The project requires some asset files that are not included. You need to add:

#### Fonts (Required)
Download and place in `Resources/Fonts/`:
- `OpenSans-Regular.ttf`
- `OpenSans-Semibold.ttf`

Get them from: https://fonts.google.com/specimen/Open+Sans

#### Images (Optional)
Place a `dotnet_bot.png` or `dotnet_bot.svg` in `Resources/Images/` for the app icon.

#### App Icon & Splash (Optional)
Add basic SVG files to:
- `Resources/AppIcon/appicon.svg`
- `Resources/AppIcon/appiconfg.svg`
- `Resources/Splash/splash.svg`

Or remove these references from `SentryMauiDemo.csproj` if not needed.

## ▶️ Running the Application

### Windows

```bash
dotnet build -t:Run -f net8.0-windows10.0.19041.0
```

Or in Visual Studio:
1. Open `SentryMauiDemo.csproj`
2. Select "Windows Machine" as the target
3. Press F5 or click Run

### macOS (Mac Catalyst)

```bash
dotnet build -t:Run -f net8.0-maccatalyst
```

Or in Visual Studio for Mac:
1. Open `SentryMauiDemo.csproj`
2. Select "Mac Catalyst" as the target
3. Press ⌘+Return or click Run

### iOS (Requires Mac)

```bash
dotnet build -t:Run -f net8.0-ios
```

### Android

```bash
dotnet build -t:Run -f net8.0-android
```

## 🔧 Configuration

### Sentry Configuration

The Sentry DSN is configured in `MauiProgram.cs`:

```csharp
options.Dsn = "https://4892d51476eb1216b7951d9eadbb8464@o1161257.ingest.us.sentry.io/4510557120954368";
```

### Key Sentry Settings

- **TracesSampleRate**: Set to `1.0` (100%) - captures all transactions
- **Debug Mode**: Enabled for development to see SDK activity
- **SendDefaultPii**: Enabled to capture user context
- **Automatic Instrumentation**: Disabled to focus on custom instrumentation

## 📱 Using the Demo

When you run the app, you'll see several buttons to trigger different types of transactions:

### Data Operations Section (Blue)
- **Fetch Data**: Creates a transaction with HTTP and processing spans
- **Validate Data**: Demonstrates nested span creation

### Payment Operations Section (Green)
- **Process Payment**: Multi-step payment flow with validation, gateway, DB, and email spans
- **Generate Invoice**: Document generation workflow with DB, PDF, and storage spans

### Database Operations Section (Purple) - 🆕 DEEP NESTING
- **Complex DB Query (4 Levels Deep)**: Connection pool → Query execution (parse + optimize + index scan + fetch) → Result processing (deserialize + transform) → Cache update
- **Batch Insert (Dynamic Spans)**: Creates dynamic batches with nested operations: prepare → begin transaction → insert → commit (repeated per batch)
- **Database Optimization**: Multi-phase maintenance: analyze (tables + indexes) → vacuum → reindex (drop + build)

### File Processing Section (Orange) - 🆕 5 LEVELS DEEP
- **Process Large File Pipeline**: Read (open + buffer) → Parse (csv + validate) → Transform (normalize + enrich[lookup + merge]) → Write (serialize + compress + save)
- **Cloud File Sync (Parallel)**: Parallel file uploads with nested operations per file: check → upload (hash + transfer + verify)

### Advanced Testing Section (Red)
- **Throw Test Exception**: Tests error capture functionality
- **Complex Multi-Span Transaction**: Demonstrates a complete checkout flow with deeply nested spans (10+ spans, 3 levels)

## 🔍 Viewing Data in Sentry

After triggering transactions:

1. **Log into Sentry** at https://sentry.io
2. **Navigate to your project** (associated with the DSN)
3. **View Transactions**: Go to Performance → Transactions
4. **View Errors**: Go to Issues for captured exceptions
5. **Trace Details**: Click any transaction to see the complete span waterfall

## 📊 Custom Instrumentation Patterns

### Basic Transaction

```csharp
var transaction = SentrySdk.StartTransaction(
    "transaction-name",
    "operation-type"
);
SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);

try
{
    // Your code here
    transaction.Status = SpanStatus.Ok;
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
```

### Adding Child Spans

```csharp
var span = transaction.StartChild(
    "operation",
    "description"
);

span.SetExtra("key", "value");
span.Status = SpanStatus.Ok;
span.Finish();
```

### Nested Spans

```csharp
var parentSpan = transaction.StartChild("parent", "Parent operation");

var childSpan = parentSpan.StartChild("child", "Child operation");
childSpan.Finish();

parentSpan.Finish();
```

## 🏗️ Project Structure

```
SentryMauiDemo/
├── App.xaml / App.xaml.cs          # Application entry point
├── AppShell.xaml / AppShell.xaml.cs # Shell navigation
├── MauiProgram.cs                   # Sentry configuration
├── MainPage.xaml / MainPage.xaml.cs # Main UI and demos
├── Services/
│   ├── IDataService.cs
│   ├── DataService.cs               # Data fetching with custom transactions
│   ├── IPaymentService.cs
│   └── PaymentService.cs            # Payment processing with spans
├── Resources/
│   ├── Styles/                      # App styling
│   ├── Images/                      # Image assets
│   ├── Fonts/                       # Font files
│   └── ...
└── Platforms/
    ├── Android/                     # Android-specific code
    ├── iOS/                         # iOS-specific code
    ├── MacCatalyst/                 # Mac-specific code
    └── Windows/                     # Windows-specific code
```

## 📚 Key Files

- **`MauiProgram.cs`**: Sentry SDK initialization and configuration
- **`MainPage.xaml.cs`**: UI event handlers triggering custom transactions
- **`Services/DataService.cs`**: Example of HTTP request instrumentation
- **`Services/PaymentService.cs`**: Example of complex multi-span transactions

## 🐛 Troubleshooting

### Build Errors

1. **Missing workload**: Run `dotnet workload install maui`
2. **Package restore issues**: Run `dotnet restore --force`
3. **Missing assets**: Ensure fonts and images are in place (see Installation step 4)

### Runtime Issues

1. **No data in Sentry**: 
   - Check Debug output for Sentry messages
   - Verify DSN is correct
   - Ensure internet connectivity

2. **Crashes on startup**:
   - Check that all required assets are present
   - Verify target framework is installed

### Platform-Specific Issues

- **Windows**: Ensure Windows SDK 10.0.19041.0 or higher is installed
- **Mac**: Ensure Xcode Command Line Tools are installed
- **Android**: Ensure Android SDK is properly configured
- **iOS**: Ensure provisioning profiles are set up

## 📖 Learn More

- [Sentry .NET MAUI Documentation](https://docs.sentry.io/platforms/dotnet/guides/maui/)
- [Sentry Performance Monitoring](https://docs.sentry.io/product/performance/)
- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [Custom Instrumentation Guide](https://docs.sentry.io/platforms/dotnet/guides/maui/tracing/instrumentation/custom-instrumentation/)

## 📝 License

This is a demo application for educational purposes.

## 🤝 Contributing

This is a demo project. Feel free to fork and modify for your own use cases!

## 📬 Support

For Sentry-specific questions:
- [Sentry Documentation](https://docs.sentry.io/)
- [Sentry Community Forum](https://forum.sentry.io/)
- [Sentry GitHub Issues](https://github.com/getsentry/sentry-dotnet/issues)

For .NET MAUI questions:
- [Microsoft Q&A](https://learn.microsoft.com/en-us/answers/tags/304/dotnet-maui)
- [.NET MAUI GitHub](https://github.com/dotnet/maui)

---

**Happy Tracing! 🎉**

# dotnet-maui-sentry-demo
