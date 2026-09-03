using InscriptionEtudiant.Models;

namespace InscriptionEtudiant.ViewModels.Admin
{
    public class DossierListItemViewModel
    {
        public int Id { get; set; }

        public string CNE { get; set; } = string.Empty;

        public string NomComplet { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime DateDepot { get; set; }

        public StatutDossier Statut { get; set; }

        public string StatutLibelle { get; set; } = string.Empty;

        public string StatutBadgeClass { get; set; } = "bg-secondary";

        public string? FilierePrincipale { get; set; }

        public int NombreDocuments { get; set; }
    }
}
