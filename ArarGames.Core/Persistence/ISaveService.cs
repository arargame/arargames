using System;

namespace ArarGames.Core.Persistence;

/// <summary>
/// Represents a generic service for managing save data.
/// </summary>
/// <typeparam name="TData">The type of the save data.</typeparam>
public interface ISaveService<TData> where TData : class, new()
{
    /// <summary>
    /// Gets the current save data.
    /// </summary>
    TData Data { get; }

    /// <summary>
    /// Loads the data from storage.
    /// </summary>
    void Load();

    /// <summary>
    /// Saves the data to storage.
    /// </summary>
    void Save();

    /// <summary>
    /// Marks the data as dirty, indicating it needs to be saved.
    /// </summary>
    void MarkDirty();

    /// <summary>
    /// Flushes the data to storage only if it is marked as dirty.
    /// </summary>
    void FlushIfDirty();

    /// <summary>
    /// Event triggered when the data has been saved.
    /// </summary>
    event Action? OnSaved;

    /// <summary>
    /// Event triggered when the data has been loaded.
    /// </summary>
    event Action? OnLoaded;
}
