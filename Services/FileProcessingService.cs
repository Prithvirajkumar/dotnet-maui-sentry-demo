using Sentry;

namespace SentryMauiDemo.Services;

public class FileProcessingService : IFileProcessingService
{
    public async Task<string> ProcessLargeFileAsync(string filePath)
    {
        var transaction = SentrySdk.StartTransaction(
            "file-processing-pipeline",
            "file.process"
        );
        
        SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);

        try
        {
            transaction.SetExtra("file_path", filePath);
            transaction.SetTag("file_type", "csv");

            // Stage 1: Read file
            var readSpan = transaction.StartChild(
                "file.read",
                "Reading file from disk"
            );
            
            var openSpan = readSpan.StartChild(
                "file.open",
                "Opening file handle"
            );
            await Task.Delay(Random.Shared.Next(50, 100));
            openSpan.Status = SpanStatus.Ok;
            openSpan.Finish();

            var bufferSpan = readSpan.StartChild(
                "file.buffer",
                "Buffering file data"
            );
            await Task.Delay(Random.Shared.Next(200, 400));
            bufferSpan.SetExtra("buffer_size_kb", 512);
            bufferSpan.Status = SpanStatus.Ok;
            bufferSpan.Finish();

            readSpan.SetExtra("file_size_mb", 45.3);
            readSpan.Status = SpanStatus.Ok;
            readSpan.Finish();

            // Stage 2: Parse content
            var parseSpan = transaction.StartChild(
                "file.parse",
                "Parsing file content"
            );
            
            var csvParseSpan = parseSpan.StartChild(
                "csv.parse",
                "Parsing CSV structure"
            );
            await Task.Delay(Random.Shared.Next(150, 300));
            csvParseSpan.SetExtra("rows_parsed", 50000);
            csvParseSpan.Status = SpanStatus.Ok;
            csvParseSpan.Finish();

            var validateSpan = parseSpan.StartChild(
                "data.validate",
                "Validating data integrity"
            );
            await Task.Delay(Random.Shared.Next(200, 350));
            validateSpan.SetExtra("validation_rules", 12);
            validateSpan.SetExtra("invalid_rows", 3);
            validateSpan.Status = SpanStatus.Ok;
            validateSpan.Finish();

            parseSpan.Status = SpanStatus.Ok;
            parseSpan.Finish();

            // Stage 3: Transform data
            var transformSpan = transaction.StartChild(
                "data.transform",
                "Transforming data"
            );
            
            var normalizeSpan = transformSpan.StartChild(
                "data.normalize",
                "Normalizing data format"
            );
            await Task.Delay(Random.Shared.Next(150, 250));
            normalizeSpan.Status = SpanStatus.Ok;
            normalizeSpan.Finish();

            var enrichSpan = transformSpan.StartChild(
                "data.enrich",
                "Enriching with metadata"
            );
            
            var lookupSpan = enrichSpan.StartChild(
                "db.lookup",
                "Looking up reference data"
            );
            await Task.Delay(Random.Shared.Next(100, 200));
            lookupSpan.Status = SpanStatus.Ok;
            lookupSpan.Finish();

            var mergeSpan = enrichSpan.StartChild(
                "data.merge",
                "Merging enrichment data"
            );
            await Task.Delay(Random.Shared.Next(100, 150));
            mergeSpan.Status = SpanStatus.Ok;
            mergeSpan.Finish();

            enrichSpan.Status = SpanStatus.Ok;
            enrichSpan.Finish();

            transformSpan.Status = SpanStatus.Ok;
            transformSpan.Finish();

            // Stage 4: Write output
            var writeSpan = transaction.StartChild(
                "file.write",
                "Writing processed file"
            );
            
            var serializeSpan = writeSpan.StartChild(
                "data.serialize",
                "Serializing to JSON"
            );
            await Task.Delay(Random.Shared.Next(200, 300));
            serializeSpan.Status = SpanStatus.Ok;
            serializeSpan.Finish();

            var compressSpan = writeSpan.StartChild(
                "file.compress",
                "Compressing output"
            );
            await Task.Delay(Random.Shared.Next(150, 250));
            compressSpan.SetExtra("compression_ratio", 0.65);
            compressSpan.Status = SpanStatus.Ok;
            compressSpan.Finish();

            var saveSpan = writeSpan.StartChild(
                "file.save",
                "Saving to disk"
            );
            await Task.Delay(Random.Shared.Next(200, 350));
            saveSpan.SetExtra("output_size_mb", 29.4);
            saveSpan.Status = SpanStatus.Ok;
            saveSpan.Finish();

            writeSpan.Status = SpanStatus.Ok;
            writeSpan.Finish();

            transaction.Status = SpanStatus.Ok;
            return "processed_output.json.gz";
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

    public async Task<bool> SyncFilesToCloudAsync(int fileCount)
    {
        var transaction = SentrySdk.StartTransaction(
            "cloud-file-sync",
            "cloud.sync"
        );
        
        SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);

        try
        {
            transaction.SetExtra("file_count", fileCount);
            transaction.SetTag("cloud_provider", "aws-s3");

            for (int i = 0; i < fileCount; i++)
            {
                var fileSpan = transaction.StartChild(
                    "file.sync",
                    $"Syncing file {i + 1} of {fileCount}"
                );

                // Check if file exists remotely
                var checkSpan = fileSpan.StartChild(
                    "cloud.check",
                    "Checking remote file"
                );
                await Task.Delay(Random.Shared.Next(50, 100));
                checkSpan.Status = SpanStatus.Ok;
                checkSpan.Finish();

                // Upload if needed
                var uploadSpan = fileSpan.StartChild(
                    "cloud.upload",
                    "Uploading file"
                );
                
                var hashSpan = uploadSpan.StartChild(
                    "crypto.hash",
                    "Computing file hash"
                );
                await Task.Delay(Random.Shared.Next(100, 150));
                hashSpan.Status = SpanStatus.Ok;
                hashSpan.Finish();

                var transferSpan = uploadSpan.StartChild(
                    "network.transfer",
                    "Transferring file data"
                );
                await Task.Delay(Random.Shared.Next(300, 600));
                transferSpan.SetExtra("bytes_transferred", Random.Shared.Next(1000000, 5000000));
                transferSpan.Status = SpanStatus.Ok;
                transferSpan.Finish();

                var verifySpan = uploadSpan.StartChild(
                    "cloud.verify",
                    "Verifying upload"
                );
                await Task.Delay(Random.Shared.Next(50, 100));
                verifySpan.Status = SpanStatus.Ok;
                verifySpan.Finish();

                uploadSpan.Status = SpanStatus.Ok;
                uploadSpan.Finish();

                fileSpan.Status = SpanStatus.Ok;
                fileSpan.Finish();
            }

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
}

