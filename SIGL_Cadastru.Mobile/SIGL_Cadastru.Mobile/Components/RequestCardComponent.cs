using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.DesignSystem.BaseViews;
using SIGL_Cadastru.Mobile.Models.Requests;
using System.Windows.Input;

namespace SIGL_Cadastru.Mobile.Components;

public class RequestCardComponent : BaseContentView
{
    public static readonly BindableProperty RequestProperty =
        BindableProperty.Create(nameof(Request), typeof(CadastralRequestDto), 
            typeof(RequestCardComponent), null);

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), 
            typeof(RequestCardComponent), null);

    public CadastralRequestDto? Request
    {
        get => (CadastralRequestDto?)GetValue(RequestProperty);
        set => SetValue(RequestProperty, value);
    }

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    protected override View BuildContent()
    {
        var border = new Border
        {
            StrokeShape = new RoundRectangle 
            { 
                CornerRadius = new CornerRadius(DesignTokens.BorderRadius.Md) 
            }
        };
        
        border.Stroke = DesignTokens.Colors.Border;
        border.StrokeThickness = 1;
        
        border
            .BackgroundColor(DesignTokens.Colors.Background)
            .Padding(DesignTokens.Spacing.Md)
            .Margins(bottom: DesignTokens.Spacing.Sm);
        
        border.Content = new VerticalStackLayout
        {
            Spacing = DesignTokens.Spacing.Xs,
            Children =
            {
                BuildHeaderRow(),
                BuildCadastralNumber(),
                BuildClientName(),
                BuildFooterRow()
            }
        };

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) =>
        {
            if (TapCommand?.CanExecute(Request) == true)
            {
                await border.ScaleToAsync(0.95, 80, Easing.CubicOut);
                await border.ScaleToAsync(1.0, 80, Easing.CubicIn);
                TapCommand.Execute(Request);
            }
        };
        border.GestureRecognizers.Add(tapGesture);

        return border;
    }

    private View BuildHeaderRow()
    {
        return new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            Children =
            {
                new Label()
                    .StyleSubtitle()
                    .Bind(Label.TextProperty, $"{nameof(Request)}.{nameof(CadastralRequestDto.Number)}", 
                        source: this,
                        convert: (string? num) => $"Request: {num ?? "N/A"}")
                    .Column(0),

                new Label()
                    .FontSize(DesignTokens.Typography.FontSizeXl)
                    .Bind(Label.TextProperty, $"{nameof(Request)}.{nameof(CadastralRequestDto.IsFullyPaid)}", 
                        source: this,
                        convert: (bool isPaid) => isPaid ? "✓" : "⚠")
                    .Bind(Label.TextColorProperty, $"{nameof(Request)}.{nameof(CadastralRequestDto.IsFullyPaid)}", 
                        source: this,
                        convert: (bool isPaid) => isPaid ? 
                            DesignTokens.Colors.Success : DesignTokens.Colors.Warning)
                    .Column(1)
            }
        };
    }

    private View BuildCadastralNumber()
    {
        return new Label()
            .StyleCaption()
            .Bind(Label.TextProperty, $"{nameof(Request)}.{nameof(CadastralRequestDto.CadastralNumber)}", 
                source: this,
                convert: (string? num) => $"Cadastral #: {num ?? "N/A"}");
    }

    private View BuildClientName()
    {
        return new Label()
            .StyleBody()
            .Bind(Label.TextProperty, $"{nameof(Request)}.{nameof(CadastralRequestDto.Client)}", 
                source: this,
                convert: (string? c) => $"Client: {c ?? "N/A"}");
    }

    private View BuildFooterRow()
    {
        return new HorizontalStackLayout
        {
            Spacing = DesignTokens.Spacing.Sm,
            Children =
            {
                new Label()
                    .FontSize(DesignTokens.Typography.FontSizeXs)
                    .Bind(Label.TextProperty, $"{nameof(Request)}.{nameof(CadastralRequestDto.CurrentState)}", 
                        source: this,
                        convert: (Models.Shared.StateType s) => s.ToString()),

                new Label()
                    .FontSize(DesignTokens.Typography.FontSizeXs)
                    .TextColor(DesignTokens.Colors.TextSecondary)
                    .Bind(Label.TextProperty, $"{nameof(Request)}.{nameof(CadastralRequestDto.AvailableFrom)}", 
                        source: this,
                        convert: (DateTime d) => d.ToString("dd/MM/yyyy"))
            }
        };
    }
}
