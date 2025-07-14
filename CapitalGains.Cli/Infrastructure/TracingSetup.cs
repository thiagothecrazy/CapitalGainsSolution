using OpenTelemetry;
using OpenTelemetry.Trace;

namespace CapitalGains.Cli.Infrastructure;

public static class TracingSetup
{
    public static TracerProvider Configure()
    {
        return Sdk.CreateTracerProviderBuilder()
            .AddSource("CapitalGains")
            .AddConsoleExporter()
            .Build();
    }
}