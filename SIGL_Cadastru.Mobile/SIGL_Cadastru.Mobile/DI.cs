using Duende.IdentityModel.OidcClient;
using SIGL_Cadastru.Mobile.Services;
using SIGL_Cadastru.Mobile.ViewModels;
using SIGL_Cadastru.Mobile.Views;

namespace SIGL_Cadastru.Mobile;

public static class DI
{
    extension(IServiceCollection services)
    {
        public void RegisterServices()
        {
            services.AddSingleton<AuthService>();
            services.AddSingleton<Duende.IdentityModel.OidcClient.Browser.IBrowser, WebAuthenticatorBrowser>();

            services.AddSingleton(sp =>
            {
                return new OidcClient(new OidcClientOptions
                {
                    Authority = "https://auth.vbtm.live/realms/sigl-dev",
                    ClientId = "mobile",
                    Scope = "openid profile email offline_access",
                    RedirectUri = "sigl.mobile://callback",
                    PostLogoutRedirectUri = "sigl.mobile://callback",
                    Browser = sp.GetRequiredService<Duende.IdentityModel.OidcClient.Browser.IBrowser>(),
                    LoadProfile = true
                });
            });

            services.AddSingleton<KeycloakAuthService>();
        }

        public void RegisterViewModels()
        {
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();
        }

        public void RegisterViews()
        {
            services.AddTransient<LoginPage>();
            services.AddTransient<MainPage>();
        }
    }
}
