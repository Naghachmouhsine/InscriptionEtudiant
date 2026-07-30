using System.ComponentModel.DataAnnotations;

namespace InscriptionEtudiant.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string CNE { get; set; } = string.Empty;


        [Required]
        public string Nom { get; set; } = string.Empty;


        [Required]
        public string Prenom { get; set; } = string.Empty;


        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;


        [Required]
        public string Telephone { get; set; } = string.Empty;


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;


        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}