using SIGL_Cadastru.Mobile.Views.RequestDetail;

namespace SIGL_Cadastru.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register RequestDetail pages
            Routing.RegisterRoute(nameof(RequestOverviewPage), typeof(RequestOverviewPage));
            Routing.RegisterRoute(nameof(RequestPaymentsPage), typeof(RequestPaymentsPage));
            Routing.RegisterRoute(nameof(RequestStatesPage), typeof(RequestStatesPage));
        }
    }
}
