using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGL_Cadastru.Mobile.Services;

namespace SIGL_Cadastru.Mobile.ViewModels;

public partial class ClientsViewModel : ObservableObject
{
    private readonly KeycloakAuthService _authService;

    [ObservableProperty]
    private string _pageTitle = "Clients";

    [ObservableProperty]
    private bool _isLoading;

    public ClientsViewModel(KeycloakAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    public async Task Initialize()
    {
        IsLoading = true;
        // TODO: Load clients data
        await Task.Delay(100);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task NavigateBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}
