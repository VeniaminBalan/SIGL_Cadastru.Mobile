using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.DesignSystem.BaseViews;
using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;

namespace SIGL_Cadastru.Mobile.Components;

public class FilterChipComponent : BaseContentView
{
    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(FilterChipComponent), string.Empty);

    public static readonly BindableProperty RemoveCommandProperty =
        BindableProperty.Create(nameof(RemoveCommand), typeof(ICommand), typeof(FilterChipComponent));

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public ICommand? RemoveCommand
    {
        get => (ICommand?)GetValue(RemoveCommandProperty);
        set => SetValue(RemoveCommandProperty, value);
    }

    protected override View BuildContent()
    {
        var closeLabel = new Label
        {
            Text = "✕",
            TextColor = DesignTokens.Colors.TextOnPrimary,
            FontSize = DesignTokens.Typography.FontSizeSm,
            FontAttributes = DesignTokens.Typography.Bold,
            VerticalOptions = LayoutOptions.Center
        };
        
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Bind(TapGestureRecognizer.CommandProperty, nameof(RemoveCommand), source: this);
        closeLabel.GestureRecognizers.Add(tapGesture);

        return new Border
        {
            Padding = new Thickness(DesignTokens.Spacing.Sm, DesignTokens.Spacing.Xs, DesignTokens.Spacing.Xs, DesignTokens.Spacing.Xs),
            StrokeThickness = 1,
            Stroke = DesignTokens.Colors.Primary,
            BackgroundColor = DesignTokens.Colors.PrimaryLight,
            StrokeShape = new RoundRectangle { CornerRadius = DesignTokens.BorderRadius.Full },
            Content = new HorizontalStackLayout
            {
                Spacing = DesignTokens.Spacing.Xs,
                Children =
                {
                    new Label
                    {
                        TextColor = DesignTokens.Colors.TextOnPrimary,
                        FontSize = DesignTokens.Typography.FontSizeXs,
                        FontAttributes = DesignTokens.Typography.Bold,
                        VerticalOptions = LayoutOptions.Center
                    }
                    .Bind(Microsoft.Maui.Controls.Label.TextProperty, nameof(Label), source: this),

                    closeLabel
                }
            }
        };
    }
}
