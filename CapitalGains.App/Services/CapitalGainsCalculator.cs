using CapitalGains.App.Interfaces;
using CapitalGains.Domain.Entities;
using CapitalGains.Domain.Models;

namespace CapitalGains.App.Services;

public class CapitalGainsCalculator : ICapitalGainsCalculator
{
    public IReadOnlyList<TaxResult> Calculate(IReadOnlyList<Trade> trades)
    {
        var results = new List<TaxResult>();
        var portfolio = new Portfolio();

        foreach (var trade in trades)
        {
            var tax = portfolio.Apply(trade);
            results.Add(new TaxResult(tax));
        }

        return results;
    }
}