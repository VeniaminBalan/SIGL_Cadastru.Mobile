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

            Children =
            {
                new Button()
                    .Text("Login")
                    .Bind(Button.CommandProperty, nameof(LoginViewModel.LoginCommand)),

                new Button()
                    .Text("Logout")
                    .Bind(Button.CommandProperty, nameof(LoginViewModel.LogoutCommand)),

                new Label()
                    .Bind(Label.TextProperty, nameof(LoginViewModel.TokenDisplay))
                    .FontSize(12)
            }
        };
    }
}
