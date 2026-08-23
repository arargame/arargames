# Platform Soyutlama (Platform Abstraction)

**ArarGames.Engine.Platform** modülü, platforma özgü işlemleri (dosya sistemi yolları, cihaz titreşimi/haptik, web tarayıcısında bağlantı açma, ekran yönü kontrolü) tek bir arayüz arkasında soyutlayarak oyun mantığının tamamen taşınabilir kalmasını sağlar.

---

## 📱 `IPlatformService` Arayüzü

```csharp
namespace ArarGames.Engine.Platform;

/// <summary>
/// Defines platform-specific services and capabilities.
/// </summary>
public interface IPlatformService
{
    /// <summary>
    /// Gets the platform name (e.g., "Desktop", "Android", "iOS").
    /// </summary>
    string PlatformName { get; }

    /// <summary>
    /// Gets the persistent save data directory path.
    /// </summary>
    string GetSaveDirectory();

    /// <summary>
    /// Triggers device vibration / haptic feedback for the given duration.
    /// </summary>
    /// <param name="milliseconds">Duration in milliseconds.</param>
    void Vibrate(int milliseconds);

    /// <summary>
    /// Opens the specified URL in the default system web browser.
    /// </summary>
    /// <param name="url">Target URL string.</param>
    void OpenUrl(string url);

    /// <summary>
    /// Sets the screen orientation dynamically (e.g. Portrait, Landscape).
    /// </summary>
    void SetOrientation(DisplayOrientation orientation);
}
```

---

## 💻 1. Masaüstü Implementasyonu (`DesktopPlatformService`)

Windows, Linux ve macOS için standart implementasyon:

```csharp
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;

namespace ArarGames.Engine.Platform;

public class DesktopPlatformService : IPlatformService
{
    private readonly string _appName;

    public string PlatformName => "Desktop";

    public DesktopPlatformService(string appName = "ArarGamesApp")
    {
        _appName = appName;
    }

    public string GetSaveDirectory()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string path = Path.Combine(appData, _appName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    public void Vibrate(int milliseconds)
    {
        // Desktop platforms do not have built-in vibration (No-Op)
    }

    public void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to open URL: {ex.Message}");
        }
    }

    public void SetOrientation(DisplayOrientation orientation)
    {
        // Desktop windows handle orientation via window sizing
    }
}
```

---

## 🤖 2. Android Implementasyonu (`AndroidPlatformService`)

Android platformu için Activity ve Context bağımlılıklarını içeren implementasyon:

```csharp
#if ANDROID
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;

namespace ArarGames.Engine.Platform;

public class AndroidPlatformService : IPlatformService
{
    private readonly Activity _activity;

    public string PlatformName => "Android";

    public AndroidPlatformService(Activity activity)
    {
        _activity = activity ?? throw new ArgumentNullException(nameof(activity));
    }

    public string GetSaveDirectory()
    {
        return _activity.FilesDir?.AbsolutePath ?? AppDomain.CurrentDomain.BaseDirectory;
    }

    public void Vibrate(int milliseconds)
    {
        try
        {
            if (_activity.GetSystemService(Context.VibratorService) is Vibrator vibrator && vibrator.HasVibrator)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    vibrator.Vibrate(VibrationEffect.CreateOneShot(milliseconds, VibrationEffect.DefaultAmplitude));
                }
                else
                {
#pragma warning disable CS0618
                    vibrator.Vibrate(milliseconds);
#pragma warning restore CS0618
                }
            }
        }
        catch
        {
            // Ignore if vibration permission is missing
        }
    }

    public void OpenUrl(string url)
    {
        try
        {
            var uri = Android.Net.Uri.Parse(url);
            var intent = new Intent(Intent.ActionView, uri);
            intent.AddFlags(ActivityFlags.NewTask);
            _activity.StartActivity(intent);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to open URL on Android: {ex.Message}");
        }
    }

    public void SetOrientation(DisplayOrientation orientation)
    {
        _activity.RequestedOrientation = orientation switch
        {
            DisplayOrientation.Portrait => Android.Content.PM.ScreenOrientation.Portrait,
            DisplayOrientation.LandscapeLeft or DisplayOrientation.LandscapeRight => Android.Content.PM.ScreenOrientation.SensorLandscape,
            _ => Android.Content.PM.ScreenOrientation.Unspecified
        };
    }
}
#endif
```

---

## 🚀 Entegrasyon ve Kullanım

Platform servisi `GameContext.Platform` üzerinden tüm ekranlara ulaştırılır:

### Başlatma (Desktop `Game1.cs` / Android `Activity1.cs`):
```csharp
// GameContext'e platform servisini ata
_context.Platform = new DesktopPlatformService("MyGame");
```

### Ekran İçinde Kullanım (Örn: `OptionsScreen.cs`):
```csharp
public class OptionsScreen : GameScreen
{
    public OptionsScreen(GameContext context) : base(context) { }

    public void OnPrivacyPolicyClicked()
    {
        // Gizlilik sözleşmesini tarayıcıda aç
        Context.Platform?.OpenUrl("https://arargames.com/privacy");
    }

    public void OnBombExploded()
    {
        // Mobil cihazda 100ms haptik titreşim ver
        Context.Platform?.Vibrate(100);
    }
}
```
