import React from "react";

export function affichageEvenement(evenement, index, listeEvenements) {
  let marge;
  let classe = "text-start card p-2 col-md-5";
  if (index === 0 || index % 2 === 0)
    marge = " mx-3";
  else if (index === listeEvenements.length - 1)
    marge = "";
  else
    marge = " ms-3";
  classe += marge;
  const lienEvenement = "/evenement/" + evenement.id;
  return (
    <div key={index} className={classe}>
      <a href={lienEvenement}>
        <h5>{evenement.titre}</h5>
      </a>
      <hr/>
      <p style={{ textAlign : "justify" }}>{capitalizeFirstLetter(evenement.description)}</p>
      <p>Lieu : {evenement.arrondissement}</p>
      <p>{evenement.estPayant ? "Payant" : "Gratuit"}, {evenement.inscription.toLowerCase()}</p>
    </div>
  );
}

export function afficherListeEvenements(listeEvenements, titre) {
  return (
    <div className="container text-center mb-3 card p-4">
      <h2 className="mb-3">{titre}</h2>
      <div className="row justify-content-center g-3">
        {listeEvenements.map((evenement, index) => {
          return affichageEvenement(evenement, index, listeEvenements);
        })}
      </div>
    </div>
  );
}

export function capitalizeFirstLetter(string) {
  return string.charAt(0).toUpperCase() + string.slice(1);
}
