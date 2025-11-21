using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Firebase.CloudMessaging;
using SIGL_Cadastru.Mobile.Services;

namespace SIGL_Cadastru.Mobile.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly KeycloakAuthService _auth;

    [ObservableProperty]
    private string _tokenDisplay = string.Empty;

    [ObservableProperty]
    private bool _isLoggedIn;

    [ObservableProperty]
    private string _fcmToken = "Loading FCM token...";

    public LoginViewModel(KeycloakAuthService auth)
    {
        _auth = auth;
        LoadFcmToken();
    }

    private async void LoadFcmToken()
    {
        try
        {
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            FcmToken = $"FCM Token: {token}";
            Console.WriteLine($"FCM token: {token}");
        }
        catch (Exception ex)
        {
            FcmToken = $"FCM Error: {ex.Message}";
            Console.WriteLine($"FCM error: {ex.Message}");
        }
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
            TokenDisplay = _auth.AccessToken ?? "No token";
            await Shell.Current.GoToAsync("//RequestsPage");
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
