using System.Net.Http.Json;
using SIGL_Cadastru.Mobile.Models.Clients;
using SIGL_Cadastru.Mobile.Models.Shared;
using SIGL_Cadastru.Mobile.Services.Base;

namespace SIGL_Cadastru.Mobile.Services.Clients;

public class ClientService : BaseApiService, IClientService
{
    public ClientService(HttpClient httpClient, KeycloakAuthService authService)
        : base(httpClient, authService)
    {
    }

    public async Task<List<ClientDto>> GetClientsAsync(PagedQueryParameters? parameters = null)
    {
        var client = await GetAuthenticatedClientAsync();
        var query = parameters != null ? BuildQueryString(new Dictionary<string, string?>
        {
            ["SearchBy"] = parameters.SearchBy,
            ["PageNumber"] = parameters.PageNumber?.ToString(),
            ["PageSize"] = parameters.PageSize?.ToString()
        }) : string.Empty;

        var response = await client.GetAsync($"/api/Clients{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ClientDto>>(JsonOptions) ?? new();
    }

    public async Task<Client> CreateClientAsync(CreateClientCommand command)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Clients", command, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Client>(JsonOptions) ?? new();
    }

    public async Task<Client> UpdateClientAsync(UpdateClientCommand command)
    {
        var client = await GetAuthenticatedClientAsync();
        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/Clients")
        {
            Content = JsonContent.Create(command, options: JsonOptions)
        };
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Client>(JsonOptions) ?? new();
    }

    public async Task<Client> GetClientByIdAsync(string id)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/Clients/{Uri.EscapeDataString(id)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Client>(JsonOptions) ?? new();
    }

    public async Task DeleteClientAsync(string id)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Clients/{Uri.EscapeDataString(id)}");
        response.EnsureSuccessStatusCode();
    }
}
