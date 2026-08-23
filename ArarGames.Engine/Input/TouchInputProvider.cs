using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace ArarGames.Engine.Input;

/// <summary>
/// Mobil cihazlar için Dokunmatik girdileri yöneten sağlayıcı.
/// Ayrıca Android cihazlardaki fiziksel/sanal Geri tuşunu da destekler.
/// </summary>
public class TouchInputProvider : IInputProvider
{
    private TouchCollection _previousTouchState;
    private TouchCollection _currentTouchState;
    private KeyboardState _previousKeyboardState;
    private KeyboardState _currentKeyboardState;
    private readonly Func<Vector2, Vector2>? _screenToVirtual;

    /// <summary>
    /// TouchInputProvider nesnesini başlatır.
    /// </summary>
    /// <param name="screenToVirtual">Dokunmatik ekran pozisyonunu sanal koordinatlara dönüştüren fonksiyon. Opsiyonel.</param>
    public TouchInputProvider(Func<Vector2, Vector2>? screenToVirtual = null)
    {
        _screenToVirtual = screenToVirtual;
    }

    /// <inheritdoc/>
    public void Update(GameTime gameTime)
    {
        _previousTouchState = _currentTouchState;
        _currentTouchState = TouchPanel.GetState();
        
        // Android'de Geri tuşu Klavye State üzerinden gelir (Keys.Escape veya Keys.BrowserBack)
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();
    }

    /// <inheritdoc/>
    public InputState GetState()
    {
        Vector2 cursorPosition = Vector2.Zero;
        bool anyTouchOrClick = _currentTouchState.Count > 0;
        bool firePressed = false;
        bool fireJustPressed = false;

        if (anyTouchOrClick)
        {
            var touch = _currentTouchState[0]; // İlk dokunmayı ana imleç olarak al
            cursorPosition = _screenToVirtual != null ? _screenToVirtual(touch.Position) : touch.Position;
            firePressed = true;
            
            // Eğer yeni dokunulduysa
            if (touch.State == TouchLocationState.Pressed)
            {
                fireJustPressed = true;
            }
        }

        // Android'de fiziksel veya sanal 'Geri' tuşu kontrolü
        bool backPressed = GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                           _currentKeyboardState.IsKeyDown(Keys.Escape) ||
                           _currentKeyboardState.IsKeyDown(Keys.BrowserBack);
                           
        bool wasBackPressed = _previousKeyboardState.IsKeyDown(Keys.Escape) ||
                              _previousKeyboardState.IsKeyDown(Keys.BrowserBack);

        bool backJustPressed = backPressed && !wasBackPressed;

        return new InputState
        {
            MoveDirection = Vector2.Zero, // Joystick bileşeni varsa UI üzerinden hesaplanacak
            CursorPosition = cursorPosition,
            FirePressed = firePressed,
            FireJustPressed = fireJustPressed,
            BackPressed = backPressed,
            BackJustPressed = backJustPressed,
            PausePressed = backPressed, // Mobilde geri genelde duraklatma için de kullanılır
            PauseJustPressed = backJustPressed,
            ConfirmPressed = firePressed,
            ConfirmJustPressed = fireJustPressed,
            AnyTouchOrClick = anyTouchOrClick,
            AimDirection = Vector2.Zero
        };
    }
}
