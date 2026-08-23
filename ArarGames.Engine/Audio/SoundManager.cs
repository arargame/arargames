using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace ArarGames.Engine.Audio;

/// <summary>
/// Ses ve müzik işlemlerini yöneten sınıf. 
/// Ses tekrarı kısıtlaması (throttling), müzik crossfade ve eksik dosya toleransını destekler.
/// </summary>
public class SoundManager : IAudioService
{
    private readonly Func<string, SoundEffect> _loadSound;
    private readonly Dictionary<string, SoundEffect> _sfxCache;
    private readonly Dictionary<string, double> _lastPlayTime;
    private readonly HashSet<string> _missingAssets;

    private SoundEffectInstance? _currentMusic;
    private SoundEffectInstance? _fadingOutMusic;

    private const double MinRepeatInterval = 0.045; // Saniye

    private bool _musicEnabled = true;
    private bool _sfxEnabled = true;
    private float _masterVolume = 1f;
    private float _musicVolume = 1f;
    private float _sfxVolume = 1f;

    // Crossfade State
    private bool _isCrossfading;
    private float _crossfadeDuration;
    private float _crossfadeTimer;
    
    // FadeOut State
    private bool _isFadingOut;
    private float _fadeOutDuration;
    private float _fadeOutTimer;

    private string? _pendingMusicTrack;
    private bool _pendingMusicLoop;

    /// <inheritdoc/>
    public bool SoundEffectsEnabled
    {
        get => _sfxEnabled;
        set => _sfxEnabled = value;
    }

    /// <inheritdoc/>
    public bool MusicEnabled
    {
        get => _musicEnabled;
        set
        {
            if (_musicEnabled == value) return;
            _musicEnabled = value;

            if (!_musicEnabled)
            {
                if (_currentMusic != null && _currentMusic.State == SoundState.Playing)
                {
                    _currentMusic.Pause();
                }
            }
            else
            {
                if (_pendingMusicTrack != null)
                {
                    PlayMusic(_pendingMusicTrack, _pendingMusicLoop);
                    _pendingMusicTrack = null;
                }
                else if (_currentMusic != null && _currentMusic.State == SoundState.Paused)
                {
                    _currentMusic.Volume = ActualMusicVolume;
                    _currentMusic.Resume();
                }
            }
        }
    }

    /// <inheritdoc/>
    public float MasterVolume
    {
        get => _masterVolume;
        set { _masterVolume = MathHelper.Clamp(value, 0f, 1f); UpdateMusicVolume(); }
    }

    /// <inheritdoc/>
    public float SfxVolume
    {
        get => _sfxVolume;
        set => _sfxVolume = MathHelper.Clamp(value, 0f, 1f);
    }

    /// <inheritdoc/>
    public float MusicVolume
    {
        get => _musicVolume;
        set { _musicVolume = MathHelper.Clamp(value, 0f, 1f); UpdateMusicVolume(); }
    }

    private float ActualMusicVolume => _masterVolume * _musicVolume;
    private float ActualSfxVolume => _masterVolume * _sfxVolume;
    private static readonly Random Rng = new Random();

    /// <summary>
    /// SoundManager nesnesi oluşturur.
    /// </summary>
    /// <param name="loadSoundFunc">Asset ismine karşılık SoundEffect dönen fonksiyon.</param>
    public SoundManager(Func<string, SoundEffect> loadSoundFunc)
    {
        _loadSound = loadSoundFunc ?? throw new ArgumentNullException(nameof(loadSoundFunc));
        _sfxCache = new Dictionary<string, SoundEffect>();
        _lastPlayTime = new Dictionary<string, double>();
        _missingAssets = new HashSet<string>();
    }

    private SoundEffect? GetSoundEffect(string name)
    {
        if (_missingAssets.Contains(name))
            return null;

        if (_sfxCache.TryGetValue(name, out var sfx))
            return sfx;

        try
        {
            sfx = _loadSound(name);
            if (sfx != null)
            {
                _sfxCache[name] = sfx;
                return sfx;
            }
        }
        catch
        {
            // İhtiyaca göre loglanabilir
        }

        _missingAssets.Add(name);
        return null;
    }

    /// <inheritdoc/>
    public void PlaySoundEffect(string name, float volume = 1f, float pitch = 0f, float pan = 0f)
    {
        if (!SoundEffectsEnabled || ActualSfxVolume <= 0f) return;

        var currentTime = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
        
        if (_lastPlayTime.TryGetValue(name, out double lastTime))
        {
            if (currentTime - lastTime < MinRepeatInterval)
                return; // Çok sık çağrılmayı engelle (Throttling)
        }

        var sfx = GetSoundEffect(name);
        if (sfx == null) return;

        _lastPlayTime[name] = currentTime;

        // Opsiyonel küçük rastgele pitch varyansı
        float finalPitch = MathHelper.Clamp(pitch + ((float)Rng.NextDouble() * 0.1f - 0.05f), -1f, 1f);
        float finalVolume = MathHelper.Clamp(volume * ActualSfxVolume, 0f, 1f);

        sfx.Play(finalVolume, finalPitch, pan);
    }

