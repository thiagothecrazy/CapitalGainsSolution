using System.Text.Json.Serialization;

namespace CapitalGains.Domain.Models;

public record TaxResult(
    [property: JsonPropertyName("tax")] decimal Tax, 
    [property: JsonPropertyName("error")] string? Error = null);
