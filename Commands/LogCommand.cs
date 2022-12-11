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

    public static void OnCommand(string command, JsonElement arguments)
    {

    }
}