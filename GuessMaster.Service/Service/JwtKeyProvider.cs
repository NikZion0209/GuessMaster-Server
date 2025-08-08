using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;
using System.Threading.Tasks;

public class JwtKeyProvider
{
    private readonly IAmazonSecretsManager _secretsManager;
    private readonly string _secretName;

    public JwtKeyProvider(IAmazonSecretsManager secretsManager, string secretName)
    {
        _secretsManager = secretsManager;
        _secretName = secretName;
    }

    public async Task<string?> GetJwtKeyAsync()
    {
        try
        {
            var response = await _secretsManager.GetSecretValueAsync(new GetSecretValueRequest
            {
                SecretId = _secretName
            });

            if (!string.IsNullOrEmpty(response.SecretString))
            {
                // Assuming secret is JSON: { "JWTKey": "your_key_here" }
                var json = JsonDocument.Parse(response.SecretString);
                if (json.RootElement.TryGetProperty("JWTKey", out var keyProperty))
                {
                    return keyProperty.GetString();
                }
            }
        }
        catch (Exception ex)
        {
            // Log error or handle it
            Console.WriteLine($"Failed to fetch JWT key from Secrets Manager: {ex.Message}");
        }
        return null;
    }
}
