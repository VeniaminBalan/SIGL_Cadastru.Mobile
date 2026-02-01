using CommunityToolkit.Maui.Converters;
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
                                        .Bind(Label.TextProperty, "Request.TotalPrice", convert: (double price) => $"{price:F2} MDL")
                                }
                            }),

                            // Payment Section
                            BuildSection("Payments", BuildPaymentSection()),

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
                                  convert: (object? req) => req != null),

                            // Issue Request Button (disabled if not fully paid)
                            new Button
                            {
                                Text = "Issue Request",
                                BackgroundColor = Colors.Green,
                                TextColor = Colors.White,
                                Margin = new Thickness(0, 5, 0, 0)
                            }
                            .Bind(Button.IsEnabledProperty, nameof(RequestDetailViewModel.CanIssueRequest))
                            .Bind(IsVisibleProperty, nameof(RequestDetailViewModel.Request), 
                                  convert: (object? req) => req != null)
                            .Invoke(btn => btn.Clicked += async (s, e) =>
                            {
                                if (!_vm.CanIssueRequest)
                                {
                                    await DisplayAlertAsync("Cannot Issue", "Request must be fully paid before issuing", "OK");
                                }
                                else
                                {
                                    // TODO: Implement issue request logic
                                    await DisplayAlertAsync("Info", "Issue request feature coming soon", "OK");
                                }
                            })
                        }
                    }
                    .Bind(IsVisibleProperty, nameof(RequestDetailViewModel.IsLoading), 
                          convert: (bool loading) => !loading)
                }
            }
        };
    }

    private View BuildPaymentSection()
    {
        return new VerticalStackLayout
        {
            Spacing = 12,
            Children =
            {
                // Payment Status Indicator
                new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Auto }
                    },
                    Children =
                    {
                        new HorizontalStackLayout
                        {
                            Spacing = 10,
                            Children =
                            {
                                new Label()
                                    .Text("Status:")
                                    .FontSize(14)
                                    .Bold()
                                    .TextColor(Colors.Gray),
                                new Label()
                                    .FontSize(14)
                                    .Bold()
                                    .Bind(Label.TextProperty, nameof(RequestDetailViewModel.PaymentStatus))
                                    .Bind(Label.TextColorProperty, nameof(RequestDetailViewModel.PaymentStatus),
                                          convert: (string status) => status switch
                                          {
                                              "Fully Paid" => Colors.Green,
                                              "Partially Paid" => Colors.Orange,
                                              "Unpaid" => Colors.Red,
                                              _ => Colors.Gray
                                          })
                            }
                        }.Column(0)
                    }
                },

                // Payment Progress
                new VerticalStackLayout
                {
                    Spacing = 5,
                    Children =
                    {
                        new HorizontalStackLayout
                        {
                            Spacing = 10,
                            Children =
                            {
                                new Label()
                                    .FontSize(14)
                                    .Bind(Label.TextProperty, "Request.TotalPayments", 
                                          convert: (double paid) => $"Paid: {paid:F2} MDL"),
                                new Label()
                                    .FontSize(14)
                                    .TextColor(Colors.Gray)
                                    .Bind(Label.TextProperty, "Request.TotalPrice", 
                                          convert: (double total) => $"/ {total:F2} MDL")
                            }
                        },
                        new ProgressBar()
                            .Invoke(pb =>
                            {
                                pb.SetBinding(ProgressBar.ProgressProperty, new MultiBinding
                                {
                                    Bindings =
                                    {
                                        new Binding("Request.TotalPayments"),
                                        new Binding("Request.TotalPrice")
                                    },
                                    Converter = new MultiMathExpressionConverter(),
                                    ConverterParameter = "x0 / max(x1, 1)"
                                });
                            })
                    }
                },

                // Remaining Amount
                new Label()
                    .FontSize(14)
                    .Bold()
                    .TextColor(Colors.Red)
                    .Bind(Label.TextProperty, nameof(RequestDetailViewModel.RemainingAmount), 
                          convert: (double remaining) => $"Remaining: {remaining:F2} MDL")
                    .Bind(Label.IsVisibleProperty, nameof(RequestDetailViewModel.RemainingAmount),
                          convert: (double remaining) => remaining > 0),

                // Add Payment Form
                new Border
                {
                    Padding = 12,
                    Margin = new Thickness(0, 10, 0, 0),
                    StrokeShape = new RoundRectangle { CornerRadius = 8 },
                    Stroke = Colors.LightBlue,
                    StrokeThickness = 1,
                    BackgroundColor = Colors.LightBlue.WithAlpha(0.1f),
                    Content = new VerticalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            new Label()
                                .Text("Add Payment")
                                .FontSize(14)
                                .Bold(),

                            new Entry
                            {
                                Placeholder = "Amount (MDL)",
                                Keyboard = Keyboard.Numeric
                            }
                            .Bind(Entry.TextProperty, nameof(RequestDetailViewModel.PaymentAmount)),

                            new Entry
                            {
                                Placeholder = "Description (optional, max 500 chars)"
                            }
                            .Bind(Entry.TextProperty, nameof(RequestDetailViewModel.PaymentDescription)),

                            new Label()
                                .FontSize(12)
                                .TextColor(Colors.Red)
                                .Bind(Label.TextProperty, nameof(RequestDetailViewModel.PaymentErrorMessage))
                                .Bind(Label.IsVisibleProperty, nameof(RequestDetailViewModel.PaymentErrorMessage),
                                      convert: (string? msg) => !string.IsNullOrWhiteSpace(msg)),

                            new Button
                            {
                                Text = "Add Payment"
                            }
                            .Bind(Button.CommandProperty, nameof(RequestDetailViewModel.AddPaymentCommand))
                            .Bind(Button.IsEnabledProperty, nameof(RequestDetailViewModel.IsAddingPayment),
                                  convert: (bool isAdding) => !isAdding)
                        }
                    }
                },

                // Payment List
                new Label()
                    .Text("Payment History")
                    .FontSize(14)
                    .Bold()
                    .Margins(0, 10, 0, 0),

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
                                RowDefinitions =
                                {
                                    new RowDefinition { Height = GridLength.Auto },
                                    new RowDefinition { Height = GridLength.Auto },
                                    new RowDefinition { Height = GridLength.Auto }
                                },
                                ColumnDefinitions =
                                {
                                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                                    new ColumnDefinition { Width = GridLength.Auto }
                                },
                                Children =
                                {
                                    new HorizontalStackLayout
                                    {
                                        Spacing = 10,
                                        Children =
                                        {
                                            new Label()
                                                .FontSize(14)
                                                .Bold()
                                                .TextColor(Colors.Green)
                                                .Bind(Label.TextProperty, "Amount", convert: (double amt) => $"{amt:F2} MDL"),
                                            new Label()
                                                .FontSize(12)
                                                .TextColor(Colors.Gray)
                                                .Bind(Label.TextProperty, "CreatedAt", convert: (DateTime dt) => dt.ToString("dd/MM/yyyy HH:mm"))
                                        }
                                    }.Row(0).Column(0),

                                    new Button
                                    {
                                        Text = "Delete",
                                        BackgroundColor = Colors.Red,
                                        TextColor = Colors.White,
                                        FontSize = 12,
                                        Padding = new Thickness(10, 5)
                                    }
                                    .Bind(Button.CommandProperty, nameof(RequestDetailViewModel.DeletePaymentCommand), source: _vm)
                                    .Bind(Button.CommandParameterProperty, ".")
                                    .Bind(Button.IsEnabledProperty, "Request.Metadata.CurrentState", source: _vm,
                                          convert: (Models.Shared.StateType state) => state != Models.Shared.StateType.Issued)
                                    .Row(0).Column(1),

                                    new Label()
                                        .FontSize(13)
                                        .TextColor(Colors.Gray)
                                        .Bind(Label.TextProperty, "Description", 
                                              convert: (string? desc) => string.IsNullOrWhiteSpace(desc) ? "No description" : desc)
                                        .Row(1).Column(0).ColumnSpan(2)
                                }
                            }
                        }
                    )
                }
                .Bind(CollectionView.ItemsSourceProperty, "Request.Payments")
                .Bind(CollectionView.IsVisibleProperty, "Request.Payments", 
                      convert: (object? payments) => payments != null),

                new Label()
                    .Text("No payments yet")
                    .TextColor(Colors.Gray)
                    .FontSize(14)
                    .Bind(Label.IsVisibleProperty, "Request.Payments", 
                          convert: (object? payments) => payments == null || (payments is System.Collections.IList list && list.Count == 0))
            }
        };
    }

    private Task DisplayAlertAsync(string title, string message, string cancel)
    {
        return Shell.Current.DisplayAlertAsync(title, message, cancel);
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
