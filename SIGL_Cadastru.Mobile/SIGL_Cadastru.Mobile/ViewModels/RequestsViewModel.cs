using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGL_Cadastru.Mobile.Models.Requests;
using SIGL_Cadastru.Mobile.Models.Shared;
using SIGL_Cadastru.Mobile.Services.Requests;

namespace SIGL_Cadastru.Mobile.ViewModels;

public partial class RequestsViewModel : ObservableObject
{
    private readonly IRequestService _requestService;

    [ObservableProperty]
    private string _pageTitle = "Top 10 Requests";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<CadastralRequestDto> _requests = new();

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string? _searchText;

    [ObservableProperty]
    private string? _filterBy;

    [ObservableProperty]
    private bool _filterIssued;

    [ObservableProperty]
    private bool _filterRejected;

    [ObservableProperty]
    private bool _filterAtReception;

    [ObservableProperty]
    private bool _filterInProgress;

    [ObservableProperty]
    private bool _filterFullyPaid;

    [ObservableProperty]
    private bool _filterUnpaid;

    [ObservableProperty]
    private string _orderBy = "AvailableFrom";

    [ObservableProperty]
    private string _direction = "desc";

    [ObservableProperty]
    private bool _isLoadingMore;

    [ObservableProperty]
    private bool _isFilterExpanded;

    [ObservableProperty]
    private bool _hasActiveFilters;

    private int _currentPage = 1;
    private int _totalPages = 1;
    private const int PageSize = 10;

    public RequestsViewModel(IRequestService requestService)
    {
        _requestService = requestService; 
    }

    [RelayCommand]
    public async Task Initialize()
    {
        await LoadRequestsAsync();
    }

    [RelayCommand]
    public async Task LoadRequests()
    {
        await LoadRequestsAsync();
    }

    [RelayCommand]
    private async Task NavigateToRequest(CadastralRequestDto request)
    {
        if (request?.Id != null)
        {
            await Shell.Current.GoToAsync($"RequestDetailPage?RequestId={request.Id}");
        }
    }

    [RelayCommand]
    public async Task LoadMoreRequests()
    {
        if (IsLoadingMore || _currentPage >= _totalPages)
            return;

        await LoadRequestsAsync(loadMore: true);
    }

    [RelayCommand]
    public async Task OpenFilterModal()
    {        
        // Modal will be handled in the View
        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task ApplyFilters()
    {
        var filters = new List<string>();
        if (FilterIssued) filters.Add("Issued");
        if (FilterRejected) filters.Add("Rejected");
        if (FilterAtReception) filters.Add("AtReception");
        if (FilterInProgress) filters.Add("InProgress");
        
        FilterBy = filters.Any() ? string.Join(",", filters) : null;
        UpdateHasActiveFilters();
        await LoadRequestsAsync(); // Reload with new filters
    }

    [RelayCommand]
    public async Task ClearPaymentFilter()
    {
        FilterFullyPaid = false;
        FilterUnpaid = false;
        UpdateHasActiveFilters();
        await LoadRequestsAsync(); // Reload after clearing payment filter
    }

    [RelayCommand]
    public async Task ClearAllFilters()
    {
        FilterIssued = false;
        FilterRejected = false;
        FilterAtReception = false;
        FilterInProgress = false;
        FilterFullyPaid = false;
        FilterUnpaid = false;
        FilterBy = null;
        SearchText = null;
        UpdateHasActiveFilters();
        await LoadRequestsAsync();
    }

    [RelayCommand]
    public void ToggleFilterExpanded()
    {
        IsFilterExpanded = !IsFilterExpanded;
    }

    private void UpdateHasActiveFilters()
    {
        HasActiveFilters = FilterIssued || FilterRejected || FilterAtReception || 
                          FilterInProgress || FilterFullyPaid || FilterUnpaid ||
                          !string.IsNullOrWhiteSpace(SearchText);
    }
    
    partial void OnSearchTextChanged(string? value)
    {
        UpdateHasActiveFilters();
        
        // Debounce search to avoid too many API calls
        _ = Task.Run(async () =>
        {
            await Task.Delay(300); // 300ms debounce
            if (SearchText == value) // Check if still the same value
            {
                await LoadRequestsAsync();
            }
        });
    }

    partial void OnOrderByChanged(string value)
    {
        // Automatically reload when sort field changes
        _ = LoadRequestsAsync();
    }

    partial void OnDirectionChanged(string value)
    {
        // Automatically reload when sort direction changes
        _ = LoadRequestsAsync();
    }

    private async Task LoadRequestsAsync(bool loadMore = false)
    {
        try
        {
            if (loadMore)
            {
                IsLoadingMore = true;
                _currentPage++;
            }
            else
            {
                IsLoading = true;
                _currentPage = 1;
                Requests.Clear();
            }

            ErrorMessage = null;

            var parameters = new RequestQueryParameters
            {
                SearchBy = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                FilterBy = string.IsNullOrWhiteSpace(FilterBy) ? null : FilterBy,
                PageNumber = _currentPage,
                PageSize = PageSize,
                OrderBy = OrderBy,
                Direction = Direction,
                IsFullyPaid = GetPaymentFilterValue()
            };

            var pagedResponse = await _requestService.GetRequestsPagedAsync(parameters);
            _totalPages = pagedResponse.TotalPages;
            
            // Add items only if they don't already exist (prevent duplicates)
            foreach (var request in pagedResponse.Data)
            {
                if (!Requests.Any(r => r.Id == request.Id))
                {
                    Requests.Add(request);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load requests: {ex.Message}";
            if (loadMore)
                _currentPage--; // Rollback page increment on error
        }
        finally
        {
            IsLoading = false;
            IsLoadingMore = false;
        }
    }
    private bool? GetPaymentFilterValue()
    {
        if (FilterFullyPaid && !FilterUnpaid) return true;
        if (FilterUnpaid && !FilterFullyPaid) return false;
        return null; // Both selected or neither selected = show all
    }
}
