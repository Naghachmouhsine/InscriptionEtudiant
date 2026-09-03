using System;
using System.ComponentModel.DataAnnotations;

namespace InscriptionEtudiant.Models
{
    public class HistoriqueStatut
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string? AncienStatut { get; set; }

        [StringLength(100)]
        public string? NouveauStatut { get; set; }

        [StringLength(1000)]
        public string? Commentaire { get; set; }

        public DateTime DateChangement { get; set; } = DateTime.UtcNow;

        // FK to Admin who processed
        public int? AdministrateurId { get; set; }
        public Administrateur? Administrateur { get; set; }

        // FK to Dossier
        public int DossierInscriptionId { get; set; }
        public DossierInscription? DossierInscription { get; set; }
    }
}
