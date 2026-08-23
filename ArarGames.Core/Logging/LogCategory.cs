namespace ArarGames.Core.Logging;

/// <summary>
/// Specifies the category of a log message for filtering and organization.
/// </summary>
public enum LogCategory
{
    /// <summary>General system log.</summary>
    General = 0,
    /// <summary>Audio system log.</summary>
    Audio = 1,
    /// <summary>Screen or rendering system log.</summary>
    Screen = 2,
    /// <summary>Input handling system log.</summary>
    Input = 3,
    /// <summary>Save data and persistence system log.</summary>
    Save = 4,
    /// <summary>Networking system log.</summary>
    Network = 5,
    /// <summary>Performance and profiling log.</summary>
    Performance = 6
}
