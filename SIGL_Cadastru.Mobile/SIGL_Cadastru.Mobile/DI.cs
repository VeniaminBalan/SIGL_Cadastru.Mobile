using Duende.IdentityModel.OidcClient;
using SIGL_Cadastru.Mobile.Services;
using SIGL_Cadastru.Mobile.Services.Accounts;
using SIGL_Cadastru.Mobile.Services.Analytics;
using SIGL_Cadastru.Mobile.Services.Clients;
using SIGL_Cadastru.Mobile.Services.Documents;
using SIGL_Cadastru.Mobile.Services.Files;
using SIGL_Cadastru.Mobile.Services.Migrations;
using SIGL_Cadastru.Mobile.Services.Requests;
using SIGL_Cadastru.Mobile.Services.Users;
using SIGL_Cadastru.Mobile.ViewModels;
using SIGL_Cadastru.Mobile.Views;

namespace SIGL_Cadastru.Mobile;

public static class DI
{
    extension(IServiceCollection services)
    {
        public void RegisterServices()
        {
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

            // Configure HttpClient for API services
            services.AddSingleton<HttpClient>(sp =>
            {
                var httpClient = new HttpClient
                {
                    // TODO: Replace with actual API base URL
                    BaseAddress = new Uri("http://192.168.1.134:5000")
                };
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                return httpClient;
            });

            // Register domain-specific API services
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IAnalyticsService, AnalyticsService>();
            services.AddSingleton<IClientService, ClientService>();
            services.AddSingleton<IDocumentService, DocumentService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IMigrationService, MigrationService>();
            services.AddSingleton<IRequestService, RequestService>();
            services.AddSingleton<IUserService, UserService>();
        }

        public void RegisterViewModels()
        {
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RequestsViewModel>();
            services.AddTransient<ClientsViewModel>();
            services.AddTransient<ProfileViewModel>();
        }

        public void RegisterViews()
        {
            services.AddTransient<LoginPage>();
            services.AddTransient<RequestsPage>();
            services.AddTransient<ClientsPage>();
            services.AddTransient<ProfilePage>();
        }
    }
}
