using System.Text.Json;

namespace Fyson;

public static class LibraryManager
{
    private static readonly Dictionary<string, FunctionHandler> Functions = new();

    public static void RegisterFunction(string function, FunctionHandler handler)
    {
        lock (Functions)
            Functions.Add(function, handler);
    }

    public static void LoadLibrary(this string library)
    {
        // TODO: Implement library loading
            switch (library)
            {
                case "@math.flib":
                    RegisterFunction("sqrt",
                        (function, parameters) => function == "sqrt" ? Math.Sqrt(parameters.GetDouble()) : null);
                    return;
                default:
                    throw new FileNotFoundException("Library not found", library);
            }
    }

    public static object? CallFunction(this JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object) return null;
        var call = element.Deserialize<FunctionCall>();
        FunctionHandler? function;
        return call is not null && call.Function.StartsWith("lib:") &&
               (function = FindFunction(call.Function[4..])) is not null
            ? function(call.Function[4..], call.Parameters)
            : null;
    }

    public static FunctionHandler? FindFunction(this string function)
    {
        lock (Functions)
            return (from pair in Functions where pair.Key == function select pair.Value).FirstOrDefault();
    }
}