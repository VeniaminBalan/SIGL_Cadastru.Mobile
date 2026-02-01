using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.ViewModels;

namespace SIGL_Cadastru.Mobile.Views;

public class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        BindingContext = vm;

        Content = new VerticalStackLayout
        {
            Padding = 30,
            Spacing = 20,
            VerticalOptions = LayoutOptions.Center,

            Children =
            {
                new Button()
                    .Text("Login")
                    .Bind(Button.CommandProperty, nameof(LoginViewModel.LoginCommand))
                    .Bind(Button.IsVisibleProperty, nameof(LoginViewModel.IsLoggedIn), convert: (bool isLoggedIn) => !isLoggedIn),

                new Button()
                    .Text("Logout")
                    .Bind(Button.CommandProperty, nameof(LoginViewModel.LogoutCommand))
                    .Bind(Button.IsVisibleProperty, nameof(LoginViewModel.IsLoggedIn))
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is LoginViewModel vm)
        {
            vm.UpdateAuthState();
        }
    }
}
