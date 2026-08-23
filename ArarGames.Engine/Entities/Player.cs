using System;
using Microsoft.Xna.Framework;

namespace ArarGames.Engine.Entities;

/// <summary>
/// Oyun içindeki oyuncu varlığı için temel yapı.
/// Oyun özelinde logic eklemek için 'partial class' olarak genişletilebilir.
/// </summary>
public partial class Player : GameEntity<Player>
{
    /// <summary>
    /// Mevcut can miktarı.
    /// </summary>
    public float Health { get; set; }

    /// <summary>
    /// Maksimum can kapasitesi.
    /// </summary>
    public float MaxHealth { get; set; }

    /// <summary>
    /// Oyuncunun hareket hızı çarpanı/sabit değeri.
    /// </summary>
    public float Speed { get; set; }

    /// <summary>
    /// Oyuncunun yaşayıp yaşamadığını belirtir.
    /// </summary>
    public bool IsAlive => Health > 0;

    /// <summary>
    /// Geçici olarak hasar görmez durumda olup olmadığını belirtir.
    /// </summary>
    public bool IsInvulnerable { get; set; }

    /// <summary>
    /// Hasar görmezlik durumunun kalan süresi (saniye cinsinden).
    /// </summary>
    public float InvulnerabilityTimer { get; set; }

    /// <summary>
    /// Player nesnesini varsayılan özelliklerle başlatır.
    /// </summary>
    public Player()
    {
        MaxHealth = 100f;
        Health = MaxHealth;
        Speed = 200f;
    }

    /// <summary>
    /// Oyuncuya hasar verir. Invulnerable ise hasar almaz.
    /// Can 0'a inerse IsActive false yapılır.
    /// </summary>
    /// <param name="amount">Hasar miktarı.</param>
    public virtual void TakeDamage(float amount)
    {
        if (IsInvulnerable || !IsAlive) return;

        Health -= amount;
        
        if (Health <= 0)
        {
            Health = 0;
            IsActive = false;
        }
    }

    /// <summary>
    /// Oyuncuyu iyileştirir (Can verir). MaxHealth'i geçemez.
    /// </summary>
    /// <param name="amount">İyileştirme miktarı.</param>
    public virtual void Heal(float amount)
    {
        if (!IsAlive) return;

        Health = Math.Min(Health + amount, MaxHealth);
    }

    /// <summary>
    /// Oyuncunun durumunu günceller (Invulnerability sayacı vb.)
    /// </summary>
    /// <param name="gameTime">Oyun zamanı nesnesi.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (IsInvulnerable)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            InvulnerabilityTimer -= dt;

            if (InvulnerabilityTimer <= 0)
            {
                IsInvulnerable = false;
                InvulnerabilityTimer = 0f;
            }
        }
    }
}
