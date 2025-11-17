using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGL_Cadastru.Mobile.Services;

namespace SIGL_Cadastru.Mobile.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly KeycloakAuthService _authService;

    [ObservableProperty]
    private string _welcomeMessage;

    [ObservableProperty]
    private bool _isLoading;

    public MainViewModel(KeycloakAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    public async Task Initialize()
    {
        IsLoading = true;

        if (!_authService.IsAuthenticated)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
        else
        {
            var userName = _authService.GetUserName();
            WelcomeMessage = $"Welcome, {userName}!";
        }

        IsLoading = false;
    }

    [RelayCommand]
    private async Task Logout()
    {
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
