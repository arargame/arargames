# Nesne Havuzlama (Object Pooling)

**ArarGames.Core.Pooling** modülü; oyun sırasında sürekli yaratılıp yok edilen mermiler, parçacıklar, patlama efektleri ve kayan yazılar (floating text) gibi kısa ömürlü nesnelerin bellek tahsisatını (heap allocation) ve Çöp Toplayıcı (Garbage Collection - GC) duraklamalarını sıfıra indirmek için tasarlanmıştır.

---

## 🎯 Neden Nesne Havuzu?

Klasik C# yaklaşımında her ateş edildiğinde:
```csharp
// ❌ KÖTÜ PRATİK: Her saniye onlarca nesne tahsis edilir ve GC tetiklenir
var bullet = new Bullet(position, velocity);
```
Bu yaklaşım, Garbage Collector'ün her birkaç saniyede bir devreye girip oyunu birkaç milisaniye dondurmasına (micro-stutter / FPS drop) neden olur.

**ArarGames ObjectPool Yaklaşımı**:
```csharp
// ✅ EN İYİ PRATİK: Önceden tahsis edilmiş nesne havuzdan alınır, işi bitince iade edilir
var bullet = _bulletPool.Get();
bullet.Spawn(position, velocity);
```

---

## 🧩 `IPoolable` Arayüzü

Havuza girecek tüm nesneler `IPoolable` arayüzünü uygulamalıdır. Nesne havuzdan her çekildiğinde veya havuza iade edildiğinde `Reset()` metodu çağrılarak önceki durum temizlenir.

```csharp
namespace ArarGames.Core.Pooling;

/// <summary>
/// Defines an object that can be reset and recycled in an object pool.
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// Resets the object state to its initial default before being reused.
    /// </summary>
    void Reset();
}
```

---

## ⚙️ `ObjectPool<T>` Sınıfı

`ObjectPool<T>`, thread-safe olmayan yüksek hızlı `Stack<T>` tabanlı bir havuzdur ve **çift iade (double-return)** koruması içerir.

```csharp
using System;
using System.Collections.Generic;

namespace ArarGames.Core.Pooling;

public class ObjectPool<T> where T : class, IPoolable, new()
{
    private readonly Stack<T> _pool;
    private readonly HashSet<T> _inPoolCheck;

    /// <summary>Havuzdaki kullanılabilir nesne sayısı.</summary>
    public int AvailableCount => _pool.Count;

    /// <summary>
    /// Belirtilen kapasitede nesne havuzu oluşturur.
    /// </summary>
    /// <param name="initialCapacity">Ön tahsis edilecek nesne sayısı.</param>
    public ObjectPool(int initialCapacity = 32)
    {
        _pool = new Stack<T>(initialCapacity);
        _inPoolCheck = new HashSet<T>(initialCapacity);

        Warmup(initialCapacity);
    }

    /// <summary>
    /// Havuzu önceden nesnelerle doldurarak oyun esnasında tahsisat oluşmasını önler.
    /// </summary>
    public void Warmup(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var item = new T();
            _pool.Push(item);
            _inPoolCheck.Add(item);
        }
    }

    /// <summary>
    /// Havuzdan bir nesne alır ve Reset() çağırır. Havuz boşsa yeni nesne üretir.
    /// </summary>
    public T Get()
    {
        T item;
        if (_pool.Count > 0)
        {
            item = _pool.Pop();
            _inPoolCheck.Remove(item);
        }
        else
        {
            item = new T();
        }

        item.Reset();
        return item;
    }

    /// <summary>
    /// Kullanımı biten nesneyi havuza geri verir.
    /// </summary>
    public void Return(T item)
    {
        if (item == null) return;

        // Çift İade Koruması (Double-return protection)
        if (_inPoolCheck.Contains(item))
            return;

        item.Reset();
        _pool.Push(item);
        _inPoolCheck.Add(item);
    }
}
```

---

## 🚀 Örnek: Mermi Havuzu (`BulletPool`)

### 1. Havuzlanabilir Mermi Sınıfı (`Bullet.cs`)
```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArarGames.Core.Pooling;

public class Bullet : IPoolable
{
    public Vector2 Position;
    public Vector2 Velocity;
    public bool IsActive;
    public float Lifetime;
    private const float MaxLifetime = 3.0f; // 3 saniye sonra kaybolur

    public void Spawn(Vector2 position, Vector2 velocity)
    {
        Position = position;
        Velocity = velocity;
        IsActive = true;
        Lifetime = 0f;
    }

    public void Reset()
    {
        Position = Vector2.Zero;
        Velocity = Vector2.Zero;
        IsActive = false;
        Lifetime = 0f;
    }

    public void Update(GameTime gameTime)
    {
        if (!IsActive) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += Velocity * dt;
        Lifetime += dt;

        if (Lifetime >= MaxLifetime)
        {
            IsActive = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D texture)
    {
        if (!IsActive) return;
        spriteBatch.Draw(texture, Position, Color.Yellow);
    }
}
```

### 2. Oyun İçi Kullanım (`GameplayScreen.cs`)
```csharp
public class GameplayScreen : GameScreen
{
    private readonly ObjectPool<Bullet> _bulletPool;
    private readonly List<Bullet> _activeBullets;
    private Texture2D _bulletTexture = null!;

    public GameplayScreen(GameContext context) : base(context)
    {
        // Oyun başlamadan önce 100 mermiyi belleğe hazırla (Warmup)
        _bulletPool = new ObjectPool<Bullet>(100);
        _activeBullets = new List<Bullet>(100);
    }

    public void FireBullet(Vector2 startPos, Vector2 direction)
    {
        var bullet = _bulletPool.Get();
        bullet.Spawn(startPos, direction * 600f);
        _activeBullets.Add(bullet);
    }

    public override void Update(GameTime gameTime, bool isCovered)
    {
        if (isCovered) return;

        for (int i = _activeBullets.Count - 1; i >= 0; i--)
        {
            var bullet = _activeBullets[i];
            bullet.Update(gameTime);

            // Mermi öldüyse veya ekrandan çıktıysa havuza geri ver
            if (!bullet.IsActive)
            {
                _activeBullets.RemoveAt(i);
                _bulletPool.Return(bullet);
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        foreach (var bullet in _activeBullets)
        {
            bullet.Draw(spriteBatch, _bulletTexture);
        }
    }
}
```
