using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGL_Cadastru.Mobile.Models.Requests;
using SIGL_Cadastru.Mobile.Models.Shared;
using SIGL_Cadastru.Mobile.Services.Requests;

namespace SIGL_Cadastru.Mobile.ViewModels;

[QueryProperty(nameof(RequestId), nameof(RequestId))]
public partial class RequestDetailViewModel : ObservableObject
{
    private readonly IRequestService _requestService;

    [ObservableProperty]
    private string? _requestId;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private DetailedCadastralRequest? _request;

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

    public bool CanIssueRequest => Request?.IsFullyPaid ?? false;

    public RequestDetailViewModel(IRequestService requestService)
    {
        _requestService = requestService;
    }

    [RelayCommand]
    public async Task Initialize()
    {
        await LoadRequestAsync();
    }

    partial void OnRequestIdChanged(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            _ = LoadRequestAsync();
        }
    }

    partial void OnRequestChanged(DetailedCadastralRequest? value)
    {
        OnPropertyChanged(nameof(RemainingAmount));
        OnPropertyChanged(nameof(PaymentStatus));
        OnPropertyChanged(nameof(CanIssueRequest));
    }

    private async Task LoadRequestAsync()
    {
        if (string.IsNullOrWhiteSpace(RequestId))
            return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            Request = await _requestService.GetRequestByIdAsync(RequestId);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load request details: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DownloadPdf()
    {
        if (string.IsNullOrWhiteSpace(RequestId))
            return;

        try
        {
            var pdfStream = await _requestService.GetRequestPdfAsync(RequestId);
            // TODO: Implement PDF saving/sharing logic
            await Shell.Current.DisplayAlertAsync("Success", "PDF download feature coming soon", "OK");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to download PDF: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task AddPayment()
    {
        if (string.IsNullOrWhiteSpace(RequestId) || Request == null)
            return;

        if (PaymentAmount <= 0)
        {
            PaymentErrorMessage = "Amount must be greater than 0";
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

            await _requestService.AddPaymentAsync(RequestId, command);
            
            // Reload request to get updated payment data
            await LoadRequestAsync();

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
        if (string.IsNullOrWhiteSpace(RequestId) || Request == null || payment == null)
            return;

        if (Request.Metadata.CurrentState == StateType.Issued)
        {
            await Shell.Current.DisplayAlertAsync("Error", "Cannot delete payments from issued requests", "OK");
            return;
        }

        var confirm = await Shell.Current.DisplayAlertAsync(
            "Confirm Delete", 
            $"Are you sure you want to delete this payment of {payment.Amount:F2} MDL?", 
            "Delete", 
            "Cancel");

        if (!confirm)
            return;

        try
        {
            IsLoading = true;
            ErrorMessage = null;

            await _requestService.DeletePaymentAsync(RequestId, payment.Id);
            
            // Reload request to get updated payment data
            await LoadRequestAsync();

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
}
