using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArarGames.Core
{
    public class Sprite : GameObject
    {
        public Texture2D Texture { get; set; }
        public Color Color { get; set; } = Color.White;
        public Vector2 Origin { get; set; }
        public Rectangle? SourceRectangle { get; set; }
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;
        public float LayerDepth { get; set; } = 0f;

        public Sprite(Texture2D texture)
        {
            Texture = texture;
            // Default origin to center? Or Top-Left? MonoGame defaults to Top-Left (0,0).
            // Let's keep it 0,0 unless specified.
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;

            if (Texture != null)
            {
                // Calculate Global Position/Scale/Rotation if Parent exists?
                // For simplicity Phase 1: Local = Global. 
                // Implementing Scene Graph Matrices is a bigger step.
                
                spriteBatch.Draw(
                    Texture,
                    Position,
                    SourceRectangle,
                    Color,
                    Rotation,
                    Origin,
                    Scale,
                    Effects,
                    LayerDepth
                );
            }

            base.Draw(spriteBatch);
        }
    }
}
