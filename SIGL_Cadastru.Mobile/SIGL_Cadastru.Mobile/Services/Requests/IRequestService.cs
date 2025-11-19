using SIGL_Cadastru.Mobile.Models.Requests;
using SIGL_Cadastru.Mobile.Models.Shared;

namespace SIGL_Cadastru.Mobile.Services.Requests;

/// <summary>
/// Interface for cadastral request management operations
/// </summary>
public interface IRequestService
{
    Task<List<CadastralRequestDto>> GetRequestsAsync(RequestQueryParameters? parameters = null);
    Task<DetailedCadastralRequest> CreateRequestAsync(CreateCadastralRequestCommand command);
    Task<DetailedCadastralRequest> GetRequestByIdAsync(string id);
    Task UpdateRequestAsync(string id, string updateData);
    Task DeleteRequestAsync(string id);
    Task<RequestState> AddRequestStateAsync(string requestId, AddStateRequest request);
    Task<RequestState> DeleteRequestStateAsync(string requestId, string stateId);
    Task<Stream> GetRequestPdfAsync(string id);
    Task<Stream> PreviewRequestPdfAsync(CreateCadastralRequestCommand command);
    Task<TreeDto> GetTreeAsync();
}
