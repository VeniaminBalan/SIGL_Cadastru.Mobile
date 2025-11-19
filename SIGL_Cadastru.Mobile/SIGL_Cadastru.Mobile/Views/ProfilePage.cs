using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.ViewModels;

namespace SIGL_Cadastru.Mobile.Views;

public class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel vm)
    {
        BindingContext = vm;

        Content = new VerticalStackLayout
        {
            Padding = 20,
            Spacing = 20,

            Children =
            {
                new ActivityIndicator()
                    .Bind(ActivityIndicator.IsRunningProperty, nameof(ProfileViewModel.IsLoading))
                    .Bind(ActivityIndicator.IsVisibleProperty, nameof(ProfileViewModel.IsLoading))
                    .Center(),

                new Label()
                    .Bind(Label.TextProperty, nameof(ProfileViewModel.PageTitle))
                    .FontSize(28)
                    .Center(),

                new Label()
                    .Text("User Information")
                    .FontSize(18)
                    .Center(),

                new Label()
                    .Bind(Label.TextProperty, nameof(ProfileViewModel.UserName))
                    .FontSize(16)
                    .Center(),

                new Label()
                    .Bind(Label.TextProperty, nameof(ProfileViewModel.Email))
                    .FontSize(16)
                    .Center(),
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is ProfileViewModel vm)
        {
            await vm.InitializeCommand.ExecuteAsync(null);
        }
    }
}
