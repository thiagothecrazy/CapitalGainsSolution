using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace CapitalGains.Tests.Unit;

public class TestLogger : ILogger
{
    public void BindMessageTemplate(string messageTemplate, object[] propertyValues, out MessageTemplate parsedTemplate, out IEnumerable<LogEventProperty> boundProperties) => throw new NotImplementedException();
    public void BindProperty(string propertyName, object value, bool destructureObjects, out LogEventProperty property) => throw new NotImplementedException();
    public void Debug(string messageTemplate) { }
    public void Debug<T>(string messageTemplate, T propertyValue) { }
    public void Error(Exception exception, string messageTemplate, params object[] propertyValues) { }
    public void Information(string messageTemplate, params object[] propertyValues) { }
    public void Warning(string messageTemplate) { }
    public void Write(LogEvent logEvent) { }
    public void Write(LogEventLevel level, string messageTemplate) { }
    public void Write<T>(LogEventLevel level, string messageTemplate, T propertyValue) { }
    public void Write(LogEventLevel level, string messageTemplate, params object[] propertyValues) { }
    public bool IsEnabled(LogEventLevel level) => false;
    public ILogger ForContext(ILogEventEnricher enricher) => this;
    public ILogger ForContext(IEnumerable<ILogEventEnricher> enrichers) => this;
    public ILogger ForContext(string propertyName, object value, bool destructureObjects = false) => this;
    public ILogger ForContext<TSource>() => this;
    public ILogger ForContext(Type source) => this;
}