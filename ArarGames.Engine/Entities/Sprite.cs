using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArarGames.Engine.Entities;

/// <summary>
/// Ekranda bir Texture (Doku) çizen temel oyun nesnesi.
/// </summary>
public class Sprite : GameEntity<Sprite>
{
    /// <summary>
    /// Çizilecek doku.
    /// </summary>
    public Texture2D? Texture { get; set; }

    /// <summary>
    /// Dönme ve ölçekleme işlemlerinin merkezi (Örn: Texture.Width/2).
    /// </summary>
    public Vector2 Origin { get; set; }

    /// <summary>
    /// Çizim ölçeği.
    /// </summary>
    public Vector2 Scale { get; set; } = Vector2.One;

    /// <summary>
    /// Dokunun sadece belirli bir alanını (Sprite Sheet) çizmek için kaynak dikdörtgen.
    /// </summary>
    public Rectangle? SourceRectangle { get; set; }

    /// <summary>
    /// Yatay veya dikey çevirme efektleri.
    /// </summary>
    public SpriteEffects Effects { get; set; } = SpriteEffects.None;

    /// <summary>
    /// Ölçekleme ve kaynak alanı dikkate alınarak hesaplanmış sınır kutusu (Bounding Box).
    /// </summary>
    public override Rectangle Bounds
    {
        get
        {
            if (SourceRectangle.HasValue)
            {
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    (int)(SourceRectangle.Value.Width * Scale.X),
                    (int)(SourceRectangle.Value.Height * Scale.Y)
                );
            }
            if (Texture != null)
            {
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    (int)(Texture.Width * Scale.X),
                    (int)(Texture.Height * Scale.Y)
                );
            }
            return base.Bounds;
        }
    }

    /// <summary>
    /// Sprite'ı ekrana çizer.
    /// </summary>
    /// <param name="spriteBatch">Çizim nesnesi.</param>
    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsVisible || Texture == null) return;

        spriteBatch.Draw(
            Texture,
            Position,
            SourceRectangle,
            Tint,
            Rotation,
            Origin,
            Scale,
            Effects,
            LayerDepth
        );
    }
}
