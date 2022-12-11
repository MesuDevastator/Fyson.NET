using System.Collections;
using System.Text.Json;

namespace Fyson.Commands;

public static class VarCommand
{
    public static void OnCommand(string command, JsonElement arguments, Context context)
    {
        if (command is not "var")
            return;
        var variable = arguments.Deserialize<Variable>();
        if (variable is null)
            return;
        object? value;
        switch (variable.Value.ValueKind)
        {
            case JsonValueKind.Object:
                value = variable.Value.CallFunction();
                break;
            case JsonValueKind.String:
                value = variable.Value.GetString();
                break;
            case JsonValueKind.Number:
                value = variable.Value.GetDouble();
                break;
            case JsonValueKind.Array:
                var list = new ArrayList();
                for (var index = 0; index < variable.Value.GetArrayLength(); index++)
                    list.Add(variable.Value[index]);
                value = list;
                break;
            case JsonValueKind.True:
                value = true;
                break;
            case JsonValueKind.False:
                value = false;
                break;
            case JsonValueKind.Undefined:
            case JsonValueKind.Null:
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(arguments.ValueKind), arguments.ValueKind,
                    $"{nameof(arguments.ValueKind)} out of range");
        }

        context.SetVariable(variable.Identifier, value);
    }
}