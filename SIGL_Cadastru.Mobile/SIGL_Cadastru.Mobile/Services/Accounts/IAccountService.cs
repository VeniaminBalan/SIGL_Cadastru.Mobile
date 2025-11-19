using SIGL_Cadastru.Mobile.Models.Accounts;
using SIGL_Cadastru.Mobile.Models.Shared;
using SIGL_Cadastru.Mobile.Models.Users;

namespace SIGL_Cadastru.Mobile.Services.Accounts;

/// <summary>
/// Interface for account management operations
/// </summary>
public interface IAccountService
{
    Task<List<UserDto>> GetAccountsAsync(PagedQueryParameters? parameters = null);
    Task DeleteAccountAsync(string userId);
    Task RegisterUserAsync(RegisterUser registerUser);
    Task BlockUserAsync(string userId, bool blocked);
    Task<UserDto> GetProfileAsync();
    Task UpdateProfileAsync(UpdateProfile profile);
}
