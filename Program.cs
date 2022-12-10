using Serilog;
using Serilog.Core;

namespace Fyson;

public static class Program
{
    private static Logger _logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

    public static void Main()
    {
        
    }
}