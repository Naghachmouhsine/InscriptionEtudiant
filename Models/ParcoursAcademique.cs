using System.ComponentModel.DataAnnotations;

namespace InscriptionEtudiant.Models
{
    public class ParcoursAcademique
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AnneeBac { get; set; }

        public decimal? NoteNationale { get; set; }

        public decimal? NoteRegionale { get; set; }

        // Foreign keys
        public int SerieBacId { get; set; }
        public SerieBac? SerieBac { get; set; }

        public int MentionId { get; set; }
        public Mention? Mention { get; set; }

        // Back reference to dossier (optional)
        public int? DossierInscriptionId { get; set; }
        public DossierInscription? DossierInscription { get; set; }
    }
}
