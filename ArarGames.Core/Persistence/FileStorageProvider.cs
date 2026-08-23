using System.IO;

namespace ArarGames.Core.Persistence;

/// <summary>
/// A file system-based implementation of the storage provider.
/// </summary>
public class FileStorageProvider : IStorageProvider
{
    private readonly string _saveDirectory;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileStorageProvider"/> class.
    /// </summary>
    /// <param name="saveDirectory">The root directory for saved files.</param>
    public FileStorageProvider(string saveDirectory)
    {
        _saveDirectory = saveDirectory;
        if (!Directory.Exists(_saveDirectory))
        {
            Directory.CreateDirectory(_saveDirectory);
        }
    }

    /// <inheritdoc />
    public bool Exists(string path) => File.Exists(GetFullPath(path));

    /// <inheritdoc />
    public Stream OpenRead(string path) => File.OpenRead(GetFullPath(path));

    /// <inheritdoc />
    public Stream OpenWrite(string path)
    {
        var fullPath = GetFullPath(path);
        var directory = Path.GetDirectoryName(fullPath);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        return new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
    }

    /// <inheritdoc />
    public void Move(string source, string destination, bool overwrite) => 
        File.Move(GetFullPath(source), GetFullPath(destination), overwrite);

    /// <inheritdoc />
    public void Delete(string path)
    {
        var fullPath = GetFullPath(path);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    /// <inheritdoc />
    public string GetSaveDirectory() => _saveDirectory;

    private string GetFullPath(string relativePath)
    {
        if (Path.IsPathRooted(relativePath)) return relativePath;
        return Path.Combine(_saveDirectory, relativePath);
    }
}
