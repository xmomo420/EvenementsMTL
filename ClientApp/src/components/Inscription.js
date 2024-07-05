import React, { Component } from 'react';

export class Inscription extends Component {
  static displayName = Inscription.name;

  render() {
    document.title = 'Inscription'; 
    const champsManquants =  {
      "courriel": "Une adresse courriel valide est obligatoire",
      "motDePasse": "Le mot de passe est obligatoire",
      "confirmationMotDePasse": "Vous devez confirmer votre mot de passe"
    }
    const champsInvalide = {
      "courriel": "Adresse courriel invalide",
      "motDePasse": "Le mot de passe ne respecte pas les critères",
      "confirmationMotDePasse": "Les mots de passe ne correspondent pas"
    }
        
    function courrielValide(courriel) {
      const regexCourriel = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
      return regexCourriel.test(courriel);      
    }
    function aideClient() {
      const champCourriel = document.getElementById('courriel');
      const champMdp = document.getElementById('motDePasse');
      const champConfirmationMdp = document.getElementById('confirmationMotDePasse');
      
      champCourriel.addEventListener('input', (event) => {
        if (champCourriel.value === '') {
          document.getElementById('courrielInvalide').textContent = champsManquants["courriel"];
          champCourriel.classList.add('is-invalid');
        } else {
          if (courrielValide(champCourriel.value)) {
            champCourriel.classList.remove('is-invalid');
          } else {
            document.getElementById('courrielInvalide').textContent = champsInvalide["courriel"];
            champCourriel.classList.add('is-invalid');
          }
        }        
      });
      champMdp.addEventListener('input', (event) => {
        const invalidFeedback = document.getElementById('motDePasseInvalide');
        invalidFeedback.innerText = champsInvalide["motDePasse"];
        const motDePasse = champMdp.value;
        let motDePasseValide = true;
        let critere;
        if (motDePasse.length < 8) {
          motDePasseValide = false;
          critere = document.getElementById('longueur');
          critere.classList.remove('text-success');
          critere.style.fontWeight = 'normal';
        } else {
          critere = document.getElementById('longueur');
          critere.classList.add('text-success');
          critere.style.fontWeight = 'bold';
        }
        if (!/(?=.*[a-z])/.test(motDePasse)) {
          motDePasseValide = false;
          critere = document.getElementById('lettreMin');
          critere.classList.remove('text-success');
          critere.style.fontWeight = 'normal';
        } else {
          critere = document.getElementById('lettreMin');
          critere.classList.add('text-success');
          critere.style.fontWeight = 'bold';
        }
        if (!/(?=.*[A-Z])/.test(motDePasse)) {
          motDePasseValide = false;
          critere = document.getElementById('lettreMaj');
          critere.classList.remove('text-success');
          critere.style.fontWeight = 'normal';
        } else {
          critere = document.getElementById('lettreMaj');
          critere.classList.add('text-success');
          critere.style.fontWeight = 'bold';
        }
        if (!/(?=.*\d)/.test(motDePasse)) {
          motDePasseValide = false;
          critere = document.getElementById('chiffre');
          critere.classList.remove('text-success');
          critere.style.fontWeight = 'normal';
        } else {
          critere = document.getElementById('chiffre');
          critere.classList.add('text-success');
          critere.style.fontWeight = 'bold';
        }
        if (!/(?=.*[!@#$%&*_\-+=])/.test(motDePasse)) {
          motDePasseValide = false;
          critere = document.getElementById('charSpecial');
          critere.classList.remove('text-success');
          critere.style.fontWeight = 'normal';
        } else {
          critere = document.getElementById('charSpecial');
          critere.classList.add('text-success');
          critere.style.fontWeight = 'bold';
        }
        if (motDePasse === '') {
          invalidFeedback.textContent = champsManquants["motDePasse"];
        }
        if (motDePasseValide)
          champMdp.classList.remove('is-invalid');
        else
          champMdp.classList.add('is-invalid');
        verifierMotsDePasse()
      });
      champConfirmationMdp.addEventListener('input', (event) => {
        verifierMotsDePasse();
      });
    }
    
    function verifierMotsDePasse() {
      const invalidFeedback = document.getElementById('confirmationInvalide');
      const confirmationMdp = document.getElementById('confirmationMotDePasse');
      const motDePasse = document.getElementById('motDePasse');
      if (confirmationMdp.value === '') {
        invalidFeedback.textContent = champsManquants["confirmationMotDePasse"];
        confirmationMdp.classList.add('is-invalid');
      } else {
        if (confirmationMdp.value === motDePasse.value) {
          confirmationMdp.classList.remove('is-invalid');
        } else {
          invalidFeedback.textContent = champsInvalide["confirmationMotDePasse"];
          confirmationMdp.classList.add('is-invalid');
        }
      }
    }
    
      
    function soumettreFormulaire(formulaire) {
      let codeRetour;
      const messageHtml = document.getElementById('message');
      const conteneurMessage = document.getElementById('conteneurMessage');
      const messageManquants = "Vous devez remplir tous les champs";
      const messageInvalide = "Un ou plusieurs champs sont invalides";
      const donnees = new FormData(formulaire);
      fetch("api/utilisateur", {
        method: 'POST',
        body: donnees
      })
        .then(reponse => {
        codeRetour = reponse.status;
        return reponse.json();
      })
        .then( data => {
          if (codeRetour === 201) {
            window.location.href = '/';
          } else if (codeRetour === 200) { // Bonne requête, mais courriel déjà utilisé
            aideClient();
            conteneurMessage.style.display = 'block';
            messageHtml.textContent = data["message"];
          }
          else if (codeRetour === 400) { // Mauvaise requête
            aideClient();
            if (data["errors"] != null) { // Champs manquants
              messageHtml.textContent = messageManquants;
              conteneurMessage.style.display = 'block';
              if (data["errors"]["courriel"] != null) {
                document.getElementById('courriel').classList.add('is-invalid');
              } else {
                const champCourriel = document.getElementById('courriel');
                champCourriel.classList.remove('is-invalid');
              }
              if (data["errors"]["motDePasse"] != null) {
                document.getElementById('motDePasse').classList.add('is-invalid');
              } else
                document.getElementById('motDePasse').classList.remove('is-invalid');
              if (data["errors"]["confirmationMotDePasse"] != null) {
                document.getElementById('confirmationMotDePasse').classList.add('is-invalid');
              } else
                document.getElementById('confirmationMotDePasse').classList.remove('is-invalid');
            } else { // Tous les champs sont remplis, mais pas tous valides
              messageHtml.textContent = messageInvalide;
            }
          } else // 500, erreur interne
            console.error(data["message"]);
        })        
        .catch(erreur => {
          console.error('Erreur : ', erreur);
        });  
    }
    
    document.addEventListener('submit', (event) => {
      event.preventDefault();
      const formulaire = document.getElementById('form-inscription');
      soumettreFormulaire(formulaire);
    });
    
    const criteresMdp = {
      "longueur": "Au moins 8 caractères", 
      "lettreMin": "Au moins une lettre minuscule", 
      "lettreMaj": "Au moins une lettre majuscule", 
      "chiffre": "Au moins un chiffre", 
      "charSpecial": "Au moins un caractère parmi (!@#$%&*-_+=)", 
    };
    
    return (
      // TODO : Ajouter les placeHolders
      <div id="inscription" className="container card p-4 conteneur-form-sm text-white">
        <div style={{display: "none"}} id="conteneurMessage" className="mb-3 text-center alert alert-danger" role="alert">
          <h3 id="message" className="alert-heading"></h3>
        </div>
        <form id="form-inscription" className="row g-3 needs-validation" action="">
          <h2 className="text-center mb-3">Inscription</h2>
          <div className="col-md-6">
            <label className="form-label" htmlFor="courriel">Adresse courriel</label>
            <input className="form-control" id="courriel" name="courriel" type="email"/>
            <div id="courrielInvalide" className="invalid-feedback">{champsManquants["courriel"]}</div>
          </div>
          <div className="col-md-6"></div>
          <div className="col-md-6">
            <label className="form-label" htmlFor="motDePasse">Mot de passe</label>
            <input aria-describedby="criteresMdp" className="form-control" name="motDePasse" id="motDePasse" type="password"/>
            <div id="motDePasseInvalide" className="invalid-feedback">{champsManquants["motDePasse"]}</div>
            <div className="form-text text-white" id="criteresMdp">
              <ul>
                {Object.keys(criteresMdp).map((critere, index) => {
                  return <li id={critere} key={index}>{criteresMdp[critere]}</li>;
                })}
              </ul>
            </div>
          </div>
          <div className="col-md-6">
            <label className="form-label" htmlFor="confirmationMotDePasse">Confirmation du mot de passe</label>
            <input className="form-control" name="confirmationMotDePasse" id="confirmationMotDePasse" type="password"/>
            <div id="confirmationInvalide" className="invalid-feedback">{champsManquants["confirmationMotDePasse"]}</div>
          </div>
          <div className="text-center">
            <button className="btn btn-primary btn-lg text-center" type="submit">S'inscrire</button>
          </div>
          <div className="mt-4 justify-content-center form-text d-flex text-white">
            <p className="mx-1">Déjà un compte ?</p>
            <a className="text-white" href="/login">Connexion</a>
          </div>
        </form>
      </div>
    );
  }
}