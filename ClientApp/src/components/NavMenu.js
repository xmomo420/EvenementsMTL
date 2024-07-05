import React, {Component} from 'react';
import './NavMenu.css';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor(props) {
    super(props);
    this.state = {
      sessionId: null,
      loggedOut: false
    };
    // Lier `this` à la méthode `deconnexion`
    this.deconnexion = this.deconnexion.bind(this);
  }
  
  componentDidMount() {
    const urlParams = new URLSearchParams(window.location.search);
    const loggedOut = urlParams.get('loggedOut');
    if (loggedOut) 
      this.setState({ loggedOut: true });
    fetch('api/Session')
      .then(response => response.text())
      .then(data => this.setState({ sessionId: data }));
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.state.loggedOut && !prevState.loggedOut) {
      const messageElement = document.getElementById("messageLogout");
      if (messageElement) {
        const urlParams = new URLSearchParams(window.location.search);
        messageElement.textContent = urlParams.get('message');
        if (urlParams.get('error')) {
          document.getElementById("conteneurMessageLogout").classList.remove("alert-success");
          document.getElementById("conteneurMessageLogout").classList.add("alert-danger");
        }
      }
    }
  }

  deconnexion() {
    let codeRetour;
    fetch('api/utilisateur/logout', {
      method: 'DELETE'
    }).then(response => {
      codeRetour = response.status;
      return response.json();
    }).then(data => {
      if (codeRetour === 200) {
        window.location.href = '/?loggedOut=true&message=' + data["message"];
      } else { // Erreur
        window.location.href = '/?loggedOut=true&message=' + data.toString() + '&error=true';
      }
    }).catch(error => {console.error('Erreur : ', error);});
  }
  render() {
    return (
      <div>
        <nav className="navbar navbar-expand-lg rounded-bottom sticky-top navbar-custom mb-3">
          <div className="container-fluid">
            <a className="navbar-brand zoom-hover mx-3 text-white fw-bold" href="/">Accueil</a>
            <ul className="navbar-nav">
              <li className="nav-item border-end ms-2"></li>
              <li className="nav-item">
                <a className="nav-link zoom-hover mx-3 text-white" href="/recherche">Évenements</a>
              </li>
            </ul>
            <ul className="navbar-nav ms-auto">
              {this.state.sessionId
                ? (
                  <React.Fragment>
                    <li className="nav-item ms-0">
                      <a className="btn btn-danger zoom-hover boutons-navbar" onClick={this.deconnexion}>Déconnexion</a>
                    </li>
                    <li className="nav-item mx-3">
                      <a href="/Profil">
                        <div className="zoom-hover" id="photoProfil"></div>
                      </a>
                    </li>
                  </React.Fragment>
                ) :
                (
                  <React.Fragment>
                    <li className="nav-item">
                      <a className="btn btn-success zoom-hover boutons-navbar" href="/login">Connexion</a>
                    </li>
                    <li className="nav-item ms-0">
                      <a className="btn btn-info zoom-hover boutons-navbar" href="/inscription">Inscription</a>
                    </li>
                  </React.Fragment>
                )}
            </ul>
          </div>
        </nav>
        {this.state.loggedOut ?
          <div style={{ width: '35%' }} id="conteneurMessageLogout" className="mb-3 text-center mx-auto alert alert-success" role="alert">
            <h3 id="messageLogout" className="alert-heading"></h3>
          </div> : null
        }
      </div>
    );
  }
}
