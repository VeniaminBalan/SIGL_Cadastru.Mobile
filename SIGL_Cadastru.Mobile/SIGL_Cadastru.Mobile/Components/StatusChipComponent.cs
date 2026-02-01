using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.DesignSystem.BaseViews;
using SIGL_Cadastru.Mobile.Models.Shared;

namespace SIGL_Cadastru.Mobile.Components;

/// <summary>
/// Status Chip Component - Displays a state with color-coded background
/// </summary>
public class StatusChipComponent : BaseContentView
{
    public static readonly BindableProperty StateProperty =
        BindableProperty.Create(nameof(State), typeof(StateType), typeof(StatusChipComponent), StateType.InProgress);

    public StateType State
    {
        get => (StateType)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    protected override View BuildContent()
    {
        var border = new Border
        {
            Padding = new Thickness(DesignTokens.Spacing.Sm, DesignTokens.Spacing.Xs),
            StrokeShape = new RoundRectangle 
            { 
                CornerRadius = new CornerRadius(DesignTokens.BorderRadius.Full) 
            },
            Content = new Label()
                .StyleCaption()
                .Font(bold: true)
                .TextColor(DesignTokens.Colors.TextOnPrimary)
                .Bind(Label.TextProperty, nameof(State), source: this)
        };

        border.Bind(Border.BackgroundColorProperty, nameof(State), source: this,
            convert: (StateType state) => state switch
            {
                StateType.Issued => DesignTokens.Colors.Success,
                StateType.Rejected => DesignTokens.Colors.Error,
                StateType.InProgress => DesignTokens.Colors.Info,
                StateType.AtReception => DesignTokens.Colors.Warning,
                _ => DesignTokens.Colors.Primary
            });

        return border;
    }
}
