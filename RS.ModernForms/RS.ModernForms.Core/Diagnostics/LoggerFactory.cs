namespace RS.ModernForms.Core.Diagnostics;

public static class LoggerFactory
{
    public static DiagnosticLogger CreateDefault(LogLevel minimumLevel = LogLevel.Information)
    {
        return new DiagnosticLogger { MinimumLevel = minimumLevel };
    }

    public static LogFiler CreateFileLogger(string filePath, LogLevel minimumLevel = LogLevel.Debug)
    {
        return new LogFiler(filePath, minimumLevel);
    }

    public static CompositeLogger CreateComposite(LogLevel minimumLevel, params IDiagnosticLogger[] loggers)
    {
        var composite = new CompositeLogger(loggers);
        composite.MinimumLevel = minimumLevel;
        return composite;
    }
}
