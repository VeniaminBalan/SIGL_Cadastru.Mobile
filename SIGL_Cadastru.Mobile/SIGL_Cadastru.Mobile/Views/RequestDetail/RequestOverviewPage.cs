using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.Components;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.ViewModels.RequestDetail;

namespace SIGL_Cadastru.Mobile.Views.RequestDetail;

/// <summary>
/// Request Overview Page - Hub page displaying request summary with navigation to specialized pages
/// </summary>
public class RequestOverviewPage : ContentPage
{
    private readonly RequestOverviewViewModel _vm;

    public RequestOverviewPage(RequestOverviewViewModel vm)
    {
        _vm = vm;
        BindingContext = vm;

        Title = "Request Overview";

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = DesignTokens.Spacing.Lg,
                Children =
                {
                    // Loading indicator
                    new ActivityIndicator()
                        .Center()
                        .Bind(ActivityIndicator.IsRunningProperty, nameof(RequestOverviewViewModel.IsLoading))
                        .Bind(ActivityIndicator.IsVisibleProperty, nameof(RequestOverviewViewModel.IsLoading)),

                    // Error message
                    new Label()
                        .StyleBody()
                        .StyleError()
                        .Bind(Label.IsVisibleProperty, nameof(RequestOverviewViewModel.ErrorMessage), 
                              convert: (string? msg) => !string.IsNullOrWhiteSpace(msg))
                        .Bind(Label.TextProperty, nameof(RequestOverviewViewModel.ErrorMessage)),

                    // Main content
                    new VerticalStackLayout
                    {
                        Spacing = DesignTokens.Spacing.Lg,
                        Children =
                        {
                            // Request Header Card
                            BuildRequestHeaderCard(),

                            // Request Info Card
                            BuildRequestInfoCard(),

                            // Client Info Card
                            BuildClientInfoCard(),

                            // Works Summary Card
                            BuildWorksSummaryCard(),

                            // Documents Card (collapsed by default)
                            BuildDocumentsCard(),

                            // Quick Actions Card
                            BuildQuickActionsCard()
                        }
                    }
                    .Bind(IsVisibleProperty, nameof(RequestOverviewViewModel.IsLoading), 
                          convert: (bool loading) => !loading)
                }
            }
            .Paddings(all: DesignTokens.Layout.PagePadding)
        };
    }

    private View BuildRequestHeaderCard()
    {
        return new CardComponent
        {
            Content = new VerticalStackLayout
            {
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    // Title Row
                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Auto },
                            new ColumnDefinition { Width = GridLength.Auto }
                        },
                        ColumnSpacing = DesignTokens.Spacing.Sm,
                        Children =
                        {
                            new Label()
                                .StyleTitle()
                                .Bind(Label.TextProperty, "Request.Metadata.Number", 
                                      convert: (string? num) => $"#{num ?? "N/A"}")
                                .Column(0),

                            new PaymentStatusChipComponent()
                                .Bind(PaymentStatusChipComponent.IsFullyPaidProperty, "Request.IsFullyPaid")
                                .Column(1),

                            new StatusChipComponent()
                                .Bind(StatusChipComponent.StateProperty, "Request.Metadata.CurrentState")
                                .Column(2)
                        }
                    },

                    // Info Grid
                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Star }
                        },
                        RowDefinitions =
                        {
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto }
                        },
                        ColumnSpacing = DesignTokens.Spacing.Md,
                        RowSpacing = DesignTokens.Spacing.Xs,
                        Children =
                        {
                            new Label()
                                .StyleCaption()
                                .Bind(Label.TextProperty, "Request.CadastalNumber", 
                                      convert: (string? num) => $"📍 {num ?? "N/A"}")
                                .Row(0).Column(0),

                            new Label()
                                .StyleCaption()
                                .Bind(Label.TextProperty, "Request.Client.FullName", 
                                      convert: (string? name) => $"👤 {name ?? "N/A"}")
                                .Row(0).Column(1),

                            new Label()
                                .StyleCaption()
                                .Bind(Label.TextProperty, "Request.AvailableFrom", 
                                      convert: (DateTime date) => $"📅 {date:dd/MM/yyyy}")
                                .Row(1).Column(0),

                            new Label()
                                .StyleBody()
                                .Font(bold: true)
                                .StyleSuccess()
                                .Bind(Label.TextProperty, "Request.TotalPrice", 
                                      convert: (double price) => $"{price:F2} MDL")
                                .Row(1).Column(1)
                        }
                    }
                }
            }
        };
    }

    private View BuildQuickActionsCard()
    {
        return new CardComponent
        {
            Content = new VerticalStackLayout
            {
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    new SectionHeaderComponent { Title = "Quick Actions" },

                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Star }
                        },
                        ColumnSpacing = DesignTokens.Spacing.Md,
                        Children =
                        {
                            new Button()
                                .Text("💰 Payments")
                                .StylePrimaryButton()
                                .Bind(Button.CommandProperty, nameof(RequestOverviewViewModel.NavigateToPaymentsCommand))
                                .Column(0),

                            new Button()
                                .Text("📊 States")
                                .StyleSecondaryButton()
                                .Bind(Button.CommandProperty, nameof(RequestOverviewViewModel.NavigateToStatesCommand))
                                .Column(1)
                        }
                    }
                }
            }
        };
    }

    private View BuildRequestInfoCard()
    {
        return new CardComponent
        {
            Content = new VerticalStackLayout
            {
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    new SectionHeaderComponent { Title = "Request Details" },

                    new InfoFieldComponent { Label = "Responsible" }
                        .Bind(InfoFieldComponent.ValueProperty, "Request.Responsible"),

                    new InfoFieldComponent { Label = "Performer" }
                        .Bind(InfoFieldComponent.ValueProperty, "Request.Performer"),

                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Star }
                        },
                        ColumnSpacing = DesignTokens.Spacing.Lg,
                        Children =
                        {
                            new InfoFieldComponent { Label = "Available Until", UseBodyStyle = false }
                                .Bind(InfoFieldComponent.ValueProperty, "Request.AvailableUntil", 
                                    convert: (DateTime date) => date.ToString("dd MMM yyyy"))
                                .Column(0),

                            new InfoFieldComponent { Label = "Due Date", UseBodyStyle = false }
                                .Bind(InfoFieldComponent.ValueProperty, "Request.Metadata.DueTo", 
                                    convert: (DateTime date) => date.ToString("dd MMM yyyy"))
                                .Column(1)
                        }
                    },

                    new InfoFieldComponent { Label = "Comment", UseBodyStyle = false }
                        .Bind(InfoFieldComponent.ValueProperty, "Request.Comment", 
                              convert: (string? comment) => string.IsNullOrWhiteSpace(comment) ? "No comment provided" : comment)
                }
            }
        };
    }

    private View BuildClientInfoCard()
    {
        return new CardComponent
        {
            Content = new VerticalStackLayout
            {
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    new SectionHeaderComponent { Title = "Client" },

                    new InfoFieldComponent { Label = "Full Name" }
                        .Bind(InfoFieldComponent.ValueProperty, nameof(RequestOverviewViewModel.Request.Client.FullName)),

                    new InfoFieldComponent { Label = "Email", UseBodyStyle = false }
                        .Bind(InfoFieldComponent.ValueProperty, nameof(RequestOverviewViewModel.Request.Client.Email)),

                    new InfoFieldComponent { Label = "Phone", UseBodyStyle = false }
                        .Bind(InfoFieldComponent.ValueProperty, nameof(RequestOverviewViewModel.Request.Client.PhoneNumber))
                }
            }
        };
    }

    private View BuildWorksSummaryCard()
    {
        return new CardComponent
        {
            Content = new VerticalStackLayout
            {
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Auto }
                        },
                        Children =
                        {
                            new Label().StyleSubtitle().Text("Works").Column(0),
                            new Label()
                                .StyleCaption()
                                .Bind(Label.TextProperty, "Request.CadastralWorks", 
                                      convert: (object? works) => works is System.Collections.IList list ? $"{list.Count} items" : "0 items")
                                .Column(1)
                        }
                    },

                    new CollectionView
                    {
                        ItemTemplate = new DataTemplate(() =>
                            new Grid
                            {
                                ColumnDefinitions =
                                {
                                    new ColumnDefinition { Width = GridLength.Star },
                                    new ColumnDefinition { Width = GridLength.Auto }
                                },
                                Padding = new Thickness(0, DesignTokens.Spacing.Xs),
                                Children =
                                {
                                    new Label()
                                        .StyleBody()
                                        .Bind(Label.TextProperty, "WorkDescription")
                                        .Column(0),

                                    new Label()
                                        .StyleBody()
                                        .Font(bold: true)
                                        .StyleSuccess()
                                        .Bind(Label.TextProperty, "Price", 
                                              convert: (double price) => $"{price:F2}")
                                        .Column(1)
                                }
                            }
                        )
                    }
                    .Bind(CollectionView.ItemsSourceProperty, "Request.CadastralWorks")
                    .Bind(CollectionView.IsVisibleProperty, "Request.CadastralWorks", 
                          convert: (object? works) => works != null && works is System.Collections.IList list && list.Count > 0),

                    new Label()
                        .StyleCaption()
                        .Text("No works added")
                        .Center()
                        .Bind(Label.IsVisibleProperty, "Request.CadastralWorks", 
                              convert: (object? works) => works == null || (works is System.Collections.IList list && list.Count == 0))
                }
            }
        };
    }

    private View BuildDocumentsCard()
    {
        return new CardComponent
        {
            Content = new VerticalStackLayout
            {
                Spacing = DesignTokens.Spacing.Sm,
                Children =
                {
                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Auto }
                        },
                        Children =
                        {
                            new Label().StyleSubtitle().Text("Documents").Column(0),
                            new Label()
                                .StyleCaption()
                                .Bind(Label.TextProperty, "Request.Documents", 
                                      convert: (object? docs) => docs is System.Collections.IList list ? $"{list.Count} files" : "0 files")
                                .Column(1)
                        }
                    },

                    new CollectionView
                    {
                        ItemTemplate = new DataTemplate(() =>
                            new Label()
                                .StyleCaption()
                                .Padding(0, DesignTokens.Spacing.Xs)
                                .Bind(Label.TextProperty, "Name", convert: (string? name) => $"📄 {name ?? "Unnamed"}")
                        )
                    }
                    .Bind(CollectionView.ItemsSourceProperty, "Request.Documents")
                    .Bind(CollectionView.IsVisibleProperty, "Request.Documents", 
                          convert: (object? docs) => docs != null && docs is System.Collections.IList list && list.Count > 0)
                }
            }
        }
        .Bind(IsVisibleProperty, "Request.Documents", 
              convert: (object? docs) => docs != null && docs is System.Collections.IList list && list.Count > 0);
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
