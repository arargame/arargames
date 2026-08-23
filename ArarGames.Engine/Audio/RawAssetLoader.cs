using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ArarGames.Engine.Audio;

/// <summary>
/// MGCB pipeline'dan bağımsız olarak ham dosyaları yüklemek için yardımcı araçlar.
/// (Örn: .wav ve .png dosyaları)
/// </summary>
public static class RawAssetLoader
{
    /// <summary>
    /// Ham bir ses dosyasını (.wav) stream üzerinden yükler.
    /// </summary>
    /// <param name="path">Dosyanın TitleContainer içindeki göreceli yolu.</param>
    /// <returns>Oluşturulan SoundEffect nesnesi.</returns>
    public static SoundEffect LoadSoundEffect(string path)
    {
        using var stream = TitleContainer.OpenStream(path);
        return SoundEffect.FromStream(stream);
    }

    /// <summary>
    /// Ham bir resim dosyasını (.png vb.) stream üzerinden yükler ve 
    /// premultiplied alpha'ya dönüştürür.
    /// </summary>
    /// <param name="device">Grafik cihazı.</param>
    /// <param name="path">Dosyanın TitleContainer içindeki göreceli yolu.</param>
    /// <returns>Oluşturulan Texture2D nesnesi.</returns>
    public static Texture2D LoadTexture(GraphicsDevice device, string path)
    {
        using var stream = TitleContainer.OpenStream(path);
        Texture2D texture = Texture2D.FromStream(device, stream);
        
        // Premultiplied alpha dönüşümü
        Color[] data = new Color[texture.Width * texture.Height];
        texture.GetData(data);
        for (int i = 0; i != data.Length; ++i)
        {
            data[i] = Color.FromNonPremultiplied(data[i].R, data[i].G, data[i].B, data[i].A);
        }
        texture.SetData(data);

        return texture;
    }
}
