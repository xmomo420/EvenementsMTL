import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import './Evenement.css';

export function Evenement() {
  const [evenement, setEvenement] = useState(null);
  const { id } = useParams();

  useEffect(() => {
    let codeRetour;
    fetch(`api/evenement/${id}`)
      .then(response => {
        codeRetour = response.status;
        return response.json();
      })
      .then(data => {
        if (codeRetour === 200) {
          setEvenement(data);
        } else if (codeRetour === 404) {
          console.error('Erreur 404 ! ', data["message"]);
          //window.location.href = "/404&message=" + data["message"];
        }
      }).catch(error => {
      console.error('Erreur : ', error);
    });
  }, [id]);

  function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
  }

  function obtenirDateFormattee(dateString) {
    const jours = ['Dimanche', 'Lundi', 'Mardi', 'Mercredi', 'Jeudi', 'Vendredi', 'Samedi'];
    const date = new Date(dateString);
    const jourSemaine = jours[date.getUTCDay()];
    return jourSemaine + ", le " + date.getUTCDate() + " " + monthToString(date.getMonth()) + " " + date.getFullYear();
  }
  
  function monthToString(month) {
    const mois = [
      "janvier", "février", "mars", "avril", "mai", "juin",
      "juillet", "août", "septembre", "octobre", "novembre", "décembre"
    ];
    return mois[month];
  }
  
  function afficherDates(dateDebut, dateFin) {
    
    const dateDebutFormatee = dateDebut.substring(0, dateDebut.indexOf('T'));
    const dateFinFormatee = dateFin.substring(0, dateFin.indexOf('T'));
    if (dateDebutFormatee === dateFinFormatee)
      return (
        <div className="d-flex">
          <div id="date"></div>
          <p className="text-start">{obtenirDateFormattee(dateDebutFormatee)}</p>
        </div>
      );
    else
      return (
        <div className="d-flex">
          <div id="date"></div><p
            className="text-start">Du {obtenirDateFormattee(dateDebutFormatee)} au {obtenirDateFormattee(dateFinFormatee)}</p>
        </div>
      );
  }

  function affichageEvenement(evenement) {
    if (evenement) {
      return (
        <div className="container card p-4 text-center">
          <h2 className="mb-3">{evenement.titre}</h2>
          <hr/>
          <p className="mb-3" style={{textAlign: "justify"}}>{capitalizeFirstLetter(evenement.description)}</p>
          {afficherDates(evenement.dateDebut, evenement.dateFin)}
          <p className="text-start"><span className="fw-semibold">Type d'évènement : </span>{evenement.typeEvenement}, pour {evenement.publicCible.toLowerCase()}</p>
          <hr/>
          <div className="row mb-3">
            <div className="col-md-7">
              <div className="table table-responsive">
                <div className="d-flex">
                  <div id="location"></div>
                  <h6 className="text-start">Informations sur le(s) lieu(x)</h6>
                </div>
                <table className="table table-bordered align align-middle">
                  <thead>
                  <tr>
                    <th className="fw-semibold" scope="col">Arrondissement</th>
                    <th className="fw-semibold" scope="col">Nom</th>
                    <th className="fw-semibold" scope="col">Adresse principale</th>
                    <th className="fw-semibold" scope="col">Code postal</th>
                    <th className="fw-semibold" scope="col">Adresse secondaire</th>
                  </tr>
                  </thead>
                  <tbody>
                  <tr>
                    <td>{evenement.arrondissement}</td>
                    <td>{evenement.titreAdresse ? evenement.titreAdresse : "-"}</td>
                    <td>{evenement.adressePrincipale ? evenement.adressePrincipale : "-"}</td>
                    <td>{evenement.codePostal ? evenement.codePostal : "-"}</td>
                    <td>{evenement.adresseSecondaire ? evenement.adresseSecondaire : "-"}</td>
                  </tr>
                  </tbody>
                </table>
              </div>
            </div>
            <div className="col-md-5">
              <div className="table table-responsive">
                <div className="d-flex">
                  <div id="dollar"></div>
                  <h6 className="text-start">Informations sur l'accès à l'évènement</h6>
                </div>
                <table className="table table-bordered align align-middle">
                  <thead>
                  <tr>
                    <th className="fw-semibold" scope="col">Prix</th>
                    <th className="fw-semibold" scope="col">Accès</th>
                    <th className="fw-semibold" scope="col">Emplacement</th>
                  </tr>
                  </thead>
                  <tbody>
                  <tr>
                    <td>{evenement.estPayant ? "Payant" : "Gratuit"}</td>
                    <td>{evenement.inscription}</td>
                    <td>{evenement.emplacement}</td>
                  </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
          <div className="d-flex mx-auto">
            <a className="btn btn-success mx-2">Ajouter</a>
            <a className="btn btn-primary ms-2" href={evenement.url}>Plus d'info</a>
          </div>
        </div>
      );
    }
  }

  document.title = "Événement";
  console.log(evenement);
  return affichageEvenement(evenement);
}