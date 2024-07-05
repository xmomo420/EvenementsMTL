import { Component } from 'react';
import { afficherListeEvenements } from './EvenementsUtils';
export class Recherche extends Component {
  static displayName = Recherche.name;
  
  constructor(props) {
    super(props);
    this.state = {
      cinqProchainsEvenements: []
    };
  }
  
  componentDidMount() {
    let codeRetour;
    fetch('api/evenement/liste/cinq-prochains')
      .then(response => {
        codeRetour = response.status;
        return response.json();
      }).then(data => {
        if (codeRetour === 200) {
          this.setState({ cinqProchainsEvenements: data });
        } else {
          // Erreur
          this.setState({ cinqProchainsEvenements: null })
          console.error('Erreur : ', data.toString());
        }      
      }).catch(error => {
        console.error('Erreur : ', error);
      });
  }
  
  render() {
    document.title = 'Recherche';
    return afficherListeEvenements(this.state.cinqProchainsEvenements, "Ces évènements vont débuter bientôt !"); 
  }
}
