using Microsoft.EntityFrameworkCore;
using TestReactOther.Models;

namespace TestReactOther.Repositories;

public class EvenementRepository
{
    private readonly DatabaseContext _databaseContext;
    
    public EvenementRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public bool EstNouvelEvenement(string idString, DateTime dateFin)
    {
        int id = int.Parse(idString);
        return _databaseContext.Evenements.Find(id) == null && dateFin >= DateTime.Today;
    }
    
    public void AjouterEvenements(List<Evenement> evenements)
    {
            _databaseContext.Evenements.AddRange(evenements);
            _databaseContext.SaveChanges();
    }
    
    public List<Evenement> ObtenirEvenements()
    {
        return _databaseContext.Evenements.ToList();
    }
    
    public Evenement? ObtenirEvenement(int id)
    {
        return _databaseContext.Evenements
            .Include(evenement => evenement.Utilisateurs)
            .FirstOrDefault(evenement => evenement.Id == id);
    }
    
    public bool ModifierImageEvenement(int id, IFormFile image)
    {
        Evenement? evenement = _databaseContext.Evenements.Find(id);
        if (evenement != null)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.CopyTo(memoryStream);
                evenement.Image = memoryStream.ToArray();
                _databaseContext.SaveChanges();
            }

            return true;
        }
        return false;
    }
    
    public List<Evenement> CinqProchainsEvenements()
    {
        // Ajouter une chance de 50% comme ça ce sera des des événements aléatoires à chaque appel de la fonction 
        return _databaseContext.Evenements
            .Where(evenement => evenement.DateDebut > DateTime.Today)
            .OrderBy(evenement => evenement.DateDebut)
            .ThenBy(evenement => Guid.NewGuid())
            .Take(5)
            .ToList();
    }
    
    // Évenements en cours qui vont finir
    public List<Evenement> CinqProchainsEvenementsQuiVontFinir()
    {
        Console.WriteLine($"Aujourd'hui : {DateTime.Today}");
        return _databaseContext.Evenements
            .Where(evenement => evenement.DateFin == DateTime.Today)
            .OrderBy(evenement => Guid.NewGuid())
            .Take(5)
            .ToList();
    }
    
    public List<Evenement> ObtenirEvenementsComplets()
    {
        return _databaseContext.Evenements
            .Include(evenement => evenement.Utilisateurs).ToList();
    }
    
    public bool SupprimerEvenementsExpires()
    {
        List<Evenement> evenements = _databaseContext.Evenements
            .Where(evenement => evenement.DateFin < DateTime.Today)
            .ToList();
        if (evenements.Count > 0)
        {
            _databaseContext.Evenements.RemoveRange(evenements);
            _databaseContext.SaveChanges();
            return true;
        }
        return false;
    }
    
    public bool SupprimerEvenement(int id)
    {
        Evenement? evenement = _databaseContext.Evenements.Find(id);
        if (evenement != null)
        {
            _databaseContext.Evenements.Remove(evenement);
            _databaseContext.SaveChanges();
            return true;
        }
        return false;
    }
}