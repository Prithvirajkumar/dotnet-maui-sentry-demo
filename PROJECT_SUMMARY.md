# Sentry MAUI Demo - Project Summary

## 📋 Overview

This is a complete **.NET MAUI cross-platform application** demonstrating **Sentry SDK custom instrumentation** for performance monitoring and error tracking. The application works on **Windows, macOS, iOS, and Android**.

## ✅ What's Included

### Core Application Files
- ✅ `SentryMauiDemo.csproj` - Project configuration with Sentry.Maui package
- ✅ `SentryMauiDemo.sln` - Visual Studio solution file
- ✅ `MauiProgram.cs` - Sentry SDK initialization and configuration
- ✅ `App.xaml` / `App.xaml.cs` - Application entry point
- ✅ `AppShell.xaml` / `AppShell.xaml.cs` - Shell navigation
- ✅ `MainPage.xaml` / `MainPage.xaml.cs` - Main UI with **11 demo buttons**

### Custom Instrumentation Services (10 Transactions Total)
- ✅ `Services/IDataService.cs` - Data service interface
- ✅ `Services/DataService.cs` - HTTP request simulation with custom transactions (2 transactions)
- ✅ `Services/IPaymentService.cs` - Payment service interface
- ✅ `Services/PaymentService.cs` - Payment processing with multi-span transactions (2 transactions)
- ✅ `Services/IDatabaseService.cs` - Database service interface 🆕
- ✅ `Services/DatabaseService.cs` - Complex database operations (3 transactions, up to 4 levels deep) 🆕
- ✅ `Services/IFileProcessingService.cs` - File processing service interface 🆕
- ✅ `Services/FileProcessingService.cs` - File pipeline and cloud sync (2 transactions, up to 5 levels deep) 🆕

### Platform-Specific Files
- ✅ **Android**: `MainActivity.cs`, `MainApplication.cs`, `AndroidManifest.xml`
- ✅ **iOS**: `AppDelegate.cs`, `Program.cs`, `Info.plist`
- ✅ **MacCatalyst**: `AppDelegate.cs`, `Program.cs`, `Info.plist`
- ✅ **Windows**: `App.xaml`, `App.xaml.cs`, `app.manifest`, `Package.appxmanifest`

### Resources
- ✅ **Styles**: `Colors.xaml`, `Styles.xaml` - Complete app theming
- ✅ **Fonts**: `OpenSans-Regular.ttf`, `OpenSans-Semibold.ttf` - Font files
- ✅ **Images**: `dotnet_bot.svg` - App icon image
- ✅ **AppIcon**: `appicon.svg`, `appiconfg.svg` - App icons
- ✅ **Splash**: `splash.svg` - Splash screen

### Documentation
- ✅ `README.md` - Comprehensive documentation
- ✅ `QUICKSTART.md` - Quick start guide
- ✅ `PROJECT_SUMMARY.md` - This file
- ✅ `.gitignore` - Git ignore configuration

## 🎯 Sentry Configuration

**DSN**: `https://4892d51476eb1216b7951d9eadbb8464@o1161257.ingest.us.sentry.io/4510557120954368`

**Key Settings**:
- ✅ TracesSampleRate: 1.0 (100% of transactions)
- ✅ Debug Mode: Enabled
- ✅ SendDefaultPii: Enabled
- ✅ Automatic Instrumentation: **Disabled** (using custom instrumentation only)

## 🚀 Custom Transactions Implemented

### ADVANCED TRANSACTIONS (NEW - DEEP NESTING) 🔥

### 6. Complex Database Query (`database-complex-query`) - 4 LEVELS DEEP
**Location**: `Services/DatabaseService.cs` → `PerformComplexQueryAsync()`

**Hierarchy**:
- `db.connection.pool` - Get connection from pool
- `db.query.execute` - Execute query (parent)
  - `db.query.parse` - Parse SQL (level 2)
  - `db.query.optimize` - Optimize query plan (level 2)
  - `db.index.scan` - Scan indexes (level 2)
  - `db.data.fetch` - Fetch result set (level 2)
- `db.result.process` - Process results (parent)
  - `serialization.deserialize` - Deserialize rows (level 2)
  - `data.transform` - Transform data (level 2)
- `cache.update` - Update query cache

**Metadata**: query, db_type, pool_size, active_connections, indexes_used, rows_scanned, rows_fetched, data_size_mb

---

### 7. Batch Insert (`database-batch-insert`) - DYNAMIC SPANS
**Location**: `Services/DatabaseService.cs` → `BatchInsertAsync()`

**Dynamic Structure**: Creates N batch spans based on record count
- Per-batch span: `db.batch.insert`
  - `db.batch.prepare` - Prepare batch data
  - `db.transaction.begin` - Begin DB transaction
  - `db.insert` - Insert records
  - `db.transaction.commit` - Commit transaction

