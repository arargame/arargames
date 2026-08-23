using Microsoft.Xna.Framework;

namespace ArarGames.Engine.Input;

/// <summary>
/// Tüm girdi cihazlarından (klavye, fare, gamepad, dokunmatik) gelen durumu 
/// birleştirilmiş ve soyutlanmış olarak tutan veri yapısı.
/// </summary>
public readonly struct InputState
{
    /// <summary>
    /// Hareket yönü (Normalize edilmiş, -1 ile 1 arası değerler).
    /// </summary>
    public Vector2 MoveDirection { get; init; }

    /// <summary>
    /// Fare veya dokunmatik ekranın imleç pozisyonu (Sanal koordinat sisteminde).
    /// </summary>
    public Vector2 CursorPosition { get; init; }

    /// <summary>
    /// Ateş/Seçim tuşuna (Fare Sol Tık, Boşluk vb.) basılı tutulup tutulmadığı.
    /// </summary>
    public bool FirePressed { get; init; }

    /// <summary>
    /// Ateş/Seçim tuşuna bu karede yeni mi basıldığı.
    /// </summary>
    public bool FireJustPressed { get; init; }

    /// <summary>
    /// Geri (Back) tuşuna basılı tutulup tutulmadığı.
    /// </summary>
    public bool BackPressed { get; init; }

    /// <summary>
    /// Geri (Back) tuşuna bu karede yeni mi basıldığı.
    /// </summary>
    public bool BackJustPressed { get; init; }

    /// <summary>
    /// Duraklat (Pause) tuşuna basılı tutulup tutulmadığı.
    /// </summary>
    public bool PausePressed { get; init; }

    /// <summary>
    /// Duraklat (Pause) tuşuna bu karede yeni mi basıldığı.
    /// </summary>
    public bool PauseJustPressed { get; init; }

    /// <summary>
    /// Onay (Enter, vb.) tuşuna basılı tutulup tutulmadığı.
    /// </summary>
    public bool ConfirmPressed { get; init; }

    /// <summary>
    /// Onay tuşuna bu karede yeni mi basıldığı.
    /// </summary>
    public bool ConfirmJustPressed { get; init; }

    /// <summary>
    /// Herhangi bir dokunma veya tıklama işleminin gerçekleşip gerçekleşmediği.
    /// </summary>
    public bool AnyTouchOrClick { get; init; }

    /// <summary>
    /// Nişan alma yönü (Örn: Çift analog çubuğun sağ çubuğu).
    /// </summary>
    public Vector2 AimDirection { get; init; }
}
