using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace MedicalBilling.BlazorServer.Services;

public class TokenHandler : DelegatingHandler
{
    private readonly TokenProvider _tokenProvider;

    public TokenHandler(TokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = _tokenProvider.AccessToken;

        if (!string.IsNullOrEmpty(accessToken))
        {
            Console.WriteLine($"[Blazor DEBUG] Attaching Token: {accessToken.Substring(0, 10)}...");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        // No else block - just proceed without token if empty
        
        return await base.SendAsync(request, cancellationToken);
    }
}
