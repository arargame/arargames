using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArarGames.Engine.Input;

namespace ArarGames.Engine.UI;

/// <summary>
/// Ekranda beliren uyarı/onay penceresi (Modal). Altındaki etkileşimi keser.
/// </summary>
public class ModalWindow
{
    private string _title;
    private string _message;
    
    private UiButton _confirmButton;
    private UiButton? _cancelButton;

    /// <summary>
    /// Modal'ın görünür olup olmadığı.
    /// </summary>
    public bool IsVisible { get; private set; }

    /// <summary>
    /// Onay (Confirm) butonuna tıklandığında tetiklenir.
    /// </summary>
    public event Action? OnConfirmed;

    /// <summary>
    /// İptal (Cancel) butonuna tıklandığında tetiklenir.
    /// </summary>
    public event Action? OnCancelled;

    /// <summary>
    /// Modal penceresinin arkaplan rengi/dokusu için kullanılacak renk.
    /// </summary>
    public Color BackgroundColor { get; set; } = new Color(0, 0, 0, 200);
    
    /// <summary>
    /// İçerik kutusunun rengi.
    /// </summary>
    public Color BoxColor { get; set; } = Color.DarkSlateBlue;

    /// <summary>
    /// ModalWindow nesnesini başlatır. Buton boyutları varsayılan atanır, güncelleme öncesi konumlandırılmalıdır.
    /// </summary>
    /// <param name="title">Pencere başlığı.</param>
    /// <param name="message">Açıklama mesajı.</param>
    /// <param name="confirmText">Onay butonu metni.</param>
    /// <param name="cancelText">İptal butonu metni (null ise tek butonlu bilgi ekranı olur).</param>
    public ModalWindow(string title, string message, string confirmText, string? cancelText = null)
    {
        _title = title;
        _message = message;

        _confirmButton = new UiButton(new Rectangle(0, 0, 150, 50), confirmText);
        _confirmButton.OnClicked += () =>
        {
            Hide();
            OnConfirmed?.Invoke();
        };

        if (cancelText != null)
        {
            _cancelButton = new UiButton(new Rectangle(0, 0, 150, 50), cancelText);
            _cancelButton.OnClicked += () =>
            {
                Hide();
                OnCancelled?.Invoke();
            };
        }
    }

    /// <summary>
    /// Butonların dokularını ayarlar.
    /// </summary>
    public void SetButtonTextures(Texture2D backgroundTexture, SpriteFont font)
    {
        _confirmButton.BackgroundTexture = backgroundTexture;
        // typeof özelliği kullanılmadığı için basit reflection ile atandı
        var prop = _confirmButton.GetType().GetField("_font", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        prop?.SetValue(_confirmButton, font);
        
        if (_cancelButton != null)
        {
            _cancelButton.BackgroundTexture = backgroundTexture;
            prop?.SetValue(_cancelButton, font);
        }
    }

    /// <summary>
    /// Modalı ekranda gösterir.
    /// </summary>
    public void Show()
    {
        IsVisible = true;
    }

    /// <summary>
    /// Modalı gizler.
    /// </summary>
    public void Hide()
    {
        IsVisible = false;
    }

    /// <summary>
    /// Modal aktifse girdi alır ve butonları günceller.
    /// </summary>
    /// <param name="input">Mevcut girdi durumu.</param>
    public void Update(in InputState input)
    {
        if (!IsVisible) return;

        _confirmButton.Update(input);
        _cancelButton?.Update(input);
    }

    /// <summary>
    /// Modalı ekranın ortasına çizer.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, SpriteFont font, Rectangle viewport)
    {
        if (!IsVisible) return;

        // 1. Yarı saydam overlay (tam ekran) - Tek piksel beyaz texture (veya benzeri) kullanıldığını varsayıyoruz
        // Eğer texture yoksa çizilemez, Engine seviyesinde bir null-texture servisi olsa iyi olur.
        // Şimdilik sadece butonlar ve metinler çizilecek (veya boş background).
        
        int boxWidth = 600;
        int boxHeight = 400;
        int x = viewport.X + (viewport.Width - boxWidth) / 2;
        int y = viewport.Y + (viewport.Height - boxHeight) / 2;

        Rectangle boxRect = new Rectangle(x, y, boxWidth, boxHeight);

        // Kutu arkaplanı (Burası içi Engine.Graphics.ShapeDrawer gibi bir şey ile çizilebilir, texture gerekli)
        
        // Başlık
        Vector2 titleSize = font.MeasureString(_title);
        spriteBatch.DrawString(font, _title, new Vector2(x + (boxWidth - titleSize.X) / 2f, y + 20), Color.White);

        // Mesaj
        Vector2 msgSize = font.MeasureString(_message);
        spriteBatch.DrawString(font, _message, new Vector2(x + (boxWidth - msgSize.X) / 2f, y + 100), Color.LightGray);

        // Buton Konumlandırma
        if (_cancelButton != null)
        {
            // İki buton
            _cancelButton.GetType().GetField("_bounds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(_cancelButton, new Rectangle(x + 100, y + boxHeight - 80, 150, 50));
            _confirmButton.GetType().GetField("_bounds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(_confirmButton, new Rectangle(x + boxWidth - 250, y + boxHeight - 80, 150, 50));
        }
        else
        {
            // Tek buton ortada
            _confirmButton.GetType().GetField("_bounds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(_confirmButton, new Rectangle(x + (boxWidth - 150) / 2, y + boxHeight - 80, 150, 50));
        }

        _confirmButton.Draw(spriteBatch);
        _cancelButton?.Draw(spriteBatch);
    }
}
