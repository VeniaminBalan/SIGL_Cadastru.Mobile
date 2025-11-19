using SIGL_Cadastru.Mobile.Models.Shared;

namespace SIGL_Cadastru.Mobile.Models.Accounts;

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
