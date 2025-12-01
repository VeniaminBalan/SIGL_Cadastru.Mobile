# Device Management & Push Notifications Integration Guide

This document describes the device management system and how mobile applications should integrate with it.

## Overview

The device management system tracks user devices, manages push notifications via Firebase Cloud Messaging (FCM), and monitors device activity. Each device is uniquely identified by a `DeviceId` (generated once per app installation) and associated with an FCM token for push notifications.

## System Architecture

### Key Components

1. **Device Registration** - Registers devices and manages FCM tokens
2. **Device Activity Middleware** - Tracks device last-seen timestamps
3. **Device Logout** - Removes devices from the system
4. **Push Notifications Service** - Sends notifications to user devices
5. **Device Cleanup Job** - Removes inactive devices automatically

### Device Data Model

```csharp
public class Device
{
    public string DeviceId { get; set; }        // Unique identifier per app installation
    public string FcmToken { get; set; }        // Firebase Cloud Messaging token
    public string Platform { get; set; }        // "android", "ios", etc.
    public string UserId { get; set; }          // Associated user ID
    public DateTimeOffset LastSeenUtc { get; set; }  // Last activity timestamp
    public DateTimeOffset CreatedUtc { get; set; }   // Registration timestamp
}
```

## Mobile Application Integration

### 1. Device Registration

**When to Register:**
- On first app installation
- After user login/signup
- When FCM token is refreshed by Firebase SDK

**Endpoint:** `POST /api/device/register`

**Request Body:**
```json
{
  "deviceId": "unique-device-identifier",
  "fcmToken": "firebase-fcm-token",
  "platform": "android"
}
```

**Headers:**
```
Authorization: Bearer <jwt-token>
Content-Type: application/json
```

**Response (200 OK):**
```json
{
  "deviceId": "unique-device-identifier",
  "fcmToken": "firebase-fcm-token",
  "platform": "android",
  "userId": "user-id",
  "lastSeenUtc": "2025-12-01T10:00:00Z",
  "createdUtc": "2025-12-01T10:00:00Z"
}
```

**Implementation Guide:**

```csharp
// .NET MAUI Example
public class DeviceManager
{
    private readonly IApiService _apiService;
    private const string DeviceIdKey = "device_id";
    
    public DeviceManager(IApiService apiService)
    {
        _apiService = apiService;
    }
    
    // Generate DeviceId once and store it persistently
    public string GetDeviceId()
    {
        var deviceId = Preferences.Get(DeviceIdKey, string.Empty);
        
        if (string.IsNullOrEmpty(deviceId))
        {
            deviceId = Guid.NewGuid().ToString();
            Preferences.Set(DeviceIdKey, deviceId);
        }
        
        return deviceId;
    }
    
    // Get platform name
    private string GetPlatform()
    {
        return DeviceInfo.Platform.ToString().ToLower(); // "android", "ios", etc.
    }
    
    // Register device after login and FCM token retrieval
    public async Task RegisterDeviceAsync(string fcmToken)
    {
        var deviceId = GetDeviceId();
        var platform = GetPlatform();
        
        var request = new RegisterDeviceRequest
        {
            DeviceId = deviceId,
            FcmToken = fcmToken,
            Platform = platform
        };
        
        await _apiService.RegisterDeviceAsync(request);
    }
    
    // Setup FCM token refresh listener (using Plugin.Firebase)
    public async Task SetupFcmTokenRefreshAsync()
    {
        try
        {
            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            await RegisterDeviceAsync(token);
            
            // Subscribe to token refresh
            CrossFirebaseCloudMessaging.Current.TokenChanged += async (s, e) =>
            {
                await RegisterDeviceAsync(e.Token);
            };
        }
        catch (Exception ex)
        {
            // Handle error
            Debug.WriteLine($"FCM setup error: {ex.Message}");
        }
    }
}
```

**Required NuGet Packages:**
```xml
<PackageReference Include="Plugin.Firebase" Version="2.0.15" />
```

**MauiProgram.cs Configuration:**
```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .RegisterFirebaseServices();
    
    // Register your services
    builder.Services.AddSingleton<IApiService, ApiService>();
    builder.Services.AddSingleton<DeviceManager>();
    
    return builder.Build();
}
```

### 2. Device Activity Tracking

The system automatically tracks device activity using a middleware that monitors requests from mobile apps.

**Implementation:**
Add the following header to **every API request** from your mobile app:

**Header:**
```
X-Mobile-Origin: <device-id>
```

