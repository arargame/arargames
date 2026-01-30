using ArarGames.Core;
using ArarGames.Core.Screens;
using ArarGames.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ArarGames.Test
{
    public class TestScreen : Screen
    {
        private List<GameObject> objects = new List<GameObject>();
        private Texture2D texture;

        public override void LoadContent()
        {
            base.LoadContent();

            // Create a simple 1x1 white texture programmatically
            texture = new Texture2D(ScreenManager.Instance.GraphicsDevice, 100, 100);
            Color[] data = new Color[100 * 100];
            for(int i=0; i < data.Length; ++i) data[i] = Color.White;
            texture.SetData(data);

            // Create Sprite
            Sprite sprite = new Sprite(texture);
            sprite.Position = new Vector2(100, 100);
            sprite.Color = Color.Red;
            objects.Add(sprite);

            // Create Button
            Button btn = new Button(texture);
            btn.Position = new Vector2(300, 100);
            btn.Color = Color.Green;
            btn.HoverColor = Color.Lime;
            btn.OnClick += () => System.Diagnostics.Debug.WriteLine("Button Clicked!");
            objects.Add(btn);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var obj in objects)
                obj.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var obj in objects)
                obj.Draw(spriteBatch);
        }
    }
}
