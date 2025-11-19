using System.Text.Json.Serialization;

namespace SIGL_Cadastru.Mobile.Models.Shared;

public enum StateType
{
    [JsonPropertyName("0")]
    InProgress = 0,
    [JsonPropertyName("1")]
    AtReception = 1,
    [JsonPropertyName("2")]
    Issued = 2,
    [JsonPropertyName("3")]
    Rejected = 3,
    [JsonPropertyName("4")]
    Extended = 4
}

public enum RoleType
{
    [JsonPropertyName("1")]
    Responsible = 1,
    [JsonPropertyName("2")]
    Performer = 2,
    [JsonPropertyName("3")]
    None = 3
}
