using System.Text.Json;

namespace Fyson;

public class CommandNotFoundException : Exception
{
    public CommandNotFoundException(string message, string command, JsonElement args) : base(message)
    {
        Command = command;
        Arguments = args;
    }

    public CommandNotFoundException(string message, string command, JsonElement args, Exception? innerException) :
        base(message, innerException)
    {
        Command = command;
        Arguments = args;
    }

    public string Command { get; }

    public JsonElement Arguments { get; }

    public override string ToString() =>
        $"{Message} (Command: {Command}, Arguments: {JsonSerializer.Serialize(Arguments)})";
}