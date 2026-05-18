using System.Diagnostics;

namespace RS.ModernForms.Core.Diagnostics;

public sealed class LogFiler : IDiagnosticLogger, IDisposable
{
    private readonly object _lock = new();
    private StreamWriter? _writer;
    private readonly string _filePath;
    private bool _fileCreationFailed;
    private bool _disposed;

    public LogLevel MinimumLevel { get; set; } = LogLevel.Debug;
    public string FilePath => _filePath;

    public LogFiler(string filePath, LogLevel minimumLevel = LogLevel.Debug)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        MinimumLevel = minimumLevel;
    }

    public bool IsEnabled(LogLevel level) => level >= MinimumLevel;

    public void Log(LogLevel level, string message, Exception? exception = null)
    {
        if (!IsEnabled(level)) return;

        var formatted = FormatMessage(level, message, exception);

        lock (_lock)
        {
            Debug.WriteLine(formatted);
            Console.Error.WriteLine(formatted);

            if (!_fileCreationFailed)
            {
                try
                {
                    EnsureWriterInitialized();
                    _writer?.WriteLine(formatted);
                }
                catch (Exception ex)
                {
                    _fileCreationFailed = true;
                    var warning = FormatMessage(LogLevel.Warning, $"日志文件写入失败，降级为仅控制台输出: {ex.Message}", null);
                    Console.Error.WriteLine(warning);
                }
            }
        }
    }

    private string FormatMessage(LogLevel level, string message, Exception? exception)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var levelStr = level switch
        {
            LogLevel.Trace => "TRC",
            LogLevel.Debug => "DBG",
            LogLevel.Information => "INF",
            LogLevel.Warning => "WRN",
            LogLevel.Error => "ERR",
            LogLevel.Critical => "CRT",
            _ => "???"
        };

        var formatted = $"[{timestamp}] [{levelStr}] {message}";
        if (exception is not null)
            formatted += $"\n  异常: {exception.GetType().Name}: {exception.Message}\n  堆栈: {exception.StackTrace}";
        return formatted;
    }

    private void EnsureWriterInitialized()
    {
        if (_writer is not null || _fileCreationFailed) return;

        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        _writer = new StreamWriter(_filePath, append: true, System.Text.Encoding.UTF8) { AutoFlush = true };
    }

    public void LogTrace(string message) => Log(LogLevel.Trace, message);
    public void LogDebug(string message) => Log(LogLevel.Debug, message);
    public void LogInformation(string message) => Log(LogLevel.Information, message);
    public void LogWarning(string message) => Log(LogLevel.Warning, message);
    public void LogError(string message, Exception? exception = null) => Log(LogLevel.Error, message, exception);
    public void LogCritical(string message, Exception? exception = null) => Log(LogLevel.Critical, message, exception);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        lock (_lock)
        {
            _writer?.Dispose();
            _writer = null;
        }
    }
}
