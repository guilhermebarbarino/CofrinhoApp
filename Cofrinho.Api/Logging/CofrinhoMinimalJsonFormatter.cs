using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Serilog.Events;
using Serilog.Formatting;

namespace Cofrinho.Api.Logging;

[ExcludeFromCodeCoverage]
public sealed class CofrinhoMinimalJsonFormatter : ITextFormatter
{
    public void Format(LogEvent logEvent, TextWriter output)
    {
        // Só loga eventos do request logging (request / request_failed)
        var msg = logEvent.MessageTemplate.Text;
        if (msg != "request" && msg != "request_failed")
            return;

        string? GetProp(string name)
        {
            if (!logEvent.Properties.TryGetValue(name, out var v)) return null;

            // Ex: "GET" (ScalarValue)
            if (v is ScalarValue sv && sv.Value is not null)
                return sv.Value.ToString();

            return v.ToString()?.Trim('"');
        }

        var payload = new Dictionary<string, object?>
        {
            ["Method"] = GetProp("Method"),
            ["Path"] = GetProp("Path"),
            ["StatusCode"] = int.TryParse(GetProp("StatusCode"), out var sc) ? sc : null,
            ["ElapsedMs"] = long.TryParse(GetProp("ElapsedMs"), out var ms) ? ms : null,
            ["CorrelationId"] = GetProp("CorrelationId")
        };

        output.Write(JsonSerializer.Serialize(payload));
        output.WriteLine();
    }
}
