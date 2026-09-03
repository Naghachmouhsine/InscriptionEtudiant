using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InscriptionEtudiant.ViewModels
{
    /// <summary>
    /// ViewModel used for creating and editing a Filiere.
    /// Implements server-side validation to ensure DateFin > DateDebut.
    /// </summary>
    public class FiliereFormViewModel : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Le nom ne peut dépasser 100 caractères.")]
        public string Nom { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La description ne peut dépasser 500 caractères.")]
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateFinInscription <= DateDebutInscription)
            {
                yield return new ValidationResult(
                    "La date de fin doit être strictement supérieure à la date de début.",
                    new[] { nameof(DateFinInscription), nameof(DateDebutInscription) }
                );
            }
        }
    }
}
