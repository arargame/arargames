using System.Collections.Generic;

namespace ArarGames.Core.Base;

/// <summary>
/// Represents an object that exists in a hierarchical structure.
/// </summary>
/// <typeparam name="T">The node type.</typeparam>
public interface IHierarchicalObject<T> where T : IHierarchicalObject<T>
{
    /// <summary>
    /// Gets or sets the parent node.
    /// </summary>
    T? Parent { get; set; }

    /// <summary>
    /// Gets the list of children nodes.
    /// </summary>
    IReadOnlyList<T> Children { get; }

    /// <summary>
    /// Gets all ancestors from the parent up to the root.
    /// </summary>
    /// <returns>An enumerable of ancestors.</returns>
    IEnumerable<T> GetAncestors()
    {
        var current = Parent;
        while (current != null)
        {
            yield return current;
            current = current.Parent;
        }
    }

    /// <summary>
    /// Gets all descendants in the hierarchy recursively.
    /// </summary>
    /// <returns>An enumerable of descendants.</returns>
    IEnumerable<T> GetAllDescendants()
    {
        foreach (var child in Children)
        {
            yield return child;
            foreach (var descendant in child.GetAllDescendants())
            {
                yield return descendant;
            }
        }
    }
}
