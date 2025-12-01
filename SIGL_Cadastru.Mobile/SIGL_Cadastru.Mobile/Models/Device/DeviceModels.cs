namespace SIGL_Cadastru.Mobile.Models.Device;

public record RegisterDeviceRequest(
    string DeviceId,
    string FcmToken,
    string Platform
);

public record LogoutDeviceRequest(
    string DeviceId
);

public record DeviceResponse(
    string DeviceId,
    string FcmToken,
    string Platform,
    string UserId,
    DateTimeOffset LastSeenUtc,
    DateTimeOffset CreatedUtc
);
