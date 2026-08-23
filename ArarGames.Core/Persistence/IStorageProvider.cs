using System.IO;

namespace ArarGames.Core.Persistence;

/// <summary>
/// Provides an abstraction over the file system for storage operations.
/// </summary>
public interface IStorageProvider
{
    /// <summary>
    /// Checks if a file exists at the specified path.
    /// </summary>
    bool Exists(string path);

    /// <summary>
    /// Opens an existing file for reading.
    /// </summary>
    Stream OpenRead(string path);

    /// <summary>
    /// Opens a file for writing, creating it if it does not exist or overwriting it if it does.
    /// </summary>
    Stream OpenWrite(string path);

    /// <summary>
    /// Moves a specified file to a new location, providing the option to specify a new file name.
    /// </summary>
    void Move(string source, string destination, bool overwrite);

    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    void Delete(string path);

    /// <summary>
    /// Gets the base directory for saving data.
    /// </summary>
    string GetSaveDirectory();
}
