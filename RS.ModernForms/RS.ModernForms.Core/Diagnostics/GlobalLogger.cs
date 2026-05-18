namespace RS.ModernForms.Core.Diagnostics;

public static class GlobalLogger
{
    private static volatile IDiagnosticLogger _current = NullLogger.Instance;

    public static IDiagnosticLogger Current
    {
        get => _current;
    }

    public static void SetCurrent(IDiagnosticLogger logger)
    {
        _current = logger ?? NullLogger.Instance;
    }

    public static void Reset()
    {
        _current = NullLogger.Instance;
    }
}
