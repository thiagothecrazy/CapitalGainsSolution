namespace CapitalGains.Domain.Models;

public record Trade(OperationType Type, decimal UnitCost, int Quantity)
{
    public decimal Total() => UnitCost * Quantity;
}