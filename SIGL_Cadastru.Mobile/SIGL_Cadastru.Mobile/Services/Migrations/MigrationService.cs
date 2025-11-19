using System.Net.Http.Headers;
using System.Net.Http.Json;
using SIGL_Cadastru.Mobile.Models.Migrations;
using SIGL_Cadastru.Mobile.Services.Base;

namespace SIGL_Cadastru.Mobile.Services.Migrations;

public class MigrationService : BaseApiService, IMigrationService
{
    public MigrationService(HttpClient httpClient, KeycloakAuthService authService)
        : base(httpClient, authService)
    {
    }

    public async Task<MigrationResult> MigrateClientsAsync(ClientMigrationCommand command)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Migrations/migrate-clients", command, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MigrationResult>(JsonOptions) ?? new();
    }

    public async Task<MigrationResult> MigrateRequestsAsync(MigrateCadastralRequestCommand command)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/Migrations/migrate-requests", command, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MigrationResult>(JsonOptions) ?? new();
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
        return await response.Content.ReadFromJsonAsync<ExctractDataResult>(JsonOptions) ?? new();
    }
}
