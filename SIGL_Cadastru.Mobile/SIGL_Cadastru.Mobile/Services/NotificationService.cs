using Microsoft.Extensions.Logging;
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.CloudMessaging.EventArgs;

namespace SIGL_Cadastru.Mobile.Services;

/// <summary>
/// Service to handle Firebase Cloud Messaging notifications
/// </summary>
public class NotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly DeviceManager _deviceManager;

    public NotificationService(ILogger<NotificationService> logger, DeviceManager deviceManager)
    {
        _logger = logger;
        _deviceManager = deviceManager;
    }

    /// <summary>
    /// Initializes FCM notification listeners
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Initializing notification service");

            // Setup notification received handler (when app is in foreground)
            CrossFirebaseCloudMessaging.Current.NotificationReceived += OnNotificationReceived;

            // Setup notification tapped handler (when user taps notification)
            CrossFirebaseCloudMessaging.Current.NotificationTapped += OnNotificationTapped;

            // Setup FCM token refresh
            await _deviceManager.SetupFcmTokenRefreshAsync();

            _logger.LogInformation("Notification service initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize notification service");
        }
    }

    /// <summary>
    /// Handles notification received while app is in foreground
    /// </summary>
    private void OnNotificationReceived(object? sender, FCMNotificationReceivedEventArgs e)
    {
        try
        {
            var title = e.Notification.Title;
            var body = e.Notification.Body;
            var data = e.Notification.Data;

            _logger.LogInformation("Notification received - Title: {Title}, Body: {Body}", title, body);

            // Display notification to user on main thread
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var currentPage = Application.Current?.Windows[0]?.Page;
                    if (currentPage != null)
                    {
                        await currentPage.DisplayAlertAsync(
                            title ?? "Notification",
                            body ?? "You have a new notification",
                            "OK");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error displaying notification alert");
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling notification received");
        }
    }

    /// <summary>
    /// Handles notification tapped by user
    /// </summary>
    private void OnNotificationTapped(object? sender, FCMNotificationTappedEventArgs e)
    {
        try
        {
            var data = e.Notification.Data;
            _logger.LogInformation("Notification tapped with data: {DataCount} items", data?.Count ?? 0);

            // Navigate based on notification data
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    if (data != null && data.TryGetValue("type", out var notificationType))
                    {
                        _logger.LogInformation("Handling notification type: {Type}", notificationType);

                        // Navigate based on notification type
                        switch (notificationType?.ToLower())
                        {
                            case "request":
                                if (data.TryGetValue("requestId", out var requestId))
                                {
                                    await Shell.Current.GoToAsync($"//RequestDetailPage?id={requestId}");
                                }
                                else
                                {
                                    await Shell.Current.GoToAsync("//RequestsPage");
                                }
                                break;

                            case "client":
                                await Shell.Current.GoToAsync("//ClientsPage");
                                break;

                            case "profile":
                                await Shell.Current.GoToAsync("//ProfilePage");
                                break;

                            default:
                                // Default to requests page
                                await Shell.Current.GoToAsync("//RequestsPage");
                                break;
                        }
                    }
                    else
                    {
                        // No specific navigation data, go to main page
                        await Shell.Current.GoToAsync("//RequestsPage");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error navigating after notification tap");
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling notification tapped");
        }
    }

    /// <summary>
    /// Cleanup resources
    /// </summary>
    public void Dispose()
    {
        CrossFirebaseCloudMessaging.Current.NotificationReceived -= OnNotificationReceived;
        CrossFirebaseCloudMessaging.Current.NotificationTapped -= OnNotificationTapped;
    }
}
