using Args;
using Serilog;
using Serilog.Core;
using System.Text.Json;

namespace Fyson;

public static class Program
{
    private static readonly Logger Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .MinimumLevel.Verbose()
        .CreateLogger();

    public static int Main(string[] args)
    {
        var parameter = Configuration.Configure<ConsoleParameter>().CreateAndBind(args);
        if (!parameter.Silent)
            Logger.Information("---[Fyson.NET]---");
        if (string.IsNullOrEmpty(parameter.Executive))
        {
            Logger.Fatal("No executive given");
            return 1;
        }

        string executiveContent;
        try
        {
            executiveContent = File.ReadAllText(parameter.Executive);
        }
        catch (FileNotFoundException ex)
        {
            Logger.Fatal(ex, "Failed to open executive {executive}", parameter.Executive);
            return 1;
        }

        Script? script;
        try
        {
            if ((script = JsonSerializer.Deserialize<Script>(executiveContent)) == null)
                throw new Exception("Null deserialization result");
            if (!script.Requirements.MeetRequirements)
                throw new InvalidOperationException("Do not meet requirements");
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Failed to deserialize executive {executive}", parameter.Executive);
            return 1;
        }

        foreach (var operation in script.Operations)
            try
            {
                operation.Execute();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Failed during executing operation {operation}",
                    JsonSerializer.Serialize(operation));
                return 1;
            }

        return 0;
    }

}