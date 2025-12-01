using SIGL_Cadastru.Mobile.Models.Device;

namespace SIGL_Cadastru.Mobile.Services.Device;

/// <summary>
/// Service interface for device registration and management
/// </summary>
public interface IDeviceService
{
    /// <summary>
    /// Registers a device with the API
    /// </summary>
    Task<DeviceResponse?> RegisterDeviceAsync(RegisterDeviceRequest request);

    /// <summary>
    /// Logs out a device from the API
    /// </summary>
    Task<bool> LogoutDeviceAsync(LogoutDeviceRequest request);
}
