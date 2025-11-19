using System.Net.Http.Json;
using SIGL_Cadastru.Mobile.Models.Shared;
using SIGL_Cadastru.Mobile.Models.Users;
using SIGL_Cadastru.Mobile.Services.Base;

namespace SIGL_Cadastru.Mobile.Services.Users;

public class UserService : BaseApiService, IUserService
{
    public UserService(HttpClient httpClient, KeycloakAuthService authService)
        : base(httpClient, authService)
    {
    }

    public async Task<UserDtoPagedResponse> GetPerformersAsync(PagedQueryParameters? parameters = null)
    {
        var client = await GetAuthenticatedClientAsync();
        var query = parameters != null ? BuildQueryString(new Dictionary<string, string?>
        {
            ["SearchBy"] = parameters.SearchBy,
            ["PageNumber"] = parameters.PageNumber?.ToString(),
            ["PageSize"] = parameters.PageSize?.ToString()
        }) : string.Empty;

        var response = await client.GetAsync($"/api/Users/performers{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDtoPagedResponse>(JsonOptions) ?? new();
    }

    public async Task<UserDtoPagedResponse> GetResponsibleUsersAsync(PagedQueryParameters? parameters = null)
    {
        var client = await GetAuthenticatedClientAsync();
        var query = parameters != null ? BuildQueryString(new Dictionary<string, string?>
        {
            ["SearchBy"] = parameters.SearchBy,
            ["PageNumber"] = parameters.PageNumber?.ToString(),
            ["PageSize"] = parameters.PageSize?.ToString()
        }) : string.Empty;

        var response = await client.GetAsync($"/api/Users/responsible{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDtoPagedResponse>(JsonOptions) ?? new();
    }
}
