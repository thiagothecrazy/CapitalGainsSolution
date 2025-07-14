using CapitalGains.App.Interfaces;
using CapitalGains.App.Services;
using CapitalGains.Cli.Infrastructure;
using CapitalGains.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CapitalGains.Cli;

class Program
{
    static void Main()
    {
        LoggingSetup.Configure();
        using var tracerProvider = TracingSetup.Configure();
        var logger = Log.Logger;

        var services = new ServiceCollection();
        ConfigureServices(services);
        var provider = services.BuildServiceProvider();
                
        var calculator = provider.GetRequiredService<ICapitalGainsCalculator>();
        var runner = new StdInRunner(calculator, logger);

        runner.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<Portfolio>();

        // Handlers
        //services.AddScoped<ITradeOperationHandler, BuyOperationHandler>();
        //services.AddScoped<ITradeOperationHandler, SellOperationHandler>();
        //services.AddScoped<ITradeOperationHandler, DividendOperationHandler>();

        // Calculator depende dos handlers
        services.AddScoped<ICapitalGainsCalculator, CapitalGainsCalculator>();
    }
}