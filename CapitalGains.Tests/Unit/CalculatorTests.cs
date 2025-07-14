using CapitalGains.App.Services;
using CapitalGains.Domain.Models;
using FluentAssertions;

namespace CapitalGains.Tests.Unit;

public class CalculatorTests
{
    [Fact]
    public void Should_Apply_Tax_When_Sale_Has_Profit_Above_Threshold()
    {
        // Arrange
        var calculator = CreateDefaultCalculator();
        var trades = new List<Trade>
        {
            new(OperationType.Buy, 10.0m, 10000),
            new(OperationType.Sell, 20.0m, 5000)
        };

        // Act
        var result = calculator.Calculate(trades);

        // Assert
        result.Should().HaveCount(2);
        result[0].Tax.Should().Be(0.0m);
        result[1].Tax.Should().Be(10000.0m);
    }

    [Fact]
    public void Should_Apply_No_Tax_When_Sale_Under_Threshold()
    {
        // Arrange
        var calculator = CreateDefaultCalculator();
        var trades = new List<Trade>
        {
            new(OperationType.Buy, 10.0m, 100),
            new(OperationType.Sell, 15.0m, 50)
        };

        // Act
        var result = calculator.Calculate(trades);

        // Assert
        result[0].Tax.Should().Be(0.0m);
        result[1].Tax.Should().Be(0.0m);
    }

    [Fact]
    public void Should_Accumulate_Loss_And_Deduct_From_Future_Profit()
    {
        // Arrange
        var calculator = CreateDefaultCalculator();
        var trades = new List<Trade>
        {
            new(OperationType.Buy, 10.0m, 10000),
            new(OperationType.Sell, 5.0m, 5000), // loss = 25000
            new(OperationType.Sell, 20.0m, 3000) // profit = 30000 - 25000 = 5000 * 20% = 1000
        };

        // Act
        var result = calculator.Calculate(trades);

        //Assert
        result[0].Tax.Should().Be(0.0m);
        result[1].Tax.Should().Be(0.0m);
        result[2].Tax.Should().Be(1000.0m);
    }

    [Fact(DisplayName = "Caso #1 - Vendas com valor total abaixo de 20 mil")]
    public void Should_Not_Apply_Tax_When_Sale_Is_Under_Threshold()
    {
        // Arrange
        var calculator = CreateDefaultCalculator();
        var trades = new List<Trade>
            {
                new(OperationType.Buy, 10.00m, 100),
                new(OperationType.Sell, 15.00m, 50),
                new(OperationType.Sell, 15.00m, 50)
            };

        // Act
        var result = calculator.Calculate(trades);

        // Assert
        result.Should().HaveCount(3);
        result[0].Tax.Should().Be(0.0m);
        result[1].Tax.Should().Be(0.0m);
        result[2].Tax.Should().Be(0.0m);
    }

    [Fact(DisplayName = "Caso #2 - Venda com lucro e prejuízo deduzido")]
    public void Should_Apply_Tax_Only_When_Lucrative_Sale_Exceeds_Threshold()
    {
        // Arrange
        var calculator = CreateDefaultCalculator();
        var trades = new List<Trade>
            {
                new(OperationType.Buy, 10.00m, 10000),
                new(OperationType.Sell, 20.00m, 5000),
                new(OperationType.Sell, 5.00m, 5000)
            };

        // Act
        var result = calculator.Calculate(trades);

        // Assert
        result[0].Tax.Should().Be(0.0m);
        result[1].Tax.Should().Be(10000.0m);
        result[2].Tax.Should().Be(0.0m);
    }

    [Fact(DisplayName = "Caso #3 - Dedução de prejuízo anterior")]
    public void Should_Apply_Tax_After_Deducting_Previous_Losses()
    {
        // Arrange
        var calculator = CreateDefaultCalculator();
        var trades = new List<Trade>
            {
                new(OperationType.Buy, 10.00m, 10000),
                new(OperationType.Sell, 5.00m, 5000),
                new(OperationType.Sell, 20.00m, 3000)
            };

        // Act
        var result = calculator.Calculate(trades);

        // Assert
        result[0].Tax.Should().Be(0.0m);
        result[1].Tax.Should().Be(0.0m);
        result[2].Tax.Should().Be(1000.0m);
    }

    [Fact(DisplayName = "Caso #4 - Preço médio igual ao valor de venda")]
    public void Should_Not_Apply_Tax_When_Sale_Equals_WeightedAverage()
    {
        // Arrange
        var calculator = CreateDefaultCalculator();
        var trades = new List<Trade>
            {
                new(OperationType.Buy, 10.00m, 10000),
                new(OperationType.Buy, 25.00m, 5000),
                new(OperationType.Sell, 15.00m, 10000)
            };

        // Act
        var result = calculator.Calculate(trades);

        // Assert
        result[0].Tax.Should().Be(0.0m);
        result[1].Tax.Should().Be(0.0m);
        result[2].Tax.Should().Be(0.0m);
    }

    [Fact(DisplayName = "Caso #5 - Venda lucrativa após venda neutra")]
    public void Should_Apply_Tax_Only_On_Lucrative_Sale_After_Neutral()
    {
        // Arrange
        var calculator = CreateDefaultCalculator();
        var trades = new List<Trade>
            {
                new(OperationType.Buy, 10.00m, 10000),
                new(OperationType.Buy, 25.00m, 5000),
                new(OperationType.Sell, 15.00m, 10000),
                new(OperationType.Sell, 25.00m, 5000)
            };

        // Act
        var result = calculator.Calculate(trades);

        // Assert
        result[0].Tax.Should().Be(0.0m);
        result[1].Tax.Should().Be(0.0m);
        result[2].Tax.Should().Be(0.0m);
        result[3].Tax.Should().Be(10000.0m);
    }

