using CapitalGains.App.Interfaces;
using CapitalGains.Domain.Entities;
using CapitalGains.Domain.Interface;
using CapitalGains.Domain.Models;

namespace CapitalGains.App.Services;

public class CapitalGainsCalculator : ICapitalGainsCalculator
{
    private readonly IEnumerable<ITradeOperationHandler> _handlers;

    public CapitalGainsCalculator(IEnumerable<ITradeOperationHandler> handlers)
    {
        _handlers = handlers;
    }

    public IReadOnlyList<TaxResult> Calculate(IReadOnlyList<Trade> trades)
    {
        var results = new List<TaxResult>();
        var portfolio = new Portfolio(_handlers);

        foreach (var trade in trades)
        {
            var taxResult = portfolio.Apply(trade);
            results.Add(taxResult);
        }

        return results;
    }
}