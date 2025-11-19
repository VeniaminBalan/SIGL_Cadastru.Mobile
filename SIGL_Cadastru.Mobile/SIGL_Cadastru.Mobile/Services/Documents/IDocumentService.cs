using SIGL_Cadastru.Mobile.Models.Documents;

namespace SIGL_Cadastru.Mobile.Services.Documents;

/// <summary>
/// Interface for document management operations
/// </summary>
public interface IDocumentService
{
    Task<Document> AddDocumentAsync(AddDocumentRequest request);
    Task<Document> UpdateDocumentAsync(UpdateDocumentCommand command);
    Task DeleteDocumentAsync(string id, bool removeFile = false);
}