**Example:**
```csharp
// .NET MAUI - Using HttpClient with DelegatingHandler
public class DeviceHeaderHandler : DelegatingHandler
{
    private readonly DeviceManager _deviceManager;
    
    public DeviceHeaderHandler(DeviceManager deviceManager)
    {
        _deviceManager = deviceManager;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        // Add device ID header to every request
        var deviceId = _deviceManager.GetDeviceId();
        request.Headers.Add("X-Mobile-Origin", deviceId);
        
        return await base.SendAsync(request, cancellationToken);
    }
}

// Register in MauiProgram.cs
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    
    // Register the handler
    builder.Services.AddTransient<DeviceHeaderHandler>();
    
    // Register HttpClient with handler
    builder.Services.AddHttpClient<IApiService, ApiService>((sp, client) =>
    {
        client.BaseAddress = new Uri("https://your-api-url.com");
    })
    .AddHttpMessageHandler<DeviceHeaderHandler>();
    
    return builder.Build();
}
```

**What happens:**
- Middleware updates `LastSeenUtc` on every authenticated request
- Device ownership is validated for security
- Activity tracking helps identify inactive devices for cleanup

### 3. Device Logout

**When to Logout:**
- User explicitly logs out
- User deletes the app (optional, can rely on auto-cleanup)

**Endpoint:** `POST /api/device/logout`

**Request Body:**
```json
{
  "deviceId": "unique-device-identifier"
}
```

**Headers:**
```
Authorization: Bearer <jwt-token>
Content-Type: application/json
```

**Response (200 OK):**
```json
{
  "isSuccess": true
}
```

**Implementation:**
```csharp
// .NET MAUI
public async Task LogoutDeviceAsync()
{
    var deviceId = _deviceManager.GetDeviceId();
    
    var request = new LogoutDeviceRequest
    {
        DeviceId = deviceId
    };
    
    await _apiService.LogoutDeviceAsync(request);
    
    // Optional: Clear local data
    Preferences.Clear();
    SecureStorage.RemoveAll();
}
```

## Server Configuration

### 1. Firebase Setup

**Required Files:**
- Development: `firebase-credentials.dev.json`
- Production: `firebase-credentials.json`

Place these files in the `Presentation.API` project root directory.

**Firebase Credentials Structure:**
```json
{
  "type": "service_account",
  "project_id": "your-project-id",
  "private_key_id": "key-id",
  "private_key": "-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n",
  "client_email": "firebase-adminsdk@your-project.iam.gserviceaccount.com",
  "client_id": "client-id",
  "auth_uri": "https://accounts.google.com/o/oauth2/auth",
  "token_uri": "https://oauth2.googleapis.com/token",
  "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
  "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/..."
}
```

**Obtaining Credentials:**
1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Select your project
3. Go to Project Settings → Service Accounts
4. Click "Generate New Private Key"
5. Save the downloaded JSON file as `firebase-credentials.json` (production) or `firebase-credentials.dev.json` (development)

**Security:**
- Add `firebase-credentials*.json` to `.gitignore`
- Never commit credentials to version control
- Use environment-specific credentials

### 2. Configuration Settings

**appsettings.json:**
```json
{
  "DeviceOptions": {
    "MaxDevicesPerAccount": 5,
    "LastSeenThresholdDays": 90
  }
}
```

**Configuration Options:**
- `MaxDevicesPerAccount`: Maximum devices allowed per user (default: 5)
- `LastSeenThresholdDays`: Days of inactivity before device is auto-removed (default: 90)

### 3. Background Jobs

The system includes an automatic cleanup job that runs daily at 00:00 UTC.

**DeviceCleanupJob:**
- Removes devices that haven't been active for `LastSeenThresholdDays`
- Runs automatically via Hangfire
- Logs cleanup operations for monitoring

## Push Notifications

### Sending Notifications

**Service Interface:**
```csharp
public interface IPushNotifications
{
    Task SendToUserAsync(string userId, string title, string body, IDictionary<string, string>? data = null);
}
```

**Usage Example:**
```csharp
public class NotificationService
{
    private readonly IPushNotifications _pushNotifications;
    
    public async Task NotifyUser(string userId, string message)
    {
        await _pushNotifications.SendToUserAsync(
            userId: userId,
            title: "New Update",
            body: message,
            data: new Dictionary<string, string>
            {
                { "type", "update" },
                { "timestamp", DateTime.UtcNow.ToString("o") }
            }
        );
    }
}
```

**Test Endpoint:**
```
POST /test/push-notification
Content-Type: application/json

{
  "userId": "user-id",
  "title": "Test Notification",
  "body": "This is a test message"
}
```

### Handling Notifications on Mobile

```csharp
// .NET MAUI - App.xaml.cs or MauiProgram.cs
public partial class App : Application
{
    private readonly DeviceManager _deviceManager;
    
    public App(DeviceManager deviceManager)
    {
        InitializeComponent();
        _deviceManager = deviceManager;
        
        // Setup Firebase notifications
        SetupFirebaseNotifications();
        
        MainPage = new AppShell();
    }
    
    private void SetupFirebaseNotifications()
    {
        // Handle notification received when app is in foreground
        CrossFirebaseCloudMessaging.Current.NotificationReceived += (s, e) =>
        {
            var title = e.Notification.Title;
            var body = e.Notification.Body;
            var data = e.Notification.Data;
            
            // Display notification or handle in-app
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Show alert or update UI
                Application.Current?.MainPage?.DisplayAlert(title, body, "OK");
            });
        };
        
        // Handle notification tapped
        CrossFirebaseCloudMessaging.Current.NotificationTapped += (s, e) =>
        {
            var data = e.Notification.Data;
            
            // Navigate to specific page based on notification data
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Handle navigation
                if (data.ContainsKey("type"))
                {
                    var notificationType = data["type"];
                    // Navigate based on type
                }
            });
        };
        
        // Handle token refresh
        CrossFirebaseCloudMessaging.Current.TokenChanged += async (s, e) =>
        {
            await _deviceManager.RegisterDeviceAsync(e.Token);
        };
    }
}
```

