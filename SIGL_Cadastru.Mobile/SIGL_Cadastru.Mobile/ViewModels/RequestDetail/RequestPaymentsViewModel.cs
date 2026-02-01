using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGL_Cadastru.Mobile.Models.Requests;
using SIGL_Cadastru.Mobile.Models.Shared;
using SIGL_Cadastru.Mobile.Services.Requests;

namespace SIGL_Cadastru.Mobile.ViewModels.RequestDetail;

/// <summary>
/// ViewModel for managing request payments - handles CRUD operations and validation
/// </summary>
[QueryProperty(nameof(Request), nameof(Request))]
public partial class RequestPaymentsViewModel : ObservableObject
{
    private readonly IRequestService _requestService;

    [ObservableProperty]
    private DetailedCadastralRequest? _request;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isAddingPayment;

    [ObservableProperty]
    private double _paymentAmount;

    [ObservableProperty]
    private string? _paymentDescription;

    [ObservableProperty]
    private string? _paymentErrorMessage;

    public double RemainingAmount => Request != null ? Request.TotalPrice - Request.TotalPayments : 0;

    public string PaymentStatus
    {
        get
        {
            if (Request == null) return "Unknown";
            if (Request.IsFullyPaid) return "Fully Paid";
            if (Request.TotalPayments > 0) return "Partially Paid";
            return "Unpaid";
        }
    }

    public double PaymentProgress => Request != null && Request.TotalPrice > 0
        ? Request.TotalPayments / Request.TotalPrice
        : 0;

    public bool CanAddPayment => Request?.Metadata.CurrentState != StateType.Issued;

    public bool CanDeletePayment => Request?.Metadata.CurrentState != StateType.Issued;

    public RequestPaymentsViewModel(IRequestService requestService)
    {
        _requestService = requestService;
    }

    partial void OnRequestChanged(DetailedCadastralRequest? value)
    {
        OnPropertyChanged(nameof(RemainingAmount));
        OnPropertyChanged(nameof(PaymentStatus));
        OnPropertyChanged(nameof(PaymentProgress));
        OnPropertyChanged(nameof(CanAddPayment));
        OnPropertyChanged(nameof(CanDeletePayment));
    }

    [RelayCommand]
    private async Task AddPayment()
    {
        if (Request == null) return;

        // Validation
        if (PaymentAmount <= 0)
        {
            PaymentErrorMessage = "Amount must be greater than 0";
            return;
        }

        if (PaymentAmount > RemainingAmount)
        {
            PaymentErrorMessage = $"Amount cannot exceed remaining balance of {RemainingAmount:F2} MDL";
            return;
        }

        if (!string.IsNullOrWhiteSpace(PaymentDescription) && PaymentDescription.Length > 500)
        {
            PaymentErrorMessage = "Description cannot exceed 500 characters";
            return;
        }

        try
        {
            IsAddingPayment = true;
            PaymentErrorMessage = null;

            var command = new AddPaymentCommand
            {
                Amount = PaymentAmount,
                Description = PaymentDescription
            };

            var newPayment = await _requestService.AddPaymentAsync(Request.Id, command);

            // Update local request object
            Request.Payments.Add(newPayment);
            Request.TotalPayments += newPayment.Amount;
            Request.IsFullyPaid = Request.TotalPayments >= Request.TotalPrice;

            // Trigger property change notifications
            OnRequestChanged(Request);

            // Reset form
            PaymentAmount = 0;
            PaymentDescription = null;

            await Shell.Current.DisplayAlertAsync("Success", "Payment added successfully", "OK");
        }
        catch (Exception ex)
        {
            PaymentErrorMessage = $"Failed to add payment: {ex.Message}";
        }
        finally
        {
            IsAddingPayment = false;
        }
    }

    [RelayCommand]
    private async Task DeletePayment(PaymentDto payment)
    {
        if (Request == null || payment == null) return;

        if (!CanDeletePayment)
        {
            await Shell.Current.DisplayAlertAsync("Error", "Cannot delete payments from issued requests", "OK");
            return;
        }

        var confirm = await Shell.Current.DisplayAlertAsync(
            "Confirm Delete",
            $"Are you sure you want to delete this payment of {payment.Amount:F2} MDL?",
            "Delete",
            "Cancel");

        if (!confirm) return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            await _requestService.DeletePaymentAsync(Request.Id, payment.Id);

            // Update local request object
            Request.Payments.Remove(payment);
            Request.TotalPayments -= payment.Amount;
            Request.IsFullyPaid = Request.TotalPayments >= Request.TotalPrice;

            // Trigger property change notifications
            OnRequestChanged(Request);

            await Shell.Current.DisplayAlertAsync("Success", "Payment deleted successfully", "OK");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete payment: {ex.Message}";
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
}
