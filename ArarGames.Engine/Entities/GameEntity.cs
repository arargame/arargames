using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArarGames.Core.Base;

namespace ArarGames.Engine.Entities;

/// <summary>
/// Oyun içindeki fiziksel/görsel tüm varlıkların temel sınıfı. 
/// Id, Name gibi ortak özellikleri ArarGames.Core.Base.BaseObject'ten alır.
/// </summary>
/// <typeparam name="T">Kalıtım alan sınıfın kendi tipi (Fluent interface vb. için).</typeparam>
public abstract class GameEntity<T> : BaseObject<T> where T : GameEntity<T>
{
    /// <summary>
    /// Varlığın 2B dünyadaki konumu.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Varlığın hareket hızı ve yönü.
    /// </summary>
    public Vector2 Velocity { get; set; }

    /// <summary>
    /// Varlığın genişliği ve yüksekliği.
    /// </summary>
    public Vector2 Size { get; set; }

    /// <summary>
    /// Varlığın dönüş açısı (Radyan).
    /// </summary>
    public float Rotation { get; set; }

    /// <summary>
    /// Ekranda çizilip çizilmeyeceğini belirtir.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Çizim rengi tonlaması.
    /// </summary>
    public Color Tint { get; set; } = Color.White;

    /// <summary>
    /// Çizim derinliği (Z-Index). 0 önde, 1 arkadadır (DepthStencilMode'a göre değişebilir).
    /// </summary>
    public float LayerDepth { get; set; }

    /// <summary>
    /// Varlığın sınırlarını (Bounding Box) döner. Çarpışma hesaplamaları için.
    /// </summary>
    public virtual Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

    /// <summary>
    /// Varlığın merkez noktasını döner.
    /// </summary>
    public Vector2 Center => Position + Size / 2f;

    /// <summary>
    /// Varlığın oyun döngüsü içindeki mantığını günceller.
    /// </summary>
    /// <param name="gameTime">Oyun zamanı nesnesi.</param>
    public virtual void Update(GameTime gameTime)
    {
    }

    /// <summary>
    /// Varlığı ekrana çizer.
    /// </summary>
    /// <param name="spriteBatch">Çizim nesnesi.</param>
    public virtual void Draw(SpriteBatch spriteBatch)
    {
    }

    /// <summary>
    /// Bu varlığın başka bir varlıkla çarpışıp çarpışmadığını kontrol eder (AABB Bounding Box kullanır).
    /// </summary>
    /// <typeparam name="TOther">Diğer varlığın tipi.</typeparam>
    /// <param name="other">Diğer varlık.</param>
    /// <returns>Kesişiyorsa true, aksi halde false.</returns>
    public bool Intersects<TOther>(GameEntity<TOther> other) where TOther : GameEntity<TOther>
    {
        return Bounds.Intersects(other.Bounds);
    }
}
