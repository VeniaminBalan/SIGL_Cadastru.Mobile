using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Plugin.Firebase.CloudMessaging;
using SIGL_Cadastru.Mobile.Services;

namespace SIGL_Cadastru.Mobile.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly KeycloakAuthService _auth;
    private readonly DeviceManager _deviceManager;
    private readonly ILogger<LoginViewModel> _logger;

    [ObservableProperty]
    private string _tokenDisplay = string.Empty;

    [ObservableProperty]
    private bool _isLoggedIn;

    [ObservableProperty]
    private string _fcmToken = "Loading FCM token...";

    public LoginViewModel(
        KeycloakAuthService auth, 
        DeviceManager deviceManager,
        ILogger<LoginViewModel> logger)
    {
        _auth = auth;
        _deviceManager = deviceManager;
        _logger = logger;
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
            
            // Register device with FCM token after successful login
            try
            {
                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                var fcmToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                await _deviceManager.RegisterDeviceAsync(fcmToken);
                _logger.LogInformation("Device registered after login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register device after login");
            }
            
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
        // Logout device from API
        try
        {
            await _deviceManager.LogoutDeviceAsync();
            _logger.LogInformation("Device logged out");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to logout device");
        }
        
        await _auth.LogoutAsync();
        TokenDisplay = "Logged out";
        UpdateAuthState();
    }


}
