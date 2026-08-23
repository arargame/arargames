using System;
using System.Collections.Generic;

namespace ArarGames.Core.Events;

/// <summary>
/// A lightweight, struct-based event bus for decoupled communication.
/// </summary>
public class EventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    /// <summary>
    /// Subscribes to an event of the specified struct type.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    /// <param name="handler">The action to perform when the event occurs.</param>
    public void Subscribe<T>(Action<T> handler) where T : struct
    {
        var type = typeof(T);
        if (!_handlers.TryGetValue(type, out var list))
        {
            list = new List<Delegate>();
            _handlers[type] = list;
        }
        list.Add(handler);
    }

    /// <summary>
    /// Unsubscribes from an event of the specified struct type.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    /// <param name="handler">The handler to remove.</param>
    public void Unsubscribe<T>(Action<T> handler) where T : struct
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var list))
        {
            list.Remove(handler);
        }
    }

    /// <summary>
    /// Publishes an event to all registered subscribers.
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    /// <param name="eventData">The event data payload.</param>
    public void Publish<T>(in T eventData) where T : struct
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var list))
        {
            for (int i = 0; i < list.Count; i++)
            {
                var handler = (Action<T>)list[i];
                handler(eventData);
            }
        }
    }

    /// <summary>
    /// Clears all subscribed handlers across all event types.
    /// </summary>
    public void ClearAll()
    {
        _handlers.Clear();
    }
}
