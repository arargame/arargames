using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArarGames.Engine.Graphics;

/// <summary>
/// Sanal çözünürlük desteği ile bağımsız çizim sağlayan render target sarmalayıcı.
/// </summary>
public class VirtualScreen : IDisposable
{
    private readonly GraphicsDevice _device;
    private RenderTarget2D _renderTarget;
    private bool _isDisposed;

    /// <summary>
    /// Sanal ekran genişliği.
    /// </summary>
    public int VirtualWidth { get; }

    /// <summary>
    /// Sanal ekran yüksekliği.
    /// </summary>
    public int VirtualHeight { get; }

    /// <summary>
    /// Pixel art tarzı çizim için PointClamp kullanılıp kullanılmayacağını belirtir.
    /// </summary>
    public bool UsePointClamp { get; set; } = true;

    /// <summary>
    /// VirtualScreen nesnesini başlatır.
    /// </summary>
    /// <param name="device">Grafik cihazı.</param>
    /// <param name="virtualWidth">Hedef genişlik.</param>
    /// <param name="virtualHeight">Hedef yükseklik.</param>
    public VirtualScreen(GraphicsDevice device, int virtualWidth, int virtualHeight)
    {
        _device = device ?? throw new ArgumentNullException(nameof(device));
        VirtualWidth = virtualWidth;
        VirtualHeight = virtualHeight;
        
        _renderTarget = new RenderTarget2D(
            _device, 
            VirtualWidth, 
            VirtualHeight, 
            false, 
            _device.PresentationParameters.BackBufferFormat, 
            DepthFormat.None, 
            0, 
            RenderTargetUsage.PreserveContents);
    }

    /// <summary>
    /// Sanal ekrana çizimi başlatır.
    /// </summary>
    public void BeginCapture()
    {
        _device.SetRenderTarget(_renderTarget);
        _device.Clear(Color.Black);
    }

    /// <summary>
    /// Sanal ekrana çizimi bitirir ve varsayılan buffer'a döner.
    /// </summary>
    public void EndCapture()
    {
        _device.SetRenderTarget(null);
    }

    /// <summary>
    /// Sanal ekranı letterbox (en-boy oranı koruyarak) ile çizer.
    /// </summary>
    /// <param name="spriteBatch">Çizim nesnesi.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        float scaleX = (float)_device.Viewport.Width / VirtualWidth;
        float scaleY = (float)_device.Viewport.Height / VirtualHeight;
        float scale = Math.Min(scaleX, scaleY);

        int width = (int)(VirtualWidth * scale);
        int height = (int)(VirtualHeight * scale);
        int x = (_device.Viewport.Width - width) / 2;
        int y = (_device.Viewport.Height - height) / 2;
        
        Rectangle destinationRectangle = new Rectangle(x, y, width, height);
        SamplerState sampler = UsePointClamp ? SamplerState.PointClamp : SamplerState.LinearClamp;

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sampler, DepthStencilState.None, RasterizerState.CullNone);
        spriteBatch.Draw(_renderTarget, destinationRectangle, Color.White);
        spriteBatch.End();
    }

    /// <summary>
    /// Fiziksel ekran koordinatlarını sanal ekran koordinatlarına dönüştürür.
    /// </summary>
    /// <param name="screenPos">Fiziksel ekran pozisyonu.</param>
    /// <returns>Sanal ekran pozisyonu.</returns>
    public Vector2 ScreenToVirtual(Vector2 screenPos)
    {
        float scaleX = (float)_device.Viewport.Width / VirtualWidth;
        float scaleY = (float)_device.Viewport.Height / VirtualHeight;
        float scale = Math.Min(scaleX, scaleY);

        int width = (int)(VirtualWidth * scale);
        int height = (int)(VirtualHeight * scale);
        int x = (_device.Viewport.Width - width) / 2;
        int y = (_device.Viewport.Height - height) / 2;

        float virtualX = (screenPos.X - x) / scale;
        float virtualY = (screenPos.Y - y) / scale;

        return new Vector2(virtualX, virtualY);
    }

    /// <summary>
    /// Kaynakları serbest bırakır.
    /// </summary>
    public void Dispose()
    {
        if (!_isDisposed)
        {
            _renderTarget?.Dispose();
            _isDisposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
