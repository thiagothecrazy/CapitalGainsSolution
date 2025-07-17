using CapitalGains.Domain.Interface;
using CapitalGains.Domain.Models;

namespace CapitalGains.App.Operations;

public class SellOperationHandler : ITradeOperationHandler
{
    public OperationType Type => OperationType.Sell;

    private const decimal TaxExemptionThreshold = 20000.0m;
    private const decimal TaxRate = 0.20m;
    private const int RoundingPrecision = 2;

    public TaxResult Apply(Trade trade, PortfolioState state)
    {
        if (trade.Quantity > state.Quantity)
        {
            return new TaxResult(0.0m, "Can't sell more stocks than you have");
        }

        var totalSale = trade.Total();
        var costBasis = trade.Quantity * state.AveragePrice;
        var profit = totalSale - costBasis;

        state.Quantity -= trade.Quantity;

        if (profit < 0)
        {
            AccumulateLoss(profit, state);
            return new TaxResult(0.0m);
        }

        if (IsExemptFromTax(totalSale, state))
        {
            return new TaxResult(0.0m);
        }

        var taxableProfit = CalculateTaxableProfit(profit, state);
        UpdateAccumulatedLoss(profit, state);

        var tax = CalculateTax(taxableProfit);

        return new TaxResult(tax);
    }
    
    private void AccumulateLoss(decimal profit, PortfolioState state)
        => state.AccumulatedLoss += Math.Abs(profit);

    private static bool IsExemptFromTax(decimal totalSale, PortfolioState state)
        => totalSale <= TaxExemptionThreshold;

    private decimal CalculateTaxableProfit(decimal profit, PortfolioState state)
        => Math.Max(0, profit - state.AccumulatedLoss);

    private void UpdateAccumulatedLoss(decimal profit, PortfolioState state)
        => state.AccumulatedLoss = Math.Max(0, state.AccumulatedLoss - profit);

    private static decimal CalculateTax(decimal taxableProfit)
        => Math.Round(taxableProfit * TaxRate, RoundingPrecision, MidpointRounding.AwayFromZero);

}