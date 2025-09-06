namespace jgz_evenemangel.Services;

public class ApiKeyService(IConfiguration configuration)
{
    private readonly string _expectedApiKey =
        configuration["ApiKey"] ?? throw new Exception("ApiKey not configured in appsettings.json");

    public bool IsValidApiKey(string? providedApiKey)
    {
        return !string.IsNullOrEmpty(providedApiKey) && string.Equals(_expectedApiKey, providedApiKey);
    }
}