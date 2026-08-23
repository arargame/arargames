using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ArarGames.Core.Events;
using ArarGames.Core.Localization;
using ArarGames.Engine.Audio;
using ArarGames.Engine.Platform;

namespace ArarGames.Engine.Screens;

/// <summary>
/// Oyunun tüm merkezi sistemlerini ve servislerini bir araya getiren bağlam sınıfı.
/// Ekranlar, bu bağlam aracılığıyla servislere erişir.
/// </summary>
public class GameContext
{
    /// <summary>
    /// Ekran yönetimini sağlayan sınıf.
    /// </summary>
    public ScreenManager Screens { get; }

    /// <summary>
    /// MonoGame grafik cihazı.
    /// </summary>
    public GraphicsDevice GraphicsDevice { get; }

    /// <summary>
    /// MonoGame içerik yöneticisi.
    /// </summary>
    public ContentManager Content { get; }

    /// <summary>
    /// Olay (Event) yönetim sistemi.
    /// </summary>
    public EventBus Events { get; }

    /// <summary>
    /// Ses servisi (Müzik ve Ses Efektleri). Opsiyoneldir, sonradan atanabilir.
    /// </summary>
    public IAudioService? Audio { get; set; }

    /// <summary>
    /// Yerelleştirme ve çoklu dil servisi. Opsiyoneldir.
    /// </summary>
    public ILocalizationService? Localization { get; set; }

    /// <summary>
    /// Platforma özel işlemler için servis (Mobil/Desktop). Opsiyoneldir.
    /// </summary>
    public IPlatformService? Platform { get; set; }

    /// <summary>
    /// GameContext sınıfının yeni bir örneğini başlatır.
    /// </summary>
    /// <param name="screens">Ekran yöneticisi.</param>
    /// <param name="graphics">Grafik cihazı.</param>
    /// <param name="content">İçerik yöneticisi.</param>
    public GameContext(ScreenManager screens, GraphicsDevice graphics, ContentManager content)
    {
        Screens = screens ?? throw new System.ArgumentNullException(nameof(screens));
        GraphicsDevice = graphics ?? throw new System.ArgumentNullException(nameof(graphics));
        Content = content ?? throw new System.ArgumentNullException(nameof(content));
        Events = new EventBus();
    }
}
