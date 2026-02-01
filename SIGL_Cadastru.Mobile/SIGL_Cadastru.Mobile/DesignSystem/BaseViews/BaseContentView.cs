using CommunityToolkit.Maui.Markup;

namespace SIGL_Cadastru.Mobile.DesignSystem.BaseViews;

/// <summary>
/// Base class for all content views with common functionality
/// </summary>
public abstract class BaseContentView : ContentView
{
    protected BaseContentView()
    {
        Content = BuildContent();
    }

    /// <summary>
    /// Override this method to build the view's content
    /// </summary>
    protected abstract View BuildContent();
}
