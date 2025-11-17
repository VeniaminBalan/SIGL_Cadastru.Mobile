using Duende.IdentityModel.OidcClient;

namespace SIGL_Cadastru.Mobile.Services;

public class KeycloakAuthService
{
    private readonly OidcClient _client;

    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public string? IdentityToken { get; private set; }

    public KeycloakAuthService(OidcClient client)
    {
        _client = client;
    }

    public async Task<bool> LoginAsync()
    {
        var result = await _client.LoginAsync();

        if (result.IsError)
            return false;

        AccessToken = result.AccessToken;
        RefreshToken = result.RefreshToken;
        IdentityToken = result.IdentityToken;
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
    }

    public async Task<bool> RefreshAsync()
    {
        if (string.IsNullOrEmpty(RefreshToken))
            return false;

        var result = await _client.RefreshTokenAsync(RefreshToken);

        AccessToken = result.AccessToken;
        RefreshToken = result.RefreshToken;

        return true;
    }
}
