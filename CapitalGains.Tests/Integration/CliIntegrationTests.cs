using CapitalGains.App.Mapping;
using CapitalGains.App.Services;
using CapitalGains.Cli;
using CapitalGains.Domain.Models;
using CapitalGains.Tests.Unit;
using FluentAssertions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CapitalGains.Tests.Integration;

public class CliIntegrationTests
{
    [Fact]
    public void Should_Process_Valid_Input_And_Produce_Correct_Output()
    {
        // Arrange
        var inputJson = ToJson(
            [
                new OperationDto { Operation = OperationType.Buy, UnitCost = 10.0m, Quantity = 10000 },
                new OperationDto { Operation = OperationType.Sell, UnitCost = 20.0m, Quantity = 5000 }
            ]
        );
        var expected = new List<List<TaxResult>> {
            new () {
                new TaxResult(0.0m),
                new TaxResult(10000.0m)
            }
        };

        var input = new StringReader(inputJson + "\n");
        var output = new StringWriter();
        var runner = CreateRunner(input, output);

        // Act
        runner.Run();

        // Assert
        // Assert
        var actual = DeserializeOutput(output);
        actual.Should().HaveCount(expected.Count);
        actual[0].Should().BeEquivalentTo(expected[0]);
    }

    [Fact]
    public void Should_Process_Multiple_Independent_Lines()
    {
        // Arrange
        var inputJson = ToJson(
            [
                new OperationDto { Operation = OperationType.Buy, UnitCost = 10.0m, Quantity = 100 },
                new OperationDto { Operation = OperationType.Sell, UnitCost = 15.0m, Quantity = 50 }
            ],
            [
                new OperationDto { Operation = OperationType.Buy, UnitCost = 10.0m, Quantity = 10000 },
                new OperationDto { Operation = OperationType.Sell, UnitCost = 20.0m, Quantity = 5000 }
            ]
        );
        var expected = new List<List<TaxResult>> {
            new () {
                new TaxResult(0.0m),
                new TaxResult(0.0m)
            },
            new () {
                new TaxResult(0.0m),
                new TaxResult(10000.0m)
            }
        };

        var input = new StringReader(inputJson + "\n");
        var output = new StringWriter();
        var runner = CreateRunner(input, output);

        // Act
        runner.Run();

        // Assert
        var actual = DeserializeOutput(output);
        actual.Should().HaveCount(expected.Count);
        actual[0].Should().BeEquivalentTo(expected[0]);
        actual[1].Should().BeEquivalentTo(expected[1]);
    }

    private static StdInRunner CreateRunner(StringReader input, StringWriter output)
    {
        var logger = new TestLogger();
        var calculator = new CapitalGainsCalculator();
        return new StdInRunner(calculator, logger, input, output);
    }

    private static List<List<TaxResult>> DeserializeOutput(StringWriter output)
    {
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var lines = output.ToString().Trim().Split('\n');
        return lines
            .Select(line => JsonSerializer.Deserialize<List<TaxResult>>(line, options)!)
            .ToList();
    }

    private static string ToJson(params Object[][] inputs)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        return string.Join('\n', inputs.Select(e => JsonSerializer.Serialize(e.ToList(), options)));
    }
}
