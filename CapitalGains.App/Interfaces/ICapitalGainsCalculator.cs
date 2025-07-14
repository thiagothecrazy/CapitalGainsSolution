using CapitalGains.Domain.Models;

namespace CapitalGains.App.Interfaces;

public interface ICapitalGainsCalculator
{
    IReadOnlyList<TaxResult> Calculate(IReadOnlyList<Trade> trades);
}