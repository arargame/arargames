using System;
using System.Text.Json;
using ArarGames.Core.Logging;

namespace ArarGames.Core.Persistence;

/// <summary>
/// A JSON-based implementation of the save service, featuring atomic writes and corruption recovery.
/// </summary>
/// <typeparam name="TData">The type of the save data.</typeparam>
public class JsonSaveService<TData> : ISaveService<TData> where TData : class, new()
{
    private readonly string _fileName;
    private readonly IStorageProvider _storageProvider;
    private readonly object _lockObj = new();
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _isDirty;

    /// <inheritdoc />
    public TData Data { get; private set; }

    /// <inheritdoc />
    public event Action? OnSaved;

    /// <inheritdoc />
    public event Action? OnLoaded;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonSaveService{TData}"/> class.
    /// </summary>
    /// <param name="fileName">The name of the save file.</param>
    /// <param name="storageProvider">The storage provider to use.</param>
    public JsonSaveService(string fileName, IStorageProvider storageProvider)
    {
        _fileName = fileName;
        _storageProvider = storageProvider;
        Data = new TData();
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc />
    public void Load()
    {
        lock (_lockObj)
        {
            if (!_storageProvider.Exists(_fileName))
            {
                Data = new TData();
                _isDirty = true;
                OnLoaded?.Invoke();
                return;
            }

            try
            {
                using var stream = _storageProvider.OpenRead(_fileName);
                var loadedData = JsonSerializer.Deserialize<TData>(stream, _jsonOptions);
                Data = loadedData ?? new TData();
            }
            catch (Exception ex)
            {
                GameLog.Error($"Failed to parse save file '{_fileName}'. Creating backup. Exception: {ex.Message}").ConsoleWriter();
                
                string corruptPath = _fileName + ".corrupt";
                if (_storageProvider.Exists(corruptPath))
                {
                    _storageProvider.Delete(corruptPath);
                }
                
                _storageProvider.Move(_fileName, corruptPath, true);
                Data = new TData();
                _isDirty = true;
            }

            _isDirty = false;
            OnLoaded?.Invoke();
        }
    }

    /// <inheritdoc />
    public void Save()
    {
        lock (_lockObj)
        {
            string tempFileName = _fileName + ".tmp";
            
            try
            {
                using (var stream = _storageProvider.OpenWrite(tempFileName))
                {
                    JsonSerializer.Serialize(stream, Data, _jsonOptions);
                }
                
                _storageProvider.Move(tempFileName, _fileName, overwrite: true);
                _isDirty = false;
                OnSaved?.Invoke();
            }
            catch (Exception ex)
            {
                GameLog.Error($"Failed to save file '{_fileName}'. Exception: {ex.Message}").ConsoleWriter();
                if (_storageProvider.Exists(tempFileName))
                {
                    _storageProvider.Delete(tempFileName);
                }
            }
        }
    }

    /// <inheritdoc />
    public void MarkDirty()
    {
        lock (_lockObj)
        {
            _isDirty = true;
        }
    }

    /// <inheritdoc />
    public void FlushIfDirty()
    {
        bool needsSave;
        lock (_lockObj)
        {
            needsSave = _isDirty;
        }
        
        if (needsSave)
        {
            Save();
        }
    }
}
