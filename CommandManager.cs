namespace Fyson;

public static class CommandManager
{
    private static readonly Dictionary<string, CommandHandler> Commands = new();

    public static void RegisterCommand(string command, CommandHandler handler)
    {
        lock (Commands)
        {
            if (Commands.ContainsKey(command))
                Commands.Remove(command);
            Commands.Add(command, handler);
        }
    }

    public static CommandHandler? FindHandler(this string command)
    {
        lock (Commands)
            return (from pair in Commands where pair.Key == command select pair.Value).FirstOrDefault();
    }
}