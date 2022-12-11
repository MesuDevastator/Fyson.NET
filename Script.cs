using System.Text.Json.Serialization;

namespace Fyson;

public class Script : BasicScript
{
    [JsonPropertyName("require")] public Requirements Requirements { get; set; } = new();

    [JsonPropertyName("debug")] public bool Debug { get; set; }
}