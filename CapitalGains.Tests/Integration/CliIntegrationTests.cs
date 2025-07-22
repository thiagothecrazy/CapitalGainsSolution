using CapitalGains.App.Mapping;
using CapitalGains.App.Operations;
using CapitalGains.App.Services;
using CapitalGains.Cli;
using CapitalGains.Cli.Infrastructure;
using CapitalGains.Domain.Interface;
using CapitalGains.Domain.Models;
using CapitalGains.Tests.Unit;
using FluentAssertions;
using System.Text.Json;

namespace CapitalGains.Tests.Integration;

public class CliIntegrationTests
{
    

    [Fact(DisplayName = "Formato JSON retornado com sucesso")]
    public void Should_Process_Valid_CorrectJson_Output()
    {
        // Arrange
        var inputJson = ToJson(
            [
                new OperationDto { Operation = OperationType.Buy, UnitCost = 10.0m, Quantity = 10000 },
                new OperationDto { Operation = OperationType.Sell, UnitCost = 20.0m, Quantity = 5000 }
            ]
        );

        var expected = "[{\"tax\":0.00},{\"tax\":10000.00}]" + "\r\n";
       
        var input = new StringReader(inputJson + "\n");
        var output = new StringWriter();
        var runner = CreateRunner(input, output);

        // Act
        runner.Run();

        // Assert
        var actual = output.ToString();
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "Processar entrada válida com resultado correto")]
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
        var actual = DeserializeOutput(output);
        actual.Should().HaveCount(expected.Count);
        actual[0].Should().BeEquivalentTo(expected[0]);
    }

    [Fact(DisplayName = "Processar varías linhas independente")]
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

    [Fact(DisplayName = "Formato JSON retornado com venda inválida")]
    public void Should_Process_SellOperationError_CorrectJson_Output()
    {
        // Arrange
        var inputJson = ToJson(
            [
                new OperationDto { Operation = OperationType.Buy, UnitCost = 10000.00m, Quantity = 10 },
                new OperationDto { Operation = OperationType.Sell, UnitCost = 11000.00m, Quantity = 20 }
            ]
        );

        var expected = "[{\"tax\":0.00},{\"tax\":0.00,\"error\":\"Can't sell more stocks than you have\"}]" + "\r\n";

        var input = new StringReader(inputJson + "\n");
        var output = new StringWriter();
        var runner = CreateRunner(input, output);

        // Act
        runner.Run();

        // Assert
        var actual = output.ToString();
        actual.Should().Be(expected);
    }

    private static StdInRunner CreateRunner(StringReader input, StringWriter output)
    {
        var logger = new TestLogger();

        // Handlers reais (ou mocks, se preferir)
        var handlers = new ITradeOperationHandler[]
        {
            new BuyOperationHandler(),
            new SellOperationHandler(),
        };

        var calculator = new CapitalGainsCalculator(handlers);
        return new StdInRunner(calculator, logger, input, output);
    }

    private static List<List<TaxResult>> DeserializeOutput(StringWriter output)
    {
        var lines = output.ToString().Trim().Split('\n');
        return lines
            .Select(line => JsonSerializer.Deserialize<List<TaxResult>>(line, JsonSetup.Default)!)
            .ToList();
    }

    private static string ToJson(params Object[][] inputs)
    {
        return string.Join('\n', inputs.Select(e => JsonSerializer.Serialize(e.ToList(), JsonSetup.Default)));
    }
}
