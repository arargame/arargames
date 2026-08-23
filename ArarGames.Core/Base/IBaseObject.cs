using System;

namespace ArarGames.Core.Base;

/// <summary>
/// Represents the base object interface with common properties.
/// </summary>
/// <typeparam name="T">The concrete type implementing this interface.</typeparam>
public interface IBaseObject<T> : IHasId where T : IBaseObject<T>
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    string? Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this object is active.
    /// </summary>
    bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the creation date and time.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the modification date and time.
    /// </summary>
    DateTime? ModifiedAt { get; set; }
}
