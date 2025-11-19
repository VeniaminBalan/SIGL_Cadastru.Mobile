using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.ViewModels;

namespace SIGL_Cadastru.Mobile.Views;

public class ClientsPage : ContentPage
{
    public ClientsPage(ClientsViewModel vm)
    {
        BindingContext = vm;

        Content = new VerticalStackLayout
        {
            Padding = 20,
            Spacing = 20,

            Children =
            {
                new ActivityIndicator()
                    .Bind(ActivityIndicator.IsRunningProperty, nameof(ClientsViewModel.IsLoading))
                    .Bind(ActivityIndicator.IsVisibleProperty, nameof(ClientsViewModel.IsLoading))
                    .Center(),

                new Label()
                    .Bind(Label.TextProperty, nameof(ClientsViewModel.PageTitle))
                    .FontSize(28)
                    .Center(),

                new Label()
                    .Text("Your clients will appear here")
                    .FontSize(16)
                    .Center(),
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is ClientsViewModel vm)
        {
            await vm.InitializeCommand.ExecuteAsync(null);
        }
    }
}
