using System;
using Microsoft.Xna.Framework;

namespace ArarGames.Engine.Audio;

/// <summary>
/// Müzik ve ses efektleri yönetimini sağlayan servis arayüzü.
/// </summary>
public interface IAudioService : IDisposable
{
    /// <summary>
    /// Ses efektlerinin (SFX) açık/kapalı durumu.
    /// </summary>
    bool SoundEffectsEnabled { get; set; }

    /// <summary>
    /// Arka plan müziklerinin açık/kapalı durumu.
    /// </summary>
    bool MusicEnabled { get; set; }

    /// <summary>
    /// Genel ana ses seviyesi (0.0f - 1.0f).
    /// </summary>
    float MasterVolume { get; set; }

    /// <summary>
    /// Ses efektleri ses seviyesi (0.0f - 1.0f).
    /// </summary>
    float SfxVolume { get; set; }

    /// <summary>
    /// Müzik ses seviyesi (0.0f - 1.0f).
    /// </summary>
    float MusicVolume { get; set; }

    /// <summary>
    /// Belirtilen ses efektini çalar.
    /// </summary>
    /// <param name="name">Ses efektinin adı veya yolu.</param>
    /// <param name="volume">Çalma ses seviyesi çarpanı (0-1).</param>
    /// <param name="pitch">Sesin perdesi (-1.0 ile 1.0 arası).</param>
    /// <param name="pan">Sesin sağ-sol dengesi (-1.0 ile 1.0 arası).</param>
    void PlaySoundEffect(string name, float volume = 1f, float pitch = 0f, float pan = 0f);

    /// <summary>
    /// Belirtilen arka plan müziğini çalar.
    /// </summary>
    /// <param name="name">Müziğin adı veya yolu.</param>
    /// <param name="loop">Döngüye alınıp alınmayacağı.</param>
    void PlayMusic(string name, bool loop = true);

    /// <summary>
    /// Mevcut arka plan müziğini durdurur.
    /// </summary>
    /// <param name="fadeDuration">Azalarak (fade out) durması için saniye cinsinden süre.</param>
    void StopMusic(float fadeDuration = 0f);

    /// <summary>
    /// Mevcut müzikten yeni müziğe çapraz geçiş (crossfade) yapar.
    /// </summary>
    /// <param name="nextTrack">Yeni müziğin adı veya yolu.</param>
    /// <param name="fadeDuration">Geçişin saniye cinsinden süresi.</param>
    void CrossFadeMusic(string nextTrack, float fadeDuration = 1f);

    /// <summary>
    /// Çalan tüm sesleri ve müziği duraklatır.
    /// </summary>
    void PauseAll();

    /// <summary>
    /// Duraklatılmış olan sesleri ve müziği devam ettirir.
    /// </summary>
    void ResumeAll();

    /// <summary>
    /// Ses servisini (fade vb. için) düzenli olarak günceller.
    /// </summary>
    /// <param name="gameTime">Oyun zamanı nesnesi.</param>
    void Update(GameTime gameTime);
}
