using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArarGames.Engine.Input;

namespace ArarGames.Engine.Screens;

/// <summary>
/// Oyun ekranları için temel sınıf. Ortak işlevselliği sağlar.
/// </summary>
public abstract class GameScreen : IGameScreen
{
    /// <summary>
    /// Oyunun bağlam bilgilerini (servisler, yöneticiler vb.) içerir.
    /// </summary>
    protected readonly GameContext Context;

    /// <summary>
    /// Bu ekranın bir kaplama (overlay) olup olmadığını belirtir.
    /// </summary>
    public virtual bool IsOverlay { get; protected set; } = false;

    /// <summary>
    /// Bu ekran aktifken altındaki ekranların güncellenip güncellenmeyeceğini belirtir.
    /// </summary>
    public virtual bool AllowBehindUpdates { get; protected set; } = false;

    /// <summary>
    /// Bu ekran aktifken altındaki ekranların çizilip çizilmeyeceğini belirtir.
    /// </summary>
    public virtual bool AllowBehindDraw { get; protected set; } = true;

    /// <summary>
    /// GameScreen nesnesi oluşturur.
    /// </summary>
    /// <param name="context">Oyun bağlamı.</param>
    protected GameScreen(GameContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public virtual void LoadContent() { }

    /// <inheritdoc/>
    public virtual void UnloadContent() { }

    /// <inheritdoc/>
    public virtual void Update(GameTime gameTime, bool isCovered) { }

    /// <inheritdoc/>
    public virtual void HandleInput(in InputState input) { }

    /// <inheritdoc/>
    public virtual void Draw(SpriteBatch spriteBatch) { }

    /// <inheritdoc/>
    public virtual void OnBackPressed()
    {
        Context.Screens.Pop();
    }

    /// <inheritdoc/>
    public virtual void OnScreenBecameActive() { }
}
