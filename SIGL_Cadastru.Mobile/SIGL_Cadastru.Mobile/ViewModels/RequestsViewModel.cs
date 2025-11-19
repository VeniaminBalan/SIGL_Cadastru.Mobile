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
    private string _orderBy = "AvailableFrom";

    [ObservableProperty]
    private string _direction = "desc";

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

    private async Task LoadRequestsAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            var parameters = new RequestQueryParameters
            {
                SearchBy = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                FilterBy = string.IsNullOrWhiteSpace(FilterBy) ? null : FilterBy,
                PageNumber = 1,
                PageSize = 10,
                OrderBy = OrderBy,
                Direction = Direction
            };

            var requestsList = await _requestService.GetRequestsAsync(parameters);
            
            Requests.Clear();
            foreach (var request in requestsList)
            {
                Requests.Add(request);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load requests: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
