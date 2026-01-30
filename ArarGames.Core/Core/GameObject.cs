using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ArarGames.Core
{
    public class GameObject
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsVisible { get; set; } = true;

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;

        public GameObject Parent { get; set; }
        public List<GameObject> Children { get; private set; } = new List<GameObject>();

        public GameObject()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            // Update children
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Update(gameTime);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;

            // Draw children
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Draw(spriteBatch);
            }
        }

        public void AddChild(GameObject child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void RemoveChild(GameObject child)
        {
            if (Children.Contains(child))
            {
                child.Parent = null;
                Children.Remove(child);
            }
        }
    }
}
