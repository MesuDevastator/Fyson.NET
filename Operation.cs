using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fyson;

public class Operation
{
    [JsonPropertyName("cmd")] public string? Command { get; set; }

    [JsonPropertyName("args")] public JsonElement Arguments { get; set; }

    public void Execute() =>
        (Command?.FindHandler() ?? ((command, args) =>
            throw new CommandNotFoundException("Command not found", command, args)))(Command ?? "", Arguments);
}