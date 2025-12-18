namespace SentryMauiDemo.Services;

public interface IDataService
{
    Task<string> FetchDataAsync(string endpoint);
    Task<bool> ValidateDataAsync(string data);
}

