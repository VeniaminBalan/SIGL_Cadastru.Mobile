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
                        Margin = new Thickness(0,10,0,0)
                    }
                    .Bind(CollectionView.ItemsSourceProperty, nameof(RequestsViewModel.Requests))
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

                // Filter and Sort Row
                new HorizontalStackLayout
                {
                    Spacing = 10,
                    Children =
                    {
                        new Entry
                        {
                            WidthRequest = 150,
                            ClearButtonVisibility = ClearButtonVisibility.WhileEditing
                        }
                        .Bind(Entry.TextProperty, nameof(RequestsViewModel.FilterBy)),

                        new Picker
                        {
                            Title = "Sort By",
                            WidthRequest = 130,
                            ItemsSource = new List<string> { "AvailableFrom", "Number", "CurrentState" },
                        }
                        .Bind(Picker.SelectedItemProperty, nameof(RequestsViewModel.OrderBy)),

                        new Picker
                        {
                            Title = "Order",
                            WidthRequest = 80,
                            ItemsSource = new List<string> { "asc", "desc" },
                        }
                        .Bind(Picker.SelectedItemProperty, nameof(RequestsViewModel.Direction)),

                        new Button
                        {
                            Text = "Search",
                            WidthRequest = 80
                        }
                        .Bind(Button.CommandProperty, nameof(RequestsViewModel.LoadRequestsCommand))
                    }
                }
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
