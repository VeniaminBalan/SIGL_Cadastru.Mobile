using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.DesignSystem.BaseViews;
using SIGL_Cadastru.Mobile.ViewModels;
using System.Windows.Input;

namespace SIGL_Cadastru.Mobile.Components;

public class PaymentFilterModalContent : BaseContentView
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(RequestsViewModel), typeof(PaymentFilterModalContent));

    public static readonly BindableProperty ApplyCommandProperty =
        BindableProperty.Create(nameof(ApplyCommand), typeof(ICommand), typeof(PaymentFilterModalContent));

    public static readonly BindableProperty ClearCommandProperty =
        BindableProperty.Create(nameof(ClearCommand), typeof(ICommand), typeof(PaymentFilterModalContent));

    public RequestsViewModel? ViewModel
    {
        get => (RequestsViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public ICommand? ApplyCommand
    {
        get => (ICommand?)GetValue(ApplyCommandProperty);
        set => SetValue(ApplyCommandProperty, value);
    }

    public ICommand? ClearCommand
    {
        get => (ICommand?)GetValue(ClearCommandProperty);
        set => SetValue(ClearCommandProperty, value);
    }

    protected override View BuildContent()
    {
        return new VerticalStackLayout
        {
            Padding = DesignTokens.Layout.PagePadding,
            Spacing = DesignTokens.Spacing.Md,
            Children =
            {
                new FilterCheckboxComponent
                {
                    LabelText = "Fully Paid"
                }
                .Bind(FilterCheckboxComponent.IsCheckedProperty, 
                    $"{nameof(ViewModel)}.{nameof(RequestsViewModel.FilterFullyPaid)}", source: this),
                
                new FilterCheckboxComponent
                {
                    LabelText = "Unpaid/Partially Paid"
                }
                .Bind(FilterCheckboxComponent.IsCheckedProperty, 
                    $"{nameof(ViewModel)}.{nameof(RequestsViewModel.FilterUnpaid)}", source: this),

                BuildApplyButton(),
                BuildClearButton()
            }
        };
    }

    private Button BuildApplyButton()
    {
        var button = new Button { Margin = new Thickness(0, DesignTokens.Spacing.Xl, 0, 0) };
        button
            .Text("Apply Filter")
            .StylePrimaryButton()
            .Bind(Button.CommandProperty, nameof(ApplyCommand), source: this);
        return button;
    }

    private Button BuildClearButton()
    {
        var button = new Button();
        button
            .Text("Clear Filter")
            .StyleTextButton()
            .Bind(Button.CommandProperty, nameof(ClearCommand), source: this);
        return button;
    }
}
