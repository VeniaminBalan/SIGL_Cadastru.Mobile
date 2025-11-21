using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Logging;
using Plugin.Firebase.Auth;
using Microsoft.Maui.LifecycleEvents;

using Plugin.Firebase.Bundled.Shared;

#if IOS
    using Plugin.Firebase.Bundled.Platforms.iOS;
#else
    using Plugin.Firebase.Bundled.Platforms.Android;
#endif

namespace SIGL_Cadastru.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .RegisterFirebaseServices()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

        // Register Services
        builder.Services.RegisterServices();

        // Register ViewModels
        builder.Services.RegisterViewModels();

        // Register Views (Pages)
        builder.Services.RegisterViews();

        return builder.Build();
    }

    private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events => {
#if IOS
            events.AddiOS(iOS => iOS.FinishedLaunching((app, launchOptions) => {
                CrossFirebase.Initialize(CreateCrossFirebaseSettings());
                return false;
            }));
#else
            events.AddAndroid(android => android.OnCreate((activity, _) =>
            {
                CrossFirebase.Initialize(activity, CreateCrossFirebaseSettings());
            }));
#endif
        });

        builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
        return builder;
    }

    private static CrossFirebaseSettings CreateCrossFirebaseSettings()
    {
        return new CrossFirebaseSettings(
            isAuthEnabled: false, 
            isCloudMessagingEnabled: true, 
            isCrashlyticsEnabled: false,
            isAnalyticsEnabled: false,
            isDynamicLinksEnabled: false,
            isFirestoreEnabled: false,
            isFunctionsEnabled: false,
            isStorageEnabled: false);
    }
}
