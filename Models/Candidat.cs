using System.ComponentModel.DataAnnotations;

namespace InscriptionEtudiant.Models
{
    public class Candidat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string CNE { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string Telephone { get; set; } = string.Empty;

        // One-to-one relation: a candidat has one dossier inscription
        public DossierInscription? DossierInscription { get; set; }
    }
}