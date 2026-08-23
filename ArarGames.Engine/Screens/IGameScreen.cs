using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArarGames.Engine.Input;

namespace ArarGames.Engine.Screens;

/// <summary>
/// Oyun ekranı arayüzü. Her bir farklı oyun durumu (Menü, Oyun İçi, Seçenekler vb.) için kullanılır.
/// </summary>
public interface IGameScreen
{
    /// <summary>
    /// Bu ekranın bir kaplama (overlay) olup olmadığını belirtir.
    /// Eğer kaplama ise, altındaki ekranların çizilmesi engellenmeyebilir.
    /// </summary>
    bool IsOverlay { get; }

    /// <summary>
    /// Bu ekran aktifken altındaki ekranların güncellenip güncellenmeyeceğini belirtir.
    /// </summary>
    bool AllowBehindUpdates { get; }

    /// <summary>
    /// Bu ekran aktifken altındaki ekranların çizilip çizilmeyeceğini belirtir.
    /// </summary>
    bool AllowBehindDraw { get; }

    /// <summary>
    /// Ekran yüklendiğinde içeriklerin yüklenmesi için çağrılır.
    /// </summary>
    void LoadContent();

    /// <summary>
    /// Ekran kapatıldığında veya değiştirildiğinde içeriklerin bellekten temizlenmesi için çağrılır.
    /// </summary>
    void UnloadContent();

    /// <summary>
    /// Her bir karede (frame) ekranın durumunu güncellemek için çağrılır.
    /// </summary>
    /// <param name="gameTime">Oyun zamanı bilgisi.</param>
    /// <param name="isCovered">Ekranın başka bir ekran tarafından örtülüp örtülmediğini belirtir.</param>
    void Update(GameTime gameTime, bool isCovered);

    /// <summary>
    /// Giriş cihazlarının (klavye, fare, dokunmatik) durumunu işlemek için çağrılır.
    /// </summary>
    /// <param name="input">Mevcut giriş durumu.</param>
    void HandleInput(in InputState input);

    /// <summary>
    /// Ekranın içeriğini çizmek için çağrılır.
    /// </summary>
    /// <param name="spriteBatch">Çizim işlemleri için kullanılan nesne.</param>
    void Draw(SpriteBatch spriteBatch);

    /// <summary>
    /// Geri (Back) tuşuna basıldığında tetiklenir.
    /// </summary>
    void OnBackPressed();

    /// <summary>
    /// Ekran en üste gelip aktif olduğunda çağrılır.
    /// </summary>
    void OnScreenBecameActive();
}
