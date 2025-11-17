using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGL_Cadastru.Mobile.Services;

namespace SIGL_Cadastru.Mobile.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService authService;

    public LoginViewModel(AuthService service)
    {
        authService = service;
    }


    [ObservableProperty]
    private string username;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string message;

    [RelayCommand]
    private async Task Login()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        var user = await authService.LoginAsync(Username, Password);

        if (user is not null)
        {
            Message = $"Welcome, {user.Username}";
        }
        else
        {
            Message = "Invalid credentials";
        }

        IsBusy = false;
    }
}
