using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace LogTest.Loggers;

public class CustomFormatter(IOptionsMonitor<CustomOptions> options) : ConsoleFormatter(nameof(CustomFormatter))
{
    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var toWrite = new CustomLogEntry<TState>(logEntry).ToJson();
        textWriter.WriteLine(toWrite);
    }
}

public class CustomLogEntry<TState>(LogEntry<TState> logEntry)
{
    public string LogLevel { get; set; } = Enum.GetName(typeof(LogLevel), logEntry.LogLevel) ?? logEntry.LogLevel.ToString();
    public EventId EventId { get; set; } = logEntry.EventId;
    public string Category { get; set; } = logEntry.Category;
    public string Message { get; set; } = logEntry.Formatter(logEntry.State, logEntry.Exception);
    public Exception? Exception { get; set; } = logEntry.Exception;
    public string Ctimestamp { get; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

    public object? Payload { get; private set; }
    public string ToJson()
    {
        // Exclude the "{OriginalFormat}" property
        if (_logEntry.State is IEnumerable<KeyValuePair<string, object>> list)
        {
            var localPayload = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> kvp in list)
            {
                if (kvp.Key == "{OriginalFormat}")
                    continue;
                localPayload.TryAdd(kvp.Key, kvp.Value);
            }

            Payload = localPayload;
        }
        else
            Payload = null;

        return JsonSerializer.Serialize(this, options: new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }
    private readonly LogEntry<TState> _logEntry = logEntry;
}