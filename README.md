# SIGL Cadastru Mobile - Architecture Documentation

## Overall Architecture (High Level)

```
[Android App] <--HTTPS--> [Keycloak (OIDC)] <--HTTPS--> [Admin/Dev]
       |                             ^
       |                             |
       v                             |
   (FCM token)                       |
       |                             |
       v                             |
[.NET Web API] <----HTTPS----> [Firebase (FCM)]
       |
       v
[Database (Postgres / SQL Server)]
       |
       v
[Background Jobs] (Hangfire / Quartz) - cleanup, retries, metrics
```

## Components

### Android App
Performs OIDC login (Auth Code + PKCE), fetches FCM token, calls .NET API to register/unregister device, receives notifications.

### Keycloak
Identity Provider (Authorization Code with PKCE for mobile). Issues access & refresh tokens.

### .NET Web API
Resource server (validates Keycloak JWTs). Stores device records, exposes `/register`, `/unregister`, `/devices`, `/send-notification`, and a secure webhook endpoint for Keycloak events (optional). Uses Firebase Admin SDK to send notifications.

### Firebase (FCM)
Push broker. .NET uses Admin SDK for sends and receives send responses to detect invalid tokens.

### Database
Stores users' device records and metadata, audit logs, and token failure states.

### Background Jobs
Scheduled cleanup, re-sending, analytics.

## Database Schema (Core Tables)

### users table
```sql
CREATE TABLE users (
  id UUID PRIMARY KEY,              -- Keycloak sub (user id)
  username TEXT,
  email TEXT,
  created_at timestamptz DEFAULT now()
);
```

### device_tokens table
```sql
CREATE TABLE device_tokens (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID REFERENCES users(id) ON DELETE CASCADE,
  device_id TEXT NOT NULL,          -- persistent device UUID stored by app
  device_name TEXT,
  platform TEXT,                    -- 'Android', 'iOS'
  app_version TEXT,
  fcm_token TEXT NOT NULL,
  is_active boolean DEFAULT true,
  last_updated timestamptz DEFAULT now(),
  UNIQUE(user_id, device_id)
);

CREATE INDEX idx_device_token ON device_tokens(fcm_token);
```

### Additional Tables
- **notification_logs** — record sends, FCM responses, errors (helps with cleanup)
- **device_failures** — counts consecutive failures per token (used to auto-delete after N failures)

## REST Endpoints (in .NET API)

All endpoints requiring user context must be protected with `[Authorize]` and JWT validation against Keycloak.

### POST /api/devices/register

**Request Body:**
```json
{
  "deviceId": "uuid-v4",
  "deviceName": "Pixel 7",
  "platform": "Android",
  "appVersion": "1.2.3",
  "fcmToken": "AAAA..."
}
```

**Action:** Upsert `device_tokens` by `(userId, deviceId)` — set `fcm_token`, `is_active=true`, update `last_updated`.

### POST /api/devices/unregister

**Request Body:**
```json
{
  "deviceId": "uuid-v4",
  "fcmToken": "AAAA..."
}
```

**Action:** Remove or mark `is_active=false` for that device record (auth required).

### GET /api/devices

**Returns:** List of devices for current user (`deviceId`, `name`, `platform`, `last_updated`).

### DELETE /api/devices/{deviceId}

Authenticated user can revoke a device (remove token). Good for "Log out this device" from UI.

### POST /api/notify/user/{userId} (internal or admin)

**Request Body:**
```json
{
  "title": "Hi",
  "body": "Message",
  "data": { "k": "v" },
  "targetDeviceId": null
}
```

**Action:** Send notification to all active device tokens for `userId` (or just `targetDeviceId`).

### POST /api/keycloak/events (optional, secured webhook)

Keycloak calls this on session events (LOGOUT, etc.). Validate a shared secret header or signature.

**Action:** Depending on event type, call `UnregisterAllDevicesForUser(userId)` or mark devices inactive.

## Auth Setup (.NET) — Validating Keycloak Tokens

In `Program.cs` (minimal):

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://keycloak.example.com/realms/yourrealm";
        options.Audience = "your-api-client-id"; // client configured in Keycloak for API
        options.RequireHttpsMetadata = true;
    });

