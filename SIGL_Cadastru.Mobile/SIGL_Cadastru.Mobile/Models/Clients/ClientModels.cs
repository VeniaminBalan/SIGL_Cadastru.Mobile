using SIGL_Cadastru.Mobile.Models.Shared;

namespace SIGL_Cadastru.Mobile.Models.Clients;

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

public class CadastralRequestForClient
{
    public string Id { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public StateType CurrentState { get; set; }
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
}
