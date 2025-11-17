using Duende.IdentityModel.OidcClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace SIGL_Cadastru.Mobile.Services;

public class KeycloakAuthService
{
    private readonly OidcClient _client;
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";
    private const string IdentityTokenKey = "identity_token";

    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public string? IdentityToken { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

    public KeycloakAuthService(OidcClient client)
    {
        _client = client;
        _ = LoadTokensAsync();
    }

    private async Task LoadTokensAsync()
    {
        try
        {
            AccessToken = await SecureStorage.GetAsync(AccessTokenKey);
            RefreshToken = await SecureStorage.GetAsync(RefreshTokenKey);
            IdentityToken = await SecureStorage.GetAsync(IdentityTokenKey);
        }
        catch
        {
            // Ignore errors during token loading
        }
    }

    private async Task SaveTokensAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(AccessToken))
                await SecureStorage.SetAsync(AccessTokenKey, AccessToken);
            if (!string.IsNullOrEmpty(RefreshToken))
                await SecureStorage.SetAsync(RefreshTokenKey, RefreshToken);
            if (!string.IsNullOrEmpty(IdentityToken))
                await SecureStorage.SetAsync(IdentityTokenKey, IdentityToken);
        }
        catch
        {
            // Ignore errors during token saving
        }
    }

    private async Task ClearTokensAsync()
    {
        try
        {
            SecureStorage.Remove(AccessTokenKey);
            SecureStorage.Remove(RefreshTokenKey);
            SecureStorage.Remove(IdentityTokenKey);
        }
        catch
        {
            // Ignore errors during token clearing
        }
    }

    public async Task<bool> LoginAsync()
    {
        var result = await _client.LoginAsync();

        if (result.IsError)
            return false;

        AccessToken = result.AccessToken;
        RefreshToken = result.RefreshToken;
        IdentityToken = result.IdentityToken;
        
        await SaveTokensAsync();
        return true;
    }

    public async Task LogoutAsync()
    {
        await _client.LogoutAsync(new LogoutRequest
        {
            IdTokenHint = IdentityToken,
            BrowserDisplayMode = Duende.IdentityModel.OidcClient.Browser.DisplayMode.Visible
        });

        AccessToken = null;
        RefreshToken = null;
        IdentityToken = null;
        
        await ClearTokensAsync();
    }

    public async Task<bool> RefreshAsync()
    {
        if (string.IsNullOrEmpty(RefreshToken))
            return false;

        var result = await _client.RefreshTokenAsync(RefreshToken);

        AccessToken = result.AccessToken;
        RefreshToken = result.RefreshToken;

        await SaveTokensAsync();
        return true;
    }

    public string GetUserName()
    {
        if (string.IsNullOrEmpty(AccessToken))
            return "Guest";

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(AccessToken);
            
            var nameClaim = token.Claims.FirstOrDefault(c => c.Type == "preferred_username" || c.Type == "name" || c.Type == "sub");
            return nameClaim?.Value ?? "User";
        }
        catch
        {
            return "User";
        }
    }
}
