using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using SIGL_Cadastru.Mobile.Models;

namespace SIGL_Cadastru.Mobile.Services;

/// <summary>
/// Service for communicating with SIGL Cadastru API
/// </summary>
public class SiglApiService : ISiglApiService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakAuthService _authService;
    private readonly JsonSerializerOptions _jsonOptions;

    public SiglApiService(HttpClient httpClient, KeycloakAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    private async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var token = _authService.AccessToken;
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }
        return _httpClient;
    }

    private string BuildQueryString(PagedQueryParameters? parameters)
    {
        if (parameters == null) return string.Empty;

        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(parameters.SearchBy))
            queryParams.Add($"SearchBy={Uri.EscapeDataString(parameters.SearchBy)}");
        if (parameters.PageNumber.HasValue)
            queryParams.Add($"PageNumber={parameters.PageNumber}");
        if (parameters.PageSize.HasValue)
            queryParams.Add($"PageSize={parameters.PageSize}");

        return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
    }

    private string BuildRequestQueryString(RequestQueryParameters? parameters)
    {
        if (parameters == null) return string.Empty;

        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(parameters.SearchBy))
            queryParams.Add($"SearchBy={Uri.EscapeDataString(parameters.SearchBy)}");
        if (!string.IsNullOrEmpty(parameters.FilterBy))
            queryParams.Add($"FilterBy={Uri.EscapeDataString(parameters.FilterBy)}");
        if (parameters.PageNumber.HasValue)
            queryParams.Add($"PageNumber={parameters.PageNumber}");
        if (parameters.PageSize.HasValue)
            queryParams.Add($"PageSize={parameters.PageSize}");
        if (!string.IsNullOrEmpty(parameters.OrderBy))
            queryParams.Add($"OrderBy={Uri.EscapeDataString(parameters.OrderBy)}");
        if (!string.IsNullOrEmpty(parameters.Direction))
            queryParams.Add($"Direction={Uri.EscapeDataString(parameters.Direction)}");

        return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
    }

    #region Accounts

    public async Task<List<UserDto>> GetAccountsAsync(PagedQueryParameters? parameters = null)
    {
        var client = await GetAuthenticatedClientAsync();
        var query = BuildQueryString(parameters);
        var response = await client.GetAsync($"/api/Accounts{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<UserDto>>(_jsonOptions) ?? new();
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
        var response = await client.PostAsJsonAsync("/api/Accounts/register-user", registerUser, _jsonOptions);
        response.EnsureSuccessStatusCode();
    }

    public async Task BlockUserAsync(string userId, bool blocked)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PutAsJsonAsync($"/api/Accounts/block-user/{Uri.EscapeDataString(userId)}", blocked, _jsonOptions);
        response.EnsureSuccessStatusCode();
    }

    public async Task<UserDto> GetProfileAsync()
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/Accounts/profile");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions) ?? new();
    }

    public async Task UpdateProfileAsync(UpdateProfile profile)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PutAsJsonAsync("/api/Accounts/update-profile", profile, _jsonOptions);
        response.EnsureSuccessStatusCode();
    }

    #endregion

    #region Analytics

    public async Task<string> GetEmbedUrlAsync(Dictionary<string, object> request)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Analytics/embed-url", request, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    #endregion

    #region Clients

    public async Task<List<ClientDto>> GetClientsAsync(PagedQueryParameters? parameters = null)
    {
        var client = await GetAuthenticatedClientAsync();
        var query = BuildQueryString(parameters);
        var response = await client.GetAsync($"/api/Clients{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ClientDto>>(_jsonOptions) ?? new();
    }

    public async Task<Client> CreateClientAsync(CreateClientCommand command)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Clients", command, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Client>(_jsonOptions) ?? new();
    }

    public async Task<Client> UpdateClientAsync(UpdateClientCommand command)
    {
        var client = await GetAuthenticatedClientAsync();
        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/Clients")
        {
            Content = JsonContent.Create(command, options: _jsonOptions)
        };
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Client>(_jsonOptions) ?? new();
    }

    public async Task<Client> GetClientByIdAsync(string id)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/Clients/{Uri.EscapeDataString(id)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Client>(_jsonOptions) ?? new();
    }

    public async Task DeleteClientAsync(string id)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Clients/{Uri.EscapeDataString(id)}");
        response.EnsureSuccessStatusCode();
    }

    #endregion

    #region Documents

    public async Task<Document> AddDocumentAsync(AddDocumentRequest request)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Documents", request, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Document>(_jsonOptions) ?? new();
    }

    public async Task<Document> UpdateDocumentAsync(UpdateDocumentCommand command)
    {
        var client = await GetAuthenticatedClientAsync();
        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/Documents")
        {
            Content = JsonContent.Create(command, options: _jsonOptions)
        };
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Document>(_jsonOptions) ?? new();
    }

    public async Task DeleteDocumentAsync(string id, bool removeFile = false)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Documents/{Uri.EscapeDataString(id)}?removeFile={removeFile}");
        response.EnsureSuccessStatusCode();
    }

    #endregion

    #region Files

    public async Task<List<FileModel>> GetFilesAsync()
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/Files");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FileModel>>(_jsonOptions) ?? new();
    }

    public async Task<FileModel> UploadFileAsync(Stream fileStream, string fileName)
    {
        var client = await GetAuthenticatedClientAsync();
        
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(streamContent, "file", fileName);

        var response = await client.PostAsync("/api/Files", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FileModel>(_jsonOptions) ?? new();
    }

    public async Task<Stream> GetFileByIdAsync(string id)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/Files/{Uri.EscapeDataString(id)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync();
    }

    public async Task DeleteFileAsync(string id)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Files/{Uri.EscapeDataString(id)}");
        response.EnsureSuccessStatusCode();
    }

    #endregion

    #region Migrations

    public async Task<MigrationResult> MigrateClientsAsync(ClientMigrationCommand command)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Migrations/migrate-clients", command, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MigrationResult>(_jsonOptions) ?? new();
    }

    public async Task<MigrationResult> MigrateRequestsAsync(MigrateCadastralRequestCommand command)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Migrations/migrate-requests", command, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MigrationResult>(_jsonOptions) ?? new();
    }

    public async Task<ExctractDataResult> UploadDatabaseAsync(Stream dbFileStream, string fileName)
    {
        var client = await GetAuthenticatedClientAsync();
        
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(dbFileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(streamContent, "dbFile", fileName);

        var response = await client.PostAsync("/api/Migrations/upload-db", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ExctractDataResult>(_jsonOptions) ?? new();
    }

    #endregion

    #region Tree

    public async Task<TreeDto> GetTreeAsync()
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync("/tree");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TreeDto>(_jsonOptions) ?? new();
    }

    #endregion

    #region Requests

    public async Task<List<CadastralRequestDto>> GetRequestsAsync(RequestQueryParameters? parameters = null)
    {
        var client = await GetAuthenticatedClientAsync();
        var query = BuildRequestQueryString(parameters);
        var response = await client.GetAsync($"/api/Requests{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<CadastralRequestDto>>(_jsonOptions) ?? new();
    }

    public async Task<DetailedCadastralRequest> CreateRequestAsync(CreateCadastralRequestCommand command)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Requests", command, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DetailedCadastralRequest>(_jsonOptions) ?? new();
    }

    public async Task<DetailedCadastralRequest> GetRequestByIdAsync(string id)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/Requests/{Uri.EscapeDataString(id)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DetailedCadastralRequest>(_jsonOptions) ?? new();
    }

    public async Task UpdateRequestAsync(string id, string updateData)
    {
        var client = await GetAuthenticatedClientAsync();
        var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/Requests/{Uri.EscapeDataString(id)}")
        {
            Content = new StringContent($"\"{updateData}\"", Encoding.UTF8, "application/json")
        };
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteRequestAsync(string id)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Requests/{Uri.EscapeDataString(id)}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<RequestState> AddRequestStateAsync(string requestId, AddStateRequest request)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync($"/api/Requests/{Uri.EscapeDataString(requestId)}/states", request, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RequestState>(_jsonOptions) ?? new();
    }

    public async Task<RequestState> DeleteRequestStateAsync(string requestId, string stateId)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Requests/{Uri.EscapeDataString(requestId)}/states/{Uri.EscapeDataString(stateId)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RequestState>(_jsonOptions) ?? new();
    }

    public async Task<Stream> GetRequestPdfAsync(string id)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync($"/pdf/{Uri.EscapeDataString(id)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync();
    }

    public async Task<Stream> PreviewRequestPdfAsync(CreateCadastralRequestCommand command)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/pdf-preview", command, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync();
    }

    #endregion

    #region Users

    public async Task<UserDtoPagedResponse> GetPerformersAsync(PagedQueryParameters? parameters = null)
    {
        var client = await GetAuthenticatedClientAsync();
        var query = BuildQueryString(parameters);
        var response = await client.GetAsync($"/api/Users/performers{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDtoPagedResponse>(_jsonOptions) ?? new();
    }

    public async Task<UserDtoPagedResponse> GetResponsibleUsersAsync(PagedQueryParameters? parameters = null)
    {
        var client = await GetAuthenticatedClientAsync();
        var query = BuildQueryString(parameters);
        var response = await client.GetAsync($"/api/Users/responsible{query}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDtoPagedResponse>(_jsonOptions) ?? new();
    }

    #endregion
}
