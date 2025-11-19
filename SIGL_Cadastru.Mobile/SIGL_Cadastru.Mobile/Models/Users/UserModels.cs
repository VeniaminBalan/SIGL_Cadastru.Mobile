using SIGL_Cadastru.Mobile.Models.Shared;

namespace SIGL_Cadastru.Mobile.Models.Users;

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
