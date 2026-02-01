using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.DesignSystem.BaseViews;

namespace SIGL_Cadastru.Mobile.Components;

public class FilterCheckboxComponent : BaseContentView
{
    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), 
            typeof(FilterCheckboxComponent), string.Empty);

    public static readonly BindableProperty IsCheckedProperty =
        BindableProperty.Create(nameof(IsChecked), typeof(bool), 
            typeof(FilterCheckboxComponent), false, BindingMode.TwoWay);

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    protected override View BuildContent()
    {
        return new HorizontalStackLayout
        {
            Spacing = DesignTokens.Spacing.Sm,
            Children =
            {
                new CheckBox()
                    .Bind(CheckBox.IsCheckedProperty, nameof(IsChecked), source: this),
                new Label()
                    .Bind(Label.TextProperty, nameof(LabelText), source: this)
                    .StyleBody()
                    .CenterVertical()
            }
        };
    }
}
