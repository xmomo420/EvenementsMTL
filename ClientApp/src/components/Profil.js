import {Component, useEffect, useState} from "react";
import {render} from "react-dom";

export function Profil() {
  const [utilisateur, setUtilisateur] = useState(null);
  const messageLogin = "Vous devez être connecté pour accéder à cette page.";

  useEffect(() => {
    const fetchSession = async () => {
      const session = await obtenirSession();
      if (session) {
        const infosUtilisateur = await obtenirUtilisateur(session);
        setUtilisateur(infosUtilisateur);
      } else {
        window.location.href = "/login?message=" + messageLogin + "&redirect=profil";
      }
    };
    fetchSession();
  }, []);
  
  async function obtenirSession() {
    return await fetch('api/Session')
      .then(response => response.text())
      .then(data => data)
      .catch(error => {
        console.error('Erreur : ', error);
      });
  }

  function affichageProfil(infosUtilisateur) {
    return (
      <div className="container card mb-3 p-4 text-center">
        <h2 className="mb-5">Profil de {infosUtilisateur.prenom} {infosUtilisateur.nom}</h2>
        <div className="row mt-3">
          <div className="text-start col-md-6">
            <div>
              <h5 className="text-start">Renseignements personnels</h5>
              <hr/>
            </div>
            <p><span className="fw-semibold">Nom : </span>{infosUtilisateur?.nom}</p>
            <p><span className="fw-semibold">Prénom : </span>{infosUtilisateur?.prenom}</p>
            <p><span className="fw-semibold">Courriel : </span>{infosUtilisateur?.courriel}</p>
            <div className="text-center">
              <button type="button" className="btn btn-primary mt-3">Modifier</button>
            </div>
          </div>
          <div className="col-md-6">
            <h5 className="text-start">Vos évènements</h5>
            <hr/>
            <div className="table-responsive">
              <table className="table table-bordered align align-middle">
                  <thead>
                    <tr>
                      <th className="fw-semibold">Nom</th>
                      <th className="fw-semibold">Date de début</th>
                      <th className="fw-semibold">Arrondissement</th>
                      <th></th>
                    </tr>
                  </thead>
                  <tbody>
                  {infosUtilisateur.evenements?.map(evenement => (
                    <tr key={evenement.id}>
                      <td>{evenement.nom}</td>
                      <td>{evenement.dateDebut}</td>
                      <td>{evenement.arrondissement}</td>
                      <td>
                        <div className="d-flex justify-content-center">
                          <a className="btn btn-primary mx-2" href="">Voir</a>
                          <a className="btn btn-danger ms-2" href="">Supprimer</a>
                        </div>
                      </td>
                    </tr>
                    ))}
                  </tbody>  
                </table>
            </div>
          </div>
        </div>
      </div>
    );
  }

  async function obtenirUtilisateur(session) {
    return await fetch('api/utilisateur/' + session)
      .then(response => response.json())
      .then(data => data)
      .catch(error => {
        console.error('Erreur : ', error);
      });        
  }
  document.title = 'Profil';
  return affichageProfil(utilisateur);  
}