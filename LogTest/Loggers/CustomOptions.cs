using Microsoft.Extensions.Logging.Console;

namespace LogTest.Loggers;

public sealed class CustomOptions : ConsoleFormatterOptions
{
    public string? CustomPrefix { get; set; }
}