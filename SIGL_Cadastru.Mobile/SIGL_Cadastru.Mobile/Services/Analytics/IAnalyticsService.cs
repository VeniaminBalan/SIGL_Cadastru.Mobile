namespace SIGL_Cadastru.Mobile.Services.Analytics;

/// <summary>
/// Interface for analytics operations
/// </summary>
public interface IAnalyticsService
{
    Task<string> GetEmbedUrlAsync(Dictionary<string, object> request);
}
