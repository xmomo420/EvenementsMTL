using System.Globalization;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TestReactOther.Models;
using TestReactOther.Repositories;

namespace TestReactOther.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvenementController : ControllerBase
{
    private readonly EvenementRepository _evenementRepository;
    private readonly HttpClient _httpClient;
    
    public EvenementController(EvenementRepository evenementRepository)
    {
        _evenementRepository = evenementRepository;
        _httpClient = new();
    }
    
    // Constantes représentants les messages réponses HTTP
    
    
    // Constantes représentants le contenu de la requête à l'api des données ouvertes de Montréal
    private const string UrlBaseDonnees = "https://donnees.montreal.ca/api/3/action/datastore_search_sql?sql=";
    private const string ColonnesRequete = 
        " _id, titre, url_fiche, description, date_debut, date_fin, type_evenement, " +
        "public_cible, emplacement, inscription, arrondissement, cout, titre_adresse, " +
        "adresse_principale, adresse_secondaire, code_postal, lat, long ";
    private const string ValeurRequete = "SELECT" + ColonnesRequete + "from \"6decf611-6f11-4f34-bb36-324d804c9bad\"";
    
    // Fonctions repérsentant les services REST de l'API
    [HttpPost]
    public async Task<IActionResult> AjouterEvenements()
    {
        // TODO : Ajouter uniquement les événements dont la date de fin n'est pas dépassée
        HttpRequestMessage requete = new HttpRequestMessage(HttpMethod.Get, UrlBaseDonnees + ValeurRequete);
        HttpResponseMessage reponse = await _httpClient.SendAsync(requete);
        if (!reponse.IsSuccessStatusCode)
        {
            return await Task.FromResult<IActionResult>(Problem(reponse.Content.ReadAsStringAsync().Result));
        }
        // Formater joliment le JSON pour une meilleure lisibilité
        JToken? reponseJson = JObject.Parse(reponse.Content.ReadAsStringAsync().Result)["result"]?["records"];
        if (reponseJson == null)
        {
            return await Task.FromResult<IActionResult>(Problem("NullReference : reponseJson ", statusCode:500));
        }
        // TODO : À Faire dans une fonction séparée
        JArray? reponseJsonArray = (JArray)reponseJson;
        if (reponseJsonArray.Count == 0)
        {
            return await Task.FromResult<IActionResult>(Ok(new { reponse = "La base de données est déja à jour" }));
        }
        List <Evenement> evenements = new();
        foreach (JToken evenement in (JArray)reponseJson)
        {
            if (_evenementRepository.EstNouvelEvenement(evenement["_id"].ToString(), evenement["date_fin"].ToObject<DateTime>()) )
            {
                Console.WriteLine(evenement["titre_adresse"]);
                Evenement nouvelEvenement = new Evenement
                {
                    Id = evenement["_id"].ToObject<int>(),
                    Titre = evenement["titre"].ToString(),
                    Url = evenement["url_fiche"].ToString(),
                    Description = evenement["description"].ToString(),
                    DateDebut = evenement["date_debut"].ToObject<DateTime>(),
                    DateFin = evenement["date_fin"].ToObject<DateTime>(),
                    TypeEvenement = evenement["type_evenement"].ToString(),
                    PublicCible = evenement["public_cible"].ToString(),
                    Emplacement = evenement["emplacement"].ToString(),
                    Inscription = evenement["inscription"].ToString(),
                    EstPayant = evenement["cout"].ToString().Equals("Payant"),
                    Arrondissement = evenement["arrondissement"].ToString(),
                    TitreAdresse = evenement["titre_adresse"].ToString().Equals("nan") || evenement["titre_adresse"].ToString().Equals("None")
                        ? null : evenement["titre_adresse"].ToString(),
                    AdressePrincipale = evenement["adresse_principale"].ToString().Equals("nan") || evenement["adresse_principale"].ToString().Equals("None")
                        ? null : evenement["adresse_principale"].ToString(),
                    AdresseSecondaire = evenement["adresse_secondaire"].ToString().Equals("nan") || evenement["adresse_secondaire"].ToString().Equals("None")
                        ? null : evenement["adresse_secondaire"].ToString(),
                    CodePostal = evenement["code_postal"].ToString().Equals("nan") || evenement["code_postal"].ToString().Equals("None")
                        ? null : evenement["code_postal"].ToString()
                };
                if (evenement["lat"].ToString().Equals("nan") || evenement["lat"].ToString().Equals("None"))
                {
                    nouvelEvenement.Latitude = 100;
                }
                else
                {
                    double latitude;
                    Double.TryParse(evenement["lat"].ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out latitude);
                    nouvelEvenement.Latitude = latitude;
                }
                
                if (evenement["long"].ToString().Equals("nan") || evenement["long"].ToString().Equals("None"))
                {
                    nouvelEvenement.Longitude = 200;
                }
                else
                {
                    double longitude;
                    Double.TryParse(evenement["long"].ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out longitude);
                    nouvelEvenement.Longitude = longitude;
                }
                evenements.Add(nouvelEvenement);
            }
            
        }
        if (evenements.Count == 0)
        {
            return await Task.FromResult<IActionResult>(Ok(new { reponse = "La base de données est déjà à jour, aucun nouvle événements n'a été détecté" }));
        }
        try
        {
            _evenementRepository.AjouterEvenements(evenements);
        } 
        catch (Exception e)
        {
            return await Task.FromResult<IActionResult>(Problem(
                "Erreur lors de l'ajout des événements à la base de données : " + e.Message, 
                statusCode:500
            ));
        }
        // Retourne les nouveaux événements ajoutés à la base de données
        string message = "Base de données mise à jour avec succès, " + evenements.Count + " nouveaux événements ont été ajoutés";
        return await Task.FromResult<IActionResult>(StatusCode(201, new { reponse = message }));
    }
    
