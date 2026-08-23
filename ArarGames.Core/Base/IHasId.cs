using System;

namespace ArarGames.Core.Base;

/// <summary>
/// Represents an entity that has a unique identifier.
/// </summary>
public interface IHasId
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    Guid Id { get; set; }
}
