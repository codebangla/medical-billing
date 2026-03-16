using System.Collections.Concurrent;

namespace MedicalBilling.BlazorServer.Services;

public class ServerSideTokenStore
{
    // Maps Username -> AccessToken
    private static readonly ConcurrentDictionary<string, string> _userTokens = new();

    public void SaveToken(string username, string token)
    {
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(token))
        {
            _userTokens[username] = token;
            Console.WriteLine($"[TokenStore] Saved token for user: {username}");
        }
    }

    public string? GetToken(string username)
    {
        if (string.IsNullOrEmpty(username)) return null;
        
        _userTokens.TryGetValue(username, out var token);
        return token;
    }
}
