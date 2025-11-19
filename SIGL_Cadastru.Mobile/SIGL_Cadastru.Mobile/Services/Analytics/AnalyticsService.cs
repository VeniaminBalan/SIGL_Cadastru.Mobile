using System.Net.Http.Json;
using SIGL_Cadastru.Mobile.Services.Base;

namespace SIGL_Cadastru.Mobile.Services.Analytics;

public class AnalyticsService : BaseApiService, IAnalyticsService
{
    public AnalyticsService(HttpClient httpClient, KeycloakAuthService authService)
        : base(httpClient, authService)
    {
    }

    public async Task<string> GetEmbedUrlAsync(Dictionary<string, object> request)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Analytics/embed-url", request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
