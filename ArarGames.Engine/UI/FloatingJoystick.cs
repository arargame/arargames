using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using ArarGames.Engine.Input;

namespace ArarGames.Engine.UI;

/// <summary>
/// Mobil cihazlar için ekranda herhangi bir yere dokunulduğunda beliren sanal joystick.
/// </summary>
public class FloatingJoystick
{
    private readonly float _deadZone;
    private readonly float _maxRadius;
    private int _touchId = -1;
    private Vector2 _basePosition;
    private Vector2 _stickPosition;

    /// <summary>
    /// Joystick yönü (Normalize edilmiş Vector2, deadzone içindeyse Zero).
    /// </summary>
    public Vector2 Direction { get; private set; }

    /// <summary>
    /// Joystick'in şu an kullanımda (dokunuluyor) olup olmadığı.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// FloatingJoystick nesnesini başlatır.
    /// </summary>
    /// <param name="deadZone">Ortadaki ufak hareketlerin yok sayılacağı alan (0.0 - 1.0 arası oransal).</param>
    /// <param name="maxRadius">Joystick topuzunun merkezden en fazla ne kadar uzaklaşabileceği (Piksel).</param>
    public FloatingJoystick(float deadZone = 0.1f, float maxRadius = 80f)
    {
        _deadZone = deadZone;
        _maxRadius = maxRadius;
    }

    /// <summary>
    /// Dokunmatik girdi ile joystick'i günceller.
    /// </summary>
    /// <param name="input">Oyunun genel girdi durumu (TouchCollection için doğrudan TouchPanel gerekebilir).</param>
    public void Update(in InputState input)
    {
        // Girdi soyutlamasından bağımsız olarak doğrudan dokunmaları okumak daha sağlıklıdır multi-touch için
        var touches = TouchPanel.GetState();
        
        IsActive = false;
        Direction = Vector2.Zero;

        foreach (var touch in touches)
        {
            if (touch.State == TouchLocationState.Pressed && _touchId == -1)
            {
                // Yeni bir joystick hareketi başlat
                _touchId = touch.Id;
                _basePosition = touch.Position;
                _stickPosition = touch.Position;
                IsActive = true;
                break; // İlk bulduğumuzla devam edelim
            }
            else if (touch.Id == _touchId)
            {
                if (touch.State == TouchLocationState.Moved || touch.State == TouchLocationState.Pressed)
                {
                    IsActive = true;
                    _stickPosition = touch.Position;

                    Vector2 delta = _stickPosition - _basePosition;
                    float distance = delta.Length();

                    if (distance > 0)
                    {
                        Direction = delta / distance; // Normalize
                        
                        // Sınırlandırma
                        if (distance > _maxRadius)
                        {
                            _stickPosition = _basePosition + Direction * _maxRadius;
                        }

                        // Deadzone kontrolü
                        if (distance < _maxRadius * _deadZone)
                        {
                            Direction = Vector2.Zero;
                        }
                    }
                }
                else if (touch.State == TouchLocationState.Released)
                {
                    _touchId = -1;
                    IsActive = false;
                }
            }
        }

        // Eğer mevcut TouchId kaybolduysa resetle
        if (_touchId != -1 && !IsActive)
        {
             _touchId = -1;
             Direction = Vector2.Zero;
        }
    }

    /// <summary>
    /// Sanal joystick'i ekrana çizer.
    /// </summary>
    /// <param name="spriteBatch">Çizim nesnesi.</param>
    /// <param name="baseTexture">Arka plan dairesi dokusu.</param>
    /// <param name="stickTexture">Hareket eden topuz dokusu.</param>
    public void Draw(SpriteBatch spriteBatch, Texture2D? baseTexture = null, Texture2D? stickTexture = null)
    {
        if (!IsActive) return;

        if (baseTexture != null)
        {
            Vector2 origin = new Vector2(baseTexture.Width / 2f, baseTexture.Height / 2f);
            spriteBatch.Draw(baseTexture, _basePosition, null, Color.White * 0.5f, 0f, origin, 1f, SpriteEffects.None, 0f);
        }

        if (stickTexture != null)
        {
            Vector2 origin = new Vector2(stickTexture.Width / 2f, stickTexture.Height / 2f);
            spriteBatch.Draw(stickTexture, _stickPosition, null, Color.White * 0.8f, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
