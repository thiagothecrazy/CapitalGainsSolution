using CapitalGains.Domain.Interface;
using CapitalGains.Domain.Models;

namespace CapitalGains.App.Operations;

public class BuyOperationHandler : ITradeOperationHandler
{
    public OperationType Type => OperationType.Buy;

    public TaxResult Apply(Trade trade, PortfolioState state)
    {
        var totalCost = (state.Quantity * state.AveragePrice) + trade.Total();
        state.Quantity += trade.Quantity;
        state.AveragePrice = state.Quantity > 0 ? totalCost / state.Quantity : 0.0m;

        return new TaxResult(0.0m);
    }
}
