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
    private string _orderBy = "AvailableFrom";

    [ObservableProperty]
    private string _direction = "desc";

    [ObservableProperty]
    private bool _isLoadingMore;

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
    public void ApplyFilters()
    {
        var filters = new List<string>();
        if (FilterIssued) filters.Add("Issued");
        if (FilterRejected) filters.Add("Rejected");
        if (FilterAtReception) filters.Add("AtReception");
        if (FilterInProgress) filters.Add("InProgress");
        
        FilterBy = filters.Any() ? string.Join(",", filters) : null;
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
                Direction = Direction
            };

            var pagedResponse = await _requestService.GetRequestsPagedAsync(parameters);
            _totalPages = pagedResponse.TotalPages;
            
            foreach (var request in pagedResponse.Data)
            {
                Requests.Add(request);
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
}
