using System.Text.Json.Serialization;

namespace Fyson;

public class Script
{
    [JsonPropertyName("require")] public Requirements Requirements { get; set; } = new();

    [JsonPropertyName("debug")] public bool Debug { get; set; }

    [JsonPropertyName("scripts")] public List<Operation> Operations { get; set; } = new();
}