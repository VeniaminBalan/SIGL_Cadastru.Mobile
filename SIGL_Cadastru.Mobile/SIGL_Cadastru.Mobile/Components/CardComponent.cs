using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.DesignSystem.BaseViews;

namespace SIGL_Cadastru.Mobile.Components;

public class CardComponent : BaseContentView
{
    public static readonly BindableProperty ContentViewProperty =
        BindableProperty.Create(nameof(ContentView), typeof(View), typeof(CardComponent), null);

    public static readonly BindableProperty HasElevationProperty =
        BindableProperty.Create(nameof(HasElevation), typeof(bool), typeof(CardComponent), true);

    public View? ContentView
    {
        get => (View?)GetValue(ContentViewProperty);
        set => SetValue(ContentViewProperty, value);
    }

    public bool HasElevation
    {
        get => (bool)GetValue(HasElevationProperty);
        set => SetValue(HasElevationProperty, value);
    }

    protected override View BuildContent()
    {
        var border = new Border()
            .Bind(Border.ContentProperty, nameof(ContentView), source: this);

        // Apply styling based on elevation
        return HasElevation 
            ? border.StyleCardElevated() 
            : border.StyleCard();
    }
}
