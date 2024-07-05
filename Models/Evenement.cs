using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace TestReactOther.Models;

[PrimaryKey("Id")]
public class Evenement
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [StringLength(128)]
    public string Titre { get; set; }
    [StringLength(256)]
    public string Url { get; set; }
    [StringLength(512)]
    public string Description { get; set; }
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime DateDebut { get; set; }
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime DateFin { get; set; }
    [StringLength(128)]
    public string TypeEvenement { get; set; }
    [StringLength(128)]
    public string PublicCible { get; set; }
    [StringLength(128)]
    public string Emplacement { get; set; }
    [StringLength(128)]
    public string Inscription { get; set; }
    public bool EstPayant { get; set; }
    [StringLength(64)]
    public string Arrondissement { get; set; }
    [StringLength(128)]
    [MaybeNull]
    public string TitreAdresse { get; set; }
    [StringLength(128)]
    [MaybeNull]
    public string AdressePrincipale { get; set; }
    [StringLength(128)]
    [MaybeNull]
    public string AdresseSecondaire { get; set; }
    [StringLength(16)]
    [MaybeNull]
    public string CodePostal { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    [MaybeNull]
    public byte[] Image { get; set; } // Ajouter par l'administrateur
    [MaybeNull]
    public List<Utilisateur> Utilisateurs { get; set; }
}

