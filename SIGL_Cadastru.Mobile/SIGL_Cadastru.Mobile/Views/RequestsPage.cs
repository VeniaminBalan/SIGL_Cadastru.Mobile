using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using SIGL_Cadastru.Mobile.ViewModels;

namespace SIGL_Cadastru.Mobile.Views;

public class RequestsPage : ContentPage
{
    private readonly RequestsViewModel _vm;

    public RequestsPage(RequestsViewModel vm)
    {
        _vm = vm;
        BindingContext = vm;

        Title = "Requests";

        // Fast, lightweight initial UI
        Content = BuildInitialUi();
    }

    private View BuildInitialUi()
    {
        return new Grid
        {
            Padding = 20,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Filter Controls
                new RowDefinition { Height = GridLength.Auto }, // Error
                new RowDefinition { Height = GridLength.Star }  // Content
            },

            Children =
            {
                // Filter Controls
                BuildFilterControls().Row(0),
                // Error message
                new Label()
                    .TextColor(Colors.Red)
                    .FontSize(14)
                    .Margins(top: 8)
                    .Bind(Label.IsVisibleProperty, nameof(RequestsViewModel.ErrorMessage), 
                          convert: (string? msg) => !string.IsNullOrWhiteSpace(msg))
                    .Bind(Label.TextProperty, nameof(RequestsViewModel.ErrorMessage))
                    .Row(1),

                // Loading indicator
                new ActivityIndicator()
                    .Center()
                    .Bind(ActivityIndicator.IsRunningProperty, nameof(RequestsViewModel.IsLoading))
                    .Bind(ActivityIndicator.IsVisibleProperty, nameof(RequestsViewModel.IsLoading))
                    .Row(2),

                // Main list placeholder – initially hidden
                new RefreshView
                {
                    Content = new CollectionView
                    {
                        ItemTemplate = CreateRequestTemplate(),
                        Margin = new Thickness(0,0,0,0),
                        RemainingItemsThreshold = 5,
                        SelectionMode = SelectionMode.Single,
                        Footer = new ActivityIndicator()
                            .Center()
                            .Margins(10)
                            .Bind(ActivityIndicator.IsRunningProperty, nameof(RequestsViewModel.IsLoadingMore))
                            .Bind(ActivityIndicator.IsVisibleProperty, nameof(RequestsViewModel.IsLoadingMore))
                    }
                    .Bind(CollectionView.ItemsSourceProperty, nameof(RequestsViewModel.Requests))
                    .Bind(CollectionView.RemainingItemsThresholdReachedCommandProperty, nameof(RequestsViewModel.LoadMoreRequestsCommand))
                    .Invoke(cv => cv.SelectionChanged += (s, e) => 
                    {
                        if (e.CurrentSelection.FirstOrDefault() is Models.Requests.CadastralRequestDto request)
                        {
                            _vm.NavigateToRequestCommand.Execute(request);
                            if (s is CollectionView collectionView)
                            {
                                collectionView.SelectedItem = null; // Clear selection
                            }
                        }
                    })
                }
                .Bind(RefreshView.CommandProperty, nameof(RequestsViewModel.LoadRequestsCommand))
                .Bind(RefreshView.IsRefreshingProperty, nameof(RequestsViewModel.IsLoading))
                .Bind(IsVisibleProperty, nameof(RequestsViewModel.IsLoading), convert: (bool loading) => !loading)
                .Row(2)
            }
        };
    }

    private View BuildFilterControls()
    {
        return new VerticalStackLayout
        {
            Spacing = 10,
            Margin = new Thickness(0, 0, 0, 15),
            Children =
            {
                // Search Entry
                new Entry
                {
                    ClearButtonVisibility = ClearButtonVisibility.WhileEditing
                }
                .Bind(Entry.TextProperty, nameof(RequestsViewModel.SearchText))
                .Bind(Entry.ReturnCommandProperty, nameof(RequestsViewModel.LoadRequestsCommand)),

                // Filter Button
                new Button
                {
                    Text = "Filter by State"
                }
                .Bind(Button.TextProperty, nameof(RequestsViewModel.FilterBy), 
                      convert: (string? filter) => string.IsNullOrWhiteSpace(filter) ? "Filter by State" : $"Filters: {filter}")
                .Invoke(btn => btn.Clicked += async (s, e) => await ShowFilterModal()),

                // Sort Controls Row
                new Grid
                {
                    ColumnSpacing = 8,
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
                new Button
                {
                    Text = "Search",
                    HorizontalOptions = LayoutOptions.Fill
                }
                .Bind(Button.CommandProperty, nameof(RequestsViewModel.LoadRequestsCommand))
            }
        };
    }

    private DataTemplate CreateRequestTemplate()
    {
        return new DataTemplate(() =>
            new Border
            {
                Padding = 12,
                Margin = new Thickness(0, 6),
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                Stroke = Colors.LightGray,
                StrokeThickness = 1,

                Content = new VerticalStackLayout
                {
                    Spacing = 4,
                    Children =
                    {
                        new Label()
                            .FontSize(18)
                            .Bold()
                            .Bind(Label.TextProperty, "Number", convert: (string? num) => $"Request: {num ?? "N/A"}"),

                        new Label()
                            .FontSize(14)
                            .TextColor(Colors.Gray)
                            .Bind(Label.TextProperty, "CadastralNumber", convert: (string? num) => $"Cadastral #: {num ?? "N/A"}"),

                        new Label()
                            .FontSize(14)
                            .Bind(Label.TextProperty, "Client", convert: (string? c) => $"Client: {c ?? "N/A"}"),

                        new HorizontalStackLayout
                        {
                            Spacing = 10,
                            Children =
                            {
                                new Label()
                                    .FontSize(12)
                                    .Bind(Label.TextProperty, "CurrentState", convert: (Models.Shared.StateType s) => s.ToString()),

                                new Label()
                                    .FontSize(12)
                                    .TextColor(Colors.DarkGray)
                                    .Bind(Label.TextProperty, "AvailableFrom", convert: (DateTime d) => d.ToString("dd/MM/yyyy"))
                            }
                        }
                    }
                }
            }
        );
    }

    private async Task ShowFilterModal()
    {
        var filterPage = new ContentPage
        {
            Title = "Filter by State",
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 12,
                Children =
                {
                    CreateFilterCheckbox("Issued", nameof(RequestsViewModel.FilterIssued)),
                    CreateFilterCheckbox("Rejected", nameof(RequestsViewModel.FilterRejected)),
                    CreateFilterCheckbox("At Reception", nameof(RequestsViewModel.FilterAtReception)),
                    CreateFilterCheckbox("In Progress", nameof(RequestsViewModel.FilterInProgress)),

                    new Button
                    {
                        Text = "Apply Filters",
                        Margin = new Thickness(0, 20, 0, 0)
                    }
                    .Invoke(btn => btn.Clicked += async (s, e) =>
                    {
                        _vm.ApplyFiltersCommand.Execute(null);
                        await _vm.LoadRequestsCommand.ExecuteAsync(null);
                        await Navigation.PopModalAsync();
                    }),

                    new Button
                    {
                        Text = "Clear All",
                        BackgroundColor = Colors.Transparent,
                        TextColor = Colors.Gray
                    }
                    .Invoke(btn => btn.Clicked += async (s, e) =>
                    {
                        _vm.FilterIssued = false;
                        _vm.FilterRejected = false;
                        _vm.FilterAtReception = false;
                        _vm.FilterInProgress = false;
                        _vm.ApplyFiltersCommand.Execute(null);
                        await _vm.LoadRequestsCommand.ExecuteAsync(null);
                        await Navigation.PopModalAsync();
                    })
                }
            }
        };

        filterPage.BindingContext = _vm;
        await Navigation.PushModalAsync(new NavigationPage(filterPage));
    }

    private HorizontalStackLayout CreateFilterCheckbox(string label, string bindingPath)
    {
        return new HorizontalStackLayout
        {
            Spacing = 10,
            Children =
            {
                new CheckBox()
                    .Bind(CheckBox.IsCheckedProperty, bindingPath),
                new Label { Text = label, VerticalOptions = LayoutOptions.Center }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Delays initialization until UI is rendered → no freeze
        Dispatcher.Dispatch(async () =>
        {
            await _vm.InitializeCommand.ExecuteAsync(null);
        });
    }
}
