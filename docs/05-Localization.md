# Çoklu Dil Desteği (Localization)

**ArarGames.Core.Localization** modülü, 40'tan fazla dili destekleyen, RTL (sağdan sola) ve Indic (karmaşık Hint alfabeleri) metinleri sorunsuz işleyen, sıfır çökme garantili (fail-safe) bir yerelleştirme sistemidir.

---

## 📜 Sözleşme ve Güvenilirlik İlkeleri

1. **Sıfır Çökme (Zero Exception)**: `T(key)` çağrısı dosya bulunamasa, JSON bozuk olsa, anahtar eksik olsa dahi **asla exception fırlatmaz**. Eksik anahtarda İngilizce'ye, o da yoksa anahtarın kendisine düşer.
2. **`Thread.CurrentCulture` Asla Değiştirilmez**: Sistem kültürü `InvariantCulture` olarak sabit tutulur. Böylece Türkçe veya Almanca sistemlerde `float.ToString()`'in virgül üretip `savegame.json` serileştirmesini bozması engellenir.
3. **Parametreli Metinlerde Invariant Biçimlendirme**: Formatlı çevirilerde `string.Format(CultureInfo.InvariantCulture, ...)` kullanılır; çeviride eksik/bozuk format parametresi varsa ham metin döndürülür, oyun çökmez.
4. **Gömülü Kaynak Yedeği (Embedded Fallback)**: `en.json` dosyası derleme sırasında DLL içine gömülüdür (`EmbeddedResource`). Mobil asset paketlemesi bozulsa dahi oyun İngilizce açılır.

---

## 🗂️ JSON Sözlük Formatı

Çeviriler `Content/Localization/{code}.json` yolunda saklanır.

Örnek `Content/Localization/tr.json`:
```json
{
  "MENU_START": "OYUNA BAŞLA",
  "MENU_SETTINGS": "AYARLAR",
  "MENU_QUIT": "ÇIKIŞ",
  "GAME_SCORE": "SKOR: {0}",
  "GAME_LEVEL_COMPLETED": "TEBRİKLER! BÖLÜM {0} TAMAMLANDI.",
  "POPUP_CONFIRM": "ONAYLA",
  "POPUP_CANCEL": "İPTAL",
  "_meta.placeholderKeys": [
    "UNTRANSLATED_KEY_1"
  ]
}
```

> [!NOTE]
> `_meta.placeholderKeys` listesindeki anahtarlar için sistem otomatik olarak İngilizce orijinal metni çizer; oyuncu ham veya taslak metinler görmez.

---

## 🌍 `LanguageCode` ve `LanguageInfo`

Tüm desteklenen diller `LanguageCode` enum'u ile listelenir. Kayıt dosyasına enum sırası değil, kalıcı metin kodu (`"tr"`, `"pt-BR"`) yazılır.

```csharp
public enum LanguageCode
{
    English,
    Turkish,
    BrazilianPortuguese,
    Spanish,
    German,
    French,
    TraditionalChinese,
    SimplifiedChinese,
    Japanese,
    Russian,
    Korean,
    Arabic,
    Persian,
    Hebrew,
    Hindi,
    // ... 40+ dil
}
```

### Font Aileleri (`ScriptFamily`)
MonoGame'in varsayılan `SpriteFont` yapısı tüm dünya alfabelerini tek bir font dosyasında toplayamaz. ArarGames dilleri font ailelerine ayırır:
- **Latin**: Standart ASCII ve aksanlı Latin karakterleri (`ThaleahFat.ttf`).
- **Pixel**: Çince, Japonca, Rusça, Kiril ve Yunanca için piksel font (`zpix.ttf`).
- **Noto**: Korece (Hangul) ve Vietnamca için (`NotoSansCJKtc`).
- **Thai**: Tay alfabesi.
- **Arabic / Hebrew**: Sağdan sola diller.
- **Devanagari / Gujarati / Gurmukhi / Bengali / Tamil / Telugu**: Hint alfabeleri.

---

## 🖋️ RTL ve Karmaşık Metin Şekillendirme (Text Shaping)

### 1. RTL (Arapça, İbranice, Farsça, Urduca)
Standart MonoGame `SpriteFont`, karakterleri soldan sağa ve harf harf çizer. Bu durum Arapça'da harflerin kopuk ve ters sırada görünmesine yol açar.
- **`RtlTextShaper`**: Harflerin kelimedeki konumuna göre (başta, ortada, sonda, izole) Arapça Presentation Forms-B (U+FE70–U+FEFF) bloklarını seçer ve metni sağdan sola görsel sıraya (BiDi) dönüştürür.

### 2. Indic (Hint Yazıları: Hintçe, Marathi, Bengali, Tamil vb.)
Hint alfabelerinde sesli harf işaretleri (matra) ünsüzün soluna geçebilir (reordering), birden çok ünsüz birleşip ligatür (conjunct) oluşturur veya 'r' harfi bir sonraki harfin üstüne taşınır (reph).
- **`IndicTextShaper`**: Önceden derlenmiş hece eşleme tablolarını kullanarak çalışma zamanında karmaşık heceleri tek bir PUA (Private Use Area) glifine indirger.

### ✨ Tek Noktadan Şekillendirme Optimizasyonu
Şekillendirme işlemi her `Draw` çağrısında **yapılmaz**. JSON sözlüğü belleğe yüklenirken (`Loc.LoadLanguage`) **yalnızca bir kez** uygulanır. Çizim katmanı metnin Arapça veya Hintçe olduğundan tamamen habersiz kalır; standart `DrawString` çağrılır.

---

## 🔍 `LanguageDetector` ile Cihaz Dilini Algılama

Oyuncu oyunu ilk açtığında cihazın işletim sistemi diline göre otomatik seçim yapılır:

```csharp
using ArarGames.Core.Localization;

LanguageCode detectedLanguage = LanguageDetector.DetectCurrentLanguage();
```

---

## 💻 Kullanım Örneği

### `ILocalizationService` Arayüzü
```csharp
public interface ILocalizationService
{
    LanguageCode CurrentLanguage { get; }
    bool IsRightToLeft { get; }
    void Initialize(LanguageCode defaultLanguage);
    bool SetLanguage(LanguageCode language);
    string GetText(string key);
    string GetText(string key, params object[] args);
}
```

### Kod İçinde Metin Çevirme
```csharp
// 1. Basit Çeviri
string playButtonText = Loc.T("MENU_START");

// 2. Formatlı Çeviri (Sayılar InvariantCulture ile biçimlendirilir)
int currentScore = 1500;
string scoreLabel = Loc.T("GAME_SCORE", currentScore); // "SKOR: 1500"

// 3. Dil Değiştirme
Loc.SetLanguage(LanguageCode.Turkish);
```
