import { Component } from 'react';
import { afficherListeEvenements } from './EvenementsUtils';
export class Home extends Component {
  static displayName = Home.name;
  
  constructor(props) {
    super(props);
    this.state = {
      cinqProchainsEvenements: []
    };
  }
  
  componentDidMount() {
    let codeRetour;
    fetch('api/evenement/liste/cinq-prochains-finis')
      .then(response => {
        codeRetour = response.status;
        return response.json();
      })
      .then(data => {
        if (codeRetour === 200) {
          this.setState({ cinqProchainsEvenements: data });
        } else {
          // Erreur
          this.setState({ cinqProchainsEvenements: null })
          console.error('Erreur : ', data);
        }      
      }).catch(error => {
        console.error('Erreur : ', error);
      }); 
  }
  
  render() {
    document.title = 'Accueil';
    return afficherListeEvenements(this.state.cinqProchainsEvenements, "Ces évènements vont se terminés aujourd'hui !");
  }
}