    [Fact(DisplayName = "Caso #6 - Dedução sequencial de prejuízos")]
    public void Should_Deduct_Sequential_Losses_Before_Taxing()
    {
        // Arrange
        var calculator = CreateDefaultCalculator();
        var trades = new List<Trade>
            {
                new(OperationType.Buy, 10.00m, 10000),
                new(OperationType.Sell, 2.00m, 5000),
                new(OperationType.Sell, 20.00m, 2000),
                new(OperationType.Sell, 20.00m, 2000),
                new(OperationType.Sell, 25.00m, 1000)
            };

        // Act
        var result = calculator.Calculate(trades);

        // Assert
        result[0].Tax.Should().Be(0.0m);
        result[1].Tax.Should().Be(0.0m);
        result[2].Tax.Should().Be(0.0m);
        result[3].Tax.Should().Be(0.0m);
        result[4].Tax.Should().Be(3000.0m);
    }

    [Fact(DisplayName = "Caso #7 - Reinício de estado entre simulações")]
    public void Should_Reset_Portfolio_State_Between_Simulations()
    {
        // Arrange
        var calculator1 = CreateDefaultCalculator();
        var trades1 = new List<Trade>
            {
                new(OperationType.Buy, 10.00m, 10000),
                new(OperationType.Sell, 2.00m, 5000),
                new(OperationType.Sell, 20.00m, 2000),
                new(OperationType.Sell, 20.00m, 2000),
                new(OperationType.Sell, 25.00m, 1000)
            };

        var calculator2 = CreateDefaultCalculator();
        var trades2 = new List<Trade>
            {
                new(OperationType.Buy, 20.00m, 10000),
                new(OperationType.Sell, 15.00m, 5000),
                new(OperationType.Sell, 30.00m, 4350),
                new(OperationType.Sell, 30.00m, 650)
            };

        // Act
        var result1 = calculator1.Calculate(trades1);
        var result2 = calculator2.Calculate(trades2);

        // Assert
        result1[4].Tax.Should().Be(3000.0m);
        result2[2].Tax.Should().Be(3700.0m);
        result2[3].Tax.Should().Be(0.0m);
    }

    [Fact(DisplayName = "Caso #8 - Grandes lucros sucessivos")]
    public void Should_Apply_Tax_On_Successive_Large_Lucrative_Sales()
    {
        // Arrange
        var calculator = CreateDefaultCalculator();
        var trades = new List<Trade>
            {
                new(OperationType.Buy, 10.00m, 10000),
                new(OperationType.Sell, 50.00m, 10000),
                new(OperationType.Buy, 20.00m, 10000),
                new(OperationType.Sell, 50.00m, 10000)
            };

        // Act
        var result = calculator.Calculate(trades);

        // Assert
        result[0].Tax.Should().Be(0.0m);
        result[1].Tax.Should().Be(80000.0m);
        result[2].Tax.Should().Be(0.0m);
        result[3].Tax.Should().Be(60000.0m);
    }

    [Fact(DisplayName = "Caso #9 - Múltiplas compras e vendas com prejuízo acumulado")]
    public void Should_Accumulate_Loss_And_Apply_It_Before_Tax()
    {
        // Arrange
        var calculator = CreateDefaultCalculator();
        var trades = new List<Trade>
            {
                new(OperationType.Buy, 5000.00m, 10),
                new(OperationType.Sell, 4000.00m, 5),
                new(OperationType.Buy, 15000.00m, 5),
                new(OperationType.Buy, 4000.00m, 2),
                new(OperationType.Buy, 23000.00m, 2),
                new(OperationType.Sell, 20000.00m, 1),
                new(OperationType.Sell, 12000.00m, 10),
                new(OperationType.Sell, 15000.00m, 3)
            };

        // Act
        var result = calculator.Calculate(trades);

        // Assert
        result[0].Tax.Should().Be(0.0m);
        result[1].Tax.Should().Be(0.0m);
        result[2].Tax.Should().Be(0.0m);
        result[3].Tax.Should().Be(0.0m);
        result[4].Tax.Should().Be(0.0m);
        result[5].Tax.Should().Be(0.0m);
        result[6].Tax.Should().Be(1000.0m);
        result[7].Tax.Should().Be(2400.0m);
    }

    //[Theory]
    //[MemberData(nameof(GetData))]
    //public void Calculate_SaleHasProfitAboveThreshold_ApplyTax(Trade[] trades, TaxResult[] expected)
    //{
    //    // Arrange
    //    var calculator = CreateDefaultCalculator();

    //    // Act
    //    var result = calculator.Calculate(trades);

    //    // Assert
    //    result.Should().HaveCount(expected.Length);
    //    result[0].Value.Should().Be(expected[0].Value);
    //    result[1].Value.Should().Be(expected[1].Value);
    //}

    //public static IEnumerable<object[]> GetData()
    //{
    //    yield return new object[]
    //    {
    //        new List<Trade>
    //        {
    //            new(OperationType.Buy, 10.0m, 10000),
    //            new(OperationType.Sell, 20.0m, 5000)
    //        },
    //        new List<TaxResult>
    //        { 
    //            new(0.0m),
    //            new(10000.0m),
    //        }
    //    };
    //}

    private static CapitalGainsCalculator CreateDefaultCalculator() => new();

}