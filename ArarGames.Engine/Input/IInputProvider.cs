using Microsoft.Xna.Framework;

namespace ArarGames.Engine.Input;

/// <summary>
/// Girdi sağlayıcı (Klavye, Dokunmatik, Gamepad) arayüzü.
/// </summary>
public interface IInputProvider
{
    /// <summary>
    /// Şu anki girdi durumunu alır.
    /// </summary>
    /// <returns>Soyutlanmış girdi durumu.</returns>
    InputState GetState();

    /// <summary>
    /// Girdi durumlarını günceller (Önceki kare durumlarını takip etmek için).
    /// </summary>
    /// <param name="gameTime">Oyun zamanı nesnesi.</param>
    void Update(GameTime gameTime);
}
