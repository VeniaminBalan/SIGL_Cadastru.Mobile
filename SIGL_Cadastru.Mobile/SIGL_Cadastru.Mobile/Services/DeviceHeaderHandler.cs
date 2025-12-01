using Microsoft.Extensions.Logging;

namespace SIGL_Cadastru.Mobile.Services;

/// <summary>
/// HTTP message handler that adds the X-Mobile-Origin header to all requests
/// </summary>
public class DeviceHeaderHandler : DelegatingHandler
{
    private readonly ILogger<DeviceHeaderHandler> _logger;
    private readonly Lazy<DeviceManager> _deviceManager;

    public DeviceHeaderHandler(ILogger<DeviceHeaderHandler> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        // Use Lazy to defer DeviceManager resolution and avoid circular dependency
        _deviceManager = new Lazy<DeviceManager>(() => 
            serviceProvider.GetRequiredService<DeviceManager>());
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Add device ID header to every request for activity tracking
            var deviceId = _deviceManager.Value.GetDeviceId();
            
            if (!string.IsNullOrEmpty(deviceId))
            {
                request.Headers.Add("X-Mobile-Origin", deviceId);
                _logger.LogDebug("Added X-Mobile-Origin header: {DeviceId}", deviceId);
            }
            else
            {
                _logger.LogDebug("DeviceId not yet generated, X-Mobile-Origin header not added");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding X-Mobile-Origin header");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
