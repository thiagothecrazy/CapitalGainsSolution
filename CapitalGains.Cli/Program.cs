using CapitalGains.App.Services;
using CapitalGains.Cli.Infrastructure;
using Serilog;

namespace CapitalGains.Cli;

class Program
{
    static void Main()
    {
        LoggingSetup.Configure();
        using var tracerProvider = TracingSetup.Configure();

        var logger = Log.Logger;
        var calculator = new CapitalGainsCalculator();
        
        var runner = new StdInRunner(calculator, logger);

        runner.Run();
    }
}