using System.ComponentModel.DataAnnotations;

namespace InscriptionEtudiant.Models
{
    public class ChoixFiliere
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1,3, ErrorMessage = "OrdreChoix doit être 1, 2 ou 3.")]
        public int OrdreChoix { get; set; }

        // FK to Filiere
        public int FiliereId { get; set; }
        public Filiere? Filiere { get; set; }

        // FK to Dossier
        public int DossierInscriptionId { get; set; }
        public DossierInscription? DossierInscription { get; set; }
    }
}
