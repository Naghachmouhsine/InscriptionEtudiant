using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using InscriptionEtudiant.Models;

namespace InscriptionEtudiant.ViewModels
{
    public class ChoixFiliereViewModel
    {
        public int OrdreChoix { get; set; }
        public int FiliereId { get; set; }
    }

    public class ParcoursAcademiqueViewModel
    {
        [Required]
        public int AnneeBac { get; set; }
        public decimal? NoteNationale { get; set; }
        public decimal? NoteRegionale { get; set; }
        [Required]
        public int SerieBacId { get; set; }
        [Required]
        public int MentionId { get; set; }
    }

    public class CreateInscriptionViewModel
    {
        // Etudiant info (partial)
        public int CandidatId { get; set; }

        public int DossierId { get; set; }

        [Required]
        public string CNE { get; set; } = string.Empty;

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Telephone { get; set; } = string.Empty;

        // Parcours
        public ParcoursAcademiqueViewModel Parcours { get; set; } = new ParcoursAcademiqueViewModel();

        // Available filieres for selection
        public IEnumerable<Filiere> FilieresDisponibles { get; set; } = new List<Filiere>();

        // Selected choices
        public List<ChoixFiliereViewModel> ChoixFilieres { get; set; } = new List<ChoixFiliereViewModel>();

        // Uploaded files
        public List<IFormFile>? Documents { get; set; }
    }
}
