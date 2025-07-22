using CapitalGains.App.Interfaces;
using CapitalGains.Domain.Entities;
using CapitalGains.Domain.Interface;
using CapitalGains.Domain.Models;

namespace CapitalGains.App.Services;

public class CapitalGainsCalculator(IEnumerable<ITradeOperationHandler> handlers) : ICapitalGainsCalculator
{
    public IReadOnlyList<TaxResult> Calculate(IReadOnlyList<Trade> trades)
    {
        var results = new List<TaxResult>();
        var portfolio = new Portfolio(handlers);

        foreach (var trade in trades)
        {
            var taxResult = portfolio.Apply(trade);
            results.Add(taxResult);
        }

        return results;
    }
}