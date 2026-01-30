using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArarGames.Core.Screens
{
    public abstract class Screen
    {
        public string Name { get; set; }
        public bool IsPopup { get; set; } = false; // If true, screens below it continue to draw
        public bool IsActive { get; set; } = true;

        public ScreenManager ScreenManager { get; set; }

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
