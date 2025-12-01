using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SIGL_Cadastru.Mobile.Models.Device;
using SIGL_Cadastru.Mobile.Services.Base;

namespace SIGL_Cadastru.Mobile.Services.Device;

/// <summary>
/// Implementation of device API service
/// </summary>
public class DeviceService : BaseApiService, IDeviceService
{
    private readonly ILogger<DeviceService> _logger;

    public DeviceService(
        HttpClient httpClient, 
        KeycloakAuthService authService,
        ILogger<DeviceService> logger) 
        : base(httpClient, authService)
    {
        _logger = logger;
    }

    public async Task<DeviceResponse?> RegisterDeviceAsync(RegisterDeviceRequest request)
    {
        try
        {
            var response = await HttpClient.PostAsJsonAsync("/api/device/register", request, JsonOptions);
            response.EnsureSuccessStatusCode();

            var deviceResponse = await response.Content.ReadFromJsonAsync<DeviceResponse>(JsonOptions);
            _logger.LogInformation("Device registered: {DeviceId}", request.DeviceId);
            
            return deviceResponse;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error registering device: {DeviceId}", request.DeviceId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering device: {DeviceId}", request.DeviceId);
            throw;
        }
    }

    public async Task<bool> LogoutDeviceAsync(LogoutDeviceRequest request)
    {
        try
        {
            var response = await HttpClient.PostAsJsonAsync("/api/device/logout", request, JsonOptions);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
            var isSuccess = result.TryGetProperty("isSuccess", out var successProperty) && 
                           successProperty.GetBoolean();

            _logger.LogInformation("Device logout: {DeviceId}, Success: {IsSuccess}", 
                request.DeviceId, isSuccess);
            
            return isSuccess;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error logging out device: {DeviceId}", request.DeviceId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging out device: {DeviceId}", request.DeviceId);
            return false;
        }
    }
}
