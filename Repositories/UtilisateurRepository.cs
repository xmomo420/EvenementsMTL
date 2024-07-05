
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TestReactOther.Models;

namespace TestReactOther.Repositories;

public class UtilisateurRepository
{
    private readonly DatabaseContext _databaseContext;
    
    public UtilisateurRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public int AjouterUtilisateur(Utilisateur utilisateur)
    {
        _databaseContext.Utilisateurs.Add(utilisateur);
        _databaseContext.SaveChanges();
        return utilisateur.Id;
    }
    
    public bool CourrielExiste(string courriel)
    {
        return _databaseContext.Utilisateurs.Any(utilisateur => utilisateur.Courriel.Equals(courriel));  
    }
    
    public int Authentifier(string courriel, string motDePasse)
    {
        Utilisateur? utilisateur = _databaseContext.Utilisateurs.FirstOrDefault(utilisateur => utilisateur.Courriel.Equals(courriel));
        if (utilisateur == null)
        {
            return -1;
        }
        if (utilisateur.HashedPassword.Equals(Utilisateur.GenererHash512(motDePasse)))
        {
            return utilisateur.Id;
        }
        return -1;
    }
    
    public Utilisateur? ObtenirUtilisateur(int id)
    {
        return _databaseContext.Utilisateurs.Find(id);
    }
    
    public void ModifierNomPrenom(int id, string? prenom, string? nom)
    
    {
        Utilisateur? utilisateur = _databaseContext.Utilisateurs.Find(id);
        if (utilisateur != null)
        {
            if (prenom != null && !prenom.Equals(utilisateur.Prenom))
            {
                utilisateur.Prenom = prenom;
            }
            if (nom != null && !nom.Equals(utilisateur.Nom))
            {
                utilisateur.Nom = nom;
            }            
            _databaseContext.SaveChanges();
        }
    }
    
    public List<Evenement>? ObtenirEvenements(int id)
    {
        Utilisateur? utilisateur = _databaseContext.Utilisateurs
            .Include(user => user.Evenements) // Inclusion explicite pour charger les Evenements
            .FirstOrDefault(user => user.Id == id);
        if (utilisateur == null)
        {
            return new List<Evenement>();
        }
        return utilisateur.Evenements;
    }
    
    public bool? ModifierCourriel(int id, string courriel)
    {
        Utilisateur? utilisateur = _databaseContext.Utilisateurs.Find(id);
        if (utilisateur != null)
        {
            if (CourrielExiste(courriel))
            {
                return false;
            }
            utilisateur.Courriel = courriel;
            _databaseContext.SaveChanges();
            return true;
        }
        return null;
    }
    
    public bool? ModifierMotDePasse(int id, string motDePasse, string confirmationMotDePasse)
    {
        Utilisateur? utilisateur = _databaseContext.Utilisateurs.Find(id);
        if (utilisateur != null)
        {
            if (Utilisateur.MotDePasseValides(motDePasse, confirmationMotDePasse))
            {
                utilisateur.HashedPassword = Utilisateur.GenererHash512(motDePasse);
                _databaseContext.SaveChanges();
                return true;
            }
            return false;
        }
        return null;
    }
    
    public int AjouterEvenement(int id, int idEvenement)
    {
        Utilisateur? utilisateur = _databaseContext.Utilisateurs
            .Include(user => user.Evenements) // Inclusion explicite pour charger les Evenements
            .FirstOrDefault(user => user.Id == id);
        if (utilisateur != null)
        {
            Evenement? evenement = _databaseContext.Evenements.Find(idEvenement);
            if (evenement != null)
            {
                if (utilisateur.Evenements == null) 
                {
                    utilisateur.Evenements = new List<Evenement>();
                    utilisateur.Evenements.Add(evenement);
                    _databaseContext.SaveChanges();
                    return 1;
                }
                if (!utilisateur.Evenements.Contains(evenement))
                {
                    utilisateur.Evenements.Add(evenement);
                    _databaseContext.SaveChanges();
                    return 1;
                }
                return 0; // Événement déjà présent
            }
            return -2; // Événement inexistant
        }
        return -1; // Utilisateur inexistant
    }
    
    
}