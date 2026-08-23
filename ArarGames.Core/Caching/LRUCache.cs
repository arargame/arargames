using System.Collections.Concurrent;
using System.Collections.Generic;
using ArarGames.Core.Validation;

namespace ArarGames.Core.Caching;

/// <summary>
/// A thread-safe Least Recently Used (LRU) cache implementation.
/// </summary>
/// <typeparam name="TKey">The type of keys in the cache.</typeparam>
/// <typeparam name="TValue">The type of values in the cache.</typeparam>
public class LRUCache<TKey, TValue> where TKey : notnull
{
    private readonly int _capacity;
    private readonly ConcurrentDictionary<TKey, LinkedListNode<CacheItem>> _cache;
    private readonly LinkedList<CacheItem> _lruList;
    private readonly object _lock = new();

    /// <summary>
    /// Gets the number of items currently in the cache.
    /// </summary>
    public int Count => _cache.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="LRUCache{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of items the cache can hold.</param>
    public LRUCache(int capacity)
    {
        Ensure.Positive(capacity, nameof(capacity));
        _capacity = capacity;
        _cache = new ConcurrentDictionary<TKey, LinkedListNode<CacheItem>>();
        _lruList = new LinkedList<CacheItem>();
    }

    /// <summary>
    /// Attempts to retrieve a value from the cache.
    /// </summary>
    public bool TryGet(TKey key, out TValue? value)
    {
        if (_cache.TryGetValue(key, out var node))
        {
            lock (_lock)
            {
                if (node.List != null)
                {
                    _lruList.Remove(node);
                    _lruList.AddFirst(node);
                }
            }
            value = node.Value.Value;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Adds or updates a value in the cache.
    /// </summary>
    public void Put(TKey key, TValue value)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(key, out var existingNode))
            {
                _lruList.Remove(existingNode);
                existingNode.Value.Value = value;
                _lruList.AddFirst(existingNode);
            }
            else
            {
                if (_cache.Count >= _capacity)
                {
                    var lastNode = _lruList.Last;
                    if (lastNode != null)
                    {
                        _lruList.RemoveLast();
                        _cache.TryRemove(lastNode.Value.Key, out _);
                    }
                }

                var cacheItem = new CacheItem(key, value);
                var newNode = new LinkedListNode<CacheItem>(cacheItem);
                _lruList.AddFirst(newNode);
                _cache.TryAdd(key, newNode);
            }
        }
    }

    /// <summary>
    /// Removes a value from the cache.
    /// </summary>
    public void Remove(TKey key)
    {
        if (_cache.TryRemove(key, out var node))
        {
            lock (_lock)
            {
                if (node.List != null)
                {
                    _lruList.Remove(node);
                }
            }
        }
    }

    /// <summary>
    /// Clears all items from the cache.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _lruList.Clear();
            _cache.Clear();
        }
    }

    private class CacheItem
    {
        public TKey Key { get; }
        public TValue Value { get; set; }

        public CacheItem(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}
