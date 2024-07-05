using System.Security.Cryptography;

namespace TestReactOther;

public class Constantes
{
    public const string MessageErreurServeur = "Une erreur est survenu du côté du serveur. L'équipe de développement en sera avisée";
    public const string MessageErreur404 = "La ressource à laquelle vous tentez d'accéder est introuvable";
    
    // Définition de la longueur de la clé en bytes (32 bytes pour AES-256)
    private const int LongueurCle = 32;
    public static byte[] CleChiffrement { get; }
    
    // Initialisation statique pour générer la clé lors du chargement de la classe
    static Constantes()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            CleChiffrement = new byte[LongueurCle];
            rng.GetBytes(CleChiffrement);
        }
    }
}