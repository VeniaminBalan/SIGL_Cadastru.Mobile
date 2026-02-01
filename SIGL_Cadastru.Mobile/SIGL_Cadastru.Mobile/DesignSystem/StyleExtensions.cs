using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;

namespace SIGL_Cadastru.Mobile.DesignSystem;

public static class StyleExtensions
{
    // ==================== LABEL STYLES ====================
    
    public static T StyleHeadline<T>(this T label) where T : Label
    {
        return label
            .FontSize(DesignTokens.Typography.FontSize3xl)
            .Font(bold: true)
            .TextColor(DesignTokens.Colors.TextPrimary);
    }

    public static T StyleTitle<T>(this T label) where T : Label
    {
        return label
            .FontSize(DesignTokens.Typography.FontSize2xl)
            .Font(bold: true)
            .TextColor(DesignTokens.Colors.TextPrimary);
    }

    public static T StyleSubtitle<T>(this T label) where T : Label
    {
        return label
            .FontSize(DesignTokens.Typography.FontSizeXl)
            .Font(bold: true)
            .TextColor(DesignTokens.Colors.TextPrimary);
    }

    public static T StyleBody<T>(this T label) where T : Label
    {
        return label
            .FontSize(DesignTokens.Typography.FontSizeMd)
            .TextColor(DesignTokens.Colors.TextPrimary);
    }

    public static T StyleCaption<T>(this T label) where T : Label
    {
        return label
            .FontSize(DesignTokens.Typography.FontSizeSm)
            .TextColor(DesignTokens.Colors.TextSecondary);
    }

    public static T StyleLabel<T>(this T label) where T : Label
    {
        return label
            .FontSize(DesignTokens.Typography.FontSizeSm)
            .Font(bold: true)
            .TextColor(DesignTokens.Colors.TextPrimary);
    }

    // ==================== BUTTON STYLES ====================

    public static T StylePrimaryButton<T>(this T button) where T : Button
    {
        button.HeightRequest = 48;
        button.CornerRadius = (int)DesignTokens.BorderRadius.Md;
        return button
            .BackgroundColor(DesignTokens.Colors.Primary)
            .TextColor(DesignTokens.Colors.TextOnPrimary)
            .FontSize(DesignTokens.Typography.FontSizeMd)
            .Font(bold: true)
            .Padding(DesignTokens.Spacing.Md, DesignTokens.Spacing.Sm);
    }

    public static T StyleSecondaryButton<T>(this T button) where T : Button
    {
        button.HeightRequest = 48;
        button.CornerRadius = (int)DesignTokens.BorderRadius.Md;
        return button
            .BackgroundColor(DesignTokens.Colors.Secondary)
            .TextColor(DesignTokens.Colors.TextOnPrimary)
            .FontSize(DesignTokens.Typography.FontSizeMd)
            .Font(bold: true)
            .Padding(DesignTokens.Spacing.Md, DesignTokens.Spacing.Sm);
    }

    public static T StyleOutlineButton<T>(this T button) where T : Button
    {
        button.HeightRequest = 48;
        button.BorderColor = DesignTokens.Colors.Primary;
        button.BorderWidth = 2;
        button.CornerRadius = (int)DesignTokens.BorderRadius.Md;
        return button
            .BackgroundColor(Microsoft.Maui.Graphics.Colors.Transparent)
            .TextColor(DesignTokens.Colors.Primary)
            .FontSize(DesignTokens.Typography.FontSizeMd)
            .Font(bold: true)
            .Padding(DesignTokens.Spacing.Md, DesignTokens.Spacing.Sm);
    }

    public static T StyleTextButton<T>(this T button) where T : Button
    {
        return button
            .BackgroundColor(Microsoft.Maui.Graphics.Colors.Transparent)
            .TextColor(DesignTokens.Colors.Primary)
            .FontSize(DesignTokens.Typography.FontSizeMd)
            .Font(bold: true);
    }

    // ==================== ENTRY STYLES ====================

    public static T StyleFormEntry<T>(this T entry) where T : Entry
    {
        entry.HeightRequest = 48;
        return entry
            .FontSize(DesignTokens.Typography.FontSizeMd)
            .TextColor(DesignTokens.Colors.TextPrimary);
    }

    // ==================== BORDER/CARD STYLES ====================

    public static T StyleCard<T>(this T border) where T : Border
    {
        border.Stroke = DesignTokens.Colors.Border;
        border.StrokeThickness = 1;
        border.StrokeShape = new RoundRectangle 
        { 
            CornerRadius = new CornerRadius(DesignTokens.BorderRadius.Md) 
        };
        return border
            .BackgroundColor(DesignTokens.Colors.Background)
            .Padding(DesignTokens.Spacing.Lg);
    }

    public static T StyleCardElevated<T>(this T border) where T : Border
    {
        border.StyleCard();
        border.Shadow = DesignTokens.Shadows.Medium;
        return border;
    }

    public static T StyleListItem<T>(this T border) where T : Border
    {
        border.Stroke = DesignTokens.Colors.Border;
        border.StrokeThickness = 1;
        border.StrokeShape = new RoundRectangle 
        { 
            CornerRadius = new CornerRadius(DesignTokens.BorderRadius.Sm) 
        };
        return border
            .BackgroundColor(DesignTokens.Colors.Background)
            .Padding(DesignTokens.Spacing.Md)
            .Margins(bottom: DesignTokens.Spacing.Sm);
    }

    // ==================== SEMANTIC STYLES ====================

    public static T StyleSuccess<T>(this T view) where T : View
    {
        if (view is Label label)
            return (T)(object)label.TextColor(DesignTokens.Colors.Success);
        if (view is Button button)
            return (T)(object)button.BackgroundColor(DesignTokens.Colors.Success);
        
        return view.BackgroundColor(DesignTokens.Colors.Success);
    }

    public static T StyleWarning<T>(this T view) where T : View
    {
        if (view is Label label)
            return (T)(object)label.TextColor(DesignTokens.Colors.Warning);
        if (view is Button button)
            return (T)(object)button.BackgroundColor(DesignTokens.Colors.Warning);
        
        return view.BackgroundColor(DesignTokens.Colors.Warning);
    }

    public static T StyleError<T>(this T view) where T : View
    {
        if (view is Label label)
            return (T)(object)label.TextColor(DesignTokens.Colors.Error);
        if (view is Button button)
            return (T)(object)button.BackgroundColor(DesignTokens.Colors.Error);
        
        return view.BackgroundColor(DesignTokens.Colors.Error);
    }

    // ==================== SPACING HELPERS ====================

    public static T Margins<T>(this T view, 
        double? left = null, 
        double? top = null, 
        double? right = null, 
        double? bottom = null,
        double? horizontal = null,
        double? vertical = null,
        double? all = null) where T : View
    {
        if (all.HasValue)
            return view.Margin(all.Value);
        
        var l = left ?? horizontal ?? 0;
        var t = top ?? vertical ?? 0;
        var r = right ?? horizontal ?? 0;
        var b = bottom ?? vertical ?? 0;

        return view.Margin(new Thickness(l, t, r, b));
    }

    public static T Paddings<T>(this T layout,
        double? left = null,
        double? top = null,
        double? right = null,
        double? bottom = null,
        double? horizontal = null,
        double? vertical = null,
        double? all = null) where T : Layout
    {
        if (all.HasValue)
            return layout.Padding(all.Value);

        var l = left ?? horizontal ?? 0;
        var t = top ?? vertical ?? 0;
        var r = right ?? horizontal ?? 0;
        var b = bottom ?? vertical ?? 0;

        return layout.Padding(new Thickness(l, t, r, b));
    }
}
