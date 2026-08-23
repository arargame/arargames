# Mevcut Projeleri ArarGames'e Geçirme Rehberi (Migration Guide)

Bu rehber, mevcut bağımsız MonoGame projelerinizi (**PaintTrek**, **Blocked** vb.) **ArarGames Framework** standartlarına taşımak için gereken adımları, dönüşüm tablolarını ve kritik mimari değişiklikleri açıklar.

---

## 🎯 Geçişin Sağladığı Avantajlar

- **Tekilleştirilmiş Kod Tabanı**: Ses, yerelleştirme, ekran yönetimi ve platform kodlarının her projede sıfırdan yazılması son bulur.
- **Yüksek Güvenilirlik**: Atomik save dosyası yazımı (`.tmp` + `.corrupt` kurtarma) ile oyuncu verilerinin kaybolması önlenir.
- **Sıfır Tahsisat / GC Performansı**: `struct` tabanlı `EventBus` ve `InputState` ile mobil cihazlarda takılmalar (micro-stutters) engellenir.
- **Çoklu Dil Mükemmelliği**: 40+ dil, RTL ve Hint dilleri için derleme/yükleme zamanı tek noktadan şekillendirme (text shaping).

---

## 🔄 Proje Bazlı Dönüşüm Tablosu

### 1. PaintTrek Projesi Dönüşümü

| Eski PaintTrek Yapısı | Yeni ArarGames Karşılığı | Notlar |
| :--- | :--- | :--- |
| `ScreenManagement.ScreenManager` | `ArarGames.Engine.Screens.ScreenManager` | `Push`, `Pop`, `Replace`, `Reset` metodlarına geçildi; ana iş parçacığı kuyruğu eklendi. |
| `ScreenManagement.GameScreen` | `ArarGames.Engine.Screens.GameScreen` | `GameContext` enjeksiyonu ve `HandleInput(in InputState)` yapısı. |
| `PaintTrek.Shared.GameSettings` | `ArarGames.Core.Persistence.JsonSaveService<PaintTrekSaveData>` | Atomik yazma ve dirty-tracking desteği. |
| `IGamePlatformServices` | `ArarGames.Engine.Platform.IPlatformService` | `DesktopPlatformService` ve `AndroidPlatformService`. |
| `Localization.Loc` | `ArarGames.Core.Localization.ILocalizationService` | `strings.{code}.json` yapısı korundu; `IndicTextShaper` ve `RtlTextShaper` çekirdeğe alındı. |

---

### 2. Blocked Projesi Dönüşümü

| Eski Blocked Yapısı | Yeni ArarGames Karşılığı | Notlar |
| :--- | :--- | :--- |
| `Blocked.Shared.Managers.ScreenManager` | `ArarGames.Engine.Screens.ScreenManager` | Static singleton yerine `GameContext.Screens` üzerinden erişim. |
| `Blocked.Shared.Audio.SoundService` | `ArarGames.Engine.Audio.SoundManager` | Throttling (45ms), dinamik pitch varyansı ve `missingAssets` güvenliği dahil edildi. |
| `Blocked.Shared.Audio.MusicService` | `ArarGames.Engine.Audio.SoundManager` | SFX ve Müzik tek bir `IAudioService` çatısında birleştirildi; `CrossFadeMusic` doğrudan desteklenir. |
| `Blocked.Shared.Managers.ObjectPool` | `ArarGames.Core.Pooling.ObjectPool<T>` | Double-return koruması ve `Warmup` yeteneği eklendi. |
| `Blocked.Shared.Localization.Loc` | `ArarGames.Core.Localization.ILocalizationService` | Statik çağrılar yerine `GameContext.Localization` veya `Loc` adaptörü. |
| `Blocked.Shared.Managers.InputManager` | `ArarGames.Engine.Input.InputState` | Static durum polling yerine ekranlara parametre olarak aktarılan `readonly struct`. |

---

## 🛠️ Adım Adım Geçiş Stratejisi

