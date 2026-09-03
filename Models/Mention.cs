using System.ComponentModel.DataAnnotations;

namespace InscriptionEtudiant.Models
{
    public class Mention
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = string.Empty;
    }
}
