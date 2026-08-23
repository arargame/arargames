using System;
using System.Collections.Generic;

namespace ArarGames.Engine.Pooling;

/// <summary>
/// Çöp toplayıcı (GC) yükünü azaltmak için nesne havuzlama sistemi.
/// Aynı nesnenin çift iade (double-return) edilmesine karşı korumalıdır.
/// </summary>
/// <typeparam name="T">Havuzda tutulacak nesne tipi. Sınıf, IPoolable ve boş kurucusu olmalıdır.</typeparam>
public class ObjectPool<T> where T : class, IPoolable, new()
{
    private readonly Stack<T> _available;
    private readonly HashSet<T> _active;
    private readonly List<T> _activeListForIteration;

    /// <summary>
    /// Şu an kullanımda olan (havuzdan alınmış) nesnelerin salt okunur listesi. 
    /// Foreach döngülerinde allocation yapmadan iterasyon sağlar.
    /// </summary>
    public IReadOnlyList<T> ActiveItems => _activeListForIteration;

    /// <summary>
    /// Kullanımdaki nesne sayısı.
    /// </summary>
    public int ActiveCount => _active.Count;

    /// <summary>
    /// Havuzda kullanılmayı bekleyen hazır nesne sayısı.
    /// </summary>
    public int AvailableCount => _available.Count;

    /// <summary>
    /// ObjectPool nesnesini başlatır.
    /// </summary>
    public ObjectPool()
    {
        _available = new Stack<T>();
        _active = new HashSet<T>();
        _activeListForIteration = new List<T>();
    }

    /// <summary>
    /// Havuzdan bir nesne alır. Havuz boşsa yeni bir tane oluşturur.
    /// </summary>
    /// <returns>Kullanıma hazır nesne.</returns>
    public T Get()
    {
        T item;
        if (_available.Count > 0)
        {
            item = _available.Pop();
        }
        else
        {
            item = new T();
        }

        _active.Add(item);
        _activeListForIteration.Add(item);
        return item;
    }

    /// <summary>
    /// Nesneyi havuza iade eder. Öncesinde IPoolable.Reset() çağrılır.
    /// Çift iade (double return) durumlarında hata vermeden yoksayar.
    /// </summary>
    /// <param name="item">İade edilecek nesne.</param>
    public void Return(T item)
    {
        if (item == null) return;

        // HashSet ile double-return engeli
        if (_active.Remove(item))
        {
            _activeListForIteration.Remove(item);
            item.Reset();
            _available.Push(item);
        }
    }

    /// <summary>
    /// Kullanımda olan tüm nesneleri havuza geri döndürür ve sıfırlar.
    /// </summary>
    public void ReturnAll()
    {
        foreach (var item in _activeListForIteration)
        {
            item.Reset();
            _available.Push(item);
        }
        _active.Clear();
        _activeListForIteration.Clear();
    }

    /// <summary>
    /// Belirtilen sayıda nesneyi önceden oluşturup havuza ekler (Oyun yüklenirken takılmaları önlemek için).
    /// </summary>
    /// <param name="count">Oluşturulacak nesne sayısı.</param>
    public void Warmup(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _available.Push(new T());
        }
    }
}
