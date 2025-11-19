using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGL_Cadastru.Mobile.Models.Requests;
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
}