builder.Services.AddAuthorization();
```

Use `User.FindFirst("sub")?.Value` to get Keycloak user id and link to `DB users.id`.

Ensure token validation includes issuer and audience checks.

## Android App Behavior (Key Points)

### Device ID Management
On first app run generate and persist `deviceId` (UUID) in secure storage. **Do not regenerate on logout**; only regenerate on uninstall / reinstall.

### Login
Start OIDC Authorization Code + PKCE flow (custom tabs / browser). Receive code → exchange for tokens (access + refresh).

### After Login
1. Retrieve FCM token (via `FirebaseMessaging.getToken()`)
2. Call `/api/devices/register` with `Authorization: Bearer <access_token>`

### Handle Token Refresh
`FirebaseMessagingService.onNewToken(String token)` → send updated token to `/register` with current access token.

### Logout
1. Call `/api/devices/unregister` with current access token and `deviceId`
2. Clear stored Keycloak tokens and app session data
3. Optionally call `FirebaseMessaging.deleteToken()` to force a new token on next login (extra safety)

### App Uninstall
No client action — backend detects this via FCM send failures.

## Login Flow (Sequence)

Textual sequence (Auth Code + PKCE):

1. **App** → open browser to Keycloak `/auth` with `client_id`, `redirect_uri`, `code_challenge`, etc.
2. User authenticates in Keycloak UI
3. **Keycloak** → redirect back to app (`redirect_uri`) with `code`
4. **App** → POST to Keycloak `/token` with `code`, `code_verifier` to obtain `access_token` (+ refresh token)
5. **App** → call `FirebaseMessaging.getToken()` to get `fcmToken`
6. **App** → POST `/api/devices/register` with bearer `access_token` and body `{ deviceId, fcmToken, … }`
7. **.NET API** validates access token, extracts `sub` (userId), upserts device record

**Result:** Backend knows user → device mapping and can safely send pushes.

## Logout / User Switch Flow (Sequence)

1. App user taps **Logout**
2. **App** → POST `/api/devices/unregister` with bearer `access_token` (or an implicit "unregister current device" if user not authenticated) and `deviceId`
3. **.NET API** removes device entry for `userId + deviceId`
4. **App** clears local Keycloak tokens and session state
5. Optionally `FirebaseMessaging.deleteToken()` to force token rotation
6. **New user logs in:** repeat Login flow — register the device under the new `userId`

**Important:** Always call unregister BEFORE clearing access token so the call can be authenticated and safely remove the correct user binding.

## Send-Notification Flow (Sequence)

1. Some backend action triggers a notification (e.g., message received, alert)
2. **.NET API** determines target `userId` (or set of users)
3. **API** queries `device_tokens` where `userId = X` and `is_active = true`
4. Use **Firebase Admin SDK** (preferred) to send:
   - If 1–500 tokens: use `SendMulticastAsync` to send to many tokens in one request
   - If many users: batch tokens into chunks (FCM multicast limit) and send in parallel
5. Parse FCM responses:
   - On **Success**: log message id
   - On **Unregistered / InvalidRegistration**: schedule immediate deletion of that token (or delete now)
   - On **transient errors**: schedule retry with exponential backoff
6. Update `notification_logs` and `device_failures` accordingly

### C# Example Using Firebase Admin SDK

```csharp
var message = new MulticastMessage()
{
    Tokens = tokensList,
    Notification = new Notification { Title = title, Body = body },
    Data = new Dictionary<string, string> { { "key", "value" } }
};

var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);

for (int i = 0; i < response.Responses.Count; i++)
{
    var resp = response.Responses[i];
    var token = tokensList[i];
    if (!resp.IsSuccess)
    {
        var code = resp.Exception?.MessagingErrorCode;
        if (code == MessagingErrorCode.Unregistered || code == MessagingErrorCode.InvalidRegistration)
        {
            // remove token from DB
        }
        else
        {
            // handle transient errors (retry later)
        }
    }
}
```

## Handling App Uninstall Detection & Cleanup

Only reliably detected when you attempt a send. FCM returns `NotRegistered` / `Unregistered`.

### When You Receive Those Errors:
1. Immediately remove device record
2. Log the event for auditing

### Background Job Strategy:
Add a background job to prune tokens with:
- `last_updated` older than X days (e.g., 90 days)
- Consecutive failure count >= N (e.g., 3)

**Optionally:** Maintain a `failure_count` column on `device_tokens`.

## Keycloak Event Automation (Optional, But Useful)

Use Keycloak Event Listener SPI or a community webhook plugin to notify your API when sessions are destroyed by Keycloak (admin logout, session expiration).

Keycloak cannot supply FCM tokens — use these events to bulk revoke all device mappings for the user (or mark inactive) as a safety net.

Secure webhook with a shared secret HMAC or static bearer token and verify on your API.

## Security Considerations (Must-Haves)

- **JWT validation** — validate `iss`, `aud`, `exp`, signature using Keycloak JWKS
- **Protect register/unregister endpoints** — require valid access token so a malicious client can't claim a device
- **Webhook security** — require `X-Webhook-Signature` or `Authorization` header; verify
- **Rate limiting** — protect `/register-device` from abuse
- **Limit token visibility** — never log full FCM tokens in non-secure logs
- **Admin actions** — restrict `/notify` endpoints to service clients or internal roles (use a machine-to-machine client credentials flow for service-to-service calls)
- **Data privacy** — allow users to view & remove registered devices from UI

## Operational Tips

1. **Use Firebase Admin SDK for .NET** rather than raw REST where possible — simpler, handles many edge cases
2. **Batch sends** with `SendMulticastAsync` for efficiency
3. **Store message logs** for debugging and audit
4. **Use Hangfire / Quartz** for background tasks:
   - Retry transient notification failures
   - Periodic cleanup of stale tokens
   - Metrics aggregation (failed sends, removed tokens)
5. **Monitor:** metrics for sends, errors per token, and device count per user to detect suspicious activity

## Example Quick Runbook (What to Implement First)

1. **Implement device model** + DB table and migrations
2. **Add `/register` and `/unregister` endpoints** with JWT validation
3. **Integrate Firebase Admin SDK** and implement `SendMulticast` + cleanup on errors
4. **Update Android app** to:
   - Generate persistent `deviceId`
   - Perform OIDC Auth Code + PKCE login
   - Call `/register` after login and on token refresh
   - Call `/unregister` on logout
5. **Add background job** to remove tokens with repeated failures and prune old ones
6. **Optionally enable Keycloak webhook listener** to proactively clear sessions/devices on forced logout
