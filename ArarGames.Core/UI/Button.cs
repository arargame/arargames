using ArarGames.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ArarGames.Core.UI
{
    public class Button : Sprite
    {
        public event Action OnClick;
        public Color HoverColor { get; set; } = Color.Gray;
        public Color NormalColor { get; set; } = Color.White;

        public bool IsHovered { get; private set; }

        public Button(Texture2D texture) : base(texture)
        {
            NormalColor = Color.White;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 mousePos = InputManager.Instance.GetMousePosition();
            // Simple bound check (assuming Origin is 0,0 for now, or handling standard rectangle)
             Rectangle bounds = new Rectangle(
                (int)Position.X, 
                (int)Position.Y, 
                Texture.Width * (int)Scale.X, 
                Texture.Height * (int)Scale.Y);

            if (bounds.Contains(mousePos))
            {
                IsHovered = true;
                Color = HoverColor;

                if (InputManager.Instance.IsLeftMouseClick())
                {
                    OnClick?.Invoke();
                }
            }
            else
            {
                IsHovered = false;
                Color = NormalColor;
            }
        }
    }
}
