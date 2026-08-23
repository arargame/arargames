using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArarGames.Engine.Input;

namespace ArarGames.Engine.Screens;

/// <summary>
/// Ekranlar arası geçişleri ve ekran yönetimini sağlayan sınıf.
/// </summary>
public class ScreenManager
{
    private readonly List<IGameScreen> _screens;
    private readonly Queue<Action> _mainThreadActions;
    private bool _isInitialized;

    /// <summary>
    /// Sanal çözünürlüğün genişliği.
    /// </summary>
    public int VirtualWidth { get; set; } = 1280;

    /// <summary>
    /// Sanal çözünürlüğün yüksekliği.
    /// </summary>
    public int VirtualHeight { get; set; } = 720;

    /// <summary>
    /// Genel ölçekleme matrisi (letterbox/pillarbox için).
    /// </summary>
    public Matrix GlobalScaleMatrix { get; private set; } = Matrix.Identity;

    /// <summary>
    /// Şu anda en üstte bulunan aktif ekran.
    /// </summary>
    public IGameScreen? ActiveScreen => _screens.Count > 0 ? _screens[^1] : null;

    /// <summary>
    /// ScreenManager sınıfının yeni bir örneğini başlatır.
    /// </summary>
    public ScreenManager()
    {
        _screens = new List<IGameScreen>();
        _mainThreadActions = new Queue<Action>();
    }

    /// <summary>
    /// Yeni bir ekranı yığının en üstüne ekler.
    /// </summary>
    /// <param name="screen">Eklenecek ekran.</param>
    public void Push(IGameScreen screen)
    {
        _mainThreadActions.Enqueue(() =>
        {
            if (_isInitialized)
            {
                screen.LoadContent();
            }
            _screens.Add(screen);
            screen.OnScreenBecameActive();
        });
    }

    /// <summary>
    /// En üstteki ekranı çıkarır.
    /// </summary>
    public void Pop()
    {
        _mainThreadActions.Enqueue(() =>
        {
            if (_screens.Count > 0)
            {
                var screen = _screens[^1];
                screen.UnloadContent();
                _screens.RemoveAt(_screens.Count - 1);

                ActiveScreen?.OnScreenBecameActive();
            }
        });
    }

    /// <summary>
    /// En üstteki ekranı belirtilen ekran ile değiştirir.
    /// </summary>
    /// <param name="screen">Yeni ekran.</param>
    public void Replace(IGameScreen screen)
    {
        _mainThreadActions.Enqueue(() =>
        {
            if (_screens.Count > 0)
            {
                var oldScreen = _screens[^1];
                oldScreen.UnloadContent();
                _screens.RemoveAt(_screens.Count - 1);
            }

            if (_isInitialized)
            {
                screen.LoadContent();
            }
            _screens.Add(screen);
            screen.OnScreenBecameActive();
        });
    }

    /// <summary>
    /// Tüm ekranları temizler ve belirtilen ekranı ekler.
    /// </summary>
    /// <param name="screen">Eklenecek olan tek ekran.</param>
    public void Reset(IGameScreen screen)
    {
        _mainThreadActions.Enqueue(() =>
        {
            foreach (var s in _screens)
            {
                s.UnloadContent();
            }
            _screens.Clear();

            if (_isInitialized)
            {
                screen.LoadContent();
            }
            _screens.Add(screen);
            screen.OnScreenBecameActive();
        });
    }

    /// <summary>
    /// İçerikleri yükler ve mevcut tüm ekranlar için LoadContent çağırır.
    /// </summary>
    public void Initialize()
    {
        _isInitialized = true;
        foreach (var screen in _screens)
        {
            screen.LoadContent();
        }
    }

    /// <summary>
    /// Ölçekleme matrisini hesaplar.
    /// </summary>
    /// <param name="device">Grafik cihazı.</param>
    public void CalculateMatrix(GraphicsDevice device)
    {
        var viewport = device.Viewport;
        float scaleX = (float)viewport.Width / VirtualWidth;
        float scaleY = (float)viewport.Height / VirtualHeight;
        float scale = Math.Min(scaleX, scaleY);

        int width = (int)(VirtualWidth * scale);
        int height = (int)(VirtualHeight * scale);

        int x = (viewport.Width - width) / 2;
        int y = (viewport.Height - height) / 2;

        device.Viewport = new Viewport(x, y, width, height);
        GlobalScaleMatrix = Matrix.CreateScale(scale, scale, 1.0f);
    }

    /// <summary>
    /// Fiziksel ekran koordinatını sanal çözünürlüğe dönüştürür.
    /// </summary>
    /// <param name="screenPoint">Ekran koordinatı.</param>
    /// <returns>Sanal koordinat.</returns>
    public Vector2 ScreenToVirtual(Vector2 screenPoint)
    {
        return Vector2.Transform(screenPoint, Matrix.Invert(GlobalScaleMatrix));
    }

    /// <summary>
    /// Ekranları günceller.
    /// </summary>
    /// <param name="gameTime">Oyun zamanı.</param>
    /// <param name="input">Girdi durumu.</param>
    public void Update(GameTime gameTime, in InputState input)
    {
        while (_mainThreadActions.Count > 0)
        {
            _mainThreadActions.Dequeue()?.Invoke();
        }

        if (_screens.Count == 0) return;

        bool isCovered = false;
        bool inputHandled = false;

        // Üstten alta doğru güncelleme
        for (int i = _screens.Count - 1; i >= 0; i--)
        {
            var screen = _screens[i];

            if (!inputHandled)
            {
                screen.HandleInput(input);
                inputHandled = true; // Sadece en üstteki ekran input alır (ya da isCovered değilse). İyileştirilebilir.
            }
            
            screen.Update(gameTime, isCovered);

            if (!screen.AllowBehindUpdates)
            {
                break;
            }

            if (!screen.IsOverlay)
            {
                isCovered = true;
            }
        }
    }

    /// <summary>
    /// Ekranları çizer.
    /// </summary>
    /// <param name="spriteBatch">Sprite çizicisi.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        if (_screens.Count == 0) return;

        int startIndex = 0;
        
        // Çizim yapmaya hangi ekrandan başlayacağımızı bul (alttan yukarı doğru)
        for (int i = _screens.Count - 1; i >= 0; i--)
        {
            if (!_screens[i].AllowBehindDraw)
            {
                startIndex = i;
                break;
            }
        }

        // Alttan üste doğru çizim
        for (int i = startIndex; i < _screens.Count; i++)
        {
            _screens[i].Draw(spriteBatch);
        }
    }
}