### Adım 1: Proje Referanslarını Güncelleme
Projenizin `.csproj` dosyasından yerel kopya yardımcı sınıfları temizleyin ve referansları ekleyin:
```xml
<ItemGroup>
  <ProjectReference Include="..\ArarGames\ArarGames.Core\ArarGames.Core.csproj" />
  <ProjectReference Include="..\ArarGames\ArarGames.Engine\ArarGames.Engine.csproj" />
</ItemGroup>
```

### Adım 2: `Game1.cs` Yeniden Yapılandırması
1. `ScreenManager` ve `GameContext` oluşturun.
2. `Update` içinde donanım girdilerini `InputState` yapısına dönüştürün.
3. `Draw` içinde `_screenManager.CalculateMatrix` ve `_screenManager.Draw` kullanın.

```csharp
protected override void Initialize()
{
    base.Initialize();

    _screenManager = new ScreenManager { VirtualWidth = 1280, VirtualHeight = 720 };
    _context = new GameContext(_screenManager, GraphicsDevice, Content)
    {
        Audio = new SoundManager(name => Content.Load<SoundEffect>(name)),
        Platform = new DesktopPlatformService("BlockedGame")
    };

    _screenManager.Initialize();
    _screenManager.Push(new MainMenuScreen(_context));
}
```

### Adım 3: Ekran Sınıflarının Güncellenmesi
Tüm `Screen` sınıflarını `GameScreen` abstract sınıfından türetin ve constructor'da `GameContext` alın:

```diff
- public class GameplayScreen : Screen
+ public class GameplayScreen : GameScreen
  {
-     public GameplayScreen()
+     public GameplayScreen(GameContext context) : base(context)
      {
      }

-     public override void HandleInput()
-     {
-         if (InputManager.IsKeyPressed(Keys.Escape))
-             ScreenManager.Instance.Pop();
-     }
+     public override void HandleInput(in InputState input)
+     {
+         if (input.BackJustPressed)
+             Context.Screens.Pop();
+     }
  }
```

### Adım 4: Kayıt Sistemini `JsonSaveService`'e Taşıma
Eski statik `GameSettings` dosyasındaki alanları bir POCO sınıfta toplayın:

```csharp
public class GameSaveData
{
    public int HighScore { get; set; }
    public int Coins { get; set; }
    public string Language { get; set; } = "en";
    public float SfxVolume { get; set; } = 1.0f;
    public float MusicVolume { get; set; } = 1.0f;
}

// Başlatma:
var storage = new FileStorageProvider(_context.Platform.GetSaveDirectory());
var saveService = new JsonSaveService<GameSaveData>("savegame.json", storage);
saveService.Load();
```

---

## ⚠️ Kritik Değişiklikler (Breaking Changes) ve Çözümler

### 1. Statik Singleton'lardan Context Enjeksiyonuna Geçiş
- **Eski**: `ScreenManager.Instance.Push(new LevelScreen());`
- **Yeni**: `Context.Screens.Push(new LevelScreen(Context));`
- **Neden**: Test edilebilirlik, modülerlik ve çoklu ekran örneklerinin yaşam döngüsü güvenliği.

### 2. Girdi Okuma Mantığı
- **Eski**: `InputManager.IsLeftMouseButtonClicked()` (Her yerden statik okuma)
- **Yeni**: `HandleInput(in InputState input)` (Ekran bazlı sıralı dağıtım)
- **Neden**: Overlay/Pause ekranı açıldığında alttaki ekranın fare tıklamalarını yanlışlıkla tüketmesini engelleme.

### 3. Ses Çalma Metotları
- **Eski**: `SoundService.Play("InSelectionSound");` ve `MusicService.PlayTrack("Theme");`
- **Yeni**: `Context.Audio?.PlaySoundEffect("Sounds/InSelectionSound");` ve `Context.Audio?.PlayMusic("Music/Theme");`
- **Neden**: SFX ve BGM yönetimini tek serviste toplayıp crossfade ve throttling sağlamak.
