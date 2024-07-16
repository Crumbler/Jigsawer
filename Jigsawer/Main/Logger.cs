using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Jigsawer.Main;

public static class Logger {
    private static readonly ILoggerFactory loggerFactory =
        LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

    private static readonly ILogger logger = loggerFactory.CreateLogger("Global");

    [Conditional("DEBUG")]
    public static void LogDebug(string message) {
        logger.LogDebug("{Message}", message);
    }

    [Conditional("DEBUG")]
    public static void LogError(string message) {
        logger.LogError("{Message}", message);
    }
}
