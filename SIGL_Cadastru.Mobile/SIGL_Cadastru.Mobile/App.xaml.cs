using SIGL_Cadastru.Mobile.Services;

namespace SIGL_Cadastru.Mobile
{
    public partial class App : Application
    {
        private readonly KeycloakAuthService _authService;

        public App(KeycloakAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await TryRefreshTokenOnStartupAsync();
        }

        private async Task TryRefreshTokenOnStartupAsync()
        {
            // If no refresh token exists, navigate to login
            if (string.IsNullOrEmpty(_authService.RefreshToken))
            {
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            // Try to refresh the token
            var refreshSuccess = await _authService.RefreshAsync();

            if (!refreshSuccess)
            {
                // Refresh failed - invalidate tokens and navigate to login
                await _authService.InvalidateTokensAsync();
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                // Refresh successful - navigate to main page
                await Shell.Current.GoToAsync("//RequestsPage");
            }
        }
    }
}