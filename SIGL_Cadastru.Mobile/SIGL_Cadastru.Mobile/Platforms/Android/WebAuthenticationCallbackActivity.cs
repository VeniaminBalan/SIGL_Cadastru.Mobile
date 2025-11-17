using Android.App;
using Android.Content.PM;

namespace SIGL_Cadastru.Mobile.Platforms.Android;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(
    [global::Android.Content.Intent.ActionView],
    Categories = new[]
    {
        global::Android.Content.Intent.CategoryDefault,
        global::Android.Content.Intent.CategoryBrowsable
    },
    DataScheme = CALLBACK_SCHEME,
    DataHost = "callback")]
public class WebAuthenticationCallbackActivity : Microsoft.Maui.Authentication.WebAuthenticatorCallbackActivity
{
    const string CALLBACK_SCHEME = "sigl.mobile";
}
