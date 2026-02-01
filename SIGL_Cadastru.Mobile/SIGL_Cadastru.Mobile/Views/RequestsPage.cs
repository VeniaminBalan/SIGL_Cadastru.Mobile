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
                BuildFilterControls().Row(0),
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
                BuildRequestsList()
            }
        };
    }

    private View BuildRequestsList()
    {
        return new RefreshView
        {
            Content = new CollectionView
            {
                ItemTemplate = new DataTemplate(() => 
                    new RequestCardComponent()
                        .Bind(RequestCardComponent.RequestProperty, ".")
                        .Bind(RequestCardComponent.TapCommandProperty, 
                            nameof(RequestsViewModel.NavigateToRequestCommand), 
                            source: ViewModel)
                ),
                Margin = new Thickness(0),
                RemainingItemsThreshold = 5,
                SelectionMode = SelectionMode.None,
                Footer = new ActivityIndicator()
                    .Center()
                    .Margins(all: DesignTokens.Spacing.Sm)
                    .Bind(ActivityIndicator.IsRunningProperty, nameof(RequestsViewModel.IsLoadingMore))
                    .Bind(ActivityIndicator.IsVisibleProperty, nameof(RequestsViewModel.IsLoadingMore))
            }
            .Bind(CollectionView.ItemsSourceProperty, nameof(RequestsViewModel.Requests))
            .Bind(CollectionView.RemainingItemsThresholdReachedCommandProperty, 
                nameof(RequestsViewModel.LoadMoreRequestsCommand))
        }
        .Bind(RefreshView.CommandProperty, nameof(RequestsViewModel.LoadRequestsCommand))
        .Bind(RefreshView.IsRefreshingProperty, nameof(RequestsViewModel.IsLoading))
        .Bind(IsVisibleProperty, nameof(RequestsViewModel.IsLoading), 
            convert: (bool loading) => !loading);
    }

    private View BuildFilterControls()
    {
        return new VerticalStackLayout
        {
            Spacing = DesignTokens.Spacing.Sm,
            Margin = new Thickness(0, 0, 0, DesignTokens.Spacing.Lg),
            Children =
            {
                // Search Entry
                new Entry
                {
                    ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
                    Placeholder = "Search requests..."
                }
                .StyleFormEntry()
                .Bind(Entry.TextProperty, nameof(RequestsViewModel.SearchText))
                .Bind(Entry.ReturnCommandProperty, nameof(RequestsViewModel.LoadRequestsCommand)),

                // Filter Buttons Row
                new HorizontalStackLayout
                {
                    Spacing = DesignTokens.Spacing.Sm,
                    Children =
                    {
                        BuildFilterButton(),
                        BuildPaymentFilterButton()
                    }
                },

                // Sort Controls Row
                new Grid
                {
                    ColumnSpacing = DesignTokens.Spacing.Sm,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    },
                    Children =
                    {
                        new Picker
                        {
                            Title = "Sort By",
                            ItemsSource = new List<string> { "AvailableFrom", "Number", "CurrentState" },
                        }
                        .Bind(Picker.SelectedItemProperty, nameof(RequestsViewModel.OrderBy))
                        .Column(0),

                        new Picker
                        {
                            Title = "Order",
                            ItemsSource = new List<string> { "asc", "desc" },
                        }
                        .Bind(Picker.SelectedItemProperty, nameof(RequestsViewModel.Direction))
                        .Column(1)
                    }
                },

                // Search Button
                BuildSearchButton()
            }
        };
    }

    private Button BuildFilterButton()
    {
        var button = new Button();
        button.StyleOutlineButton();
        button
            .Bind(Button.TextProperty, nameof(RequestsViewModel.FilterBy), 
                  convert: (string? filter) => string.IsNullOrWhiteSpace(filter) ? 
                      "State Filter" : $"State: {filter}");
        button.Clicked += async (s, e) => await ShowFilterModal();
        return button;
    }

    private Button BuildPaymentFilterButton()
    {
        var button = new Button();
        button.StyleOutlineButton();
        button
            .Bind(Button.TextProperty, nameof(RequestsViewModel.FilterFullyPaid), 
                  convert: (bool fullyPaid, bool unpaid) =>
                  {
                      if (fullyPaid && !unpaid) return "Pay: Paid";
                      if (unpaid && !fullyPaid) return "Pay: Unpaid";
                      return "Payment";
                  });
        button.Clicked += async (s, e) => await ShowPaymentFilterModal();
        return button;
    }

    private Button BuildSearchButton()
    {
        var button = new Button { HorizontalOptions = LayoutOptions.Fill };
        button
            .Text("Search")
            .StylePrimaryButton()
            .Bind(Button.CommandProperty, nameof(RequestsViewModel.LoadRequestsCommand));
        return button;
    }

    private async Task ShowFilterModal()
    {
        var filterPage = new ContentPage
        {
            Title = "Filter by State",
            Content = new VerticalStackLayout
            {
                Padding = DesignTokens.Layout.PagePadding,
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    new FilterCheckboxComponent
                    {
                        LabelText = "Issued"
                    }
                    .Bind(FilterCheckboxComponent.IsCheckedProperty, nameof(RequestsViewModel.FilterIssued)),
                    
                    new FilterCheckboxComponent
                    {
                        LabelText = "Rejected"
                    }
                    .Bind(FilterCheckboxComponent.IsCheckedProperty, nameof(RequestsViewModel.FilterRejected)),
                    
                    new FilterCheckboxComponent
                    {
                        LabelText = "At Reception"
                    }
                    .Bind(FilterCheckboxComponent.IsCheckedProperty, nameof(RequestsViewModel.FilterAtReception)),
                    
                    new FilterCheckboxComponent
                    {
                        LabelText = "In Progress"
                    }
                    .Bind(FilterCheckboxComponent.IsCheckedProperty, nameof(RequestsViewModel.FilterInProgress)),

                    BuildApplyFiltersButton(),
                    BuildClearAllButton()
                }
            }
        };

        filterPage.BindingContext = ViewModel;
        await Navigation.PushModalAsync(new NavigationPage(filterPage));
    }

    private Button BuildApplyFiltersButton()
    {
        var button = new Button { Margin = new Thickness(0, DesignTokens.Spacing.Xl, 0, 0) };
        button
            .Text("Apply Filters")
            .StylePrimaryButton();
        button.Clicked += async (s, e) =>
        {
            ViewModel.ApplyFiltersCommand.Execute(null);
            await ViewModel.LoadRequestsCommand.ExecuteAsync(null);
            await Navigation.PopModalAsync();
        };
        return button;
    }

    private Button BuildClearAllButton()
    {
        var button = new Button();
        button
            .Text("Clear All")
            .StyleTextButton();
        button.Clicked += async (s, e) =>
        {
            ViewModel.FilterIssued = false;
            ViewModel.FilterRejected = false;
            ViewModel.FilterAtReception = false;
            ViewModel.FilterInProgress = false;
            ViewModel.ApplyFiltersCommand.Execute(null);
            await ViewModel.LoadRequestsCommand.ExecuteAsync(null);
            await Navigation.PopModalAsync();
        };
        return button;
    }

    private async Task ShowPaymentFilterModal()
    {
        var paymentFilterPage = new ContentPage
        {
            Title = "Filter by Payment Status",
            Content = new VerticalStackLayout
            {
                Padding = DesignTokens.Layout.PagePadding,
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    new FilterCheckboxComponent
                    {
                        LabelText = "Fully Paid"
                    }
                    .Bind(FilterCheckboxComponent.IsCheckedProperty, nameof(RequestsViewModel.FilterFullyPaid)),
                    
                    new FilterCheckboxComponent
                    {
                        LabelText = "Unpaid/Partially Paid"
                    }
                    .Bind(FilterCheckboxComponent.IsCheckedProperty, nameof(RequestsViewModel.FilterUnpaid)),

                    BuildApplyPaymentFilterButton(),
                    BuildClearPaymentFilterButton()
                }
            }
        };

        paymentFilterPage.BindingContext = ViewModel;
        await Navigation.PushModalAsync(new NavigationPage(paymentFilterPage));
    }

    private Button BuildApplyPaymentFilterButton()
    {
        var button = new Button { Margin = new Thickness(0, DesignTokens.Spacing.Xl, 0, 0) };
        button
            .Text("Apply Filter")
            .StylePrimaryButton();
        button.Clicked += async (s, e) =>
        {
            await ViewModel.LoadRequestsCommand.ExecuteAsync(null);
            await Navigation.PopModalAsync();
        };
        return button;
    }

    private Button BuildClearPaymentFilterButton()
    {
        var button = new Button();
        button
            .Text("Clear Filter")
            .StyleTextButton();
        button.Clicked += async (s, e) =>
        {
            ViewModel.ClearPaymentFilterCommand.Execute(null);
            await ViewModel.LoadRequestsCommand.ExecuteAsync(null);
            await Navigation.PopModalAsync();
        };
        return button;
    }

    protected override void OnPageAppearing()
    {
        // Delays initialization until UI is rendered → no freeze
        Dispatcher.Dispatch(async () =>
        {
            await ViewModel.InitializeCommand.ExecuteAsync(null);
        });
    }
}
