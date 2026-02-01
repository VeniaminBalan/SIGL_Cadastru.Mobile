using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.DesignSystem.BaseViews;
using SIGL_Cadastru.Mobile.ViewModels;
using System.Windows.Input;

namespace SIGL_Cadastru.Mobile.Components;

public class RequestFilterComponent : BaseContentView
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(RequestsViewModel), typeof(RequestFilterComponent));

    public static readonly BindableProperty StateFilterClickedProperty =
        BindableProperty.Create(nameof(StateFilterClicked), typeof(ICommand), typeof(RequestFilterComponent));

    public static readonly BindableProperty PaymentFilterClickedProperty =
        BindableProperty.Create(nameof(PaymentFilterClicked), typeof(ICommand), typeof(RequestFilterComponent));

    public RequestsViewModel? ViewModel
    {
        get => (RequestsViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public ICommand? StateFilterClicked
    {
        get => (ICommand?)GetValue(StateFilterClickedProperty);
        set => SetValue(StateFilterClickedProperty, value);
    }

    public ICommand? PaymentFilterClicked
    {
        get => (ICommand?)GetValue(PaymentFilterClickedProperty);
        set => SetValue(PaymentFilterClickedProperty, value);
    }

    protected override View BuildContent()
    {
        return BuildFilterControls();
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
                .Bind(Entry.TextProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.SearchText)}", source: this)
                .Bind(Entry.ReturnCommandProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.LoadRequestsCommand)}", source: this),

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
                        .Bind(Picker.SelectedItemProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.OrderBy)}", source: this)
                        .Column(0),

                        new Picker
                        {
                            Title = "Order",
                            ItemsSource = new List<string> { "asc", "desc" },
                        }
                        .Bind(Picker.SelectedItemProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.Direction)}", source: this)
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
            .Bind(Button.TextProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.FilterBy)}", 
                  source: this,
                  convert: (string? filter) => string.IsNullOrWhiteSpace(filter) ? 
                      "State Filter" : $"State: {filter}");
        button
            .Bind(Button.CommandProperty, nameof(StateFilterClicked), source: this);
        return button;
    }

    private Button BuildPaymentFilterButton()
    {
        var button = new Button();
        button.StyleOutlineButton();
        button
            .Bind(Button.TextProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.FilterFullyPaid)}", 
                  source: this,
                  convert: (bool fullyPaid) =>
                  {
                      var vm = ViewModel;
                      if (vm == null) return "Payment";
                      if (fullyPaid && !vm.FilterUnpaid) return "Pay: Paid";
                      if (vm.FilterUnpaid && !fullyPaid) return "Pay: Unpaid";
                      return "Payment";
                  });
        button
            .Bind(Button.CommandProperty, nameof(PaymentFilterClicked), source: this);
        return button;
    }

    private Button BuildSearchButton()
    {
        var button = new Button { HorizontalOptions = LayoutOptions.Fill };
        button
            .Text("Search")
            .StylePrimaryButton()
            .Bind(Button.CommandProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.LoadRequestsCommand)}", source: this);
        return button;
    }
}
