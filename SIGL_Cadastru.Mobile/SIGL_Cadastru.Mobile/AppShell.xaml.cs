using SIGL_Cadastru.Mobile.Views;

namespace SIGL_Cadastru.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RequestDetailPage), typeof(RequestDetailPage));
        }
    }
}
