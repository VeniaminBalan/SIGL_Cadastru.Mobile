using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.DesignSystem.BaseViews;
using SIGL_Cadastru.Mobile.ViewModels;
using System.Windows.Input;

namespace SIGL_Cadastru.Mobile.Components;

public class RequestFilterComponent : BaseContentView
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(RequestsViewModel), typeof(RequestFilterComponent));

    public static readonly BindableProperty RemoveFilterCommandProperty =
        BindableProperty.Create(nameof(RemoveFilterCommand), typeof(ICommand), typeof(RequestFilterComponent));

    public RequestsViewModel? ViewModel
    {
        get => (RequestsViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public ICommand? RemoveFilterCommand
    {
        get => (ICommand?)GetValue(RemoveFilterCommandProperty);
        set => SetValue(RemoveFilterCommandProperty, value);
    }

    protected override View BuildContent()
    {
        return BuildModernFilterUi();
    }

    private View BuildModernFilterUi()
    {
        return new VerticalStackLayout
        {
            Spacing = DesignTokens.Spacing.Md,
            Children =
            {
                BuildSearchBar(),
                BuildActiveFiltersRow(),
                BuildFilterExpanderSection(),
                BuildSortSection()
            }
        };
    }

    private View BuildSearchBar()
    {
        return new Frame
        {
            Padding = 0,
            CornerRadius = (float)DesignTokens.BorderRadius.Lg,
            BorderColor = DesignTokens.Colors.Border,
            HasShadow = false,
            BackgroundColor = DesignTokens.Colors.Background,
            Content = new HorizontalStackLayout
            {
                Spacing = DesignTokens.Spacing.Sm,
                Padding = new Thickness(DesignTokens.Spacing.Md, DesignTokens.Spacing.Sm),
                Children =
                {
                    new Label
                    {
                        Text = "🔍",
                        FontSize = DesignTokens.Typography.FontSizeLg,
                        VerticalOptions = LayoutOptions.Center
                    },
                    new Entry
                    {
                        Placeholder = "Search by number, client name...",
                        BackgroundColor = Colors.Transparent,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Center
                    }
                    .Bind(Entry.TextProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.SearchText)}", source: this)
                }
            }
        };
    }

    private View BuildActiveFiltersRow()
    {
        var scrollView = new ScrollView
        {
            Orientation = ScrollOrientation.Horizontal,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
            Content = new HorizontalStackLayout
            {
                Spacing = DesignTokens.Spacing.Xs
            }
        };

        // This will be dynamically populated based on active filters
        scrollView
            .Bind(ScrollView.IsVisibleProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.HasActiveFilters)}", source: this);

        return scrollView;
    }

    private View BuildFilterExpanderSection()
    {
        var contentGrid = new Grid
        {
            Padding = DesignTokens.Spacing.Md,
            RowSpacing = DesignTokens.Spacing.Md,
            ColumnSpacing = DesignTokens.Spacing.Sm,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            },
            Children =
            {
                BuildSectionHeader("State Filters")
                    .Row(0).ColumnSpan(2),
                
                new FilterCheckboxComponent { LabelText = "At Reception" }
                    .Bind(FilterCheckboxComponent.IsCheckedProperty, 
                        $"{nameof(ViewModel)}.{nameof(RequestsViewModel.FilterAtReception)}", source: this)
                    .Row(1).Column(0),
                
                new FilterCheckboxComponent { LabelText = "In Progress" }
                    .Bind(FilterCheckboxComponent.IsCheckedProperty, 
                        $"{nameof(ViewModel)}.{nameof(RequestsViewModel.FilterInProgress)}", source: this)
                    .Row(1).Column(1),
                
                new FilterCheckboxComponent { LabelText = "Issued" }
                    .Bind(FilterCheckboxComponent.IsCheckedProperty, 
                        $"{nameof(ViewModel)}.{nameof(RequestsViewModel.FilterIssued)}", source: this)
                    .Row(2).Column(0),
                
                new FilterCheckboxComponent { LabelText = "Rejected" }
                    .Bind(FilterCheckboxComponent.IsCheckedProperty, 
                        $"{nameof(ViewModel)}.{nameof(RequestsViewModel.FilterRejected)}", source: this)
                    .Row(2).Column(1)
            }
        };

        var paymentGrid = new Grid
        {
            Padding = new Thickness(DesignTokens.Spacing.Md, 0, DesignTokens.Spacing.Md, DesignTokens.Spacing.Md),
            RowSpacing = DesignTokens.Spacing.Md,
            ColumnSpacing = DesignTokens.Spacing.Sm,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            },
            Children =
            {
                BuildSectionHeader("Payment Status")
                    .Row(0).ColumnSpan(2),
                
                new FilterCheckboxComponent { LabelText = "Fully Paid" }
                    .Bind(FilterCheckboxComponent.IsCheckedProperty, 
                        $"{nameof(ViewModel)}.{nameof(RequestsViewModel.FilterFullyPaid)}", source: this)
                    .Row(1).Column(0),
                
                new FilterCheckboxComponent { LabelText = "Unpaid/Partial" }
                    .Bind(FilterCheckboxComponent.IsCheckedProperty, 
                        $"{nameof(ViewModel)}.{nameof(RequestsViewModel.FilterUnpaid)}", source: this)
                    .Row(1).Column(1)
            }
        };

        var actionButtons = new HorizontalStackLayout
        {
            Padding = new Thickness(DesignTokens.Spacing.Md, 0, DesignTokens.Spacing.Md, DesignTokens.Spacing.Md),
            Spacing = DesignTokens.Spacing.Sm,
            Children =
            {
                BuildApplyFiltersButton(),
                BuildClearFiltersButton()
            }
        };

        var filterContent = new VerticalStackLayout
        {
            Spacing = 0,
            Children = { contentGrid, paymentGrid, actionButtons }
        };

        var expander = new Border
        {
            StrokeThickness = 1,
            Stroke = DesignTokens.Colors.Border,
            BackgroundColor = DesignTokens.Colors.Background,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = DesignTokens.BorderRadius.Lg },
            Content = new VerticalStackLayout
            {
                Spacing = 0,
                Children =
                {
                    BuildExpanderHeader(),
                    filterContent
                }
            }
        };

        filterContent
            .Bind(VerticalStackLayout.IsVisibleProperty, 
                $"{nameof(ViewModel)}.{nameof(RequestsViewModel.IsFilterExpanded)}", source: this);

        return expander;
    }

    private View BuildExpanderHeader()
    {
        var label = new Label
        {
            Text = "Filters",
            FontSize = DesignTokens.Typography.FontSizeMd,
            FontAttributes = DesignTokens.Typography.Bold,
            TextColor = DesignTokens.Colors.TextPrimary,
            VerticalOptions = LayoutOptions.Center
        };

        var chevron = new Label
        {
            FontSize = DesignTokens.Typography.FontSizeLg,
            VerticalOptions = LayoutOptions.Center
        };
        chevron.Bind(Label.TextProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.IsFilterExpanded)}", 
            source: this,
            convert: (bool isExpanded) => isExpanded ? "▲" : "▼");

        var header = new HorizontalStackLayout
        {
            Padding = DesignTokens.Spacing.Md,
            Spacing = DesignTokens.Spacing.Sm,
            Children = { label, new BoxView { HorizontalOptions = LayoutOptions.Fill }, chevron }
        };

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Bind(TapGestureRecognizer.CommandProperty, 
            $"{nameof(ViewModel)}.{nameof(RequestsViewModel.ToggleFilterExpandedCommand)}", source: this);
        header.GestureRecognizers.Add(tapGesture);

        return header;
    }

    private View BuildSectionHeader(string title)
    {
        return new Label
        {
            Text = title,
            FontSize = DesignTokens.Typography.FontSizeSm,
            FontAttributes = DesignTokens.Typography.Bold,
            TextColor = DesignTokens.Colors.TextSecondary,
            Margin = new Thickness(0, DesignTokens.Spacing.Xs, 0, 0)
        };
    }

    private Button BuildApplyFiltersButton()
    {
        var button = new Button
        {
            // Remove obsolete FillAndExpand, use default or set HorizontalOptions = LayoutOptions.Fill
            HorizontalOptions = LayoutOptions.Fill
        };
        button
            .Text("Apply Filters")
            .StylePrimaryButton()
            .Bind(Button.CommandProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.ApplyFiltersCommand)}", source: this);
        return button;
    }

    private Button BuildClearFiltersButton()
    {
        var button = new Button
        {
            // Remove obsolete FillAndExpand, use default or set HorizontalOptions = LayoutOptions.Fill
            HorizontalOptions = LayoutOptions.Fill
        };
        button
            .Text("Clear All")
            .StyleOutlineButton()
            .Bind(Button.CommandProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.ClearAllFiltersCommand)}", source: this);
        return button;
    }

    private View BuildSortSection()
    {
        return new Border
        {
            StrokeThickness = 1,
            Stroke = DesignTokens.Colors.Border,
            BackgroundColor = DesignTokens.Colors.Surface,
            StrokeShape = new RoundRectangle { CornerRadius = DesignTokens.BorderRadius.Lg },
            Padding = DesignTokens.Spacing.Md,
            Content = new Grid
            {
                ColumnSpacing = DesignTokens.Spacing.Sm,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                Children =
                {
                    new Label
                    {
                        Text = "Sort:",
                        FontSize = DesignTokens.Typography.FontSizeSm,
                        FontAttributes = DesignTokens.Typography.Bold,
                        VerticalOptions = LayoutOptions.Center,
                        TextColor = DesignTokens.Colors.TextSecondary
                    }.Column(0),

                    new Picker
                    {
                        Title = "Field",
                        FontSize = DesignTokens.Typography.FontSizeSm,
                        ItemsSource = new List<string> { "AvailableFrom", "Number", "CurrentState" }
                    }
                    .Bind(Picker.SelectedItemProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.OrderBy)}", source: this)
                    .Column(1),

                    new Picker
                    {
                        Title = "Dir",
                        FontSize = DesignTokens.Typography.FontSizeSm,
                        ItemsSource = new List<string> { "asc", "desc" }
                    }
                    .Bind(Picker.SelectedItemProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.Direction)}", source: this)
                    .Column(2)
                }
            }
        };
    }
}
