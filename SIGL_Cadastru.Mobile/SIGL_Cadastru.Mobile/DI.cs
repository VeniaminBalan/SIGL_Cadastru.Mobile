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
        }

        public void RegisterViewModels()
        {
            services.AddTransient<LoginViewModel>();
        }

        public void RegisterViews()
        {
            services.AddTransient<LoginPage>();
        }
    }
}
