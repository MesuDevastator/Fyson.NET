using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fyson;

public class Operation
{
    [JsonPropertyName("cmd")] public string? Command { get; set; }

    [JsonPropertyName("args")] public JsonElement Arguments { get; set; }

    [JsonPropertyName("when")] public string? Condition { get; set; }

    public void Execute(Context context)
    {
        if (Condition is not null && new Expression(Condition, context).ExpressionResult != "0")
            (Command?.FindHandler() ?? ((command, arguments, _) =>
                throw new CommandNotFoundException("Command not found", command, arguments)))(Command ?? "", Arguments,
                context);
    }
}