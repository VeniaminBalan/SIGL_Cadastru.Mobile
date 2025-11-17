using SIGL_Cadastru.Mobile.Models;

namespace SIGL_Cadastru.Mobile.Services;

public class AuthService
{
    public async Task<UserModel?> LoginAsync(string user, string pass)
    {
        await Task.Delay(500); // simulate API

        if (user == "admin" && pass == "1234")
        {
            return new UserModel
            {
                Username = "admin",
                Token = "fake-jwt-token"
            };
        }

        return null;
    }
}
