using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ArarGames.Engine.Input;

/// <summary>
/// Masaüstü platformları için Klavye ve Fare girdilerini yöneten sağlayıcı.
/// </summary>
public class KeyboardMouseInputProvider : IInputProvider
{
    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;
    private KeyboardState _currentKeyboardState;
    private MouseState _currentMouseState;
    private readonly Func<Vector2, Vector2>? _screenToVirtual;

    /// <summary>
    /// KeyboardMouseInputProvider nesnesini başlatır.
    /// </summary>
    /// <param name="screenToVirtual">Fare pozisyonunu sanal koordinatlara dönüştüren fonksiyon. Opsiyonel.</param>
    public KeyboardMouseInputProvider(Func<Vector2, Vector2>? screenToVirtual = null)
    {
        _screenToVirtual = screenToVirtual;
    }

    /// <inheritdoc/>
    public void Update(GameTime gameTime)
    {
        _previousKeyboardState = _currentKeyboardState;
        _previousMouseState = _currentMouseState;
        _currentKeyboardState = Keyboard.GetState();
        _currentMouseState = Mouse.GetState();
    }

    /// <inheritdoc/>
    public InputState GetState()
    {
        Vector2 moveDirection = Vector2.Zero;

        if (_currentKeyboardState.IsKeyDown(Keys.W) || _currentKeyboardState.IsKeyDown(Keys.Up)) moveDirection.Y -= 1;
        if (_currentKeyboardState.IsKeyDown(Keys.S) || _currentKeyboardState.IsKeyDown(Keys.Down)) moveDirection.Y += 1;
        if (_currentKeyboardState.IsKeyDown(Keys.A) || _currentKeyboardState.IsKeyDown(Keys.Left)) moveDirection.X -= 1;
        if (_currentKeyboardState.IsKeyDown(Keys.D) || _currentKeyboardState.IsKeyDown(Keys.Right)) moveDirection.X += 1;

        if (moveDirection != Vector2.Zero)
        {
            moveDirection.Normalize();
        }

        Vector2 rawCursor = new Vector2(_currentMouseState.X, _currentMouseState.Y);
        Vector2 cursorPosition = _screenToVirtual != null ? _screenToVirtual(rawCursor) : rawCursor;

        bool firePressed = _currentMouseState.LeftButton == ButtonState.Pressed || _currentKeyboardState.IsKeyDown(Keys.Space);
        bool fireJustPressed = (_currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released) ||
                               (_currentKeyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space));

        bool backPressed = _currentKeyboardState.IsKeyDown(Keys.Escape);
        bool backJustPressed = backPressed && !_previousKeyboardState.IsKeyDown(Keys.Escape);

        bool pausePressed = _currentKeyboardState.IsKeyDown(Keys.P) || _currentKeyboardState.IsKeyDown(Keys.Escape);
        bool pauseJustPressed = pausePressed && (!_previousKeyboardState.IsKeyDown(Keys.P) && !_previousKeyboardState.IsKeyDown(Keys.Escape));

        bool confirmPressed = _currentKeyboardState.IsKeyDown(Keys.Enter);
        bool confirmJustPressed = confirmPressed && !_previousKeyboardState.IsKeyDown(Keys.Enter);

        bool anyTouchOrClick = _currentMouseState.LeftButton == ButtonState.Pressed;

        return new InputState
        {
            MoveDirection = moveDirection,
            CursorPosition = cursorPosition,
            FirePressed = firePressed,
            FireJustPressed = fireJustPressed,
            BackPressed = backPressed,
            BackJustPressed = backJustPressed,
            PausePressed = pausePressed,
            PauseJustPressed = pauseJustPressed,
            ConfirmPressed = confirmPressed,
            ConfirmJustPressed = confirmJustPressed,
            AnyTouchOrClick = anyTouchOrClick,
            AimDirection = Vector2.Zero // Fare için imleç pozisyonuna bakılarak oyun içinde hesaplanabilir
        };
    }
}
