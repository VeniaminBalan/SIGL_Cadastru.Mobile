using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.ViewModels;

namespace SIGL_Cadastru.Mobile.Views;

public class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        BindingContext = vm;

        Content = new VerticalStackLayout
        {
            Padding = 20,
            Spacing = 20,
            VerticalOptions = LayoutOptions.Center,

            Children =
            {
                new ActivityIndicator()
                    .Bind(ActivityIndicator.IsRunningProperty, nameof(MainViewModel.IsLoading))
                    .Bind(ActivityIndicator.IsVisibleProperty, nameof(MainViewModel.IsLoading))
                    .Center(),

                new Label()
                    .Text("SIGL Cadastru")
                    .FontSize(28)
                    .Center(),

                new Label()
                    .Bind(Label.TextProperty, nameof(MainViewModel.WelcomeMessage))
                    .FontSize(18)
                    .Center(),

                new Button()
                    .Text("Logout")
                    .Bind(Button.CommandProperty, nameof(MainViewModel.LogoutCommand))
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is MainViewModel vm)
        {
            await vm.InitializeCommand.ExecuteAsync(null);
        }
    }
}
