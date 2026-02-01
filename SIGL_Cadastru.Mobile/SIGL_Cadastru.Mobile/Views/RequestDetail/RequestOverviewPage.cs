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

                            // Quick Actions Card
                            BuildQuickActionsCard(),

                            // Request Information Card
                            BuildRequestInfoCard(),

                            // Client Information Card
                            BuildClientInfoCard(),

                            // Works Summary Card
                            BuildWorksSummaryCard(),

                            // Documents Card
                            BuildDocumentsCard(),

                            // PDF Download Button
                            new Button()
                                .Text("Download PDF")
                                .StylePrimaryButton()
                                .Bind(Button.CommandProperty, nameof(RequestOverviewViewModel.DownloadPdfCommand))
                                .Bind(IsVisibleProperty, nameof(RequestOverviewViewModel.Request), 
                                      convert: (object? req) => req != null)
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
                    new Label()
                        .StyleTitle()
                        .Bind(Label.TextProperty, "Request.Metadata.Number", 
                              convert: (string? num) => $"Request #{num ?? "N/A"}"),

                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Auto }
                        },
                        Children =
                        {
                            new Label()
                                .StyleCaption()
                                .Bind(Label.TextProperty, "Request.CadastalNumber", 
                                      convert: (string? num) => $"Cadastral: {num ?? "N/A"}")
                                .Column(0),

                            // Status Badge
                            new StatusChipComponent()
                                .Bind(StatusChipComponent.StateProperty, "Request.Metadata.CurrentState")
                                .Column(1)
                        }
                    },

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
                            new Label()
                                .StyleBody()
                                .Bind(Label.TextProperty, "Request.TotalPayments", 
                                      convert: (double paid) => $"Paid: {paid:F2} MDL"),

                            new Label()
                                .StyleBody()
                                .Font(bold: true)
                                .Bind(Label.TextProperty, "Request.TotalPrice", 
                                      convert: (double total) => $"{total:F2} MDL")
                                .Column(1)
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
                    new SectionHeaderComponent { Title = "Request Information" },

                    BuildInfoRow("Number:", "Request.Metadata.Number"),
                    BuildInfoRow("Cadastral Number:", "Request.CadastalNumber"),
                    BuildInfoRow("Status:", "Request.Metadata.CurrentState"),
                    BuildInfoRow("Available From:", "Request.AvailableFrom", isDate: true),
                    BuildInfoRow("Available Until:", "Request.AvailableUntil", isDate: true),
                    BuildInfoRow("Due To:", "Request.Metadata.DueTo", isDate: true),
                    BuildInfoRow("Responsible:", "Request.Responsible"),
                    BuildInfoRow("Performer:", "Request.Performer"),

                    // Comment section (if present)
                    new VerticalStackLayout
                    {
                        Spacing = DesignTokens.Spacing.Xs,
                        Children =
                        {
                            new Label()
                                .StyleLabel()
                                .Text("Comment:"),

                            new Label()
                                .StyleBody()
                                .Bind(Label.TextProperty, "Request.Comment", 
                                      convert: (string? comment) => string.IsNullOrWhiteSpace(comment) ? "No comment" : comment)
                                .Bind(Label.TextColorProperty, "Request.Comment", 
                                      convert: (string? comment) => string.IsNullOrWhiteSpace(comment) 
                                          ? DesignTokens.Colors.TextSecondary 
                                          : DesignTokens.Colors.TextPrimary)
                        }
                    }
                    .Margins(top: DesignTokens.Spacing.Sm)
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
                    new SectionHeaderComponent { Title = "Client Information" },

                    BuildInfoRow("Name:", "Request.Client.FullName"),
                    BuildInfoRow("Email:", "Request.Client.Email"),
                    BuildInfoRow("Phone:", "Request.Client.PhoneNumber"),
                    BuildInfoRow("Address:", "Request.Client.Address")
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
                    new SectionHeaderComponent { Title = "Cadastral Works" },

                    new CollectionView
                    {
                        ItemTemplate = new DataTemplate(() =>
                            new Border()
                                .StyleListItem()
                                .Invoke(border =>
                                {
                                    border.Content = new Grid
                                    {
                                        ColumnDefinitions =
                                        {
                                            new ColumnDefinition { Width = GridLength.Star },
                                            new ColumnDefinition { Width = GridLength.Auto }
                                        },
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
                                                      convert: (double price) => $"{price:F2} MDL")
                                                .Column(1)
                                        }
                                    };
                                })
                        )
                    }
                    .Bind(CollectionView.ItemsSourceProperty, "Request.CadastralWorks")
                    .Bind(CollectionView.IsVisibleProperty, "Request.CadastralWorks", 
                          convert: (object? works) => works != null && works is System.Collections.IList list && list.Count > 0),

                    new Label()
                        .StyleCaption()
                        .Text("No works added")
                        .Bind(Label.IsVisibleProperty, "Request.CadastralWorks", 
                              convert: (object? works) => works == null || (works is System.Collections.IList list && list.Count == 0)),

                    // Total Price
                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Auto }
                        },
                        Margin = new Thickness(0, DesignTokens.Spacing.Sm, 0, 0),
                        Children =
                        {
                            new Label()
                                .StyleSubtitle()
                                .Text("Total:"),

                            new Label()
                                .StyleSubtitle()
                                .StyleSuccess()
                                .Bind(Label.TextProperty, "Request.TotalPrice", 
                                      convert: (double price) => $"{price:F2} MDL")
                                .Column(1)
                        }
                    }
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
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    new SectionHeaderComponent { Title = "Documents" },

                    new CollectionView
                    {
                        ItemTemplate = new DataTemplate(() =>
                            new Border()
                                .StyleListItem()
                                .Invoke(border =>
                                {
                                    border.Content = new Label()
                                        .StyleBody()
                                        .Bind(Label.TextProperty, "Name");
                                })
                        )
                    }
                    .Bind(CollectionView.ItemsSourceProperty, "Request.Documents")
                    .Bind(CollectionView.IsVisibleProperty, "Request.Documents", 
                          convert: (object? docs) => docs != null && docs is System.Collections.IList list && list.Count > 0),

                    new Label()
                        .StyleCaption()
                        .Text("No documents attached")
                        .Bind(Label.IsVisibleProperty, "Request.Documents", 
                              convert: (object? docs) => docs == null || (docs is System.Collections.IList list && list.Count == 0))
                }
            }
        };
    }

    private HorizontalStackLayout BuildInfoRow(string label, string bindingPath, bool isDate = false)
    {
        var valueLabel = new Label()
            .StyleBody()
            .TextColor(DesignTokens.Colors.TextSecondary);

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
            Spacing = DesignTokens.Spacing.Sm,
            Children =
            {
                new Label()
                    .Text(label)
                    .StyleLabel()
                    .Invoke(lbl => lbl.WidthRequest = 150),
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
