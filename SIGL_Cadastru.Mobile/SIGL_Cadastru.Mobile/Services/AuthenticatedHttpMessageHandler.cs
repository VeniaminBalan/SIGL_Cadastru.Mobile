using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;

namespace SIGL_Cadastru.Mobile.Services;

public class AuthenticatedHttpMessageHandler : DelegatingHandler
{
    private readonly KeycloakAuthService _authService;
    private readonly ILogger<AuthenticatedHttpMessageHandler> _logger;
    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);

    public AuthenticatedHttpMessageHandler(
        KeycloakAuthService authService,
        ILogger<AuthenticatedHttpMessageHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Add access token to the request if available
        if (!string.IsNullOrEmpty(_authService.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authService.AccessToken);
            _logger.LogDebug("Added Bearer token to request: {Method} {Uri}", request.Method, request.RequestUri);
        }
        else
        {
            _logger.LogWarning("No access token available for request: {Method} {Uri}", request.Method, request.RequestUri);
        }

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);

        // If we get a 401 Unauthorized, try to refresh the token and retry once
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Received 401 Unauthorized, attempting token refresh");

            // Use semaphore to ensure only one thread attempts refresh at a time
            await _refreshSemaphore.WaitAsync(cancellationToken);
            try
            {
                var refreshSuccess = await _authService.RefreshAsync();

                if (refreshSuccess)
                {
                    _logger.LogInformation("Token refresh successful, retrying request");

                    // Clone the request (original request can't be sent again)
                    var clonedRequest = await CloneHttpRequestMessageAsync(request);
                    
                    // Add the new access token
                    clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authService.AccessToken);

                    // Retry the request with the new token
                    response = await base.SendAsync(clonedRequest, cancellationToken);
                }
                else
                {
                    _logger.LogError("Token refresh failed, user needs to re-authenticate");
                    // Clear tokens so user is prompted to log in again
                    await _authService.InvalidateTokensAsync();
                }
            }
            finally
            {
                _refreshSemaphore.Release();
            }
        }

        return response;
    }

    private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version
        };

        // Copy headers
        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Copy content if present
        if (request.Content != null)
        {
            var originalContent = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(originalContent);

            // Copy content headers
            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshSemaphore?.Dispose();
        }
        base.Dispose(disposing);
    }
}
