using CommunityToolkit.Maui.Markup;
using SIGL_Cadastru.Mobile.DesignSystem;
using SIGL_Cadastru.Mobile.DesignSystem.BaseViews;
using SIGL_Cadastru.Mobile.ViewModels;
using System.Collections.Specialized;
using System.Windows.Input;

namespace SIGL_Cadastru.Mobile.Components;

public class RequestsListComponent : BaseContentView
{
    private CollectionView? _collectionView;
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(RequestsViewModel), typeof(RequestsListComponent));

    public static readonly BindableProperty NavigateCommandProperty =
        BindableProperty.Create(nameof(NavigateCommand), typeof(ICommand), typeof(RequestsListComponent));

    public RequestsViewModel? ViewModel
    {
        get => (RequestsViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public ICommand? NavigateCommand
    {
        get => (ICommand?)GetValue(NavigateCommandProperty);
        set => SetValue(NavigateCommandProperty, value);
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        
        if (propertyName == nameof(ViewModel))
        {
            if (ViewModel?.Requests is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged += OnRequestsCollectionChanged;
            }
        }
    }

    private void OnRequestsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // When collection is reset (cleared and reloaded) or first item added after clear, scroll to top
        if ((e.Action == NotifyCollectionChangedAction.Reset || 
             (e.Action == NotifyCollectionChangedAction.Add && e.NewStartingIndex == 0 && ViewModel?.Requests?.Count == 1)) 
            && _collectionView != null)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // Small delay to ensure UI has updated
                await Task.Delay(50);
                if (ViewModel?.Requests?.Count > 0)
                {
                    _collectionView?.ScrollTo(0, position: ScrollToPosition.Start, animate: false);
                }
            });
        }
    }

    protected override View BuildContent()
    {
        _collectionView = new CollectionView
            {
                ItemTemplate = new DataTemplate(() => 
                    new RequestCardComponent()
                        .Bind(RequestCardComponent.RequestProperty, ".")
                        .Bind(RequestCardComponent.TapCommandProperty, 
                            nameof(NavigateCommand), 
                            source: this)
                ),
                Margin = new Thickness(0),
                RemainingItemsThreshold = 5,
                SelectionMode = SelectionMode.None,
                Footer = new ActivityIndicator()
                    .Center()
                    .Margins(all: DesignTokens.Spacing.Sm)
                    .Bind(ActivityIndicator.IsRunningProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.IsLoadingMore)}", source: this)
                    .Bind(ActivityIndicator.IsVisibleProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.IsLoadingMore)}", source: this)
            }
            .Bind(CollectionView.ItemsSourceProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.Requests)}", source: this)
            .Bind(CollectionView.RemainingItemsThresholdReachedCommandProperty, 
                $"{nameof(ViewModel)}.{nameof(RequestsViewModel.LoadMoreRequestsCommand)}", source: this);
        
        return new RefreshView
        {
            Content = _collectionView
        }
        .Bind(RefreshView.CommandProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.LoadRequestsCommand)}", source: this)
        .Bind(RefreshView.IsRefreshingProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.IsLoading)}", source: this)
        .Bind(IsVisibleProperty, $"{nameof(ViewModel)}.{nameof(RequestsViewModel.IsLoading)}", 
            source: this,
            convert: (bool loading) => !loading);
    }
}
