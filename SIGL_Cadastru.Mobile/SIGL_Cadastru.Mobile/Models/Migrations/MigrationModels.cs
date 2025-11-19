using SIGL_Cadastru.Mobile.Models.Requests;
using SIGL_Cadastru.Mobile.Models.Shared;

namespace SIGL_Cadastru.Mobile.Models.Migrations;

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
