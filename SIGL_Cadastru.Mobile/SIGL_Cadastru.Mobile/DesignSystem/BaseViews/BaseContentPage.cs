using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SIGL_Cadastru.Mobile.DesignSystem.BaseViews;

/// <summary>
/// Base class for all pages with common loading/error UI patterns
/// </summary>
public abstract class BaseContentPage<TViewModel> : ContentPage 
    where TViewModel : ObservableObject
{
    protected TViewModel ViewModel { get; }

    protected BaseContentPage(TViewModel viewModel)
    {
        ViewModel = viewModel;
        BindingContext = viewModel;
        
        Content = BuildPageContent();
    }

    /// <summary>
    /// Override to build the main page content
    /// </summary>
    protected abstract View BuildPageContent();

    /// <summary>
    /// Standard loading indicator that can be reused
    /// </summary>
    protected ActivityIndicator BuildLoadingIndicator(string loadingPropertyName = "IsLoading")
    {
        return new ActivityIndicator()
            .Center()
            .Bind(ActivityIndicator.IsRunningProperty, loadingPropertyName)
            .Bind(ActivityIndicator.IsVisibleProperty, loadingPropertyName);
    }

    /// <summary>
    /// Standard error message display
    /// </summary>
    protected Label BuildErrorLabel(string errorPropertyName = "ErrorMessage")
    {
        return new Label()
            .StyleError()
            .FontSize(DesignTokens.Typography.FontSizeSm)
            .Margins(top: DesignTokens.Spacing.Sm)
            .Bind(Label.IsVisibleProperty, errorPropertyName,
                convert: (string? msg) => !string.IsNullOrWhiteSpace(msg))
            .Bind(Label.TextProperty, errorPropertyName);
    }

    /// <summary>
    /// Standard page padding wrapper
    /// </summary>
    protected VerticalStackLayout BuildPageLayout(params View[] children)
    {
        var layout = new VerticalStackLayout
        {
            Padding = DesignTokens.Layout.PagePadding,
            Spacing = DesignTokens.Layout.ComponentSpacing
        };

        foreach (var child in children)
        {
            layout.Children.Add(child);
        }

        return layout;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        OnPageAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        OnPageDisappearing();
    }

    /// <summary>
    /// Override for page appearing logic
    /// </summary>
    protected virtual void OnPageAppearing() { }

    /// <summary>
    /// Override for page disappearing logic
    /// </summary>
    protected virtual void OnPageDisappearing() { }
}