**Metadata**: record_count, batch_size, records_inserted, total_inserted

---

### 8. Database Optimization (`database-optimization`)
**Location**: `Services/DatabaseService.cs` → `OptimizeDatabaseAsync()`

**Spans**:
1. `db.analyze` - Analysis phase (parent)
   - `db.analyze.tables` - Analyze table statistics
   - `db.analyze.indexes` - Analyze index usage
2. `db.vacuum` - Vacuum database
3. `db.reindex` - Rebuild indexes (parent)
   - `db.index.drop` - Drop old indexes
   - `db.index.build` - Build new indexes

**Metadata**: tables_analyzed, indexes_analyzed, space_reclaimed_mb, indexes_rebuilt

---

### 9. File Processing Pipeline (`file-processing-pipeline`) - 5 LEVELS DEEP
**Location**: `Services/FileProcessingService.cs` → `ProcessLargeFileAsync()`

**Hierarchy**:
1. `file.read` - Read file (parent)
   - `file.open` - Open file handle (level 2)
   - `file.buffer` - Buffer file data (level 2)
2. `file.parse` - Parse content (parent)
   - `csv.parse` - Parse CSV structure (level 2)
   - `data.validate` - Validate data integrity (level 2)
3. `data.transform` - Transform data (parent)
   - `data.normalize` - Normalize data format (level 2)
   - `data.enrich` - Enrich with metadata (level 2, parent)
     - `db.lookup` - Lookup reference data (level 3)
     - `data.merge` - Merge enrichment data (level 3)
4. `file.write` - Write output (parent)
   - `data.serialize` - Serialize to JSON (level 2)
   - `file.compress` - Compress output (level 2)
   - `file.save` - Save to disk (level 2)

**Metadata**: file_path, file_type, file_size_mb, buffer_size_kb, rows_parsed, validation_rules, invalid_rows, compression_ratio, output_size_mb

---

### 10. Cloud File Sync (`cloud-file-sync`) - PARALLEL OPERATIONS
**Location**: `Services/FileProcessingService.cs` → `SyncFilesToCloudAsync()`

**Structure**: Creates N parallel file sync operations
- Per-file span: `file.sync`
  - `cloud.check` - Check if file exists remotely
  - `cloud.upload` - Upload file (parent)
    - `crypto.hash` - Compute file hash (level 2)
    - `network.transfer` - Transfer file data (level 2)
    - `cloud.verify` - Verify upload (level 2)

**Metadata**: file_count, cloud_provider, bytes_transferred

---

### ORIGINAL TRANSACTIONS

### 1. Data Fetch Transaction (`data-fetch`)
**Location**: `Services/DataService.cs` → `FetchDataAsync()`

**Spans**:
- `http.client` - Simulated API call
- `data.processing` - Data processing

**Metadata**: endpoint, method, data_size

---

### 2. Data Validation (`data-validation`)
**Location**: `Services/DataService.cs` → `ValidateDataAsync()`

**Spans**:
- `data.validation` - Validation logic

**Metadata**: data_length, is_valid

---

### 3. Payment Processing (`payment-checkout`)
**Location**: `Services/PaymentService.cs` → `ProcessPaymentAsync()`

**Spans**:
1. `payment.validation` - Payment details validation
2. `payment.gateway` - Payment gateway processing
3. `db.update` - Database record update
4. `email.send` - Confirmation email

**Metadata**: amount, currency, gateway, user context

---

### 4. Invoice Generation (`invoice-generation`)
**Location**: `Services/PaymentService.cs` → `GenerateInvoiceAsync()`

**Spans**:
1. `db.query` - Fetch payment details
2. `pdf.generate` - Generate PDF invoice
3. `storage.upload` - Upload to cloud storage

**Metadata**: payment_id

---

### 5. Complex Multi-Span Transaction (`complex-operation`)
**Location**: `MainPage.xaml.cs` → `OnComplexTransactionClicked()`

**Demonstrates**: Deeply nested span hierarchy

**Spans**:
1. `auth` - User authentication
2. `cart.validate` - Shopping cart validation
3. `address.verify` - Shipping address verification
4. `payment.process` (parent)
   - `payment.validate` (child)
   - `payment.gateway` (child)
5. `order.create` - Order creation
6. `notifications` (parent)
   - `email.send` (child)
   - `sms.send` (child)

**Total**: 10+ spans with 3 levels of nesting

---

### 6. Error Capture Test
**Location**: `MainPage.xaml.cs` → `OnThrowErrorClicked()`

**Demonstrates**: Exception capture with `SentrySdk.CaptureException()`

## 🎨 User Interface

The app features a clean, modern UI with **five** color-coded sections:

### 🔵 Data Operations (Blue)
- Fetch Data button
- Validate Data button

### 🟢 Payment Operations (Green)
- Process Payment button
- Generate Invoice button

