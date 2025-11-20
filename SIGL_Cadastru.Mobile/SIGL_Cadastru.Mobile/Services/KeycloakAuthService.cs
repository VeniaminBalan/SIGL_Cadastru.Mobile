using Duende.IdentityModel.OidcClient;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace SIGL_Cadastru.Mobile.Services;

public class KeycloakAuthService
{
    private readonly OidcClient _client;
    private readonly ILogger<KeycloakAuthService> _logger;
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";
    private const string IdentityTokenKey = "identity_token";

    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public string? IdentityToken { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

    public KeycloakAuthService(OidcClient client, ILogger<KeycloakAuthService> logger)
    {
        _client = client;
        _logger = logger;
        _ = LoadTokensAsync();
    }

    private async Task LoadTokensAsync()
    {
        try
        {
            AccessToken = await SecureStorage.GetAsync(AccessTokenKey);
            RefreshToken = await SecureStorage.GetAsync(RefreshTokenKey);
            IdentityToken = await SecureStorage.GetAsync(IdentityTokenKey);
            _logger.LogInformation("Tokens loaded from secure storage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load tokens from secure storage");
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
            _logger.LogInformation("Tokens saved to secure storage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save tokens to secure storage");
        }
    }

    private async Task ClearTokensAsync()
    {
        try
        {
            SecureStorage.Remove(AccessTokenKey);
            SecureStorage.Remove(RefreshTokenKey);
            SecureStorage.Remove(IdentityTokenKey);
            _logger.LogInformation("Tokens cleared from secure storage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear tokens from secure storage");
        }
    }

    public async Task InvalidateTokensAsync()
    {
        _logger.LogInformation("Invalidating tokens");
        AccessToken = null;
        RefreshToken = null;
        IdentityToken = null;
        await ClearTokensAsync();
    }

    public async Task<bool> LoginAsync()
    {
        _logger.LogInformation("Starting login process");
        var result = await _client.LoginAsync();

        if (result.IsError)
        {
            _logger.LogError("Login failed: {Error}", result.Error);
            return false;
        }

        AccessToken = result.AccessToken;
        RefreshToken = result.RefreshToken;
        IdentityToken = result.IdentityToken;
        
        await SaveTokensAsync();
        _logger.LogInformation("Login successful");
        return true;
    }

    public async Task LogoutAsync()
    {
        try
        {
            _logger.LogInformation("Starting logout process");
            
            if (!string.IsNullOrEmpty(IdentityToken))
            {
                await _client.LogoutAsync(new LogoutRequest
                {
                    IdTokenHint = IdentityToken,
                    BrowserDisplayMode = Duende.IdentityModel.OidcClient.Browser.DisplayMode.Visible
                });
            }
            else
            {
                _logger.LogWarning("IdentityToken is null or empty, skipping server logout");
            }

            AccessToken = null;
            RefreshToken = null;
            IdentityToken = null;

            await ClearTokensAsync();
            _logger.LogInformation("Logout successful");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Logout failed, but proceeding to clear tokens locally");
            
            AccessToken = null;
            RefreshToken = null;
            IdentityToken = null;
            await ClearTokensAsync();
        }
    }

    public async Task<bool> RefreshAsync()
    {
        if (string.IsNullOrEmpty(RefreshToken))
        {
            _logger.LogWarning("Cannot refresh token: RefreshToken is null or empty");
            return false;
        }

        try
        {
            _logger.LogInformation("Refreshing access token");
            var result = await _client.RefreshTokenAsync(RefreshToken);

            if (result.IsError)
            {
                _logger.LogError("Token refresh failed: {Error}", result.Error);
                return false;
            }

            AccessToken = result.AccessToken;
            RefreshToken = result.RefreshToken;

            await SaveTokensAsync();
            _logger.LogInformation("Token refresh successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during token refresh");
            return false;
        }
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract username from access token");
            return "User";
        }
    }
}
