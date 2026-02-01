using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.Components;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.ViewModels.RequestDetail;
using SIGL_Cadastru.Mobile.Models.Shared;
using Microsoft.Maui.Controls.Shapes;

namespace SIGL_Cadastru.Mobile.Views.RequestDetail;

/// <summary>
/// Request States Page - State history and workflow management
/// </summary>
public class RequestStatesPage : ContentPage
{
    private readonly RequestStatesViewModel _vm;

    public RequestStatesPage(RequestStatesViewModel vm)
    {
        _vm = vm;
        BindingContext = vm;

        Title = "State Management";

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
                            .Bind(ActivityIndicator.IsRunningProperty, nameof(RequestStatesViewModel.IsLoading))
                            .Bind(ActivityIndicator.IsVisibleProperty, nameof(RequestStatesViewModel.IsLoading)),

                        // Error message
                        new Label()
                            .StyleBody()
                            .StyleError()
                            .Bind(Label.IsVisibleProperty, nameof(RequestStatesViewModel.ErrorMessage), 
                                  convert: (string? msg) => !string.IsNullOrWhiteSpace(msg))
                            .Bind(Label.TextProperty, nameof(RequestStatesViewModel.ErrorMessage)),

                        // Main content
                        new VerticalStackLayout
                        {
                            Spacing = DesignTokens.Spacing.Lg,
                            Children =
                            {
                                // Current State Card
                                BuildCurrentStateCard(),

                                // Issue Request Action Card
                                BuildIssueRequestCard(),

                                // Add State Form Card
                                BuildAddStateCard(),

                                // State History Timeline Card
                                BuildStateHistoryCard()
                            }
                        }
                        .Bind(IsVisibleProperty, nameof(RequestStatesViewModel.IsLoading), 
                              convert: (bool loading) => !loading)
                    }
                }
                .Paddings(all: DesignTokens.Layout.PagePadding)
            }
        }
        .Bind(RefreshView.CommandProperty, nameof(RequestStatesViewModel.RefreshCommand))
        .Bind(RefreshView.IsRefreshingProperty, nameof(RequestStatesViewModel.IsLoading));
    }

    private View BuildCurrentStateCard()
    {
        return new CardComponent
        {
            Content = new VerticalStackLayout
            {
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    new SectionHeaderComponent { Title = "Current State" },

                    new Border
                    {
                        Padding = new Thickness(DesignTokens.Spacing.Lg),
                        StrokeShape = new RoundRectangle 
                        { 
                            CornerRadius = new CornerRadius(DesignTokens.BorderRadius.Md) 
                        },
                        StrokeThickness = 2,
                        Content = new Label()
                            .StyleTitle()
                            .Center()
                            .TextColor(DesignTokens.Colors.TextOnPrimary)
                            .Bind(Label.TextProperty, nameof(RequestStatesViewModel.CurrentState))
                    }
                    .Bind(Border.BackgroundColorProperty, "Request.Metadata.CurrentState",
                          convert: (StateType state) => state switch
                          {
                              StateType.Issued => DesignTokens.Colors.Success,
                              StateType.Rejected => DesignTokens.Colors.Error,
                              StateType.InProgress => DesignTokens.Colors.Info,
                              StateType.AtReception => DesignTokens.Colors.Warning,
                              _ => DesignTokens.Colors.Primary
                          })
                    .Bind(Border.StrokeProperty, "Request.Metadata.CurrentState",
                          convert: (StateType state) => new SolidColorBrush(state switch
                          {
                              StateType.Issued => DesignTokens.Colors.Success,
                              StateType.Rejected => DesignTokens.Colors.Error,
                              StateType.InProgress => DesignTokens.Colors.Info,
                              StateType.AtReception => DesignTokens.Colors.Warning,
                              _ => DesignTokens.Colors.Primary
                          }))
                }
            }
        };
    }

    private View BuildIssueRequestCard()
    {
        return new CardComponent
        {
            Content = new VerticalStackLayout
            {
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    new SectionHeaderComponent { Title = "Issue Request" },

                    new Label()
                        .StyleBody()
                        .Text("Issue this request to mark it as completed. The request must be fully paid."),

                    new Button()
                        .Text("✓ Issue Request")
                        .Invoke(btn =>
                        {
                            btn.StylePrimaryButton();
                            btn.BackgroundColor(DesignTokens.Colors.Success);
                        })
                        .Bind(Button.CommandProperty, nameof(RequestStatesViewModel.IssueRequestCommand))
                        .Bind(IsEnabledProperty, nameof(RequestStatesViewModel.CanIssueRequest)),

                    new Label()
                        .StyleCaption()
                        .StyleWarning()
                        .Text("⚠️ Request must be fully paid before issuing")
                        .Bind(IsVisibleProperty, nameof(RequestStatesViewModel.CanIssueRequest),
                              convert: (bool canIssue) => !canIssue)
                }
            }
        }
        .Bind(IsVisibleProperty, "Request.Metadata.CurrentState",
              convert: (StateType state) => state != StateType.Issued);
    }

    private View BuildAddStateCard()
    {
        return new CardComponent
        {
            Content = new VerticalStackLayout
            {
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    new SectionHeaderComponent { Title = "Add State" },

                    // State Type Picker
                    new VerticalStackLayout
                    {
                        Spacing = DesignTokens.Spacing.Xs,
                        Children =
                        {
                            new Label()
                                .StyleLabel()
                                .Text("State Type"),

                            new Picker
                            {
                                ItemsSource = Enum.GetValues<StateType>().ToList(),
                                SelectedIndex = 0
                            }
                            .Bind(Picker.SelectedItemProperty, nameof(RequestStatesViewModel.SelectedStateType))
                        }
                    },

                    // State Date Picker
                    new VerticalStackLayout
                    {
                        Spacing = DesignTokens.Spacing.Xs,
                        Children =
                        {
                            new Label()
                                .StyleLabel()
                                .Text("Date"),

                            new DatePicker()
                                .Bind(DatePicker.DateProperty, nameof(RequestStatesViewModel.StateDate))
                        }
                    },

                    // State Error Message
                    new Label()
                        .StyleCaption()
                        .StyleError()
                        .Bind(Label.TextProperty, nameof(RequestStatesViewModel.StateErrorMessage))
                        .Bind(Label.IsVisibleProperty, nameof(RequestStatesViewModel.StateErrorMessage),
                              convert: (string? msg) => !string.IsNullOrWhiteSpace(msg)),

                    // Add State Button
                    new Button()
                        .Text("Add State")
                        .StylePrimaryButton()
                        .Bind(Button.CommandProperty, nameof(RequestStatesViewModel.AddStateCommand))
                        .Bind(Button.IsEnabledProperty, nameof(RequestStatesViewModel.IsAddingState),
                              convert: (bool isAdding) => !isAdding)
                        .Bind(IsEnabledProperty, nameof(RequestStatesViewModel.CanAddState))
                }
            }
        };
    }

    private View BuildStateHistoryCard()
    {
        return new CardComponent
        {
            Content = new VerticalStackLayout
            {
                Spacing = DesignTokens.Spacing.Md,
                Children =
                {
                    new SectionHeaderComponent { Title = "State History" },

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
                                            new RowDefinition { Height = GridLength.Auto }
                                        },
                                        ColumnDefinitions =
                                        {
                                            new ColumnDefinition { Width = new GridLength(8) },
                                            new ColumnDefinition { Width = GridLength.Star },
                                            new ColumnDefinition { Width = GridLength.Auto }
                                        },
                                        ColumnSpacing = DesignTokens.Spacing.Md,
                                        RowSpacing = DesignTokens.Spacing.Xs,
                                        Children =
                                        {
                                            // Timeline indicator (colored vertical bar)
                                            new BoxView()
                                                .Bind(BoxView.ColorProperty, "State",
                                                      convert: (StateType state) => state switch
                                                      {
                                                          StateType.Issued => DesignTokens.Colors.Success,
                                                          StateType.Rejected => DesignTokens.Colors.Error,
                                                          StateType.InProgress => DesignTokens.Colors.Info,
                                                          StateType.AtReception => DesignTokens.Colors.Warning,
                                                          _ => DesignTokens.Colors.Primary
                                                      })
                                                .Row(0).RowSpan(2).Column(0),

                                            // State Name
                                            new Label()
                                                .StyleBody()
                                                .Font(bold: true)
                                                .Bind(Label.TextProperty, "State")
                                                .Row(0).Column(1),

                                            // Delete Button
                                            new Button()
                                                .Text("Delete")
                                                .StyleOutlineButton()
                                                .Invoke(btn =>
                                                {
                                                    btn.HeightRequest = 32;
                                                    btn.Padding = new Thickness(DesignTokens.Spacing.Xs);
                                                    btn.TextColor(DesignTokens.Colors.Error);
                                                    btn.BorderColor = DesignTokens.Colors.Error;
                                                })
                                                .Bind(Button.CommandProperty, nameof(RequestStatesViewModel.DeleteStateCommand), source: _vm)
                                                .Bind(Button.CommandParameterProperty, ".")
                                                .Bind(IsEnabledProperty, nameof(RequestStatesViewModel.CanDeleteState), source: _vm)
                                                .Row(0).Column(2),

                                            // Date
                                            new Label()
                                                .StyleCaption()
                                                .Bind(Label.TextProperty, "Created", 
                                                      convert: (DateTime dt) => dt.ToString("dd/MM/yyyy HH:mm"))
                                                .Row(1).Column(1).ColumnSpan(2)
                                        }
                                    };
                                })
                        )
                    }
                    .Bind(CollectionView.ItemsSourceProperty, "Request.States")
                    .Bind(CollectionView.IsVisibleProperty, "Request.States", 
                          convert: (object? states) => states != null && states is System.Collections.IList list && list.Count > 0),

                    new Label()
                        .StyleCaption()
                        .Text("No state history")
                        .Center()
                        .Bind(Label.IsVisibleProperty, "Request.States", 
                              convert: (object? states) => states == null || (states is System.Collections.IList list && list.Count == 0))
                }
            }
        };
    }
}
