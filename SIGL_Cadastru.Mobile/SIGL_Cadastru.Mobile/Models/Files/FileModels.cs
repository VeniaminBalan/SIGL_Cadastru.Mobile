namespace SIGL_Cadastru.Mobile.Models.Files;

public class FileModel
{
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long Size { get; set; }
}
