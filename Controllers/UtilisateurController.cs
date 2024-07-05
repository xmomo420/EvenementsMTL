using Microsoft.AspNetCore.Mvc;
using TestReactOther.Models;
using TestReactOther.Repositories;

namespace TestReactOther.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UtilisateurController : ControllerBase
{    
    private readonly UtilisateurRepository _utilisateurRepository;
    
    public UtilisateurController(UtilisateurRepository utilisateurRepository)
    {
        _utilisateurRepository = utilisateurRepository;
    }
    
    // Constantes représentants les messages réponses HTTP
    private const string MessageBadRequestInscription = "Mauvaise requête, un ou plusieurs champs sont manquants ou invalides";
    private const string MessageBadRequestModification = "Mauvaise requête, aucun champ à modifier";
    private const string MessageInscriptionReussi = "Inscription réussi";
    private const string MessageAuthentificationReussi = "Authentification réussi";
    private const string MessageCombinaisonInvalide = "Courriel et/ou mot de passe invalide";
    private const string PrenomDefaut = "Prénom pas encore défini";
    private const string NomDefaut = "Nom pas encore défini";
    private const string MessageUtilisateurModifie = "Utilisateur modifié avec succès";
    private const string MessageNonAuthorise = "Vous n'êtes pas autorisé à effectuer cette action";
    
    // Fonctions repérsentant les services REST de l'API 
    [HttpPost("login")]
    public IActionResult Authentification(
        [FromForm]string courriel,
        [FromForm]string motDePasse
    )
    {
        int idUtilisateur = _utilisateurRepository.Authentifier(courriel, motDePasse);
        if (idUtilisateur == -1)
        {
            return Ok(new { message = MessageCombinaisonInvalide });
        }
        HttpContext.Session.SetInt32("idUtilisateur", idUtilisateur);
        return Ok(new { message = MessageAuthentificationReussi, idUtilisateur });
    }
    
    [HttpDelete("logout")]
    public IActionResult Deconnexion()
    {
        HttpContext.Session.Remove("idUtilisateur");
        return Ok(new { message = "Déconnexion réussi" });
    }
    
