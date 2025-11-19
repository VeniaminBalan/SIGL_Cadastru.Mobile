using SIGL_Cadastru.Mobile.Models.Files;

namespace SIGL_Cadastru.Mobile.Services.Files;

/// <summary>
/// Interface for file management operations
/// </summary>
public interface IFileService
{
    Task<List<FileModel>> GetFilesAsync();
    Task<FileModel> UploadFileAsync(Stream fileStream, string fileName);
    Task<Stream> GetFileByIdAsync(string id);
    Task DeleteFileAsync(string id);
}
