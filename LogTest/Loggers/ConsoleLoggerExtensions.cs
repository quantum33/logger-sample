namespace LogTest.Loggers;

public static class ConsoleLoggerExtensions
{
    public static ILoggingBuilder AddCustomFormatter(this ILoggingBuilder builder, Action<CustomOptions> configure) =>
        builder
            .AddConsole(options => options.FormatterName = nameof(CustomFormatter))
            .AddConsoleFormatter<CustomFormatter, CustomOptions>(configure);
}