    [HttpPost]
    public IActionResult Inscription(
        [FromForm]string courriel,
        [FromForm]string motDePasse,
        [FromForm]string confirmationMotDePasse
        )
    {
        // Valider les données
        if (Utilisateur.EstFormulaireValide(courriel, motDePasse, confirmationMotDePasse))
        {
            // Vérifier si le courriel est déjà utilisée
            if (_utilisateurRepository.CourrielExiste(courriel))
            {
                return Ok(new { message = "Le courriel \"" + courriel + "\" est déjà utilisée" });
            }
            // Ajouter à la base de données
            Utilisateur utilisateur = new Utilisateur
            {
                Courriel = courriel,
                HashedPassword = Utilisateur.GenererHash512(motDePasse)
            };
            int idUtilisateur;
            try
            {   // Pour attraper les exceptions liées à la base de données
                idUtilisateur = _utilisateurRepository.AjouterUtilisateur(utilisateur);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = Constantes.MessageErreurServeur + " : "  + e.Message });
            }
            // TODO : Authentifier l'utilisateur en stockant l'id dans la session
            HttpContext.Session.SetInt32("idUtilisateur", idUtilisateur);
            return CreatedAtAction(nameof(Inscription), new { id = idUtilisateur }, new { message = MessageInscriptionReussi, id = idUtilisateur });
        }
        return BadRequest(new { message = MessageBadRequestInscription });
    }
    
    // TODO : Protéger avec un JWT 
    [HttpGet("{id}")]
    public IActionResult GetUtilisateur(int id)
    {
        Utilisateur? utilisateur = _utilisateurRepository.ObtenirUtilisateur(id);
        if (utilisateur == null)
        {
            return NotFound(new { message = Constantes.MessageErreur404 });
        }
        var donneesReponse = new
        {
            courriel = Utilisateur.EncrypterAes(utilisateur.Courriel), 
            prenom = utilisateur.Prenom != null ? Utilisateur.EncrypterAes(utilisateur.Prenom) : PrenomDefaut, 
            nom = utilisateur.Nom != null ? Utilisateur.EncrypterAes(utilisateur.Nom) : NomDefaut
        };
        return Ok(donneesReponse);
    }
    
    // TODO : Protéger avec un JWT
    [HttpPatch("{id}/nom-prenom")]
    public IActionResult ModifierNomPrenom(int id, [FromForm]string? prenom, [FromForm]string? nom)
    {
        Utilisateur? utilisateur = _utilisateurRepository.ObtenirUtilisateur(id);
        if (utilisateur == null)
        {
            return NotFound(new { message = Constantes.MessageErreur404 });
        }
        if (prenom != null && !prenom.Equals(utilisateur.Prenom) || nom != null && !nom.Equals(utilisateur.Nom))
        {
            _utilisateurRepository.ModifierNomPrenom(id, prenom, nom);
            return Ok(new { message = MessageUtilisateurModifie });
        }
        return BadRequest(new { message = MessageBadRequestModification });
    }
    
    // TODO : Service pour modifier le mot de passe
    [HttpPatch("{id}/mot-de-passe")]
    public IActionResult ModifierMotDePasse(int id, [FromForm]string motDePasse, [FromForm]string confirmationMotDePasse)
    {
        try
        {
            bool? modifie = _utilisateurRepository.ModifierMotDePasse(id, motDePasse, confirmationMotDePasse);
            if (modifie == null)
            {
                return NotFound(Constantes.MessageErreur404);
            }
            if (!modifie.Value)
            {
                return BadRequest("Les mots de passe ne correspondent pas ou sont invalides");
            }
            return Ok(new { reponse = "Le mot de passe a été modifié avec succès" });
        }
        catch (Exception e)
        {
            return Problem(
                "Erreur lors de la modification du mot de passe de l'utilisateur: " + e.Message, 
                statusCode:500
            );
        }        
    }
    
    [HttpPatch("{id}/courriel")]
    public IActionResult ModifierCourriel(int id, [FromForm]string courriel)
    {
        try
        {
            bool? modifie = _utilisateurRepository.ModifierCourriel(id, courriel);
            if (modifie == null)
            {
                return NotFound(Constantes.MessageErreur404);
            }
            if (!modifie.Value)
            {
                return Ok("L'adresse courriel '" + courriel + "' est déjà utilisée par un autre utilisateur");
            }
            return Ok(new { reponse = MessageUtilisateurModifie, courriel });
        }
        catch (Exception e)
        {
            return Problem(
                "Erreur lors de la modification du courriel de l'utilisateur: " + e.Message, 
                statusCode:500
            );
        }
    }
    
    [HttpPatch("{id}/evenement")]
    public IActionResult AjouterEvenement(int id, [FromForm] int idEvenement)
    {
        try
        {
            int reponseRequeteDb = _utilisateurRepository.AjouterEvenement(id, idEvenement);
            if (reponseRequeteDb == -2) // -2 : Événement inexistant
            {
                return NotFound(new { message = Constantes.MessageErreur404 + ", l'événement n'existe pas "});
            }
            if (reponseRequeteDb == -1) // -1 : Utilisateur inexistant
            {
                return NotFound(new { message = Constantes.MessageErreur404 + ", l'utilisateur n'existe pas "});
            }
            if (reponseRequeteDb == 0) // 0 : Événement déjà présent
            {
                return BadRequest(new { message = "L'événement '" + idEvenement + "' est déjà dans la liste de l'utilisateur" });
            }
            return Ok(new { message = "Événement ajouté à l'utilisateur avec succès" });
        }
        catch (Exception e)
        {
            return Problem(
                "Erreur lors de l'ajout de l'événement à l'utilisateur: " + e.Message, 
                statusCode:500
            );
        }
    }
    
    
    [HttpGet("{id}/evenement/liste")]
    public IActionResult ObtenirEvenements(int id)
    {        
        try
        {
            List<Evenement>? evenements = _utilisateurRepository.ObtenirEvenements(id);
            if (evenements == null)
            {
                return Ok(new { message = "Aucun événement pour cet utilisateur" });
            }
            if (evenements.Count == 0)
            {
                return NotFound(new { message = Constantes.MessageErreur404 });
            }
            var evenementsMapped = evenements.Select(evenement => new
            {
                evenement.Id,
                evenement.Titre,
                evenement.Description,
                evenement.Url,
                evenement.DateDebut,
                evenement.DateFin,
            });
            return Ok(new { evenements = evenementsMapped, nombre_evenements = evenements.Count });
        }
        catch (Exception e)
        {
            return Problem(
                "Erreur lors de l'ajout des événements à l'utilisateur: " + e.Message, 
                statusCode:500
            );
        }
    }    
    
}