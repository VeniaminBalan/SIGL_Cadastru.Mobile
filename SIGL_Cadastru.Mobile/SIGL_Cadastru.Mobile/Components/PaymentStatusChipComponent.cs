using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.DesignSystem.BaseViews;

namespace SIGL_Cadastru.Mobile.Components;

/// <summary>
/// Payment Status Chip Component - Displays payment status with icon
/// </summary>
public class PaymentStatusChipComponent : BaseContentView
{
    public static readonly BindableProperty IsFullyPaidProperty =
        BindableProperty.Create(nameof(IsFullyPaid), typeof(bool), typeof(PaymentStatusChipComponent), false);

    public bool IsFullyPaid
    {
        get => (bool)GetValue(IsFullyPaidProperty);
        set => SetValue(IsFullyPaidProperty, value);
    }

    protected override View BuildContent()
    {
        var label = new Label()
            .FontSize(DesignTokens.Typography.FontSizeXl);

        label.Bind(Label.TextProperty, nameof(IsFullyPaid), source: this,
            convert: (bool isPaid) => isPaid ? "✓" : "⚠");

        label.Bind(Label.TextColorProperty, nameof(IsFullyPaid), source: this,
            convert: (bool isPaid) => isPaid ? DesignTokens.Colors.Success : DesignTokens.Colors.Warning);

        return label;
    }
}
