using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InscriptionEtudiant.Models
{
    public class DossierInscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateDepot { get; set; } = DateTime.UtcNow;

        [Required]
        public StatutDossier StatutActuel { get; set; } = StatutDossier.EnAttente;

        // One-to-one with Candidat
        public int CandidatId { get; set; }
        public Candidat? Candidat { get; set; }

        // One-to-one Parcours
        public ParcoursAcademique? ParcoursAcademique { get; set; }

        // Documents
        public ICollection<Document> Documents { get; set; } = new List<Document>();

        // Choix filieres (1..3)
        public ICollection<ChoixFiliere> ChoixFilieres { get; set; } = new List<ChoixFiliere>();

        // Historique
        public ICollection<HistoriqueStatut> HistoriqueStatuts { get; set; } = new List<HistoriqueStatut>();

        // Filière affectée après traitement par l'administrateur
        public int? FiliereAffecteeId { get; set; }
        public Filiere? FiliereAffectee { get; set; }
    }
}
