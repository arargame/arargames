using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArarGames.Engine.Graphics;

/// <summary>
/// Parallax arka plan için tek bir katmanı temsil eden veri yapısı.
/// </summary>
/// <param name="Texture">Katman dokusu.</param>
/// <param name="ScrollSpeed">Kamera hızına göre kayma hızı çarpanı (Örn: 0.5 arka plan, 1.0 ön plan).</param>
/// <param name="Scale">Katmanın çizim ölçeği.</param>
public record ParallaxLayer(Texture2D Texture, float ScrollSpeed, float Scale = 1f);

/// <summary>
/// Çoklu katmanlı, sonsuz döngü (wrap-around) destekli parallax arka plan sistemi.
/// </summary>
public class ParallaxBackground
{
    private readonly List<ParallaxLayer> _layers;
    private readonly float[] _offsetsX;

    /// <summary>
    /// ParallaxBackground nesnesini başlatır.
    /// </summary>
    /// <param name="layers">Arka plan katmanları listesi.</param>
    public ParallaxBackground(List<ParallaxLayer> layers)
    {
        _layers = layers ?? new List<ParallaxLayer>();
        _offsetsX = new float[_layers.Count];
    }

    /// <summary>
    /// Kamera hareketine göre katmanları günceller.
    /// </summary>
    /// <param name="gameTime">Oyun zamanı nesnesi.</param>
    /// <param name="cameraVelocity">Kameranın veya oyuncunun o anki hızı.</param>
    public void Update(GameTime gameTime, Vector2 cameraVelocity)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        for (int i = 0; i < _layers.Count; i++)
        {
            var layer = _layers[i];
            
            // Kamera sağa gidiyorsa arka plan sola kayar (ters yön)
            _offsetsX[i] -= cameraVelocity.X * layer.ScrollSpeed * dt;
            
            float scaledWidth = layer.Texture.Width * layer.Scale;

            // Sonsuz döngü kontrolü
            if (_offsetsX[i] <= -scaledWidth)
            {
                _offsetsX[i] += scaledWidth;
            }
            else if (_offsetsX[i] >= scaledWidth)
            {
                _offsetsX[i] -= scaledWidth;
            }
        }
    }

    /// <summary>
    /// Katmanları çizer. Sonsuz döngüyü sağlamak için her katman gerektiği kadar tekrar edilir.
    /// </summary>
    /// <param name="spriteBatch">Çizim nesnesi.</param>
    /// <param name="viewport">Görünür alan (Viewport).</param>
    public void Draw(SpriteBatch spriteBatch, Rectangle viewport)
    {
        for (int i = 0; i < _layers.Count; i++)
        {
            var layer = _layers[i];
            float scaledWidth = layer.Texture.Width * layer.Scale;
            
            float startX = _offsetsX[i];
            
            // Eğer başlangıç noktası viewport'un sağındaysa sola al
            while (startX > 0)
            {
                startX -= scaledWidth;
            }

            // Ekranı doldurana kadar tekrar tekrar çiz
            float currentX = startX;
            while (currentX < viewport.Width)
            {
                spriteBatch.Draw(
                    layer.Texture,
                    new Vector2(currentX, viewport.Y),
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    layer.Scale,
                    SpriteEffects.None,
                    0f
                );
                currentX += scaledWidth;
            }
        }
    }
}
