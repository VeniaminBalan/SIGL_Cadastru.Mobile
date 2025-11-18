using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.ViewModels;

namespace SIGL_Cadastru.Mobile.Views;

public class RequestsPage : ContentPage
{
    public RequestsPage(RequestsViewModel vm)
    {
        BindingContext = vm;

        Content = new VerticalStackLayout
        {
            Padding = 20,
            Spacing = 20,

            Children =
            {
                new ActivityIndicator()
                    .Bind(ActivityIndicator.IsRunningProperty, nameof(RequestsViewModel.IsLoading))
                    .Bind(ActivityIndicator.IsVisibleProperty, nameof(RequestsViewModel.IsLoading))
                    .Center(),

                new Label()
                    .Bind(Label.TextProperty, nameof(RequestsViewModel.PageTitle))
                    .FontSize(28)
                    .Center(),

                new Label()
                    .Text("Your requests will appear here")
                    .FontSize(16)
                    .Center(),

                new Button()
                    .Text("Back")
                    .Bind(Button.CommandProperty, nameof(RequestsViewModel.NavigateBackCommand))
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is RequestsViewModel vm)
        {
            await vm.InitializeCommand.ExecuteAsync(null);
        }
    }
}
