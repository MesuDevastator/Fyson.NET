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
                    RegisterFunction("sqrt", (_, parameters) => Math.Sqrt(parameters.GetDouble()));
                    return;
                default:
                    throw new FileNotFoundException("Library not found", library);
            }
    }

    public static FunctionHandler? FindFunction(this string function)
    {
        lock (Functions)
            return (from pair in Functions where pair.Key == function select pair.Value).FirstOrDefault();
    }
}