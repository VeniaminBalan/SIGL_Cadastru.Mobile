using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGL_Cadastru.Mobile.Services;

namespace SIGL_Cadastru.Mobile.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly KeycloakAuthService _auth;

    [ObservableProperty]
    private string _tokenDisplay;

    [ObservableProperty]
    private bool _isLoggedIn;

    public LoginViewModel(KeycloakAuthService auth)
    {
        _auth = auth;
    }

    public void UpdateAuthState()
    {
        IsLoggedIn = _auth.IsAuthenticated;
    }

    [RelayCommand]
    private async Task Login()
    {
        if (await _auth.LoginAsync())
        {
            TokenDisplay = _auth.AccessToken;
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            TokenDisplay = "Login failed";
        }
        
        UpdateAuthState();
    }

    [RelayCommand]
    private async Task Logout()
    {
        await _auth.LogoutAsync();
        TokenDisplay = "Logged out";
        UpdateAuthState();
    }
}
