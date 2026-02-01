using SIGL_Cadastru.Mobile.Models.Clients;
using SIGL_Cadastru.Mobile.Models.Documents;
using SIGL_Cadastru.Mobile.Models.Shared;

namespace SIGL_Cadastru.Mobile.Models.Requests;

public class CadastralRequestDto
{
    public string Id { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string CadastralNumber { get; set; } = string.Empty;
    public string Client { get; set; } = string.Empty;
    public string Performer { get; set; } = string.Empty;
    public string Responsible { get; set; } = string.Empty;
    public DateTime AvailableFrom { get; set; }
    public DateTime AvailableUntil { get; set; }
    public StateType CurrentState { get; set; }
    public DateTime? ExtendedUntil { get; set; }
    public DateTime? AtReceptionOn { get; set; }
    public DateTime? IssuedOn { get; set; }
    public DateTime? RejectedOn { get; set; }
    public bool IsFullyPaid { get; set; }
}

public class DetailedCadastralRequest
{
    public string Id { get; set; } = string.Empty;
    public ClientDto Client { get; set; } = new();
    public string Responsible { get; set; } = string.Empty;
    public string Performer { get; set; } = string.Empty;
    public DateTime AvailableFrom { get; set; }
    public DateTime AvailableUntil { get; set; }
    public CadastralRequestMetadata Metadata { get; set; } = new();
    public string CadastalNumber { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public double TotalPrice { get; set; }
    public double TotalPayments { get; set; }
    public bool IsFullyPaid { get; set; }
    public List<PaymentDto> Payments { get; set; } = new();
    public List<CadastralWork> CadastralWorks { get; set; } = new();
    public List<Document> Documents { get; set; } = new();
    public List<RequestState> States { get; set; } = new();
}

public class CreateCadastralRequestCommand
{
    public string CadastralNumber { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string PerformerId { get; set; } = string.Empty;
    public string ResponsibleId { get; set; } = string.Empty;
    public DateTime AvailableFrom { get; set; }
    public int Deadline { get; set; }
    public List<CadastralWorkRegistration>? CadastralWorks { get; set; }
    public double Addition { get; set; }
    public string? Comment { get; set; }
}



public class CadastralRequestMetadata
{
    public string? Id { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime? AtReception { get; set; }
    public DateTime? ExtendedUntil { get; set; }
    public DateTime DueTo { get; set; }
    public StateType CurrentState { get; set; }
    public string? Number { get; set; }
}

public class RequestNumber
{
    public int Year { get; set; }
    public int Index { get; set; }
}

public class RequestState
{
    public string? Id { get; set; }
    public string? RequestId { get; set; }
    public StateType State { get; set; }
    public DateTime Created { get; set; }
}

public class AddStateRequest
{
    public StateType Type { get; set; }
    public DateTime Date { get; set; }
}

public class CadastralWork
{
    public string? Id { get; set; }
    public string? RequestId { get; set; }
    public string? WorkDescription { get; set; }
    public double Price { get; set; }
}

public class CadastralWorkRegistration
{
    public string RequestId { get; set; } = string.Empty;
    public string WorkDescription { get; set; } = string.Empty;
    public double Price { get; set; }
}

public class TreeDto
{
    public string? Id { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public double? Price { get; set; }
    public int? Deadline { get; set; }
    public List<TreeDto>? Childrens { get; set; }
}

public class PaymentDto
{
    public string Id { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AddPaymentCommand
{
    public double Amount { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
}
