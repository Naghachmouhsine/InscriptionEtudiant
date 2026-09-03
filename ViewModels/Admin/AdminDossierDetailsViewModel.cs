using InscriptionEtudiant.Models;

namespace InscriptionEtudiant.ViewModels.Admin
{
    public class AdminDossierDetailsViewModel
    {
        public int Id { get; set; }

        public DateTime DateDepot { get; set; }

        public StatutDossier Statut { get; set; }

        public string StatutLibelle { get; set; } = string.Empty;

        public string StatutBadgeClass { get; set; } = "bg-secondary";

        public EtudiantViewModel Etudiant { get; set; } = new();

        public ParcoursAcademiqueViewModel? Parcours { get; set; }

        public List<DocumentViewModel> Documents { get; set; } = new();

        public List<ChoixFiliereViewModel> ChoixFilieres { get; set; } = new();

        public List<HistoriqueStatutViewModel> Historique { get; set; } = new();

        // Affectation finale
        public int? FiliereAffecteeId { get; set; }
        public string? FiliereAffecteeNom { get; set; }
    }
}
