using System.ComponentModel.DataAnnotations;

namespace InscriptionEtudiant.Models
{
    public class Administrateur
    {
        [Key]
        public int Id { get; set; }

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
    }
}