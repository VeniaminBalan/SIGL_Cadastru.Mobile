using SIGL_Cadastru.Mobile.Models.Clients;
using SIGL_Cadastru.Mobile.Models.Shared;

namespace SIGL_Cadastru.Mobile.Services.Clients;

/// <summary>
/// Interface for client management operations
/// </summary>
public interface IClientService
{
    Task<List<ClientDto>> GetClientsAsync(PagedQueryParameters? parameters = null);
    Task<Client> CreateClientAsync(CreateClientCommand command);
    Task<Client> UpdateClientAsync(UpdateClientCommand command);
    Task<Client> GetClientByIdAsync(string id);
    Task DeleteClientAsync(string id);
}
