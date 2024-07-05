import React, { Component } from 'react';

export class Login extends Component {
  static displayName = Login.name;
  
  constructor(props) {
    super(props);
    this.state = {
      redirect: false,
      message: "",
      routeRedirection: ""
    };
  }

  aideClient() {
    const courriel = document.getElementById("courriel");
    const motDePasse = document.getElementById("motDePasse");
    courriel.addEventListener("input", function() {
      if (courriel.value !== "")
        courriel.classList.remove("is-invalid");
      else
        courriel.classList.add("is-invalid");
    });
    motDePasse.addEventListener("input", function() {
      if (motDePasse.value !== "")
        motDePasse.classList.remove("is-invalid");
      else
        motDePasse.classList.add("is-invalid");
    });
  }

  soumettreFormulaire() {
    const formulaire = document.getElementById("form-login");
    const donnees = new FormData(formulaire);
    const message = document.getElementById("message");
    const mauvaiseRequete = "Un ou plusieurs champs sont manquants";
    const conteneurMessage = document.getElementById("conteneurMessage");
    const courriel = document.getElementById("courriel");
    const motDePasse = document.getElementById("motDePasse");
    let codeRetour;
    fetch("api/utilisateur/login", {
      method: "POST",
      body: donnees
    })
      .then(response => {
        codeRetour = response.status;
        return  response.json();
      })
      .then(data => {
        if (codeRetour === 200) {
          if (data["idUtilisateur"]) {
            window.location.href = "/" + this.state.routeRedirection;
          } else {
            courriel.classList.remove("is-invalid");
            motDePasse.classList.remove("is-invalid");
            conteneurMessage.style.display = "block";
            message.textContent = data["message"];
          }
        } else { // 400
          this.aideClient();
          if (data["errors"]["courriel"])
            courriel.classList.add("is-invalid");
          else
            courriel.classList.remove("is-invalid");
          if (data["errors"]["motDePasse"])
            motDePasse.classList.add("is-invalid");
          else
            motDePasse.classList.remove("is-invalid");
          conteneurMessage.style.display = "block";
          message.textContent = mauvaiseRequete;
        }
      })
      .catch(error => {
        console.error("Erreur:", error);
      });
  }
  
  afficherMessage() {
    if (this.state.redirect)
      return (
        <div className="mb-3 alert alert-danger text-center" role="alert">
          <h3 className="alert-heading">{this.state.message}</h3>
        </div>
      );
  }
  
  componentDidMount() {
    // Récupérer les paramètres de l'URL : ?message={message}&redirect={page}
    const urlParams = new URLSearchParams(window.location.search);
    const message = urlParams.get('message');
    const redirect = urlParams.get('redirect');
    if (message !== "" && redirect) {
      this.setState({ redirect: true });
      this.setState({ message: message });
      this.setState({ routeRedirection: redirect });
    }
  }
  
  render() {
    document.title = 'Authentification';
    const champsManquants = {
      "courriel": "Entrer votre adresse courriel",
      "motDePasse": "Entrer votre mot de passe"
    };
            
    document.addEventListener("submit", (event) => {
      event.preventDefault();
      const formulaire = document.getElementById('form-login');
      this.soumettreFormulaire(formulaire);
    });
        
    return (
      <div id="login" className="container card px-5 ps-5 pb-2 pt-4 text-white conteneur-form-sm">
        <div style={{display: "none"}} id="conteneurMessage" className="mb-3 text-center alert alert-danger" role="alert">
          <h3 id="message" className="alert-heading"></h3>
        </div>
        {this.afficherMessage()}
        <form id="form-login">
          <h2 className="text-center mb-3">Authentification</h2>
          <div className="row mb-3">
            <label className="form-label" htmlFor="courriel">Adresse courriel</label>
            <input className="form-control" id="courriel" name="courriel" type="email"/>
            <div className="invalid-feedback">{champsManquants["courriel"]}</div>
          </div>
          <div className="row mb-3">
            <div className="d-flex">
              <label className="form-label" htmlFor="motDePasse">Mot de passe</label>
              <a id="motDePasseOublie" className="form-text text-white ms-auto" href="#">Mot de passe oublié ?</a>
            </div>
            <input aria-describedby="motDePasseOublie" name="motDePasse" className="form-control" id="motDePasse" type="password"/>
            <div className="invalid-feedback">{champsManquants["motDePasse"]}</div>
          </div>
          <div className="text-center mb-4">
            <button className="btn btn-primary" type="submit">Se connecter</button>
          </div>
          <div className="form-text text-white d-flex justify-content-center">
            <p className="mx-1"> Vous n'avez pas de compte ?</p>
            <a className="text-white" href="/inscription">Inscription</a>
          </div>
        </form>
      </div>
    );
  }
}