using System.ComponentModel.DataAnnotations;
using InscriptionEtudiant.Models;

namespace InscriptionEtudiant.ViewModels.Admin
{
    public class ChangerStatutViewModel
    {
        public int DossierId { get; set; }

        [Required(ErrorMessage = "Le statut est obligatoire.")]
        public StatutDossier NouveauStatut { get; set; }

        // When accepting a dossier, the admin must provide the assigned filiere id
        public int? FiliereAffecteeId { get; set; }

        [Required(ErrorMessage = "Le commentaire est obligatoire.")]
        [StringLength(1000, ErrorMessage = "Le commentaire ne peut pas dépasser 1000 caractères.")]
        public string Commentaire { get; set; } = string.Empty;
    }
}
