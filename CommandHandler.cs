using System.Text.Json;

namespace Fyson;

public delegate void CommandHandler(string command, JsonElement arguments, Context context);