namespace CapitalGains.Domain.Models;
public class PortfolioState
{
    public int Quantity { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal AccumulatedLoss { get; set; }
}
