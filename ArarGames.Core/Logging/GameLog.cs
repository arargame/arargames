using System;
using System.IO;
using ArarGames.Core.Base;

namespace ArarGames.Core.Logging;

/// <summary>
/// Represents a structured log entry for the game.
/// </summary>
public class GameLog : BaseObject<GameLog>
{
    /// <summary>Gets the type of the log.</summary>
    public LogType Type { get; private set; }

    /// <summary>Gets the category of the log.</summary>
    public LogCategory Category { get; private set; }

    /// <summary>Gets the log message.</summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>Gets the stack trace information, if applicable.</summary>
    public string? StackInfo { get; private set; }

    /// <summary>Gets the associated payload or additional data.</summary>
    public object? Payload { get; private set; }

    /// <summary>
    /// Sets the log category fluently.
    /// </summary>
    public GameLog SetCategory(LogCategory category)
    {
        Category = category;
        return this;
    }

    /// <summary>
    /// Sets the log message fluently.
    /// </summary>
    public GameLog SetMessage(string message)
    {
        Message = message;
        return this;
    }

    /// <summary>
    /// Sets the log type fluently.
    /// </summary>
    public GameLog SetLogType(LogType type)
    {
        Type = type;
        return this;
    }

    /// <summary>
    /// Sets the stack information fluently.
    /// </summary>
    public GameLog SetStackInfo(string stackInfo)
    {
        StackInfo = stackInfo;
        return this;
    }
    
    /// <summary>
    /// Sets the payload fluently.
    /// </summary>
    public GameLog SetPayload(object payload)
    {
        Payload = payload;
        return this;
    }

    /// <summary>Creates an info log entry.</summary>
    public static GameLog Info(string message) => Create(LogType.Info, message);
    
    /// <summary>Creates a warning log entry.</summary>
    public static GameLog Warning(string message) => Create(LogType.Warning, message);
    
    /// <summary>Creates an error log entry.</summary>
    public static GameLog Error(string message) => Create(LogType.Error, message);
    
    /// <summary>Creates a debug log entry.</summary>
    public static GameLog Debug(string message) => Create(LogType.Debug, message);

    private static GameLog Create(LogType type, string message)
    {
        var log = new GameLog { Type = type, Message = message, Category = LogCategory.General };
        log.Initialize();
        return log;
    }

    /// <summary>
    /// Writes the log to the console with appropriate coloring.
    /// </summary>
    public void ConsoleWriter()
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = Type switch
        {
            LogType.Error => ConsoleColor.Red,
            LogType.Warning => ConsoleColor.Yellow,
            LogType.Info => ConsoleColor.Cyan,
            LogType.Debug => ConsoleColor.Gray,
            _ => ConsoleColor.White
        };

        Console.WriteLine($"[{CreatedAt:HH:mm:ss}] [{Type}] [{Category}] {Message}");
        if (!string.IsNullOrEmpty(StackInfo))
            Console.WriteLine(StackInfo);

        Console.ForegroundColor = prevColor;
    }

    /// <summary>
    /// Writes the log entry to a daily file in the specified directory.
    /// </summary>
    public void FileWriter(string logDirectory)
    {
        try
        {
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            string fileName = Path.Combine(logDirectory, $"{DateTime.UtcNow:yyyy-MM-dd}.log");
            string line = $"[{CreatedAt:O}] [{Type}] [{Category}] {Message}";
            
            if (!string.IsNullOrEmpty(StackInfo))
                line += Environment.NewLine + StackInfo;

            File.AppendAllText(fileName, line + Environment.NewLine);
        }
        catch
        {
            // Fail silently for file writer to avoid crashing the game loop
        }
    }
}
