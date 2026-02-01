using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGL_Cadastru.Mobile.Models.Requests;
using SIGL_Cadastru.Mobile.Models.Shared;
using SIGL_Cadastru.Mobile.Services.Requests;

namespace SIGL_Cadastru.Mobile.ViewModels.RequestDetail;

/// <summary>
/// ViewModel for managing request states - displays state history and workflow actions
/// </summary>
[QueryProperty(nameof(Request), nameof(Request))]
public partial class RequestStatesViewModel : ObservableObject
{
    private readonly IRequestService _requestService;

    [ObservableProperty]
    private DetailedCadastralRequest? _request;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isAddingState;

    [ObservableProperty]
    private StateType _selectedStateType;

    [ObservableProperty]
    private DateTime _stateDate = DateTime.Now;

    [ObservableProperty]
    private string? _stateErrorMessage;

    public string CurrentState => Request?.Metadata.CurrentState.ToString() ?? "Unknown";

    public bool CanAddState => Request != null;

    public bool CanDeleteState => Request != null;

    public bool CanIssueRequest => Request?.IsFullyPaid ?? false;

    public RequestStatesViewModel(IRequestService requestService)
    {
        _requestService = requestService;
        _selectedStateType = StateType.InProgress;
    }

    partial void OnRequestChanged(DetailedCadastralRequest? value)
    {
        OnPropertyChanged(nameof(CurrentState));
        OnPropertyChanged(nameof(CanAddState));
        OnPropertyChanged(nameof(CanDeleteState));
        OnPropertyChanged(nameof(CanIssueRequest));
    }

    [RelayCommand]
    private async Task AddState()
    {
        if (Request == null) return;

        try
        {
            IsAddingState = true;
            StateErrorMessage = null;

            var command = new AddStateRequest
            {
                Type = SelectedStateType,
                Date = StateDate
            };

            var newState = await _requestService.AddRequestStateAsync(Request.Id, command);

            // Update local request object
            Request.States.Add(newState);
            Request.Metadata.CurrentState = newState.State;

            // Update metadata dates based on state type
            UpdateMetadataForState(newState);

            // Trigger property change notifications
            OnRequestChanged(Request);

            // Reset form
            StateDate = DateTime.Now;

            await Shell.Current.DisplayAlertAsync("Success", "State added successfully", "OK");
        }
        catch (Exception ex)
        {
            StateErrorMessage = $"Failed to add state: {ex.Message}";
        }
        finally
        {
            IsAddingState = false;
        }
    }

    [RelayCommand]
    private async Task DeleteState(RequestState state)
    {
        if (Request == null || state == null) return;

        var confirm = await Shell.Current.DisplayAlertAsync(
            "Confirm Delete",
            $"Are you sure you want to delete this state: {state.State}?",
            "Delete",
            "Cancel");

        if (!confirm) return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            await _requestService.DeleteRequestStateAsync(Request.Id, state.Id!);

            // Update local request object
            Request.States.Remove(state);

            // Recalculate current state (use the most recent state)
            if (Request.States.Any())
            {
                var latestState = Request.States.OrderByDescending(s => s.Created).First();
                Request.Metadata.CurrentState = latestState.State;
            }

            // Trigger property change notifications
            OnRequestChanged(Request);

            await Shell.Current.DisplayAlertAsync("Success", "State deleted successfully", "OK");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete state: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task IssueRequest()
    {
        if (Request == null) return;

        if (!CanIssueRequest)
        {
            await Shell.Current.DisplayAlertAsync(
                "Cannot Issue",
                "Request must be fully paid before issuing",
                "OK");
            return;
        }

        var confirm = await Shell.Current.DisplayAlertAsync(
            "Issue Request",
            "Are you sure you want to issue this request? This action will mark it as completed.",
            "Issue",
            "Cancel");

        if (!confirm) return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            // Add Issued state
            var command = new AddStateRequest
            {
                Type = StateType.Issued,
                Date = DateTime.Now
            };

            var newState = await _requestService.AddRequestStateAsync(Request.Id, command);

            // Update local request object
            Request.States.Add(newState);
            Request.Metadata.CurrentState = StateType.Issued;
            Request.Metadata.IssuedAt = newState.Created;

            // Trigger property change notifications
            OnRequestChanged(Request);

            await Shell.Current.DisplayAlertAsync("Success", "Request issued successfully", "OK");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to issue request: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        if (Request == null) return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            // Reload request from API
            var updatedRequest = await _requestService.GetRequestByIdAsync(Request.Id);
            Request = updatedRequest;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to refresh: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateMetadataForState(RequestState state)
    {
        if (Request == null) return;

        switch (state.State)
        {
            case StateType.Issued:
                Request.Metadata.IssuedAt = state.Created;
                break;
            case StateType.Rejected:
                Request.Metadata.RejectedAt = state.Created;
                break;
            case StateType.AtReception:
                Request.Metadata.AtReception = state.Created;
                break;
        }
    }
}
