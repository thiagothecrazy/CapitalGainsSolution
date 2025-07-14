using Serilog;

namespace CapitalGains.Cli.Infrastructure;

public static class LoggingSetup
{
    public static void Configure()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
    }
}