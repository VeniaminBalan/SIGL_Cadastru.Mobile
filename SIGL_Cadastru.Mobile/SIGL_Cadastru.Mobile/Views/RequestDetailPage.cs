using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using SIGL_Cadastru.Mobile.ViewModels;

namespace SIGL_Cadastru.Mobile.Views;

public class RequestDetailPage : ContentPage
{
    private readonly RequestDetailViewModel _vm;

    public RequestDetailPage(RequestDetailViewModel vm)
    {
        _vm = vm;
        BindingContext = vm;

        Title = "Request Details";

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 15,
                Children =
                {
                    // Loading indicator
                    new ActivityIndicator()
                        .Center()
                        .Bind(ActivityIndicator.IsRunningProperty, nameof(RequestDetailViewModel.IsLoading))
                        .Bind(ActivityIndicator.IsVisibleProperty, nameof(RequestDetailViewModel.IsLoading)),

                    // Error message
                    new Label()
                        .TextColor(Colors.Red)
                        .FontSize(14)
                        .Bind(Label.IsVisibleProperty, nameof(RequestDetailViewModel.ErrorMessage), 
                              convert: (string? msg) => !string.IsNullOrWhiteSpace(msg))
                        .Bind(Label.TextProperty, nameof(RequestDetailViewModel.ErrorMessage)),

                    // Main content
                    new VerticalStackLayout
                    {
                        Spacing = 15,
                        Children =
                        {
                            // Header Section
                            BuildSection("Request Information", new VerticalStackLayout
                            {
                                Spacing = 8,
                                Children =
                                {
                                    BuildInfoRow("Number:", "Request.Metadata.Number"),
                                    BuildInfoRow("Cadastral Number:", "Request.CadastalNumber"),
                                    BuildInfoRow("Status:", "Request.Metadata.CurrentState"),
                                    BuildInfoRow("Available From:", "Request.AvailableFrom", isDate: true),
                                    BuildInfoRow("Available Until:", "Request.AvailableUntil", isDate: true),
                                    BuildInfoRow("Due To:", "Request.Metadata.DueTo", isDate: true),
                                }
                            }),

                            // Client Section
                            BuildSection("Client Information", new VerticalStackLayout
                            {
                                Spacing = 8,
                                Children =
                                {
                                    BuildInfoRow("Name:", "Request.Client.FullName"),
                                    BuildInfoRow("Email:", "Request.Client.Email"),
                                    BuildInfoRow("Phone:", "Request.Client.PhoneNumber"),
                                    BuildInfoRow("Address:", "Request.Client.Address"),
                                }
                            }),

                            // Personnel Section
                            BuildSection("Personnel", new VerticalStackLayout
                            {
                                Spacing = 8,
                                Children =
                                {
                                    BuildInfoRow("Responsible:", "Request.Responsible"),
                                    BuildInfoRow("Performer:", "Request.Performer"),
                                }
                            }),

                            // Works Section
                            BuildSection("Cadastral Works", new VerticalStackLayout
                            {
                                Spacing = 8,
                                Children =
                                {
                                    new CollectionView
                                    {
                                        ItemTemplate = new DataTemplate(() =>
                                            new Border
                                            {
                                                Padding = 10,
                                                Margin = new Thickness(0, 4),
                                                StrokeShape = new RoundRectangle { CornerRadius = 8 },
                                                Stroke = Colors.LightGray,
                                                StrokeThickness = 1,
                                                Content = new Grid
                                                {
                                                    ColumnDefinitions =
                                                    {
                                                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                                                        new ColumnDefinition { Width = GridLength.Auto }
                                                    },
                                                    Children =
                                                    {
                                                        new Label()
                                                            .FontSize(14)
                                                            .Bind(Label.TextProperty, "WorkDescription")
                                                            .Column(0),
                                                        new Label()
                                                            .FontSize(14)
                                                            .Bold()
                                                            .TextColor(Colors.Green)
                                                            .Bind(Label.TextProperty, "Price", convert: (double price) => $"${price:F2}")
                                                            .Column(1)
                                                    }
                                                }
                                            }
                                        )
                                    }
                                    .Bind(CollectionView.ItemsSourceProperty, "Request.CadastralWorks")
                                    .Bind(CollectionView.IsVisibleProperty, "Request.CadastralWorks", 
                                          convert: (object? works) => works != null),

                                    new Label()
                                        .Text("No works added")
                                        .TextColor(Colors.Gray)
                                        .FontSize(14)
                                        .Bind(Label.IsVisibleProperty, "Request.CadastralWorks", 
                                              convert: (object? works) => works == null)
                                }
                            }),

                            // Total Price Section
                            BuildSection("Total", new HorizontalStackLayout
                            {
                                Spacing = 10,
                                Children =
                                {
                                    new Label()
                                        .Text("Total Price:")
                                        .FontSize(16)
                                        .Bold(),
                                    new Label()
                                        .FontSize(16)
                                        .Bold()
                                        .TextColor(Colors.Green)
                                        .Bind(Label.TextProperty, "Request.TotalPrice", convert: (double price) => $"${price:F2}")
                                }
                            }),

                            // Comment Section
                            BuildSection("Comment", new Label()
                                .FontSize(14)
                                .Bind(Label.TextProperty, "Request.Comment", 
                                      convert: (string? comment) => string.IsNullOrWhiteSpace(comment) ? "No comment" : comment)
                                .Bind(Label.TextColorProperty, "Request.Comment", 
                                      convert: (string? comment) => string.IsNullOrWhiteSpace(comment) ? Colors.Gray : Colors.Black)),

                            // Documents Section
                            BuildSection("Documents", new VerticalStackLayout
                            {
                                Spacing = 8,
                                Children =
                                {
                                    new CollectionView
                                    {
                                        ItemTemplate = new DataTemplate(() =>
                                            new Border
                                            {
                                                Padding = 10,
                                                Margin = new Thickness(0, 4),
                                                StrokeShape = new RoundRectangle { CornerRadius = 8 },
                                                Stroke = Colors.LightGray,
                                                StrokeThickness = 1,
                                                Content = new Label()
                                                    .FontSize(14)
                                                    .Bind(Label.TextProperty, "Name")
                                            }
                                        )
                                    }
                                    .Bind(CollectionView.ItemsSourceProperty, "Request.Documents")
                                    .Bind(CollectionView.IsVisibleProperty, "Request.Documents", 
                                          convert: (object? docs) => docs != null),

                                    new Label()
                                        .Text("No documents attached")
                                        .TextColor(Colors.Gray)
                                        .FontSize(14)
                                        .Bind(Label.IsVisibleProperty, "Request.Documents", 
                                              convert: (object? docs) => docs == null)
                                }
                            }),

                            // Action Buttons
                            new Button
                            {
                                Text = "Download PDF",
                                Margin = new Thickness(0, 10, 0, 0)
                            }
                            .Bind(Button.CommandProperty, nameof(RequestDetailViewModel.DownloadPdfCommand))
                            .Bind(IsVisibleProperty, nameof(RequestDetailViewModel.Request), 
                                  convert: (object? req) => req != null)
                        }
                    }
                    .Bind(IsVisibleProperty, nameof(RequestDetailViewModel.IsLoading), 
                          convert: (bool loading) => !loading)
                }
            }
        };
    }

    private Border BuildSection(string title, View content)
    {
        return new Border
        {
            Padding = 15,
            Margin = new Thickness(0, 5),
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Stroke = Colors.LightGray,
            StrokeThickness = 1,
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label()
                        .Text(title)
                        .FontSize(16)
                        .Bold(),
                    content
                }
            }
        };
    }

    private HorizontalStackLayout BuildInfoRow(string label, string bindingPath, bool isDate = false)
    {
        var valueLabel = new Label()
            .FontSize(14);

        if (isDate)
        {
            valueLabel.Bind(Label.TextProperty, bindingPath, 
                convert: (DateTime date) => date.ToString("dd/MM/yyyy"));
        }
        else
        {
            valueLabel.Bind(Label.TextProperty, bindingPath, 
                convert: (object? value) => value?.ToString() ?? "N/A");
        }

        return new HorizontalStackLayout
        {
            Spacing = 10,
            Children =
            {
                new Label()
                    .Text(label)
                    .FontSize(14)
                    .Bold()
                    .TextColor(Colors.Gray),
                valueLabel
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Dispatcher.Dispatch(async () =>
        {
            await _vm.InitializeCommand.ExecuteAsync(null);
        });
    }
}
