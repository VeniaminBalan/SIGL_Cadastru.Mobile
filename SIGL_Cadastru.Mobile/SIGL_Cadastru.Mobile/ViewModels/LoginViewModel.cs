using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGL_Cadastru.Mobile.Services;

namespace SIGL_Cadastru.Mobile.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly KeycloakAuthService _auth;

    [ObservableProperty]
    private string _tokenDisplay;

    public LoginViewModel(KeycloakAuthService auth)
    {
        _auth = auth;
    }

    [RelayCommand]
    private async Task Login()
    {
        if (await _auth.LoginAsync())
            TokenDisplay = _auth.AccessToken;
        else
            TokenDisplay = "Login failed";
    }

    [RelayCommand]
    private async Task Logout()
    {
        await _auth.LogoutAsync();
        TokenDisplay = "Logged out";
    }
}
