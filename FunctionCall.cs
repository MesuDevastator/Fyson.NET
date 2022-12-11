using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fyson;

public class FunctionCall
{
    [JsonPropertyName("from")] public string Function { get; set; }

    [JsonPropertyName("param")] public JsonElement Parameters { get; set; }
}