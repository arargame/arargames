using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArarGames.Engine.Input;

namespace ArarGames.Engine.UI;

/// <summary>
/// Buton durumları.
/// </summary>
public enum ButtonState
{
    Normal,
    Hovered,
    Pressed,
    Disabled
}

/// <summary>
/// Basit metin veya ikon içeren UI buton bileşeni.
/// </summary>
public class UiButton
{
    private Rectangle _bounds;
    private string _text;
    private SpriteFont? _font;
    private ButtonState _state;
    private bool _isEnabled = true;

    /// <summary>
    /// Buton tıklandığında tetiklenen olay.
    /// </summary>
    public event Action? OnClicked;

    /// <summary>
    /// Butonun arka plan dokusu.
    /// </summary>
    public Texture2D? BackgroundTexture { get; set; }

    /// <summary>
    /// Buton içindeki ikon dokusu.
    /// </summary>
    public Texture2D? IconTexture { get; set; }

    /// <summary>
    /// Normal durumda çizilecek renk.
    /// </summary>
    public Color NormalColor { get; set; } = Color.White;

    /// <summary>
    /// Fare ile üzerine gelindiğinde çizilecek renk.
    /// </summary>
    public Color HoverColor { get; set; } = Color.LightGray;

    /// <summary>
    /// Basılı tutulduğunda çizilecek renk.
    /// </summary>
    public Color PressedColor { get; set; } = Color.Gray;

    /// <summary>
    /// Devre dışıyken çizilecek renk.
    /// </summary>
    public Color DisabledColor { get; set; } = Color.DarkGray;

    /// <summary>
    /// Buton metni rengi.
    /// </summary>
    public Color TextColor { get; set; } = Color.Black;

    /// <summary>
    /// Butonun aktif olup olmadığını belirtir. Devre dışıysa tıklanamaz.
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            if (!_isEnabled) _state = ButtonState.Disabled;
            else _state = ButtonState.Normal;
        }
    }

    /// <summary>
    /// UiButton nesnesini başlatır.
    /// </summary>
    /// <param name="bounds">Butonun ekrandaki alanı (sanal koordinat).</param>
    /// <param name="text">Buton metni.</param>
    /// <param name="font">Metin fontu.</param>
    public UiButton(Rectangle bounds, string text, SpriteFont? font = null)
    {
        _bounds = bounds;
        _text = text;
        _font = font;
        _state = ButtonState.Normal;
    }

    /// <summary>
    /// Buton durumunu girdiye göre günceller.
    /// </summary>
    /// <param name="input">Mevcut girdi durumu.</param>
    public void Update(in InputState input)
    {
        if (!IsEnabled) return;

        bool isHovered = _bounds.Contains(input.CursorPosition);

        if (isHovered)
        {
            if (input.FirePressed)
            {
                _state = ButtonState.Pressed;
            }
            else
            {
                if (_state == ButtonState.Pressed)
                {
                    // Tıklama bırakıldı
                    OnClicked?.Invoke();
                }
                _state = ButtonState.Hovered;
            }
        }
        else
        {
            _state = ButtonState.Normal;
        }
    }

    /// <summary>
    /// Butonu çizer.
    /// </summary>
    /// <param name="spriteBatch">Çizim nesnesi.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        Color drawColor = _state switch
        {
            ButtonState.Hovered => HoverColor,
            ButtonState.Pressed => PressedColor,
            ButtonState.Disabled => DisabledColor,
            _ => NormalColor,
        };

        if (BackgroundTexture != null)
        {
            spriteBatch.Draw(BackgroundTexture, _bounds, drawColor);
        }

        if (IconTexture != null)
        {
            // Ortala
            Vector2 iconPos = new Vector2(
                _bounds.X + (_bounds.Width - IconTexture.Width) / 2f,
                _bounds.Y + (_bounds.Height - IconTexture.Height) / 2f
            );
            spriteBatch.Draw(IconTexture, iconPos, drawColor);
        }

        if (!string.IsNullOrEmpty(_text) && _font != null)
        {
            Vector2 size = _font.MeasureString(_text);
            Vector2 textPos = new Vector2(
                _bounds.X + (_bounds.Width - size.X) / 2f,
                _bounds.Y + (_bounds.Height - size.Y) / 2f
            );
            spriteBatch.DrawString(_font, _text, textPos, TextColor);
        }
    }
}
