namespace SIGL_Cadastru.Mobile.Models;

public class ApiConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;
    public int Timeout { get; set; } = 30;
}

public class KeycloakConfiguration
{
    public string Authority { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
}
