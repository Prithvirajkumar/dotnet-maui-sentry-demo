namespace SentryMauiDemo.Services;

public interface IDatabaseService
{
    Task<bool> PerformComplexQueryAsync(string query);
    Task<int> BatchInsertAsync(int recordCount);
    Task OptimizeDatabaseAsync();
}

