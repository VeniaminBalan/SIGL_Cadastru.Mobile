using SIGL_Cadastru.Mobile.Models.Files;

namespace SIGL_Cadastru.Mobile.Models.Documents;

public class Document
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Number { get; set; }
    public DateOnly Date { get; set; }
    public string? Mentions { get; set; }
    public int NrOfCopies { get; set; }
    public FileModel? File { get; set; }
}

public class AddDocumentRequest
{
    public string CadastralRequestId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public string? Mentions { get; set; }
    public int NrOfCopies { get; set; }
    public string? FileId { get; set; }
}

public class UpdateDocumentCommand
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public string? Mentions { get; set; }
    public int NrOfCopies { get; set; }
    public string? FileId { get; set; }
}
