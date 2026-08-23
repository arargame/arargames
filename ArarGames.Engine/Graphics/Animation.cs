using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArarGames.Engine.Graphics;

/// <summary>
/// Sprite atlası üzerinden kare animasyonu oynatan sınıf.
/// </summary>
public class Animation
{
    private readonly Texture2D _spriteSheet;
    private readonly int _frameWidth;
    private readonly int _frameHeight;
    private readonly int _columns;
    private readonly int _totalFrames;
    private readonly float _frameDuration;

    private int _currentFrame;
    private float _timer;
    private bool _isPlaying;

    /// <summary>
    /// Animasyonun bitip bitmediğini belirtir (döngülü değilse).
    /// </summary>
    public bool IsFinished { get; private set; }

    /// <summary>
    /// Animasyonun döngüsel oynayıp oynamayacağını belirler.
    /// </summary>
    public bool IsLooping { get; set; } = true;

    /// <summary>
    /// Şu anki animasyon karesinin SpriteSheet üzerindeki alanını döner.
    /// </summary>
    public Rectangle CurrentFrameRectangle
    {
        get
        {
            int row = _currentFrame / _columns;
            int col = _currentFrame % _columns;
            return new Rectangle(col * _frameWidth, row * _frameHeight, _frameWidth, _frameHeight);
        }
    }

    /// <summary>
    /// Animation nesnesini başlatır.
    /// </summary>
    /// <param name="spriteSheet">Animasyon karesi atlası.</param>
    /// <param name="frameWidth">Bir karenin genişliği.</param>
    /// <param name="frameHeight">Bir karenin yüksekliği.</param>
    /// <param name="frameDuration">Bir karenin saniye cinsinden süresi.</param>
    public Animation(Texture2D spriteSheet, int frameWidth, int frameHeight, float frameDuration)
    {
        _spriteSheet = spriteSheet ?? throw new ArgumentNullException(nameof(spriteSheet));
        _frameWidth = frameWidth;
        _frameHeight = frameHeight;
        _frameDuration = frameDuration;

        _columns = _spriteSheet.Width / _frameWidth;
        int rows = _spriteSheet.Height / _frameHeight;
        _totalFrames = _columns * rows;
        
        _isPlaying = true;
    }

    /// <summary>
    /// Animasyonu devam ettirir.
    /// </summary>
    public void Play()
    {
        _isPlaying = true;
    }

    /// <summary>
    /// Animasyonu duraklatır.
    /// </summary>
    public void Pause()
    {
        _isPlaying = false;
    }

    /// <summary>
    /// Animasyonu durdurur ve başa sarar.
    /// </summary>
    public void Stop()
    {
        _isPlaying = false;
        Reset();
    }

    /// <summary>
    /// Animasyonu baştan başlatır.
    /// </summary>
    public void Reset()
    {
        _currentFrame = 0;
        _timer = 0f;
        IsFinished = false;
    }

    /// <summary>
    /// Animasyonu oyun zamanına göre günceller.
    /// </summary>
    /// <param name="gameTime">Oyun zamanı nesnesi.</param>
    public void Update(GameTime gameTime)
    {
        if (!_isPlaying || IsFinished) return;

        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timer >= _frameDuration)
        {
            _timer -= _frameDuration;
            _currentFrame++;

            if (_currentFrame >= _totalFrames)
            {
                if (IsLooping)
                {
                    _currentFrame = 0;
                }
                else
                {
                    _currentFrame = _totalFrames - 1;
                    IsFinished = true;
                    _isPlaying = false;
                }
            }
        }
    }

    /// <summary>
    /// Mevcut kareyi belirtilen parametrelerle çizer.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float depth)
    {
        spriteBatch.Draw(_spriteSheet, position, CurrentFrameRectangle, color, rotation, origin, scale, effects, depth);
    }
}