    [HttpGet("liste")]
    public IActionResult ObtenirEvenements()
    {
        try
        {
            List<Evenement> evenements = _evenementRepository.ObtenirEvenements();
            if (evenements.Count == 0)
            {
                return Ok(new { message = "Aucun événement dans la base de données" });
            }
            return Ok(evenements);
        }
        catch (Exception e)
        {
            return Problem(
                "Erreur lors de la récupération des événements dans la base de données: " + e.Message, 
                statusCode:500
            );
        }         
    }
    
    [HttpGet("{id}")]
    public IActionResult ObtenirEvenement(int id)
    {
        try
        {
            Evenement? evenement = _evenementRepository.ObtenirEvenement(id);
            if (evenement == null)
            {
                return NotFound(new { message = Constantes.MessageErreur404 });
            }

            var evenementReponse = new
            {
                evenement.Titre,
                evenement.Url,
                evenement.Description,
                evenement.DateDebut,
                evenement.DateFin,
                evenement.TypeEvenement,
                evenement.PublicCible,
                evenement.Emplacement,
                evenement.Inscription,
                evenement.EstPayant,
                evenement.Arrondissement,
                evenement.TitreAdresse,
                evenement.AdressePrincipale,
                evenement.AdresseSecondaire,
                evenement.CodePostal,
                evenement.Latitude,
                evenement.Longitude,
                NombreParticipants = evenement.Utilisateurs == null ? 0 : evenement.Utilisateurs.Count
            };
            return Ok(evenementReponse);
        }
        catch (Exception e)
        {
            return Problem(
                "Erreur lors de la récupération de l'événement dans la base de données: " + e.Message, 
                statusCode:500
            );
        }         
    }
    
    
    [HttpPatch("{id}")]
    public IActionResult ModifierImageEvenement(int id, [FromBody]IFormFile image)
    {
        // TODO : À Faire
        if (!image.ContentType.Equals("image/jpeg") && 
            !image.ContentType.Equals("image/png") && 
            !image.ContentType.Equals("image/jpeg"))
        {
            return BadRequest(new { message = "Le format de l'image n'est pas supporté" });
        }
        try
        {
            if (_evenementRepository.ModifierImageEvenement(id, image))
            {
                return Ok(new { message = "Image modifiée avec succès" });
            }
            return NotFound(new { message = Constantes.MessageErreur404 });
        }
        catch (Exception e)
        {
            return Problem(
                "Erreur lors de l'opération effectué sur la base de données : " + e.Message, 
                statusCode:500
            );
        }        
    }
    
    [HttpGet("liste/cinq-prochains")]
    public IActionResult CinqProchainsEvenements()
    {
        try
        {
            List<Evenement> cinqProchainsEvenements = _evenementRepository.CinqProchainsEvenements();
            return Ok(cinqProchainsEvenements);
        }
        catch (Exception e)
        {
            return Problem(
                "Erreur lors de l'opération effectué sur la base de données : " + e.Message, 
                statusCode:500
            );
        }
    }
    
    [HttpGet("liste/cinq-prochains-finis")]
    public IActionResult CinqProchainsEvenementsFinis()
    {
        try
        {
            List<Evenement> cinqProchainsEvenementsFinis = _evenementRepository.CinqProchainsEvenementsQuiVontFinir();
            return Ok(cinqProchainsEvenementsFinis);
        }
        catch (Exception e)
        {
            return Problem(
                "Erreur lors de l'opération effectué sur la base de données : " + e.Message, 
                statusCode:500
            );
        }
    }
    
    [HttpDelete("/{id}")]
    public IActionResult SupprimerEvenement(int id)
    {
        try
        {
            if (_evenementRepository.SupprimerEvenement(id))
            {
                return Ok(new { reponse = $"L'évenement dont l'id est '{id}' a été supprimé avec succès" });
            }
            return NotFound(new { reponse = Constantes.MessageErreur404 });
        }
        catch (Exception e)
        {
            return Problem(
                "Erreur lors de l'opération effectué sur la base de données : " + e.Message, 
                statusCode:500
            );
        }
    }
}