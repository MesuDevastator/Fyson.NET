using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fyson.Commands;

public class Variable
{
    [JsonPropertyName("id")] public string Identifier { get; set; }

    [JsonPropertyName("val")] public JsonElement Value { get; set; }
}