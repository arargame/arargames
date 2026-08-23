using System;

namespace ArarGames.Core.Base;

/// <summary>
/// Base object implementation using the CRTP pattern, devoid of any ORM dependencies.
/// </summary>
/// <typeparam name="T">The concrete type.</typeparam>
public abstract class BaseObject<T> : IBaseObject<T> where T : BaseObject<T>
{
    /// <inheritdoc />
    public Guid Id { get; set; }

    /// <inheritdoc />
    public string? Name { get; set; }

    /// <inheritdoc />
    public string? Description { get; set; }

    /// <inheritdoc />
    public bool IsActive { get; set; } = true;

    /// <inheritdoc />
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the object is persistent.
    /// </summary>
    public bool IsPersistent { get; set; }

    /// <summary>
    /// Initializes the base object, assigning a Guid version 7 and creation time.
    /// </summary>
    public virtual void Initialize()
    {
        Id = Guid.CreateVersion7();
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the name fluently.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>The current instance.</returns>
    public T SetName(string name)
    {
        Name = name;
        return (T)this;
    }

    /// <summary>
    /// Sets the description fluently.
    /// </summary>
    /// <param name="description">The description to set.</param>
    /// <returns>The current instance.</returns>
    public T SetDescription(string description)
    {
        Description = description;
        return (T)this;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object using the unique identifier.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not BaseObject<T> other) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Serves as the default hash function using the unique identifier.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode() => Id.GetHashCode();
}
