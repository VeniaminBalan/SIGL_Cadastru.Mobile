using System.Net.Http.Headers;
using System.Text.Json;

namespace SIGL_Cadastru.Mobile.Services.Base;

/// <summary>
/// Base service class with common HTTP functionality
/// </summary>
public abstract class BaseApiService
{
    protected readonly HttpClient HttpClient;
    protected readonly KeycloakAuthService AuthService;
    protected readonly JsonSerializerOptions JsonOptions;

    protected BaseApiService(HttpClient httpClient, KeycloakAuthService authService)
    {
        HttpClient = httpClient;
        AuthService = authService;
        
        JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    protected async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var token = AuthService.AccessToken;
        if (!string.IsNullOrEmpty(token))
        {
            HttpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }
        return HttpClient;
    }

    protected string BuildQueryString(IDictionary<string, string?> parameters)
    {
        var queryParams = new List<string>();
        
        foreach (var param in parameters)
        {
            if (!string.IsNullOrEmpty(param.Value))
            {
                queryParams.Add($"{param.Key}={Uri.EscapeDataString(param.Value)}");
            }
        }

        return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
    }
}
