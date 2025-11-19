using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGL_Cadastru.Mobile.Services;

namespace SIGL_Cadastru.Mobile.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly KeycloakAuthService _authService;

    [ObservableProperty]
    private string _pageTitle = "Profile";

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public ProfileViewModel(KeycloakAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    public async Task Initialize()
    {
        IsLoading = true;
        
        if (_authService.IsAuthenticated)
        {
            UserName = _authService.GetUserName();
            // Email retrieval will be added when GetUserEmail method is implemented
            Email = "email@example.com";
        }
        
        await Task.Delay(100);
        IsLoading = false;
    }
}
