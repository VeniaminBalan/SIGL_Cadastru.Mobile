using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.DesignSystem.BaseViews;

namespace SIGL_Cadastru.Mobile.Components;

/// <summary>
/// Info Field Component - Displays a label and value in a vertical stack
/// </summary>
public class InfoFieldComponent : BaseContentView
{
    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(InfoFieldComponent), string.Empty);

    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(string), typeof(InfoFieldComponent), string.Empty);

    public static readonly BindableProperty UseBodyStyleProperty =
        BindableProperty.Create(nameof(UseBodyStyle), typeof(bool), typeof(InfoFieldComponent), true);

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public bool UseBodyStyle
    {
        get => (bool)GetValue(UseBodyStyleProperty);
        set => SetValue(UseBodyStyleProperty, value);
    }

    protected override View BuildContent()
    {
        var valueLabel = new Microsoft.Maui.Controls.Label();
        
        valueLabel.Bind(Microsoft.Maui.Controls.Label.TextProperty, nameof(Value), source: this);

        if (UseBodyStyle)
            valueLabel.StyleBody();
        else
            valueLabel.StyleCaption();

        return new VerticalStackLayout
        {
            Spacing = DesignTokens.Spacing.Xs,
            Children =
            {
                new Microsoft.Maui.Controls.Label()
                    .StyleLabel()
                    .Bind(Microsoft.Maui.Controls.Label.TextProperty, nameof(Label), source: this),
                valueLabel
            }
        };
    }
}
