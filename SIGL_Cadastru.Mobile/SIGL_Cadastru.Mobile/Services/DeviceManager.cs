using Microsoft.Extensions.Logging;
using Plugin.Firebase.CloudMessaging;
using SIGL_Cadastru.Mobile.Models.Device;
using SIGL_Cadastru.Mobile.Services.Device;

namespace SIGL_Cadastru.Mobile.Services;

/// <summary>
/// Manages device registration, identification, and FCM token handling
/// </summary>
public class DeviceManager
{
    private readonly IDeviceService _deviceService;
    private readonly ILogger<DeviceManager> _logger;
    private const string DeviceIdKey = "device_id";
    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    private static string? _cachedDeviceId;

    public DeviceManager(IDeviceService deviceService, ILogger<DeviceManager> logger)
    {
        _deviceService = deviceService;
        _logger = logger;
    }

    /// <summary>
    /// Gets or generates the unique device identifier (thread-safe)
    /// </summary>
    public string GetDeviceId()
    {
        // Return cached value if available
        if (!string.IsNullOrEmpty(_cachedDeviceId))
        {
            return _cachedDeviceId;
        }

        _semaphore.Wait();
        try
        {
            // Double-check after acquiring lock
            if (!string.IsNullOrEmpty(_cachedDeviceId))
            {
                return _cachedDeviceId;
            }

            // Try to get from SecureStorage first
            var deviceId = SecureStorage.GetAsync(DeviceIdKey).Result;

            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();
                SecureStorage.SetAsync(DeviceIdKey, deviceId).Wait();
                _logger.LogInformation("Generated new DeviceId: {DeviceId}", deviceId);
            }

            _cachedDeviceId = deviceId;
            return deviceId;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Gets the current platform name (android, ios, etc.)
    /// </summary>
    public string GetPlatform()
    {
        return DeviceInfo.Platform.ToString().ToLower();
    }

    /// <summary>
    /// Registers the device with the API
    /// </summary>
    public async Task<DeviceResponse?> RegisterDeviceAsync(string fcmToken)
    {
        try
        {
            var deviceId = GetDeviceId();
            var platform = GetPlatform();

            var request = new RegisterDeviceRequest(deviceId, fcmToken, platform);
            var response = await _deviceService.RegisterDeviceAsync(request);

            _logger.LogInformation("Device registered successfully: {DeviceId}, Platform: {Platform}", 
                deviceId, platform);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register device");
            return null;
        }
    }

    /// <summary>
    /// Sets up FCM token refresh listener and performs initial registration
    /// </summary>
    public async Task SetupFcmTokenRefreshAsync()
    {
        try
        {
            // Check if FCM is available
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();

            // Get initial token and register
            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            _logger.LogInformation("FCM Token retrieved: {Token}", token);

            await RegisterDeviceAsync(token);

            // Subscribe to token refresh events
            CrossFirebaseCloudMessaging.Current.TokenChanged += async (s, e) =>
            {
                _logger.LogInformation("FCM Token changed: {NewToken}", e.Token);
                await RegisterDeviceAsync(e.Token);
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup FCM token refresh");
        }
    }

    /// <summary>
    /// Logs out the device from the API
    /// </summary>
    public async Task<bool> LogoutDeviceAsync()
    {
        try
        {
            var deviceId = GetDeviceId();
            var request = new LogoutDeviceRequest(deviceId);
            
            var success = await _deviceService.LogoutDeviceAsync(request);

            if (success)
            {
                _logger.LogInformation("Device logged out successfully: {DeviceId}", deviceId);
            }
            else
            {
                _logger.LogWarning("Device logout returned false: {DeviceId}", deviceId);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to logout device");
            return false;
        }
    }

    /// <summary>
    /// Clears local device data (call after logout)
    /// Note: DeviceId is preserved as it represents the device installation, not the user session
    /// </summary>
    public async Task ClearLocalDataAsync()
    {
        try
        {
            // Preserve DeviceId before clearing
            var deviceId = await SecureStorage.GetAsync(DeviceIdKey);
            
            Preferences.Clear();
            SecureStorage.RemoveAll();
            
            // Restore DeviceId after clearing
            if (!string.IsNullOrEmpty(deviceId))
            {
                await SecureStorage.SetAsync(DeviceIdKey, deviceId);
            }
            
            _logger.LogInformation("Local device data cleared (DeviceId preserved)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear local data");
        }
    }
}
