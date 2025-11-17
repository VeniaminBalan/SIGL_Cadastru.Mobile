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
            Spacing = 15,

            Children =
            {
                new Label()
                    .Text("Login")
                    .FontSize(28)
                    .Center(),

                new Entry()
                    .Placeholder("Username")
                    .Bind(Entry.TextProperty, nameof(vm.Username)),

                new Entry()
                    .Placeholder("Password")
                    .Bind(Entry.TextProperty, nameof(vm.Password)),

                new Button()
                    .Text("Login")
                    .Bind(Button.CommandProperty, nameof(vm.LoginCommand)),

                new Label()
                    .Bind(Label.TextProperty, nameof(vm.Message))
                    .Center(),

                new ActivityIndicator()
                    .Bind(ActivityIndicator.IsVisibleProperty, nameof(vm.IsBusy))
                    .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
            }
        };
    }
}
