using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArarGames.Core.UI
{
    public class Label : GameObject
    {
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; } = Color.Black;

        public Label(SpriteFont font, string text)
        {
            Font = font;
            Text = text;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;
            
            if (Font != null && !string.IsNullOrEmpty(Text))
            {
                spriteBatch.DrawString(Font, Text, Position, Color, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            }
        }
    }
}
