using SIGL_Cadastru.Mobile.Models;

namespace SIGL_Cadastru.Mobile.Services;

/// <summary>
/// Interface for SIGL Cadastru API service
/// </summary>
public interface ISiglApiService
{
    // Accounts Endpoints
    Task<List<UserDto>> GetAccountsAsync(PagedQueryParameters? parameters = null);
    Task DeleteAccountAsync(string userId);
    Task RegisterUserAsync(RegisterUser registerUser);
    Task BlockUserAsync(string userId, bool blocked);
    Task<UserDto> GetProfileAsync();
    Task UpdateProfileAsync(UpdateProfile profile);

    // Analytics Endpoints
    Task<string> GetEmbedUrlAsync(Dictionary<string, object> request);

    // Clients Endpoints
    Task<List<ClientDto>> GetClientsAsync(PagedQueryParameters? parameters = null);
    Task<Client> CreateClientAsync(CreateClientCommand command);
    Task<Client> UpdateClientAsync(UpdateClientCommand command);
    Task<Client> GetClientByIdAsync(string id);
    Task DeleteClientAsync(string id);

    // Documents Endpoints
    Task<Document> AddDocumentAsync(AddDocumentRequest request);
    Task<Document> UpdateDocumentAsync(UpdateDocumentCommand command);
    Task DeleteDocumentAsync(string id, bool removeFile = false);

    // Files Endpoints
    Task<List<FileModel>> GetFilesAsync();
    Task<FileModel> UploadFileAsync(Stream fileStream, string fileName);
    Task<Stream> GetFileByIdAsync(string id);
    Task DeleteFileAsync(string id);

    // Migrations Endpoints
    Task<MigrationResult> MigrateClientsAsync(ClientMigrationCommand command);
    Task<MigrationResult> MigrateRequestsAsync(MigrateCadastralRequestCommand command);
    Task<ExctractDataResult> UploadDatabaseAsync(Stream dbFileStream, string fileName);

    // Tree Endpoint
    Task<TreeDto> GetTreeAsync();

    // Requests Endpoints
    Task<List<CadastralRequestDto>> GetRequestsAsync(RequestQueryParameters? parameters = null);
    Task<DetailedCadastralRequest> CreateRequestAsync(CreateCadastralRequestCommand command);
    Task<DetailedCadastralRequest> GetRequestByIdAsync(string id);
    Task UpdateRequestAsync(string id, string updateData);
    Task DeleteRequestAsync(string id);
    Task<RequestState> AddRequestStateAsync(string requestId, AddStateRequest request);
    Task<RequestState> DeleteRequestStateAsync(string requestId, string stateId);
    Task<Stream> GetRequestPdfAsync(string id);
    Task<Stream> PreviewRequestPdfAsync(CreateCadastralRequestCommand command);

    // Users Endpoints
    Task<UserDtoPagedResponse> GetPerformersAsync(PagedQueryParameters? parameters = null);
    Task<UserDtoPagedResponse> GetResponsibleUsersAsync(PagedQueryParameters? parameters = null);
}
