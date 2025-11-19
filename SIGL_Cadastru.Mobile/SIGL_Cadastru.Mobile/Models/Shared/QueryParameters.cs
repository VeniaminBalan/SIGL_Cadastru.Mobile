namespace SIGL_Cadastru.Mobile.Models.Shared;

public class PagedQueryParameters
{
    public string? SearchBy { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}

public class RequestQueryParameters : PagedQueryParameters
{
    public string? FilterBy { get; set; }
    public string? OrderBy { get; set; }
    public string? Direction { get; set; }
}
