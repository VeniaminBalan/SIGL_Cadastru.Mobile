using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.Components;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.DesignSystem.BaseViews;
using SIGL_Cadastru.Mobile.ViewModels;

namespace SIGL_Cadastru.Mobile.Views;

public class RequestsPage : BaseContentPage<RequestsViewModel>
{
    public RequestsPage(RequestsViewModel vm) : base(vm)
    {
        Title = "Requests";
    }

    protected override View BuildPageContent()
    {
        return BuildInitialUi();
    }

    private View BuildInitialUi()
    {
        return new Grid
        {
            Padding = DesignTokens.Layout.PagePadding,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Filter Controls
                new RowDefinition { Height = GridLength.Auto }, // Error
                new RowDefinition { Height = GridLength.Star }  // Content
            },

            Children =
            {
                new RequestFilterComponent
                {
                    ViewModel = ViewModel,
                    StateFilterClicked = new Command(async () => await ShowFilterModal()),
                    PaymentFilterClicked = new Command(async () => await ShowPaymentFilterModal())
                }.Row(0),
                BuildErrorLabel().Row(1),
                BuildContentArea().Row(2)
            }
        };
    }

    private View BuildContentArea()
    {
        return new Grid
        {
            Children =
            {
                BuildLoadingIndicator(),
                new RequestsListComponent
                {
                    ViewModel = ViewModel,
                    NavigateCommand = ViewModel.NavigateToRequestCommand
                }
            }
        };
    }

    private async Task ShowFilterModal()
    {
        var filterPage = new ContentPage
        {
            Title = "Filter by State",
            Content = new StateFilterModalContent
            {
                ViewModel = ViewModel,
                ApplyCommand = new Command(async () =>
                {
                    ViewModel.ApplyFiltersCommand.Execute(null);
                    await ViewModel.LoadRequestsCommand.ExecuteAsync(null);
                    await Navigation.PopModalAsync();
                }),
                ClearCommand = new Command(async () =>
                {
                    ViewModel.FilterIssued = false;
                    ViewModel.FilterRejected = false;
                    ViewModel.FilterAtReception = false;
                    ViewModel.FilterInProgress = false;
                    ViewModel.ApplyFiltersCommand.Execute(null);
                    await ViewModel.LoadRequestsCommand.ExecuteAsync(null);
                    await Navigation.PopModalAsync();
                })
            }
        };

        await Navigation.PushModalAsync(new NavigationPage(filterPage));
    }

    private async Task ShowPaymentFilterModal()
    {
        var paymentFilterPage = new ContentPage
        {
            Title = "Filter by Payment Status",
            Content = new PaymentFilterModalContent
            {
                ViewModel = ViewModel,
                ApplyCommand = new Command(async () =>
                {
                    await ViewModel.LoadRequestsCommand.ExecuteAsync(null);
                    await Navigation.PopModalAsync();
                }),
                ClearCommand = new Command(async () =>
                {
                    ViewModel.ClearPaymentFilterCommand.Execute(null);
                    await ViewModel.LoadRequestsCommand.ExecuteAsync(null);
                    await Navigation.PopModalAsync();
                })
            }
        };

        await Navigation.PushModalAsync(new NavigationPage(paymentFilterPage));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Delays initialization until UI is rendered → no freeze
        Dispatcher.Dispatch(async () =>
        {
            await ViewModel.InitializeCommand.ExecuteAsync(null);
        });
    }
}