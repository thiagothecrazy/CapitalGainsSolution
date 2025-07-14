using CapitalGains.Domain.Models;

namespace CapitalGains.Domain.Entities;

public class Portfolio
{
    private const decimal TaxExemptionThreshold = 20000.0m;
    private const decimal TaxRate = 0.20m;
    private const int RoundingPrecision = 2;
    private int _quantity;
    private decimal _averagePrice;
    private decimal _accumulatedLoss;

    public decimal Apply(Trade trade)
    {
        return trade.Type switch
        {
            OperationType.Buy => ProcessBuy(trade),
            OperationType.Sell => ProcessSell(trade),
            _ => throw new NotImplementedException()
        };
    }

    private decimal ProcessBuy(Trade trade)
    {
        var totalCost = (_quantity * _averagePrice) + trade.Total();
        _quantity += trade.Quantity;
        _averagePrice = _quantity > 0 ? totalCost / _quantity : 0.0m;

        return 0.0m;
    }

    private decimal ProcessSell(Trade trade)
    {
        var totalSale = trade.Total();
        var costBasis = trade.Quantity * _averagePrice;
        var profit = totalSale - costBasis;
        _quantity -= trade.Quantity;

        if (profit < 0)
        {
            AccumulateLoss(profit);
            return 0.0m;
        }

        if (IsExemptFromTax(totalSale))
        {
            return 0.0m;
        }

        var taxableProfit = CalculateTaxableProfit(profit);
        UpdateAccumulatedLoss(profit);

        return CalculateTax(taxableProfit);
    }
    private void AccumulateLoss(decimal profit)
        => _accumulatedLoss += Math.Abs(profit);

    private static bool IsExemptFromTax(decimal totalSale) 
        => totalSale <= TaxExemptionThreshold;

    private decimal CalculateTaxableProfit(decimal profit)
        => Math.Max(0, profit - _accumulatedLoss);

    private void UpdateAccumulatedLoss(decimal profit)
        => _accumulatedLoss = Math.Max(0, _accumulatedLoss - profit);

    private static decimal CalculateTax(decimal taxableProfit)
        => Math.Round(taxableProfit * TaxRate, RoundingPrecision, MidpointRounding.AwayFromZero);
}