### 🟣 Database Operations (Purple) - NEW
- Complex DB Query (4 Levels Deep) button
- Batch Insert (Dynamic Spans) button
- Database Optimization button

### 🟠 File Processing (Orange) - NEW
- Process Large File Pipeline button
- Cloud File Sync (Parallel) button

### 🔴 Advanced Testing (Red)
- Throw Test Exception button
- Complex Multi-Span Transaction button

All buttons include:
- Visual feedback (disable during operation)
- Status label updates
- Alert dialogs with results
- Error handling

## 📊 Key Features

### ✨ Custom Instrumentation Patterns
1. **Basic Transactions**: Simple transaction creation
2. **Child Spans**: Adding spans to transactions
3. **Nested Spans**: Multi-level span hierarchies
4. **Metadata**: Adding custom data to spans
5. **User Context**: Associating users with transactions
6. **Error Handling**: Capturing exceptions within transactions
7. **Status Tracking**: Setting span and transaction statuses

### 🔧 Best Practices Demonstrated
- ✅ Transaction lifecycle management (start → finish)
- ✅ Proper span nesting and cleanup
- ✅ Error handling with span status updates
- ✅ Adding contextual metadata
- ✅ Using `try-catch-finally` for reliability
- ✅ Setting transaction on scope for automatic span association
- ✅ Using `SentrySdk.GetSpan()` to retrieve active spans

## 🛠️ Building the Project

### Prerequisites
- .NET 8 SDK
- .NET MAUI workload
- Visual Studio 2022 or Visual Studio for Mac

### Quick Build
```bash
cd "/Users/prithvi/Development/2025/dotNET FE demo"
dotnet restore
dotnet build
```

### Platform-Specific Builds
```bash
# Windows
dotnet build -f net8.0-windows10.0.19041.0

# Mac Catalyst
dotnet build -f net8.0-maccatalyst

# iOS
dotnet build -f net8.0-ios

# Android
dotnet build -f net8.0-android
```

## 📈 Expected Sentry Data

### Performance Tab
After running the demos, you should see:
- **Multiple transactions** with names like `data-fetch`, `payment-checkout`, etc.
- **Waterfall views** showing span relationships
- **Duration metrics** for each operation
- **Custom metadata** in span details

### Issues Tab
After clicking "Throw Test Exception":
- **Exception details** with stack trace
- **Device information** (OS, model, etc.)
- **Breadcrumbs** showing user actions
- **User context** when available

## 🎓 Learning Objectives

This demo teaches you:
1. ✅ How to initialize Sentry in a MAUI app
2. ✅ How to create custom transactions
3. ✅ How to add child spans to transactions
4. ✅ How to nest spans multiple levels deep
5. ✅ How to add metadata to spans
6. ✅ How to capture exceptions
7. ✅ How to manage transaction lifecycle
8. ✅ How to set user context
9. ✅ How to use span statuses
10. ✅ How to disable automatic instrumentation

## 📂 Project Structure

```
SentryMauiDemo/
├── 📄 Core App Files (App.xaml, AppShell.xaml, MainPage.xaml, MauiProgram.cs)
├── 📁 Services/ (Custom instrumentation examples)
├── 📁 Resources/ (Fonts, images, styles, icons)
├── 📁 Platforms/ (Android, iOS, MacCatalyst, Windows)
├── 📄 SentryMauiDemo.csproj (Project file)
├── 📄 SentryMauiDemo.sln (Solution file)
├── 📄 README.md (Full documentation)
├── 📄 QUICKSTART.md (Quick start guide)
└── 📄 .gitignore (Git configuration)
```

## 🔗 Resources

- **Sentry .NET MAUI Docs**: https://docs.sentry.io/platforms/dotnet/guides/maui/
- **Custom Instrumentation Guide**: https://docs.sentry.io/platforms/dotnet/guides/maui/tracing/instrumentation/custom-instrumentation/
- **.NET MAUI Docs**: https://learn.microsoft.com/en-us/dotnet/maui/
- **Sentry Performance**: https://docs.sentry.io/product/performance/

## ✅ Ready to Use

This project is **100% complete** and ready to:
- ✅ Build on all platforms
- ✅ Run on Windows and Mac immediately
- ✅ Deploy to iOS and Android with minimal setup
- ✅ Send transactions and errors to Sentry
- ✅ Demonstrate custom instrumentation patterns
- ✅ Serve as a reference for your own projects

## 🎉 Next Steps

1. **Build and run** the application
2. **Click buttons** to generate transactions
3. **View data** in your Sentry dashboard
4. **Explore the code** in the Services folder
5. **Customize** for your own use cases
6. **Learn** from the instrumentation patterns

---

**Project Created**: December 2024  
**SDK Version**: Sentry.Maui 5.16.2  
**Framework**: .NET 8 with MAUI  
**Platforms**: Windows, macOS, iOS, Android

