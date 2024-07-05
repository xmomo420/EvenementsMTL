using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace TestReactOther.Models;


[PrimaryKey("Id")]
[Index(nameof(Courriel), IsUnique = true)]
public class Utilisateur
{
    public int Id { get; set; }
    [MaybeNull]
    [StringLength(16)]
    public string Prenom { get; set; }
    [MaybeNull]
    [StringLength(16)]
    public string Nom { get; set; }
    [StringLength(32)]
    public string Courriel { get; set; }
    [StringLength(128)]
    public string HashedPassword { get; set; }
    [MaybeNull]
    public List<Evenement> Evenements { get; set; }
    
    // Fonctions de classes publiques
    public static bool EstFormulaireValide(string courriel, string motDePasse, string confirmationMotDePasse)
    {
        var formatCourriel = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        return formatCourriel.IsMatch(courriel) && MotDePasseValides(motDePasse, confirmationMotDePasse);
    }

    public static bool MotDePasseValides(string motDePasse, string confirmationMotDePasse)
    {
        var formatMotDePasse = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%&*_\-+=]).{8,}$");
        return  formatMotDePasse.IsMatch(motDePasse) && motDePasse.Equals(confirmationMotDePasse);
    }
    
    /**
     * Génère un hash SHA-512 pour les mots de passe
     * @param motDePasse : le mot de passe à hasher
     * @return le hash en string
     */
    public static string GenererHash512(string motDePasse)
    {
        using (SHA512 sha512Hash = SHA512.Create())
        {
            byte[] octets = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(motDePasse));
            return Convert.ToHexString(octets);
        }
    }
    
    public static string EncrypterAes(string donnee)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Constantes.CleChiffrement;
            aes.IV = new byte[16]; // IV (Vecteur d'initialisation) doit être de 16 octets pour AES
            
            // Créer un encrypteur pour effectuer le cryptage
            ICryptoTransform encrypteur = aes.CreateEncryptor(aes.Key, aes.IV);
            
            // Encrypter la donnée envoyé en paramètre
            byte[] donneeEnOctets = Encoding.UTF8.GetBytes(donnee);
            byte[] donneeEncryptee = encrypteur.TransformFinalBlock(donneeEnOctets, 0, donneeEnOctets.Length);
            
            // Retourner les données chiffrées sous forme de chaîne de base64
            return Convert.ToBase64String(donneeEncryptee);
        }
    }
}