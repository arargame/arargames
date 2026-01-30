using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ArarGames.Core.Input
{
    public class InputManager
    {
        private static InputManager instance;
        public static InputManager Instance
        {
            get
            {
                if (instance == null) instance = new InputManager();
                return instance;
            }
        }

        public KeyboardState CurrentKeyboardState { get; private set; }
        public KeyboardState PreviousKeyboardState { get; private set; }

        public MouseState CurrentMouseState { get; private set; }
        public MouseState PreviousMouseState { get; private set; }

        public void Update()
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        // Helper methods
        public bool IsKeyPressed(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyUp(key);
        }

        public bool IsKeyDown(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key);
        }

        public bool IsLeftMouseClick()
        {
            return CurrentMouseState.LeftButton == ButtonState.Pressed && 
                   PreviousMouseState.LeftButton == ButtonState.Released;
        }

        public Vector2 GetMousePosition()
        {
            return new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
        }
    }
}
