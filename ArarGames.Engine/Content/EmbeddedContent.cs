using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArarGames.Engine.Content;

/// <summary>
/// Projeye gömülü (Embedded Resource) olan kaynakları okumak için yardımcı sınıf.
/// MGCB pipeline kullanmadan basit Asset'leri dağıtmak için kullanışlıdır.
/// </summary>
public static class EmbeddedContent
{
    /// <summary>
    /// Gömülü bir PNG vb. resim dosyasını okuyup Texture2D nesnesine dönüştürür.
    /// </summary>
    /// <param name="device">Grafik cihazı.</param>
    /// <param name="fullResourceName">Kaynağın tam namespace yolu (Örn: "ArarGames.Engine.Resources.Logo.png").</param>
    /// <returns>Oluşturulan Texture2D nesnesi veya bulunamazsa null.</returns>
    public static Texture2D? LoadTexture(GraphicsDevice device, string fullResourceName)
    {
        using var stream = GetStream(fullResourceName);
        if (stream == null) return null;

        Texture2D texture = Texture2D.FromStream(device, stream);
        
        // Premultiplied alpha düzeltmesi
        Color[] data = new Color[texture.Width * texture.Height];
        texture.GetData(data);
        for (int i = 0; i != data.Length; ++i)
        {
            data[i] = Microsoft.Xna.Framework.Color.FromNonPremultiplied(data[i].R, data[i].G, data[i].B, data[i].A);
        }
        texture.SetData(data);
        
        return texture;
    }

    /// <summary>
    /// ArarGames stüdyo logosunu gömülü kaynaklardan yükler.
    /// </summary>
    /// <param name="device">Grafik cihazı.</param>
    /// <returns>Logo dokusu.</returns>
    public static Texture2D? LoadDefaultLogo(GraphicsDevice device)
    {
        // Varsayılan resource yolu varsayımı
        return LoadTexture(device, "ArarGames.Engine.Resources.ArarGamesLogo.png");
    }

    /// <summary>
    /// Gömülü bir metin dosyasını okur.
    /// </summary>
    /// <param name="fullResourceName">Kaynağın tam namespace yolu.</param>
    /// <returns>Metin içeriği.</returns>
    public static string LoadString(string fullResourceName)
    {
        using var stream = GetStream(fullResourceName);
        if (stream == null) return string.Empty;

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Gömülü kaynağın veri akışını (Stream) döner.
    /// </summary>
    /// <param name="fullResourceName">Kaynağın tam namespace yolu.</param>
    /// <returns>Okunabilir stream veya bulunamazsa null.</returns>
    public static Stream? GetStream(string fullResourceName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        return assembly.GetManifestResourceStream(fullResourceName);
    }
}
