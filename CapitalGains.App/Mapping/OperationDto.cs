using CapitalGains.Domain.Models;
using System.Text.Json.Serialization;

namespace CapitalGains.App.Mapping;

public class OperationDto
{
    [JsonPropertyName("operation")]
    public OperationType Operation { get; set; }

    [JsonPropertyName("unit-cost")]
    public decimal UnitCost { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    public Trade ToTrade() => new(Operation, UnitCost, Quantity);
}

