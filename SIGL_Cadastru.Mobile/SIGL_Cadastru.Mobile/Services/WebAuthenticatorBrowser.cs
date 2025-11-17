using Duende.IdentityModel.OidcClient.Browser;

namespace SIGL_Cadastru.Mobile.Services;

public class WebAuthenticatorBrowser : Duende.IdentityModel.OidcClient.Browser.IBrowser
{
    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
    {
        var result = await WebAuthenticator.Default.AuthenticateAsync(
            new Uri(options.StartUrl),
            new Uri(options.EndUrl));

        // Convert result into redirect URI format used by OidcClient
        var responseUrl = new Uri(options.EndUrl + "#" +
            string.Join("&", result.Properties.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}")));

        return new BrowserResult
        {
            ResultType = BrowserResultType.Success,
            Response = responseUrl.ToString()
        };
    }
}
