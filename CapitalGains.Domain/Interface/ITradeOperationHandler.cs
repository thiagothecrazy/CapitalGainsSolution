using CapitalGains.Domain.Models;

namespace CapitalGains.Domain.Interface;

public interface ITradeOperationHandler
{
    OperationType Type { get; }
    TaxResult Apply(Trade trade, PortfolioState state);
}
