using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InscriptionEtudiant.Models
{
    public enum StatutAdmission
    {
        EnAttente = 0,
        Admis = 1,
        ListeAttente = 2,
        Refuse = 3
    }

    public class Admission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DossierInscriptionId { get; set; }

        [ForeignKey(nameof(DossierInscriptionId))]
        public virtual DossierInscription DossierInscription { get; set; }

        /// <summary>
        /// Choix de filière sélectionné par l'administrateur.
        /// Garantit que la filière appartient bien aux choix du candidat.
        /// </summary>
        [Required]
        public int ChoixFiliereId { get; set; }

        [ForeignKey(nameof(ChoixFiliereId))]
        public virtual ChoixFiliere ChoixFiliere { get; set; }

        /// <summary>
        /// Administrateur ayant traité le dossier.
        /// </summary>
        public int? AdministrateurId { get; set; }

        [ForeignKey(nameof(AdministrateurId))]
        public virtual Administrateur? Administrateur { get; set; }

        [Required]
        public StatutAdmission Statut { get; set; } = StatutAdmission.EnAttente;

        [MaxLength(1000)]
        public string? Commentaire { get; set; }

        /// <summary>
        /// Score de classement (optionnel).
        /// </summary>
        public decimal? Score { get; set; }

        public DateTime? DateDecision { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public DateTime? DateModification { get; set; }
    }
}