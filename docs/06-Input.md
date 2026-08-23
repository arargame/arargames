# Girdi Sistemi (Input System)

**ArarGames.Engine.Input** modülü; klavye, fare, gamepad ve mobil dokunmatik ekranlardan gelen tüm ham girdileri tek bir birleşik, soyut ve hafif (allocation-free) veri yapısında toplar.

---

## 🎮 `InputState` Yapısı

`InputState`, her karede (frame) girdi durumunu taşımak üzere tasarlanmış bir `readonly struct`'tır. Heap tahsisatı (allocation) yapmaz ve GC yükü oluşturmaz.

```csharp
public readonly struct InputState
{
    /// <summary>
    /// Normalize edilmiş hareket yönü (Örn: WASD, Yön tuşları veya Sol Analog Çubuk).
    /// </summary>
    public Vector2 MoveDirection { get; init; }

    /// <summary>
    /// Fare veya dokunmatik ekranın sanal çözünürlük koordinatlarındaki pozisyonu.
    /// </summary>
    public Vector2 CursorPosition { get; init; }

    /// <summary>Ateş tuşuna basılı tutulup tutulmadığı.</summary>
    public bool FirePressed { get; init; }

    /// <summary>Ateş tuşuna bu karede yeni mi basıldığı (Edge-trigger).</summary>
    public bool FireJustPressed { get; init; }

    /// <summary>Geri (Back / Escape) tuşuna basılı tutulup tutulmadığı.</summary>
    public bool BackPressed { get; init; }

    /// <summary>Geri tuşuna bu karede yeni mi basıldığı.</summary>
    public bool BackJustPressed { get; init; }

    /// <summary>Duraklatma (Pause) tuşuna bu karede yeni mi basıldığı.</summary>
    public bool PauseJustPressed { get; init; }

    /// <summary>Onay (Enter / A Tuşu) tuşuna bu karede yeni mi basıldığı.</summary>
    public bool ConfirmJustPressed { get; init; }

    /// <summary>Herhangi bir tıklama veya dokunma gerçekleşip gerçekleşmediği.</summary>
    public bool AnyTouchOrClick { get; init; }

    /// <summary>Nişan alma yönü (Örn: Çift kollu nişancı oyunlarında sağ çubuk).</summary>
    public Vector2 AimDirection { get; init; }
}
```

---

## 🔌 `IInputProvider` Arayüzü

Platforma özel girdi toplama mantığı `IInputProvider` arayüzü ile yönetilir:

```csharp
public interface IInputProvider
{
    /// <summary>
    /// Donanım durumlarını okur ve birleşik InputState nesnesi üretir.
    /// </summary>
    InputState GetState(ScreenManager screenManager);

    /// <summary>
    /// Kare sonunda durumları sıfırlar veya önceki durumları günceller.
    /// </summary>
    void Update();
}
```

---

## 🖥️ Platform Sağlayıcıları

### 1. `KeyboardMouseInputProvider` (Masaüstü: Windows, Linux, macOS)
- **Klavye**: WASD ve Ok Tuşları `MoveDirection`'a haritalanır; `Space` ve `Ctrl` ateş tuşuna bağlanır; `Escape` geri tuşudur.
- **Fare**: `MouseState` okunur, `screenManager.ScreenToVirtual` ile sanal çözünürlük piksellerine dönüştürülür.
- **Gamepad / Kontrolcü**: `GamePadState` üzerinden analog çubuklar ve tuşlar okunur.

### 2. `TouchInputProvider` (Mobil: Android, iOS)
- **Dokunmatik Ekran**: `TouchPanel.GetState()` ile okunur.
- **Sanal Joystick / Alan**: Ekranın sol yarısındaki dokunmalar sanal joystick olarak `MoveDirection` oluşturur.
- **Dokunma ve Bırakma (Tap/Release)**: Ekranın sağ yarısı veya buton alanları `FireJustPressed` ve UI etkileşimlerini tetikler.
- **Android Geri Tuşu**: Donanım geri tuşu `BackJustPressed` olarak yakalanır.

---

## 🎯 Sanal Koordinat Dönüşümü (Virtual Screen Mapping)

Pencere boyutu veya cihaz çözünürlüğü ne olursa olsun, arayüz butonları ve oyun nesneleri sabit sanal koordinatlarla (örn. 1280x720) çalışır.

```
Fiziksel Ekran (1920x1080) ────► [ ScreenToVirtual ] ────► Sanal Ekran (1280x720)
       (X: 960, Y: 540)                                          (X: 640, Y: 360)
```

```csharp
Vector2 physicalMouse = new Vector2(mouseState.X, mouseState.Y);
Vector2 virtualMouse = screenManager.ScreenToVirtual(physicalMouse);
```

---

## 🕹️ Örnek: Oyuncu Hareketi ve Ateş Etme

Bir `GameScreen` veya oyun varlığı içinde girdi işleme örneği:

```csharp
public class Player
{
    public Vector2 Position;
    public float Speed = 300f; // Saniyede 300 sanal piksel

    public void UpdateInput(in InputState input, float deltaTime)
    {
        // 1. Hareket
        if (input.MoveDirection != Vector2.Zero)
        {
            Position += input.MoveDirection * Speed * deltaTime;
        }

        // 2. Ateş Etme
        if (input.FireJustPressed)
        {
            ShootLaser();
        }

        // 3. Nişan Alma (İmlece doğru veya analog çubukla)
        if (input.AimDirection != Vector2.Zero)
        {
            // Gamepad ile nişan
        }
        else
        {
            // Fare / Dokunmatik imlecine doğru nişan
            Vector2 aimVector = input.CursorPosition - Position;
            // Rotation = MathF.Atan2(aimVector.Y, aimVector.X);
        }
    }

    private void ShootLaser()
    {
        // Mermi oluştur
    }
}
```
