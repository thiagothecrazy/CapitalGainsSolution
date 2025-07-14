using System.Text.Json;
using System.Text.Json.Serialization;

namespace CapitalGains.Cli.Infrastructure
{
    public class JsonSetup
    {
        public static readonly JsonSerializerOptions Default = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            WriteIndented = false
        };
    }
}
