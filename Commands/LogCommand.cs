using System.Globalization;
using System.Text;
using System.Text.Json;
using Serilog;
using Serilog.Core;

namespace Fyson.Commands;

public static class LogCommand
{
    private static readonly Logger Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console()
        .CreateLogger();

    public static void OnCommand(string command, JsonElement arguments, Context context)
    {
        if (command != "log") return;
        string? message;
        switch (arguments.ValueKind)
        {
            case JsonValueKind.String when (message = arguments.GetString()) is not null:
            case JsonValueKind.Object when (message = arguments.CallFunction()?.ToString()) is not null:
            case JsonValueKind.Number
                when (message = arguments.GetDouble().ToString(CultureInfo.CurrentCulture)) is not null:
            case JsonValueKind.True
                when (message = arguments.GetBoolean().ToString(CultureInfo.CurrentCulture)) is not null:
            case JsonValueKind.False
                when (message = arguments.GetBoolean().ToString(CultureInfo.CurrentCulture)) is not null:
                Logger.Information(new Expression(message, context).ExpressionResult);
                break;
            case JsonValueKind.Array:
                StringBuilder messageBuilder = new();
                for (var index = 0; index < arguments.GetArrayLength(); index++)
                {
                    JsonElement element;
                    if ((element = arguments[index]).ValueKind == JsonValueKind.Object)
                        messageBuilder.Append(element.CallFunction());
                    else
                        messageBuilder.Append(arguments[index].GetString());
                }

                Logger.Information(new Expression(messageBuilder.ToString(), context).ExpressionResult);
                break;
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
            default:
                break;
        }
    }
}