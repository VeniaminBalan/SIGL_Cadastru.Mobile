using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.Components;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.ViewModels.RequestDetail;
using Microsoft.Maui.Controls.Shapes;

namespace SIGL_Cadastru.Mobile.Views.RequestDetail;

/// <summary>
/// Request Payments Page - Complete payment management (CRUD + validation)
/// </summary>
public class RequestPaymentsPage : ContentPage
{
    private readonly RequestPaymentsViewModel _vm;

    public RequestPaymentsPage(RequestPaymentsViewModel vm)
    {
        _vm = vm;
        BindingContext = vm;

        Title = "Payment Management";

        Content = new RefreshView
        {
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
                            .Bind(ActivityIndicator.IsRunningProperty, nameof(RequestPaymentsViewModel.IsLoading))
                            .Bind(ActivityIndicator.IsVisibleProperty, nameof(RequestPaymentsViewModel.IsLoading)),

                        // Error message
                        new Label()
                            .StyleBody()
                            .StyleError()
                            .Bind(Label.IsVisibleProperty, nameof(RequestPaymentsViewModel.ErrorMessage), 
                                  convert: (string? msg) => !string.IsNullOrWhiteSpace(msg))
                            .Bind(Label.TextProperty, nameof(RequestPaymentsViewModel.ErrorMessage)),

                        // Main content
                        new VerticalStackLayout
                        {
                            Spacing = DesignTokens.Spacing.Lg,
                            Children =
                            {
                                // Payment Status Card
                                BuildPaymentStatusCard(),

                                // Add Payment Form Card
                                BuildAddPaymentCard(),

                                // Payment History Card
                                BuildPaymentHistoryCard()
                            }
                        }
                        .Bind(IsVisibleProperty, nameof(RequestPaymentsViewModel.IsLoading), 
                              convert: (bool loading) => !loading)
                    }
                }
                .Paddings(all: DesignTokens.Layout.PagePadding)
            }
        }
        .Bind(RefreshView.CommandProperty, nameof(RequestPaymentsViewModel.RefreshCommand))
        .Bind(RefreshView.IsRefreshingProperty, nameof(RequestPaymentsViewModel.IsLoading));
    }

    private View BuildPaymentStatusCard()
    {
        return new CardComponent
        {
            Content = new VerticalStackLayout
            {
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    new SectionHeaderComponent { Title = "Payment Status" },

                    // Status Indicator
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
                                .StyleLabel()
                                .Text("Status:")
                                .Column(0),

                            new Border
                            {
                                Padding = new Thickness(DesignTokens.Spacing.Sm, DesignTokens.Spacing.Xs),
                                StrokeShape = new RoundRectangle 
                                { 
                                    CornerRadius = new CornerRadius(DesignTokens.BorderRadius.Full) 
                                },
                                Content = new Label()
                                    .StyleCaption()
                                    .Font(bold: true)
                                    .TextColor(DesignTokens.Colors.TextOnPrimary)
                                    .Bind(Label.TextProperty, nameof(RequestPaymentsViewModel.PaymentStatus))
                            }
                            .Bind(Border.BackgroundColorProperty, nameof(RequestPaymentsViewModel.PaymentStatus),
                                  convert: (string? status) => status switch
                                  {
                                      "Fully Paid" => DesignTokens.Colors.Success,
                                      "Partially Paid" => DesignTokens.Colors.Warning,
                                      "Unpaid" => DesignTokens.Colors.Error,
                                      _ => DesignTokens.Colors.TextSecondary
                                  })
                            .Column(1)
                        }
                    },

                    // Payment Amounts
                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Auto }
                        },
                        RowDefinitions =
                        {
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto }
                        },
                        RowSpacing = DesignTokens.Spacing.Xs,
                        Children =
                        {
                            new Label()
                                .StyleBody()
                                .Text("Total:")
                                .Row(0).Column(0),

                            new Label()
                                .StyleBody()
                                .Font(bold: true)
                                .Bind(Label.TextProperty, "Request.TotalPrice", 
                                      convert: (double total) => $"{total:F2} MDL")
                                .Row(0).Column(1),

                            new Label()
                                .StyleBody()
                                .Text("Paid:")
                                .Row(1).Column(0),

                            new Label()
                                .StyleBody()
                                .Font(bold: true)
                                .StyleSuccess()
                                .Bind(Label.TextProperty, "Request.TotalPayments", 
                                      convert: (double paid) => $"{paid:F2} MDL")
                                .Row(1).Column(1),

                            new Label()
                                .StyleBody()
                                .Text("Remaining:")
                                .Row(2).Column(0),

                            new Label()
                                .StyleBody()
                                .Font(bold: true)
                                .StyleError()
                                .Bind(Label.TextProperty, nameof(RequestPaymentsViewModel.RemainingAmount), 
                                      convert: (double remaining) => $"{remaining:F2} MDL")
                                .Row(2).Column(1)
                        }
                    },

                    // Progress Bar
                    new VerticalStackLayout
                    {
                        Spacing = DesignTokens.Spacing.Xs,
                        Children =
                        {
                            new Label()
                                .StyleCaption()
                                .Text("Payment Progress"),

                            new ProgressBar()
                                .Bind(ProgressBar.ProgressProperty, nameof(RequestPaymentsViewModel.PaymentProgress))
                        }
                    }
                }
            }
        };
    }

    private View BuildAddPaymentCard()
    {
        return new CardComponent
        {
            Content = new VerticalStackLayout
            {
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    new SectionHeaderComponent { Title = "Add Payment" },

                    // Amount Entry
                    new VerticalStackLayout
                    {
                        Spacing = DesignTokens.Spacing.Xs,
                        Children =
                        {
                            new Label()
                                .StyleLabel()
                                .Text("Amount (MDL)"),

                            new Entry
                            {
                                Keyboard = Keyboard.Numeric,
                                Placeholder = "0.00"
                            }
                            .StyleFormEntry()
                            .Bind(Entry.TextProperty, nameof(RequestPaymentsViewModel.PaymentAmount))
                        }
                    },

                    // Description Entry
                    new VerticalStackLayout
                    {
                        Spacing = DesignTokens.Spacing.Xs,
                        Children =
                        {
                            new Label()
                                .StyleLabel()
                                .Text("Description (Optional)"),

                            new Entry
                            {
                                Placeholder = "Payment description (max 500 characters)"
                            }
                            .StyleFormEntry()
                            .Bind(Entry.TextProperty, nameof(RequestPaymentsViewModel.PaymentDescription))
                        }
                    },

                    // Payment Error Message
                    new Label()
                        .StyleCaption()
                        .StyleError()
                        .Bind(Label.TextProperty, nameof(RequestPaymentsViewModel.PaymentErrorMessage))
                        .Bind(Label.IsVisibleProperty, nameof(RequestPaymentsViewModel.PaymentErrorMessage),
                              convert: (string? msg) => !string.IsNullOrWhiteSpace(msg)),

                    // Add Payment Button
                    new Button()
                        .Text("Add Payment")
                        .StylePrimaryButton()
                        .Bind(Button.CommandProperty, nameof(RequestPaymentsViewModel.AddPaymentCommand))
                        .Bind(Button.IsEnabledProperty, nameof(RequestPaymentsViewModel.IsAddingPayment),
                              convert: (bool isAdding) => !isAdding)
                        .Bind(IsEnabledProperty, nameof(RequestPaymentsViewModel.CanAddPayment))
                }
            }
        }
        .Bind(IsVisibleProperty, nameof(RequestPaymentsViewModel.CanAddPayment));
    }

    private View BuildPaymentHistoryCard()
    {
        return new CardComponent
        {
            Content = new VerticalStackLayout
            {
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    new SectionHeaderComponent { Title = "Payment History" },

                    new CollectionView
                    {
                        ItemTemplate = new DataTemplate(() =>
                            new Border()
                                .StyleListItem()
                                .Invoke(border =>
                                {
                                    border.Content = new Grid
                                    {
                                        RowDefinitions =
                                        {
                                            new RowDefinition { Height = GridLength.Auto },
                                            new RowDefinition { Height = GridLength.Auto },
                                            new RowDefinition { Height = GridLength.Auto }
                                        },
                                        ColumnDefinitions =
                                        {
                                            new ColumnDefinition { Width = GridLength.Star },
                                            new ColumnDefinition { Width = GridLength.Auto }
                                        },
                                        RowSpacing = DesignTokens.Spacing.Xs,
                                        Children =
                                        {
                                            // Amount and Date
                                            new HorizontalStackLayout
                                            {
                                                Spacing = DesignTokens.Spacing.Sm,
                                                Children =
                                                {
                                                    new Label()
                                                        .StyleBody()
                                                        .Font(bold: true)
                                                        .StyleSuccess()
                                                        .Bind(Label.TextProperty, "Amount", 
                                                              convert: (double amt) => $"{amt:F2} MDL"),

                                                    new Label()
                                                        .StyleCaption()
                                                        .Bind(Label.TextProperty, "CreatedAt", 
                                                              convert: (DateTime dt) => dt.ToString("dd/MM/yyyy HH:mm"))
                                                }
                                            }
                                            .Row(0).Column(0),

                                            // Delete Button
                                            new Button()
                                                .Text("Delete")
                                                .StyleOutlineButton()
                                                .Invoke(btn =>
                                                {
                                                    btn.HeightRequest = 36;
                                                    btn.Padding = new Thickness(DesignTokens.Spacing.Sm, DesignTokens.Spacing.Xs);
                                                    btn.TextColor(DesignTokens.Colors.Error);
                                                    btn.BorderColor = DesignTokens.Colors.Error;
                                                })
                                                .Bind(Button.CommandProperty, nameof(RequestPaymentsViewModel.DeletePaymentCommand), source: _vm)
                                                .Bind(Button.CommandParameterProperty, ".")
                                                .Bind(IsEnabledProperty, nameof(RequestPaymentsViewModel.CanDeletePayment), source: _vm)
                                                .Row(0).Column(1),

                                            // Description
                                            new Label()
                                                .StyleCaption()
                                                .Bind(Label.TextProperty, "Description", 
                                                      convert: (string? desc) => string.IsNullOrWhiteSpace(desc) ? "No description" : desc)
                                                .Row(1).Column(0).ColumnSpan(2)
                                        }
                                    };
                                })
                        )
                    }
                    .Bind(CollectionView.ItemsSourceProperty, "Request.Payments")
                    .Bind(CollectionView.IsVisibleProperty, "Request.Payments", 
                          convert: (object? payments) => payments != null && payments is System.Collections.IList list && list.Count > 0),

                    new Label()
                        .StyleCaption()
                        .Text("No payments yet")
                        .Center()
                        .Bind(Label.IsVisibleProperty, "Request.Payments", 
                              convert: (object? payments) => payments == null || (payments is System.Collections.IList list && list.Count == 0))
                }
            }
        };
    }
}
