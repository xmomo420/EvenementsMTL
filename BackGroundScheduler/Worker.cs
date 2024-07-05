using Hangfire;
using TestReactOther.Controllers;
using TestReactOther.Mail;
using TestReactOther.Models;
using TestReactOther.Repositories;

namespace TestReactOther.BackGroundScheduler;

public class Worker
{
    private readonly EvenementController _evenementController;
    private readonly EvenementRepository _evenementRepository;
    private readonly IMailService _mailService;
    
    public Worker(EvenementController evenementController, EvenementRepository evenementRepository, IMailService mailService)
    {
        _evenementController = evenementController;
        _evenementRepository = evenementRepository;
        _mailService = mailService;
    }
    
    // TODO : Ajouter les différentes tâches planifiées de l'application 
    
    public void PlanifierToutesLesTaches()
    {
        PlanifierUpdateEvenements();
        PlanifierRappelEvenements();
        PlanifierSuppressionEvenements();
    }
    
    public void PlanifierUpdateEvenements()
    {
        RecurringJob.AddOrUpdate("MettreAJourEvenements", () => _evenementController.AjouterEvenements(), Cron.Daily);
    }
    
    public void EnvoyerRappelEvenements()
    {
        // TODO : Envoyer un courriel de rappel pour les événements du jour
        // Evenements : Titre, date_debut, url, Utilisateur : nom complet, courriel
        List<Evenement> evenements = _evenementRepository.ObtenirEvenementsComplets();
        MailData mailData = new MailData();
        foreach (Evenement evenement in evenements)
        {
            if (evenement.Utilisateurs != null && evenement.DateDebut == DateTime.Today.AddDays(1))
            {
                foreach (Utilisateur utilisateur in evenement.Utilisateurs)
                {
                    mailData.EmailBody = $"Bonjour {utilisateur.Prenom} {utilisateur.Nom},\n\n" +
                                        $"Ceci est un rappel pour l'événement {evenement.Titre} qui aura lieu demain.\n" +
                                        $"Date de l'événement : {evenement.DateDebut}\n" +
                                        $"Lien de l'événement : {evenement.Url}\n\n" +
                                        $"Cordialement,\n" +
                                        $"L'équipe d'EventsMTL";
                    mailData.EmailSubject = "Rappel pour l'événement de demain";
                    mailData.EmailToId = utilisateur.Courriel;
                    mailData.EmailToName = $"{utilisateur.Prenom} {utilisateur.Nom}";
                    _mailService.SendMail(mailData);
                }
            }
        }
        
    }
    
    public void PlanifierRappelEvenements()
    {
        RecurringJob.AddOrUpdate("RappelEvenements", () => EnvoyerRappelEvenements(), Cron.Daily);
    }
    
    public void PlanifierSuppressionEvenements()
    {
        RecurringJob.AddOrUpdate("SupprimerEvenements", () => _evenementRepository.SupprimerEvenementsExpires(), Cron.Daily);
    }
}