# Quick Start Guide

Get the Sentry MAUI Demo up and running in 5 minutes!

## Prerequisites Check

```bash
# Check .NET SDK version (should be 8.0 or higher)
dotnet --version

# Check if MAUI workload is installed
dotnet workload list
```

If MAUI is not listed, install it:
```bash
dotnet workload install maui
```

## Build and Run

### Option 1: Command Line

**For Windows:**
```bash
cd "/Users/prithvi/Development/2025/dotNET FE demo"
dotnet build -f net8.0-windows10.0.19041.0
dotnet run -f net8.0-windows10.0.19041.0
```

**For macOS (Mac Catalyst):**
```bash
cd "/Users/prithvi/Development/2025/dotNET FE demo"
dotnet build -f net8.0-maccatalyst
dotnet run -f net8.0-maccatalyst
```

**For Android:**
```bash
cd "/Users/prithvi/Development/2025/dotNET FE demo"
dotnet build -f net8.0-android
# Deploy to connected device/emulator
dotnet run -f net8.0-android
```

**For iOS (requires Mac with Xcode):**
```bash
cd "/Users/prithvi/Development/2025/dotNET FE demo"
dotnet build -f net8.0-ios
dotnet run -f net8.0-ios
```

### Option 2: Visual Studio

1. Open `SentryMauiDemo.sln` in Visual Studio 2022
2. Select your target platform from the dropdown (Windows Machine, Mac Catalyst, Android Emulator, etc.)
3. Press **F5** or click the **Run** button

### Option 3: Visual Studio for Mac

1. Open `SentryMauiDemo.sln` in Visual Studio for Mac
2. Select your target (Mac Catalyst, iOS Simulator, Android, etc.)
3. Press **⌘+Return** or click **Run**

## Testing Sentry Integration

Once the app is running:

1. **Click any button** to trigger a custom transaction
2. **Open Sentry Dashboard**: https://sentry.io
3. **View Performance Data**:
   - Navigate to **Performance** → **Transactions**
   - You should see transactions like `data-fetch`, `payment-checkout`, `invoice-generation`, etc.
4. **View Error Data**:
   - Click **"Throw Test Exception"** button in the app
   - Navigate to **Issues** in Sentry
   - You should see the captured exception

## What Each Button Does

| Button | Transaction Name | What It Demonstrates |
|--------|-----------------|---------------------|
| **Fetch Data** | `data-fetch` | HTTP simulation with processing spans |
| **Validate Data** | `data-validation` | Nested span creation |
| **Process Payment** | `payment-checkout` | Multi-step workflow (4 spans) |
| **Generate Invoice** | `invoice-generation` | Document generation workflow (3 spans) |
| **Throw Test Exception** | N/A | Error capture testing |
| **Complex Transaction** | `complex-operation` | Deeply nested spans (10+ spans) |

## Expected Output

### In the App
- Status messages showing operation results
- Alert dialogs with success/error information

### In Sentry Performance Tab
- Waterfall view of transaction spans
- Duration metrics for each operation
- Span details with custom metadata

### In Sentry Issues Tab
- Captured exceptions with full stack traces
- Device and runtime information
- User context (when available)

## Troubleshooting

### "Cannot find package Sentry.Maui"
```bash
dotnet restore --force
```

### "Project targets framework 'net8.0' which is not installed"
```bash
# Install .NET 8 SDK from:
# https://dotnet.microsoft.com/download/dotnet/8.0
```

### Build succeeds but app won't start
- Check that all fonts are present in `Resources/Fonts/`
- Ensure target platform SDK is installed
- For Windows: Check Windows SDK version
- For Mac: Check Xcode is installed
- For Android: Check Android SDK is configured

### No data appears in Sentry
- Check console output for Sentry debug messages
- Verify internet connection
- Confirm DSN is correct in `MauiProgram.cs`
- Wait a few seconds and refresh Sentry dashboard

## Next Steps

1. **Explore the Code**: Check out the `Services/` folder to see custom instrumentation examples
2. **Modify Transactions**: Edit `MainPage.xaml.cs` to add your own custom transactions
3. **Add More Spans**: Experiment with nested spans in the service classes
4. **Configure Sentry**: Adjust settings in `MauiProgram.cs` (sample rates, debug mode, etc.)

## Key Files to Explore

- 📁 `Services/DataService.cs` - HTTP request instrumentation
- 📁 `Services/PaymentService.cs` - Complex multi-span transactions
- 📁 `MainPage.xaml.cs` - UI event handlers with transactions
- 📁 `MauiProgram.cs` - Sentry SDK configuration

## Support

- 📚 [Full README](./README.md)
- 🔗 [Sentry .NET MAUI Docs](https://docs.sentry.io/platforms/dotnet/guides/maui/)
- 💬 [Sentry Community Forum](https://forum.sentry.io/)

Happy coding! 🚀

