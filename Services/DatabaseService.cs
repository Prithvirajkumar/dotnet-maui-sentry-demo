using Sentry;

namespace SentryMauiDemo.Services;

public class DatabaseService : IDatabaseService
{
    public async Task<bool> PerformComplexQueryAsync(string query)
    {
        var transaction = SentrySdk.StartTransaction(
            "database-complex-query",
            "db.query.complex"
        );
        
        SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);

        try
        {
            transaction.SetExtra("query", query);
            transaction.SetTag("db_type", "postgresql");
            transaction.SetTag("query_complexity", "high");

            // Span 1: Get connection from pool
            var connectionSpan = transaction.StartChild(
                "db.connection.pool",
                "Acquiring connection from pool"
            );
            await Task.Delay(Random.Shared.Next(100, 300));
            connectionSpan.SetExtra("pool_size", 10);
            connectionSpan.SetExtra("active_connections", 3);
            connectionSpan.Status = SpanStatus.Ok;
            connectionSpan.Finish();

            // Span 2: Query execution with nested operations
            var executionSpan = transaction.StartChild(
                "db.query.execute",
                "Executing complex query"
            );
            
            // Nested: Query parsing
            var parseSpan = executionSpan.StartChild(
                "db.query.parse",
                "Parsing SQL query"
            );
            await Task.Delay(Random.Shared.Next(50, 150));
            parseSpan.SetExtra("query_length", query.Length);
            parseSpan.Status = SpanStatus.Ok;
            parseSpan.Finish();

            // Nested: Query optimization
            var optimizeSpan = executionSpan.StartChild(
                "db.query.optimize",
                "Optimizing query plan"
            );
            await Task.Delay(Random.Shared.Next(100, 200));
            optimizeSpan.SetExtra("optimization_level", "aggressive");
            optimizeSpan.Status = SpanStatus.Ok;
            optimizeSpan.Finish();

            // Nested: Index scan
            var indexSpan = executionSpan.StartChild(
                "db.index.scan",
                "Scanning indexes"
            );
            await Task.Delay(Random.Shared.Next(150, 300));
            indexSpan.SetExtra("indexes_used", 2);
            indexSpan.SetExtra("rows_scanned", 1500);
            indexSpan.Status = SpanStatus.Ok;
            indexSpan.Finish();

            // Nested: Data fetch
            var fetchSpan = executionSpan.StartChild(
                "db.data.fetch",
                "Fetching result set"
            );
            await Task.Delay(Random.Shared.Next(200, 400));
            fetchSpan.SetExtra("rows_fetched", 250);
            fetchSpan.SetExtra("data_size_mb", 2.5);
            fetchSpan.Status = SpanStatus.Ok;
            fetchSpan.Finish();

            executionSpan.Status = SpanStatus.Ok;
            executionSpan.Finish();

            // Span 3: Result processing
            var processingSpan = transaction.StartChild(
                "db.result.process",
                "Processing query results"
            );
            
            // Nested: Deserialization
            var deserializeSpan = processingSpan.StartChild(
                "serialization.deserialize",
                "Deserializing rows"
            );
            await Task.Delay(Random.Shared.Next(100, 200));
            deserializeSpan.Status = SpanStatus.Ok;
            deserializeSpan.Finish();

            // Nested: Transformation
            var transformSpan = processingSpan.StartChild(
                "data.transform",
                "Transforming data"
            );
            await Task.Delay(Random.Shared.Next(150, 250));
            transformSpan.Status = SpanStatus.Ok;
            transformSpan.Finish();

            processingSpan.Status = SpanStatus.Ok;
            processingSpan.Finish();

            // Span 4: Cache update
            var cacheSpan = transaction.StartChild(
                "cache.update",
                "Updating query cache"
            );
            await Task.Delay(Random.Shared.Next(50, 150));
            cacheSpan.SetExtra("cache_key", $"query_{query.GetHashCode()}");
            cacheSpan.Status = SpanStatus.Ok;
            cacheSpan.Finish();

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

    public async Task<int> BatchInsertAsync(int recordCount)
    {
        var transaction = SentrySdk.StartTransaction(
            "database-batch-insert",
            "db.batch.operation"
        );
        
        SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);

        try
        {
            transaction.SetExtra("record_count", recordCount);
            transaction.SetTag("operation", "batch_insert");

            var batchSize = 100;
            var batches = (int)Math.Ceiling(recordCount / (double)batchSize);
            var totalInserted = 0;

            for (int i = 0; i < batches; i++)
            {
                var batchSpan = transaction.StartChild(
                    "db.batch.insert",
                    $"Batch {i + 1} of {batches}"
                );

                // Nested: Prepare batch
                var prepareSpan = batchSpan.StartChild(
                    "db.batch.prepare",
                    "Preparing batch data"
                );
                await Task.Delay(Random.Shared.Next(50, 100));
                prepareSpan.Status = SpanStatus.Ok;
                prepareSpan.Finish();

                // Nested: Begin transaction
                var txnSpan = batchSpan.StartChild(
                    "db.transaction.begin",
                    "Begin database transaction"
                );
                await Task.Delay(Random.Shared.Next(20, 50));
                txnSpan.Status = SpanStatus.Ok;
                txnSpan.Finish();

                // Nested: Insert records
                var insertSpan = batchSpan.StartChild(
                    "db.insert",
                    $"Inserting {batchSize} records"
                );
                await Task.Delay(Random.Shared.Next(200, 400));
                insertSpan.SetExtra("batch_size", batchSize);
                insertSpan.Status = SpanStatus.Ok;
                insertSpan.Finish();

                // Nested: Commit transaction
                var commitSpan = batchSpan.StartChild(
                    "db.transaction.commit",
                    "Commit transaction"
                );
                await Task.Delay(Random.Shared.Next(50, 100));
                commitSpan.Status = SpanStatus.Ok;
                commitSpan.Finish();

                totalInserted += batchSize;
                batchSpan.SetExtra("records_inserted", batchSize);
                batchSpan.Status = SpanStatus.Ok;
                batchSpan.Finish();
            }

            transaction.SetExtra("total_inserted", totalInserted);
            transaction.Status = SpanStatus.Ok;
            return totalInserted;
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

    public async Task OptimizeDatabaseAsync()
    {
        var transaction = SentrySdk.StartTransaction(
            "database-optimization",
            "db.maintenance"
        );
        
        SentrySdk.ConfigureScope(scope => scope.Transaction = transaction);

        try
        {
            // Phase 1: Analysis
            var analysisSpan = transaction.StartChild(
                "db.analyze",
                "Analyzing database statistics"
            );
            
            var tableAnalysisSpan = analysisSpan.StartChild(
                "db.analyze.tables",
                "Analyzing table statistics"
            );
            await Task.Delay(Random.Shared.Next(300, 500));
            tableAnalysisSpan.SetExtra("tables_analyzed", 15);
            tableAnalysisSpan.Status = SpanStatus.Ok;
            tableAnalysisSpan.Finish();

            var indexAnalysisSpan = analysisSpan.StartChild(
                "db.analyze.indexes",
                "Analyzing index usage"
            );
            await Task.Delay(Random.Shared.Next(200, 400));
            indexAnalysisSpan.SetExtra("indexes_analyzed", 23);
            indexAnalysisSpan.Status = SpanStatus.Ok;
            indexAnalysisSpan.Finish();

            analysisSpan.Status = SpanStatus.Ok;
            analysisSpan.Finish();

            // Phase 2: Vacuum
            var vacuumSpan = transaction.StartChild(
                "db.vacuum",
                "Vacuuming database"
            );
            await Task.Delay(Random.Shared.Next(500, 1000));
            vacuumSpan.SetExtra("space_reclaimed_mb", 124.5);
            vacuumSpan.Status = SpanStatus.Ok;
            vacuumSpan.Finish();

            // Phase 3: Reindex
            var reindexSpan = transaction.StartChild(
                "db.reindex",
                "Rebuilding indexes"
            );
            
            var dropIndexSpan = reindexSpan.StartChild(
                "db.index.drop",
                "Dropping old indexes"
            );
            await Task.Delay(Random.Shared.Next(150, 250));
            dropIndexSpan.Status = SpanStatus.Ok;
            dropIndexSpan.Finish();

            var buildIndexSpan = reindexSpan.StartChild(
                "db.index.build",
                "Building new indexes"
            );
            await Task.Delay(Random.Shared.Next(400, 700));
            buildIndexSpan.SetExtra("indexes_rebuilt", 23);
            buildIndexSpan.Status = SpanStatus.Ok;
            buildIndexSpan.Finish();

            reindexSpan.Status = SpanStatus.Ok;
            reindexSpan.Finish();

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
    }
}

