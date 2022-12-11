using System.Text.Json;

namespace Fyson;

public delegate object? FunctionHandler(string function, JsonElement parameters);