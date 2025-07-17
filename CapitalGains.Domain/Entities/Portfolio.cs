using CapitalGains.Domain.Interface;
using CapitalGains.Domain.Models;

namespace CapitalGains.Domain.Entities;

public class Portfolio(IEnumerable<ITradeOperationHandler> handlers)
{
    private readonly Dictionary<OperationType, ITradeOperationHandler> _handlers = handlers.ToDictionary(h => h.Type);
    private readonly PortfolioState _state = new();

    public TaxResult Apply(Trade trade)
    {
        if (_handlers.TryGetValue(trade.Type, out var handler))
            return handler.Apply(trade, _state);

        throw new InvalidOperationException($"Handler não encontrado para operação: {trade.Type}");
    }
}
