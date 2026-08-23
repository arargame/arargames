using System.IO;

namespace ArarGames.Engine.Platform;

/// <summary>
/// Ekran yönelimi seçenekleri.
/// </summary>
public enum ScreenOrientation
{
    /// <summary>
    /// Yatay (Geniş) ekran modu.
    /// </summary>
    Landscape,
    
    /// <summary>
    /// Dikey (Uzun) ekran modu.
    /// </summary>
    Portrait,
    
    /// <summary>
    /// Cihazın serbest dönmesine izin veren mod.
    /// </summary>
    FreeRotation
}

/// <summary>
/// Platformlara özgü (Desktop/Mobil vb.) işlevleri sarmalayan servis arayüzü.
/// </summary>
public interface IPlatformService
{
    /// <summary>
    /// Cihazda oyun verilerinin (kayıtlar vb.) kaydedilebileceği dizinin yolu.
    /// </summary>
    string SaveDirectory { get; }

    /// <summary>
    /// Mevcut platformun bir mobil cihaz (Android/iOS) olup olmadığını belirtir.
    /// </summary>
    bool IsMobile { get; }

    /// <summary>
    /// Mevcut platformun bir masaüstü (Windows/Linux/Mac) olup olmadığını belirtir.
    /// </summary>
    bool IsDesktop { get; }

    /// <summary>
    /// Platformun adı (örn. "Android", "Windows").
    /// </summary>
    string PlatformName { get; }

    /// <summary>
    /// Sistem varsayılan tarayıcısı ile belirtilen URL'yi açar.
    /// </summary>
    /// <param name="url">Açılacak web adresi.</param>
    void OpenUrl(string url);

    /// <summary>
    /// Cihazı belirtilen süre kadar titreştirir (Sadece mobil veya desteklenen cihazlarda).
    /// </summary>
    /// <param name="milliseconds">Titreşim süresi (Milisaniye).</param>
    void Vibrate(int milliseconds);

    /// <summary>
    /// Mobil cihazlarda ekran yönelimini ayarlar.
    /// </summary>
    /// <param name="orientation">Yeni ekran yönü.</param>
    void SetOrientation(ScreenOrientation orientation);

    /// <summary>
    /// Uygulamayı tamamen sonlandırır.
    /// </summary>
    void Exit();

    /// <summary>
    /// Platforma özel yöntemlerle içerik dosyasını bir stream olarak açar.
    /// </summary>
    /// <param name="relativePath">Dosyanın göreceli yolu.</param>
    /// <returns>Okunabilir stream veya bulunamazsa null.</returns>
    Stream? OpenContentStream(string relativePath);
}
