using SIGL_Cadastru.Mobile.Models.Shared;
using SIGL_Cadastru.Mobile.Models.Users;

namespace SIGL_Cadastru.Mobile.Services.Users;

/// <summary>
/// Interface for user management operations
/// </summary>
public interface IUserService
{
    Task<UserDtoPagedResponse> GetPerformersAsync(PagedQueryParameters? parameters = null);
    Task<UserDtoPagedResponse> GetResponsibleUsersAsync(PagedQueryParameters? parameters = null);
}
