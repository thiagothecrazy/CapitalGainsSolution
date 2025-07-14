using CapitalGains.App.Interfaces;
using CapitalGains.App.Mapping;
using CapitalGains.Cli.Infrastructure;
using Serilog;
using System.Text.Json;

namespace CapitalGains.Cli;

public class StdInRunner(ICapitalGainsCalculator calculator, ILogger logger, TextReader? input = null, TextWriter? output = null)
{
    private readonly ICapitalGainsCalculator _calculator = calculator;
    private readonly ILogger _logger = logger;
    private readonly TextReader _input = input ?? Console.In;
    private readonly TextWriter _output = output ?? Console.Out;

    public void Run()
    {
        string? line;
        while (!string.IsNullOrEmpty(line = _input.ReadLine()))
        {
            _logger.Information("Read line: {line}", line);

            try
            {
                var output = ProcessLine(line);
                if (output != null)
                {
                    _output.WriteLine(output);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error processing input: {line}", line);
            }
        }
    }

    private string? ProcessLine(string line)
    {
        var operations = JsonSerializer.Deserialize<List<OperationDto>>(line, JsonSetup.Default);
        if (operations == null)
        {
            _logger.Warning("Valid JSON line but no operations: {line}", line);
            return null;
        }

        var trades = operations.Select(o => o.ToTrade()).ToList();
        var result = _calculator.Calculate(trades);

        return JsonSerializer.Serialize(result, JsonSetup.Default);
    }
}