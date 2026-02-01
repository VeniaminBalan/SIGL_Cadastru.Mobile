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
        return new Grid
        {
            Padding = DesignTokens.Layout.PagePadding,
            RowSpacing = DesignTokens.Spacing.Md,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Filter section
                new RowDefinition { Height = GridLength.Auto }, // Error message
                new RowDefinition { Height = GridLength.Star }  // Requests list (fills remaining space)
            },
            Children =
            {
                new ScrollView
                {
                    Content = new RequestFilterComponent
                    {
                        ViewModel = ViewModel,
                        RemoveFilterCommand = new Command<string>(filter => RemoveFilter(filter))
                    }
                }.Row(0),
                
                BuildErrorLabel().Row(1),
                BuildRequestsList().Row(2)
            }
        };
    }

    private void RemoveFilter(string filterName)
    {
        switch (filterName)
        {
            case "Issued":
                ViewModel.FilterIssued = false;
                break;
            case "Rejected":
                ViewModel.FilterRejected = false;
                break;
            case "AtReception":
                ViewModel.FilterAtReception = false;
                break;
            case "InProgress":
                ViewModel.FilterInProgress = false;
                break;
            case "FullyPaid":
                ViewModel.FilterFullyPaid = false;
                break;
            case "Unpaid":
                ViewModel.FilterUnpaid = false;
                break;
        }
        ViewModel.ApplyFiltersCommand.Execute(null);
    }

    private View BuildRequestsList()
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