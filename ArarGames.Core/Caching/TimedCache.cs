using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ArarGames.Core.Caching;

/// <summary>
/// A thread-safe cache with Time-To-Live (TTL) based automatic expiration.
/// </summary>
/// <typeparam name="TKey">The type of keys.</typeparam>
/// <typeparam name="TValue">The type of values.</typeparam>
public sealed class TimedCache<TKey, TValue> : IDisposable where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, CacheItem> _cache;
    private readonly TimeSpan _defaultTtl;
    private readonly Timer _cleanupTimer;

    /// <summary>
    /// Gets the number of items currently in the cache.
    /// </summary>
    public int Count => _cache.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimedCache{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="defaultTtl">The default time-to-live for cache entries.</param>
    /// <param name="cleanupInterval">The interval at which expired items are cleaned up. Defaults to 1 minute if not provided.</param>
    public TimedCache(TimeSpan defaultTtl, TimeSpan? cleanupInterval = null)
    {
        _cache = new ConcurrentDictionary<TKey, CacheItem>();
        _defaultTtl = defaultTtl;
        var interval = cleanupInterval ?? TimeSpan.FromMinutes(1);
        _cleanupTimer = new Timer(CleanupExpiredItems, null, interval, interval);
    }

    /// <summary>
    /// Attempts to retrieve a value from the cache.
    /// </summary>
    public bool TryGet(TKey key, out TValue? value)
    {
        if (_cache.TryGetValue(key, out var item))
        {
            if (item.IsExpired)
            {
                _cache.TryRemove(key, out _);
                value = default;
                return false;
            }
            value = item.Value;
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Adds or updates a value in the cache with an optional specific TTL.
    /// </summary>
    public void Put(TKey key, TValue value, TimeSpan? ttl = null)
    {
        var expiration = DateTime.UtcNow.Add(ttl ?? _defaultTtl);
        _cache[key] = new CacheItem(value, expiration);
    }

    /// <summary>
    /// Removes a value from the cache.
    /// </summary>
    public void Remove(TKey key)
    {
        _cache.TryRemove(key, out _);
    }

    /// <summary>
    /// Clears all items from the cache.
    /// </summary>
    public void Clear()
    {
        _cache.Clear();
    }

    private void CleanupExpiredItems(object? state)
    {
        var now = DateTime.UtcNow;
        foreach (var kvp in _cache)
        {
            if (kvp.Value.Expiration <= now)
            {
                _cache.TryRemove(kvp.Key, out _);
            }
        }
    }

    /// <summary>
    /// Disposes the underlying timer used for cleanup.
    /// </summary>
    public void Dispose()
    {
        _cleanupTimer.Dispose();
    }

    private class CacheItem
    {
        public TValue Value { get; }
        public DateTime Expiration { get; }
        public bool IsExpired => DateTime.UtcNow >= Expiration;

        public CacheItem(TValue value, DateTime expiration)
        {
            Value = value;
            Expiration = expiration;
        }
    }
}
