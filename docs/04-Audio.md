# Ses Sistemi (Audio System)

**ArarGames.Engine.Audio** modülü, ses efektleri (SFX) ve arka plan müziklerinin (BGM) yönetimi için yüksek performanslı, hata toleranslı ve esnek bir altyapı sunar.

---

## 🎧 `IAudioService` Arayüzü

Oyun motorunun tüm ses operasyonları `IAudioService` arayüzü ile soyutlanmıştır:

```csharp
public interface IAudioService : IDisposable
{
    bool SoundEffectsEnabled { get; set; }
    bool MusicEnabled { get; set; }

    float MasterVolume { get; set; } // 0.0f - 1.0f
    float SfxVolume { get; set; }    // 0.0f - 1.0f
    float MusicVolume { get; set; }  // 0.0f - 1.0f

    void PlaySoundEffect(string name, float volume = 1f, float pitch = 0f, float pan = 0f);
    void PlayMusic(string name, bool loop = true);
    void StopMusic(float fadeDuration = 0f);
    void CrossFadeMusic(string nextTrack, float fadeDuration = 1f);
    void PauseAll();
    void ResumeAll();
    void Update(GameTime gameTime);
}
```

---

## ⚡ `SoundManager` Özellikleri

`SoundManager`, `IAudioService` arayüzünü uygulayan ana sınıftır ve aşağıdaki üretim düzeyinde optimizasyonları içerir:

### 1. SFX Throttling (Aşırı Çalma Koruması)
Aksiyon oyunlarında (örneğin patlamalar veya makineli tüfek atışları) aynı ses efekti tek bir karede veya saniyenin onda birinde yüzlerce kez tetiklenebilir. Bu durum ses patlamalarına, aşırı ses bozulmalarına ve CPU kilitlenmelerine yol açar.
- `SoundManager`, her bir ses efekti için minimum tekrar aralığı (`MinRepeatInterval = 0.045s`) uygular.
- 45 milisaniye dolmadan gelen mükerrer istekler sessizce elenir.

### 2. Dinamik Pitch Varyansı (Dynamic Pitch Variance)
Aynı ses efekti arka arkaya çaldığında makineleşmiş bir his uyandırmaması için:
- Her çalma isteğinde perdede küçük rastgele varyasyon uygulanır (`pitch ± 0.05f`).
- Bu sayede oyun sesleri çok daha doğal ve organik duyulur.

### 3. Eksik Varlık Toleransı (Missing Asset Tolerance)
Bir ses dosyası Content klasöründe bulunamazsa veya silinirse:
- Oyun **asla çökmez**.
- Dosya `_missingAssets` listesine eklenir ve bir daha diski/içerik yöneticisini boş yere sorgulayarak performans kaybına yol açmaz.

### 4. Müzik Crossfade ve Fade-Out
Bölüm geçişlerinde veya menüden oyuna geçerken müzikler aniden kesilmez:
- `CrossFadeMusic("Music/BossTheme", 1.5f)`: Mevcut müziğin sesini 1.5 saniyede kısarak yeni müziğe pürüzsüz geçiş yapar.
- `StopMusic(1.0f)`: Müziği 1 saniyede yumuşakça durdurur.

---

## 📂 `RawAssetLoader` (MGCB'siz Doğrudan Yükleme)

MonoGame projelerinde büyük `.wav` sesleri veya dinamik `.png` resimleri MGCB (`Content.mgcb`) ile `.xnb` formatına dönüştürmeden doğrudan yüklemek gerekebilir. `RawAssetLoader`, `TitleContainer` üzerinden ham dosya okumayı sağlar.

```csharp
using ArarGames.Engine.Audio;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

// 1. WAV dosyasını doğrudan yükleme:
SoundEffect explosion = RawAssetLoader.LoadSoundEffect("Content/Sounds/explosion.wav");

// 2. PNG dosyasını Premultiplied Alpha formatında yükleme:
Texture2D texture = RawAssetLoader.LoadTexture(GraphicsDevice, "Content/Textures/player.png");
```

---

## 🎮 Kullanım Örneği

### Servisin Başlatılması (Game1.cs)
```csharp
// ContentManager ile yükleyici tanımlama
_context.Audio = new SoundManager(name => Content.Load<SoundEffect>(name))
{
    MasterVolume = 0.8f,
    MusicVolume = 0.7f,
    SfxVolume = 1.0f
};
```

### Ekran İçinde Kullanım
```csharp
public class GameplayScreen : GameScreen
{
    public GameplayScreen(GameContext context) : base(context) { }

    public override void LoadContent()
    {
        // Bölüm müziğini başlat veya yumuşak geçiş yap
        Context.Audio?.CrossFadeMusic("Music/Level1_Theme", fadeDuration: 2.0f);
    }

    public void OnPlayerShoot()
    {
        // Lazer ses efekti çal
        Context.Audio?.PlaySoundEffect("Sounds/LaserShoot", volume: 0.9f);
    }

    public void OnBossAppeared()
    {
        // Boss müziğine geçiş
        Context.Audio?.CrossFadeMusic("Music/Boss_Fight", fadeDuration: 1.0f);
    }

    public void OnPlayerDeath()
    {
        // Patlama sesi ve müziğin kısılması
        Context.Audio?.PlaySoundEffect("Sounds/BigExplosion", volume: 1.0f);
        Context.Audio?.StopMusic(fadeDuration: 1.5f);
    }
}
```
