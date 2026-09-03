using System;
using System.ComponentModel.DataAnnotations;

namespace InscriptionEtudiant.Models
{
    /// <summary>
    /// Represents a study program (filière) in the system.
    /// </summary>
    public class Filiere
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La capacité doit être supérieure à 0.")]
        public int Capacite { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateDebutInscription { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateFinInscription { get; set; }

        // Convenience property to know availability
        public bool IsAvailable => DateDebutInscription <= DateTime.UtcNow && DateFinInscription >= DateTime.UtcNow;
    }
}
