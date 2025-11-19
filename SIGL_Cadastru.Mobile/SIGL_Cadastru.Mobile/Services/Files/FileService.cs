using System.Net.Http.Headers;
using System.Net.Http.Json;
using SIGL_Cadastru.Mobile.Models.Files;
using SIGL_Cadastru.Mobile.Services.Base;

namespace SIGL_Cadastru.Mobile.Services.Files;

public class FileService : BaseApiService, IFileService
{
    public FileService(HttpClient httpClient, KeycloakAuthService authService)
        : base(httpClient, authService)
    {
    }

    public async Task<List<FileModel>> GetFilesAsync()
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/Files");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FileModel>>(JsonOptions) ?? new();
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
        return await response.Content.ReadFromJsonAsync<FileModel>(JsonOptions) ?? new();
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
}
