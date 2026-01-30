using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ArarGames.Core.Screens
{
    public class ScreenManager : DrawableGameComponent
    {
        private Stack<Screen> screens = new Stack<Screen>();
        private Screen newScreen;
        
        public SpriteBatch SpriteBatch { get; private set; }

        private static ScreenManager instance;
        public static ScreenManager Instance => instance;

        public ScreenManager(Game game) : base(game)
        {
            instance = this;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }

        public void AddScreen(Screen screen)
        {
            screen.ScreenManager = this;
            screen.LoadContent();
            screens.Push(screen);
        }

        public void RemoveScreen()
        {
            if (screens.Count > 0)
            {
                var screen = screens.Pop();
                screen.UnloadContent();
            }
        }

        public void ChangeScreen(Screen screen)
        {
            while (screens.Count > 0)
            {
                RemoveScreen();
            }
            AddScreen(screen);
        }

        public override void Update(GameTime gameTime)
        {
            if (screens.Count > 0)
            {
                var topScreen = screens.Peek();
                if (topScreen.IsActive)
                    topScreen.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            
            // Draw logic: 
            // If popup, might need to draw screens below. 
            // Simplified reverse iteration for drawing background screens first
            
            var screenArray = screens.ToArray();
            for (int i = screenArray.Length - 1; i >= 0; i--)
            {
                if (screenArray[i].IsActive || screenArray[i].IsPopup)
                    screenArray[i].Draw(SpriteBatch);
            }

            SpriteBatch.End();
        }
    }
}
