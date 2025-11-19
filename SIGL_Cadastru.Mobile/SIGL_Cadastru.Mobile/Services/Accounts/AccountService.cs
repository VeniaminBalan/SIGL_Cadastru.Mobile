using System.Net.Http.Json;
using SIGL_Cadastru.Mobile.Models.Accounts;
using SIGL_Cadastru.Mobile.Models.Shared;
using SIGL_Cadastru.Mobile.Models.Users;
using SIGL_Cadastru.Mobile.Services.Base;

namespace SIGL_Cadastru.Mobile.Services.Accounts;

public class AccountService : BaseApiService, IAccountService
{
    public AccountService(HttpClient httpClient, KeycloakAuthService authService)
        : base(httpClient, authService)
    {
    }

    public async Task<List<UserDto>> GetAccountsAsync(PagedQueryParameters? parameters = null)
    {
        var client = await GetAuthenticatedClientAsync();
        var query = parameters != null ? BuildQueryString(new Dictionary<string, string?>
        {
            ["SearchBy"] = parameters.SearchBy,
            ["PageNumber"] = parameters.PageNumber?.ToString(),
            ["PageSize"] = parameters.PageSize?.ToString()
        }) : string.Empty;

        var response = await client.GetAsync($"/api/Accounts{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<UserDto>>(JsonOptions) ?? new();
    }

    public async Task DeleteAccountAsync(string userId)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Accounts?userId={Uri.EscapeDataString(userId)}");
        response.EnsureSuccessStatusCode();
    }

    public async Task RegisterUserAsync(RegisterUser registerUser)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Accounts/register-user", registerUser, JsonOptions);
        response.EnsureSuccessStatusCode();
    }

    public async Task BlockUserAsync(string userId, bool blocked)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PutAsJsonAsync($"/api/Accounts/block-user/{Uri.EscapeDataString(userId)}", blocked, JsonOptions);
        response.EnsureSuccessStatusCode();
    }

    public async Task<UserDto> GetProfileAsync()
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/Accounts/profile");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>(JsonOptions) ?? new();
    }

    public async Task UpdateProfileAsync(UpdateProfile profile)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PutAsJsonAsync("/api/Accounts/update-profile", profile, JsonOptions);
        response.EnsureSuccessStatusCode();
    }
}
