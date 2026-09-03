using System;
using System.ComponentModel.DataAnnotations;

namespace InscriptionEtudiant.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TypeDocument { get; set; } = string.Empty;

        [Required]
        [StringLength(260)]
        public string NomFichier { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Chemin { get; set; } = string.Empty;

        public DateTime DateDepot { get; set; } = DateTime.UtcNow;

        // FK to Dossier
        public int DossierInscriptionId { get; set; }
        public DossierInscription? DossierInscription { get; set; }
    }
}
