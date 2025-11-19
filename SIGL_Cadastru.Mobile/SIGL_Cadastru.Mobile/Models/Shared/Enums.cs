using System.Text.Json.Serialization;

namespace SIGL_Cadastru.Mobile.Models.Shared;

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