**Platform-Specific Configuration:**

**Android - Platforms/Android/MainApplication.cs:**
```csharp
[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

**Android - AndroidManifest.xml:**
```xml
<application>
    <!-- Other configurations -->
    
    <!-- Firebase Messaging Service -->
    <service
        android:name="com.google.firebase.messaging.FirebaseMessagingService"
        android:exported="false">
        <intent-filter>
            <action android:name="com.google.firebase.MESSAGING_EVENT" />
        </intent-filter>
    </service>
</application>

<uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
```

**iOS - Platforms/iOS/AppDelegate.cs:**
```csharp
[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    
    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        // Request notification permissions
        UNUserNotificationCenter.Current.RequestAuthorization(
            UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
            (granted, error) =>
            {
                if (granted)
                {
                    InvokeOnMainThread(() =>
                    {
                        UIApplication.SharedApplication.RegisterForRemoteNotifications();
                    });
                }
            });
        
        return base.FinishedLaunching(application, launchOptions);
    }
}
```

**iOS - Entitlements.plist:**
```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>aps-environment</key>
    <string>development</string>
</dict>
</plist>
```

## Security Considerations

### Device Identification
- **DeviceId** is safe to transmit in headers (it's just an identifier)
- **FCM Token** should only be sent via request body during registration
- Both require authentication (JWT) to prevent unauthorized access

### Validation
- Device ownership is validated on every activity update
- Mismatches trigger critical security logs
- System prevents users from tracking devices they don't own

### Monitoring
The system logs critical events:
- Device not found during activity tracking
- Device ownership mismatches (security alerts)
- Failed push notification deliveries
- Device cleanup operations

## Troubleshooting

### Device Not Registering
1. Verify JWT token is valid
2. Check FCM token is obtained correctly from Firebase SDK
3. Ensure DeviceId is persistent across app restarts
4. Verify device limit hasn't been reached (MaxDevicesPerAccount)

### Push Notifications Not Received
1. Verify Firebase credentials are configured correctly
2. Check FCM token is valid and not expired
3. Ensure device is registered (check via logs)
4. Verify app has notification permissions
5. Check Firebase Console for delivery reports

### Device Activity Not Tracking
1. Verify `X-Mobile-Origin` header is included in requests
2. Check user is authenticated (JWT token)
3. Ensure DeviceId matches registered device
4. Verify middleware is not being bypassed

### Devices Not Being Cleaned Up
1. Check `LastSeenThresholdDays` configuration
2. Verify Hangfire background jobs are running
3. Check server logs for cleanup job execution
4. Ensure database connection is stable

## API Reference

### Endpoints

#### Register Device
```
POST /api/device/register
Authorization: Bearer <token>
Content-Type: application/json

Body:
{
  "deviceId": "string",
  "fcmToken": "string",
  "platform": "string"
}

Response: 200 OK - Device object
Response: 400 Bad Request - Device limit reached
Response: 401 Unauthorized - Invalid/missing token
Response: 500 Internal Server Error - Server error
```

#### Logout Device
```
POST /api/device/logout
Authorization: Bearer <token>
Content-Type: application/json

Body:
{
  "deviceId": "string"
}

Response: 200 OK - Success
Response: 401 Unauthorized - Invalid/missing token
```

## Best Practices

### Mobile Apps
1. Generate DeviceId once and store persistently
2. Include `X-Mobile-Origin` header in all API requests
3. Re-register device when FCM token is refreshed
4. Handle logout gracefully by calling logout endpoint
5. Request notification permissions at appropriate time
6. Test notifications in both foreground and background states

### Backend
1. Monitor device cleanup logs for anomalies
2. Adjust `LastSeenThresholdDays` based on user behavior
3. Review device limit per user periodically
4. Keep Firebase credentials secure and rotate regularly
5. Monitor push notification failure rates
6. Set up alerts for critical security events (ownership mismatches)

## Migration Notes

If migrating from an existing system:
1. Existing users will need to register their devices after update
2. Old FCM tokens will not be migrated automatically
3. Consider implementing a grace period for inactive devices
4. Notify users about the new device management system

## Support

For issues or questions:
1. Check server logs for detailed error messages
2. Review Firebase Console for delivery metrics
3. Verify configuration settings in appsettings.json
4. Contact the development team with relevant logs
