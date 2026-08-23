using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArarGames.Engine.Input;

namespace ArarGames.Engine.UI;

/// <summary>
/// Açık/Kapalı durumu tutan Switch/Toggle UI bileşeni.
/// </summary>
public class UiToggle
{
    private Rectangle _bounds;
    private string _label;
    private SpriteFont? _font;
    private bool _isOn;

    /// <summary>
    /// Toggle değeri değiştiğinde tetiklenir (Yeni durumu parametre olarak verir).
    /// </summary>
    public event Action<bool>? OnToggled;

    /// <summary>
    /// Toggle'ın mevcut durumu (Açık veya Kapalı).
    /// </summary>
    public bool IsOn
    {
        get => _isOn;
        set
        {
            if (_isOn != value)
            {
                _isOn = value;
                OnToggled?.Invoke(_isOn);
            }
        }
    }

    /// <summary>
    /// Checkbox veya Switch'in arka plan dokusu.
    /// </summary>
    public Texture2D? BackgroundTexture { get; set; }

    /// <summary>
    /// Aktif (On) durumunu gösteren onay işareti vb. doku.
    /// </summary>
    public Texture2D? CheckTexture { get; set; }

    /// <summary>
    /// Etiket metninin rengi.
    /// </summary>
    public Color TextColor { get; set; } = Color.White;

    /// <summary>
    /// UiToggle nesnesini başlatır.
    /// </summary>
    /// <param name="bounds">Tıklanabilir alan.</param>
    /// <param name="label">Toggle yanındaki metin.</param>
    /// <param name="initialValue">Başlangıç durumu.</param>
    /// <param name="font">Metin fontu.</param>
    public UiToggle(Rectangle bounds, string label, bool initialValue, SpriteFont? font = null)
    {
        _bounds = bounds;
        _label = label;
        _isOn = initialValue;
        _font = font;
    }

    /// <summary>
    /// Toggle durumunu günceller.
    /// </summary>
    /// <param name="input">Mevcut girdi durumu.</param>
    public void Update(in InputState input)
    {
        if (input.FireJustPressed && _bounds.Contains(input.CursorPosition))
        {
            IsOn = !IsOn;
        }
    }

    /// <summary>
    /// Toggle bileşenini çizer.
    /// </summary>
    /// <param name="spriteBatch">Çizim nesnesi.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        // Kutu/arkaplan (Kare varsayıyoruz, sola dayalı)
        Rectangle boxRect = new Rectangle(_bounds.X, _bounds.Y, _bounds.Height, _bounds.Height);
        
        if (BackgroundTexture != null)
        {
            spriteBatch.Draw(BackgroundTexture, boxRect, Color.White);
        }

        if (_isOn && CheckTexture != null)
        {
            spriteBatch.Draw(CheckTexture, boxRect, Color.White);
        }

        // Metin (Kutunun sağında)
        if (!string.IsNullOrEmpty(_label) && _font != null)
        {
            Vector2 textSize = _font.MeasureString(_label);
            Vector2 textPos = new Vector2(
                _bounds.X + _bounds.Height + 10,
                _bounds.Y + (_bounds.Height - textSize.Y) / 2f
            );
            spriteBatch.DrawString(_font, _label, textPos, TextColor);
        }
    }
}
