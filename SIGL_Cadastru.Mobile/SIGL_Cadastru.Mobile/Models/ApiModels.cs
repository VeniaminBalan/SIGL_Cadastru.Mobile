using System.Text.Json.Serialization;

namespace SIGL_Cadastru.Mobile.Models;

// Enums
public enum StateType
{
    [JsonPropertyName("0")]
    State0 = 0,
    [JsonPropertyName("1")]
    State1 = 1,
    [JsonPropertyName("2")]
    State2 = 2,
    [JsonPropertyName("3")]
    State3 = 3,
    [JsonPropertyName("4")]
    State4 = 4
}

public enum RoleType
{
    [JsonPropertyName("1")]
    Role1 = 1,
    [JsonPropertyName("2")]
    Role2 = 2,
    [JsonPropertyName("3")]
    Role3 = 3
}

// Client Models
public class ClientDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Idnp { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Domicile { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<CadastralRequestForClient> Requests { get; set; } = new();
}

public class CreateClientCommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Idnp { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Domicile { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}

public class UpdateClientCommand
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Idnp { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Domicile { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}

public class Client
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Idnp { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? Domicile { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public List<CadastralRequest>? Requests { get; set; }
}

// Request Models
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
}

public class CadastralRequestForClient
{
    public string Id { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public StateType CurrentState { get; set; }
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

public class CadastralRequest
{
    public string? Id { get; set; }
    public Client? Client { get; set; }
    public User? Performer { get; set; }
    public User? Responsible { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime AvailableUntil { get; set; }
    public CadastralRequestMetadata? Metadata { get; set; }
    public string? CadastralNumber { get; set; }
    public string? Comment { get; set; }
    public RequestNumber? Number { get; set; }
    public List<RequestState>? States { get; set; }
    public List<Document>? Documents { get; set; }
    public List<CadastralWork>? CadastralWorks { get; set; }
    public double Addition { get; set; }
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

// Request State Models
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

// Cadastral Work Models
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

// Document Models
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

// File Models
public class FileModel
{
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long Size { get; set; }
}

// User Models
public class User
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? IdentificationNumber { get; set; }
    public bool IsActive { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public RoleType Role { get; set; }
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class UserDtoPagedResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public List<UserDto>? Data { get; set; }
}

// Account Models
public class RegisterUser
{
    public string? Email { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Domicile { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public RoleType Role { get; set; }
}

public class UpdateProfile
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
}

// Tree Models
public class TreeDto
{
    public string? Id { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public double? Price { get; set; }
    public int? Deadline { get; set; }
    public List<TreeDto>? Childrens { get; set; }
}

// Migration Models
public class ClientToMigrate
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Idnp { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Domicile { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class ClientMigrationCommand
{
    public List<ClientToMigrate>? Clients { get; set; }
}

public class CadastralRequestToMigrate
{
    public string Nr { get; set; } = string.Empty;
    public string CadastralNumber { get; set; } = string.Empty;
    public string ClientIdnp { get; set; } = string.Empty;
    public string PerformerIdnp { get; set; } = string.Empty;
    public string ResponsibleIdnp { get; set; } = string.Empty;
    public DateTime AvailableFrom { get; set; }
    public DateTime AvailableUntil { get; set; }
    public double Addition { get; set; }
    public List<CadastralWorkRegistration> CadastralWorks { get; set; } = new();
    public List<RequestStateToMigrate> RequestStates { get; set; } = new();
    public string Comment { get; set; } = string.Empty;
    public StateType CurrentState { get; set; }
}

public class RequestStateToMigrate
{
    public StateType State { get; set; }
    public DateTime Created { get; set; }
}

public class MigrateCadastralRequestCommand
{
    public List<CadastralRequestToMigrate>? Requests { get; set; }
}

public class MigrationResult
{
    public bool IsSuccess { get; set; }
    public int TotalRecords { get; set; }
    public int RecordsMigrated { get; set; }
    public int RecordsFailedToMigrate { get; set; }
    public int Skipped { get; set; }
    public List<MigrationFailedDetails>? RecordsFailedToMigrateDetails { get; set; }
}

public class MigrationFailedDetails
{
    public string? Id { get; set; }
    public string? Error { get; set; }
    public string? Message { get; set; }
}

public class ExctractDataResult
{
    public List<CadastralRequestToMigrate>? Requests { get; set; }
    public List<ClientToMigrate>? Clients { get; set; }
}

// Query Parameters
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
