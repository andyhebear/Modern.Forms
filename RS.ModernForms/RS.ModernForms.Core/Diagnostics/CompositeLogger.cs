namespace RS.ModernForms.Core.Diagnostics;

public sealed class CompositeLogger : IDiagnosticLogger
{
    private readonly IDiagnosticLogger[] _loggers;

    public CompositeLogger(params IDiagnosticLogger[] loggers)
    {
        _loggers = loggers ?? throw new ArgumentNullException(nameof(loggers));
    }

    public LogLevel MinimumLevel
    {
        get
        {
            if (_loggers.Length == 0) return LogLevel.None;
            var min = _loggers[0].MinimumLevel;
            for (int i = 1; i < _loggers.Length; i++)
                if (_loggers[i].MinimumLevel < min)
                    min = _loggers[i].MinimumLevel;
            return min;
        }
        set
        {
            foreach (var logger in _loggers)
                logger.MinimumLevel = value;
        }
    }

    public bool IsEnabled(LogLevel level)
    {
        foreach (var logger in _loggers)
            if (logger.IsEnabled(level))
                return true;
        return false;
    }

    public void Log(LogLevel level, string message, Exception? exception = null)
    {
        foreach (var logger in _loggers)
            logger.Log(level, message, exception);
    }

    public void LogTrace(string message) => Log(LogLevel.Trace, message);
    public void LogDebug(string message) => Log(LogLevel.Debug, message);
    public void LogInformation(string message) => Log(LogLevel.Information, message);
    public void LogWarning(string message) => Log(LogLevel.Warning, message);
    public void LogError(string message, Exception? exception = null) => Log(LogLevel.Error, message, exception);
    public void LogCritical(string message, Exception? exception = null) => Log(LogLevel.Critical, message, exception);
}
