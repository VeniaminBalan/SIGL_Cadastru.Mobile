using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.DesignSystem.BaseViews;

namespace SIGL_Cadastru.Mobile.Components;

public class SectionHeaderComponent : BaseContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(SectionHeaderComponent), string.Empty);

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(SectionHeaderComponent), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    protected override View BuildContent()
    {
        return new VerticalStackLayout
        {
            Spacing = DesignTokens.Spacing.Xs,
            Children =
            {
                new Label()
                    .Bind(Label.TextProperty, nameof(Title), source: this)
                    .StyleSubtitle(),

                new Label()
                    .Bind(Label.TextProperty, nameof(Subtitle), source: this)
                    .Bind(Label.IsVisibleProperty, nameof(Subtitle), source: this,
                        convert: (string? s) => !string.IsNullOrWhiteSpace(s))
                    .StyleCaption()
            }
        };
    }
}
