using System.Net.Http.Json;
using System.Text;
using SIGL_Cadastru.Mobile.Models.Requests;
using SIGL_Cadastru.Mobile.Models.Shared;
using SIGL_Cadastru.Mobile.Services.Base;

namespace SIGL_Cadastru.Mobile.Services.Requests;

public class RequestService : BaseApiService, IRequestService
{
    public RequestService(HttpClient httpClient, KeycloakAuthService authService)
        : base(httpClient, authService)
    {
    }

    public async Task<List<CadastralRequestDto>> GetRequestsAsync(RequestQueryParameters? parameters = null)
    {
        var pagedResponse = await GetRequestsPagedAsync(parameters);
        return pagedResponse.Data?.ToList() ?? new();
    }

    public async Task<Models.PagedResponse<CadastralRequestDto>> GetRequestsPagedAsync(RequestQueryParameters? parameters = null)
    {
        var client = await GetAuthenticatedClientAsync();
        var queryParams = new Dictionary<string, string?>();
        
        if (parameters != null)
        {
            queryParams["SearchBy"] = parameters.SearchBy;
            queryParams["FilterBy"] = parameters.FilterBy;
            queryParams["PageNumber"] = parameters.PageNumber?.ToString();
            queryParams["PageSize"] = parameters.PageSize?.ToString();
            queryParams["OrderBy"] = parameters.OrderBy;
            queryParams["Direction"] = parameters.Direction;
            if (parameters.IsFullyPaid.HasValue)
            {
                queryParams["IsFullyPaid"] = parameters.IsFullyPaid.Value.ToString().ToLower();
            }
        }
        
        var query = BuildQueryString(queryParams);

        var response = await client.GetAsync($"/api/Requests{query}");
        response.EnsureSuccessStatusCode();
        var pagedResponse = await response.Content.ReadFromJsonAsync<Models.PagedResponse<CadastralRequestDto>>(JsonOptions);
        return pagedResponse ?? new Models.PagedResponse<CadastralRequestDto>(new List<CadastralRequestDto>(), 1, 10, 0);
    }

    public async Task<DetailedCadastralRequest> CreateRequestAsync(CreateCadastralRequestCommand command)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Requests", command, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DetailedCadastralRequest>(JsonOptions) ?? new();
    }

    public async Task<DetailedCadastralRequest> GetRequestByIdAsync(string id)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/Requests/{Uri.EscapeDataString(id)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DetailedCadastralRequest>(JsonOptions) ?? new();
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
        var response = await client.PostAsJsonAsync($"/api/Requests/{Uri.EscapeDataString(requestId)}/states", request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RequestState>(JsonOptions) ?? new();
    }

    public async Task<RequestState> DeleteRequestStateAsync(string requestId, string stateId)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Requests/{Uri.EscapeDataString(requestId)}/states/{Uri.EscapeDataString(stateId)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RequestState>(JsonOptions) ?? new();
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
        var response = await client.PostAsJsonAsync("/pdf-preview", command, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync();
    }

    public async Task<TreeDto> GetTreeAsync()
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync("/tree");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TreeDto>(JsonOptions) ?? new();
    }

    public async Task<PaymentDto> AddPaymentAsync(string requestId, AddPaymentCommand command)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync($"/api/Requests/{Uri.EscapeDataString(requestId)}/payments", command, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PaymentDto>(JsonOptions) ?? new();
    }

    public async Task DeletePaymentAsync(string requestId, string paymentId)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Requests/{Uri.EscapeDataString(requestId)}/payments/{Uri.EscapeDataString(paymentId)}");
        response.EnsureSuccessStatusCode();
    }
}
