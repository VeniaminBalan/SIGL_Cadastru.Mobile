using System.Net.Http.Json;
using SIGL_Cadastru.Mobile.Models.Documents;
using SIGL_Cadastru.Mobile.Services.Base;

namespace SIGL_Cadastru.Mobile.Services.Documents;

public class DocumentService : BaseApiService, IDocumentService
{
    public DocumentService(HttpClient httpClient, KeycloakAuthService authService)
        : base(httpClient, authService)
    {
    }

    public async Task<Document> AddDocumentAsync(AddDocumentRequest request)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Documents", request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Document>(JsonOptions) ?? new();
    }

    public async Task<Document> UpdateDocumentAsync(UpdateDocumentCommand command)
    {
        var client = await GetAuthenticatedClientAsync();
        var request = new HttpRequestMessage(HttpMethod.Patch, "/api/Documents")
        {
            Content = JsonContent.Create(command, options: JsonOptions)
        };
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Document>(JsonOptions) ?? new();
    }

    public async Task DeleteDocumentAsync(string id, bool removeFile = false)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.DeleteAsync($"/api/Documents/{Uri.EscapeDataString(id)}?removeFile={removeFile}");
        response.EnsureSuccessStatusCode();
    }
}
