namespace SentryMauiDemo.Services;

public interface IFileProcessingService
{
    Task<string> ProcessLargeFileAsync(string filePath);
    Task<bool> SyncFilesToCloudAsync(int fileCount);
}

