namespace ArarGames.Engine.Pooling;

/// <summary>
/// Nesne havuzu (Object Pool) içerisine konulabilecek sınıflar için arayüz.
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// Nesne havuza geri döndürüldüğünde durumunu sıfırlamak için çağrılır.
    /// </summary>
    void Reset();
}