    /// <inheritdoc/>
    public void PlayMusic(string name, bool loop = true)
    {
        if (!MusicEnabled)
        {
            _pendingMusicTrack = name;
            _pendingMusicLoop = loop;
            return;
        }

        var sfx = GetSoundEffect(name);
        if (sfx == null) return;

        StopMusicImmediate();

        _currentMusic = sfx.CreateInstance();
        _currentMusic.IsLooped = loop;
        _currentMusic.Volume = ActualMusicVolume;
        _currentMusic.Play();
    }

    /// <inheritdoc/>
    public void StopMusic(float fadeDuration = 0f)
    {
        if (_currentMusic == null || _currentMusic.State != SoundState.Playing)
            return;

        if (fadeDuration <= 0f)
        {
            StopMusicImmediate();
        }
        else
        {
            _isFadingOut = true;
            _fadeOutDuration = fadeDuration;
            _fadeOutTimer = 0f;
            _fadingOutMusic = _currentMusic;
            _currentMusic = null;
        }
    }

    private void StopMusicImmediate()
    {
        if (_currentMusic != null)
        {
            _currentMusic.Stop();
            _currentMusic.Dispose();
            _currentMusic = null;
        }
        _isCrossfading = false;
        _isFadingOut = false;
    }

    /// <inheritdoc/>
    public void CrossFadeMusic(string nextTrack, float fadeDuration = 1f)
    {
        if (!MusicEnabled)
        {
            _pendingMusicTrack = nextTrack;
            return;
        }

        if (_currentMusic == null || fadeDuration <= 0f)
        {
            PlayMusic(nextTrack);
            return;
        }

        var nextSfx = GetSoundEffect(nextTrack);
        if (nextSfx == null) return;

        _fadingOutMusic = _currentMusic;
        
        _currentMusic = nextSfx.CreateInstance();
        _currentMusic.IsLooped = true;
        _currentMusic.Volume = 0f;
        _currentMusic.Play();

        _isCrossfading = true;
        _crossfadeDuration = fadeDuration;
        _crossfadeTimer = 0f;
    }

    /// <inheritdoc/>
    public void PauseAll()
    {
        if (_currentMusic != null && _currentMusic.State == SoundState.Playing)
            _currentMusic.Pause();
    }

    /// <inheritdoc/>
    public void ResumeAll()
    {
        if (MusicEnabled && _currentMusic != null && _currentMusic.State == SoundState.Paused)
            _currentMusic.Resume();
    }

    private void UpdateMusicVolume()
    {
        if (!_isCrossfading && !_isFadingOut && _currentMusic != null)
        {
            _currentMusic.Volume = ActualMusicVolume;
        }
    }

    /// <inheritdoc/>
    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_isCrossfading)
        {
            _crossfadeTimer += dt;
            float progress = MathHelper.Clamp(_crossfadeTimer / _crossfadeDuration, 0f, 1f);

            if (_fadingOutMusic != null)
                _fadingOutMusic.Volume = MathHelper.Lerp(ActualMusicVolume, 0f, progress);
            
            if (_currentMusic != null)
                _currentMusic.Volume = MathHelper.Lerp(0f, ActualMusicVolume, progress);

            if (progress >= 1f)
            {
                _isCrossfading = false;
                if (_fadingOutMusic != null)
                {
                    _fadingOutMusic.Stop();
                    _fadingOutMusic.Dispose();
                    _fadingOutMusic = null;
                }
            }
        }
        else if (_isFadingOut)
        {
            _fadeOutTimer += dt;
            float progress = MathHelper.Clamp(_fadeOutTimer / _fadeOutDuration, 0f, 1f);

            if (_fadingOutMusic != null)
            {
                _fadingOutMusic.Volume = MathHelper.Lerp(ActualMusicVolume, 0f, progress);
                if (progress >= 1f)
                {
                    _isFadingOut = false;
                    _fadingOutMusic.Stop();
                    _fadingOutMusic.Dispose();
                    _fadingOutMusic = null;
                }
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        StopMusicImmediate();
        if (_fadingOutMusic != null)
        {
            _fadingOutMusic.Stop();
            _fadingOutMusic.Dispose();
        }

        foreach (var sfx in _sfxCache.Values)
        {
            sfx.Dispose();
        }
        _sfxCache.Clear();
    }
}
