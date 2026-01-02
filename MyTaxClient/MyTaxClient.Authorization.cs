using System.Net.Http.Json;
using MyTaxClient.Models.Dto.Authorization;

namespace MyTaxClient;

public partial class MyTaxClient
{
    private string? _authToken;
    private DateTimeOffset _authTokenExpiresAt;
    private string? _refreshToken;
    private readonly string _inn = options.Value.Username!;
    private bool IsAuthTokenValid()
        => string.IsNullOrEmpty(_authToken) is false
           && DateTimeOffset.UtcNow < _authTokenExpiresAt.AddSeconds(-30);
    
    private readonly SemaphoreSlim _authSemaphore = new(1, 1);
    private async Task EnsureAuthorized(CancellationToken ct)
    {
        if (IsAuthTokenValid())
        {
            return;
        }

        await _authSemaphore.WaitAsync(ct);
        try
        {
            if (IsAuthTokenValid())
            {
                return;
            }

            if (string.IsNullOrEmpty(_refreshToken) is false)
            {
                if (await TryRefreshToken(ct))
                {
                    return;
                }
            }

            if (await Login(ct) is false)
            {
                throw new InvalidOperationException("Failed to authorize by login/password.");
            }
        }
        finally
        {
            _authSemaphore.Release();
        }
    }
    
    private async Task<bool> TryRefreshToken(CancellationToken ct)
    {
        try
        {
            var client = _http.Value;

            using var request = new HttpRequestMessage(HttpMethod.Post, "auth/token");
            request.Content = JsonContent.Create(
                new RefreshRequest(_refreshToken!, GetDeviceInfo(_deviceId)), null, JsonOptions);
            request.Headers.Referrer = new Uri("https://lknpd.nalog.ru/sales");
            
            using var response = await client.SendAsync(request, ct);
            if (response.IsSuccessStatusCode is false)
            {
                return false;
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions, cancellationToken: ct);
            ApplyAuth(authResponse!);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    private async Task<bool> Login(CancellationToken ct)
    {
        try
        {
            var client = _http.Value;

            using var request = new HttpRequestMessage(HttpMethod.Post, "auth/lkfl");
            request.Content = JsonContent.Create(new LoginRequest(
                options.Value.Username!,
                options.Value.Password!,
                GetDeviceInfo(_deviceId)), null, JsonOptions);
            request.Headers.Referrer = new Uri("https://lknpd.nalog.ru/auth/login");
            
            var response = await client.SendAsync(request, ct);
            if (response.IsSuccessStatusCode is false)
            {
                return false;
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions, cancellationToken: ct);
            ApplyAuth(authResponse!);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    private void ApplyAuth(AuthResponse auth)
    {
        _authToken = auth.Token;
        _authTokenExpiresAt = auth.TokenExpireIn;
        _refreshToken = auth.RefreshToken;
    }
}