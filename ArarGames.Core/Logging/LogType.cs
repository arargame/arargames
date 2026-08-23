namespace ArarGames.Core.Logging;

/// <summary>
/// Specifies the severity level of a log message.
/// </summary>
public enum LogType
{
    /// <summary>Unspecified log level.</summary>
    Unspecified = 0,
    /// <summary>Debug information, typically used for troubleshooting.</summary>
    Debug = 1,
    /// <summary>Informational message highlighting the progress of the application.</summary>
    Info = 2,
    /// <summary>A warning message indicating a potential issue.</summary>
    Warning = 3,
    /// <summary>An error message indicating a failure.</summary>
    Error = 4
}
