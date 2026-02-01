using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.DesignSystem.BaseViews;

namespace SIGL_Cadastru.Mobile.Components;

public class InfoRowComponent : BaseContentView
{
    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(InfoRowComponent), string.Empty);

    public static readonly BindableProperty ValueTextProperty =
        BindableProperty.Create(nameof(ValueText), typeof(string), typeof(InfoRowComponent), string.Empty);

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public string ValueText
    {
        get => (string)GetValue(ValueTextProperty);
        set => SetValue(ValueTextProperty, value);
    }

    protected override View BuildContent()
    {
        var labelLabel = new Label { WidthRequest = 120 };
        labelLabel
            .Bind(Label.TextProperty, nameof(LabelText), source: this)
            .StyleLabel();

        return new HorizontalStackLayout
        {
            Spacing = DesignTokens.Spacing.Sm,
            Children =
            {
                labelLabel,

                new Label()
                    .Bind(Label.TextProperty, nameof(ValueText), source: this)
                    .StyleBody()
                    .TextColor(DesignTokens.Colors.TextSecondary)
            }
        };
    }
}
