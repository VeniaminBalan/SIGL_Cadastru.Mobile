using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGL_Cadastru.Mobile.Services;

namespace SIGL_Cadastru.Mobile.ViewModels;

public partial class RequestsViewModel : ObservableObject
{
    private readonly KeycloakAuthService _authService;

    [ObservableProperty]
    private string _pageTitle = "Requests";

    [ObservableProperty]
    private bool _isLoading;

    public RequestsViewModel(KeycloakAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    public async Task Initialize()
    {
        IsLoading = true;
        // TODO: Load requests data
        await Task.Delay(100);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task NavigateBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}
