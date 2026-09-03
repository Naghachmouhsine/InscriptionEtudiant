namespace InscriptionEtudiant.ViewModels.Admin
{
    public class HistoriqueStatutViewModel
    {
        public string? AncienStatut { get; set; }

        public string? AncienStatutLibelle { get; set; }

        public string AncienStatutBadgeClass { get; set; } = "bg-secondary";

        public string? NouveauStatut { get; set; }

        public string NouveauStatutLibelle { get; set; } = string.Empty;

        public string NouveauStatutBadgeClass { get; set; } = "bg-secondary";

        public string? Commentaire { get; set; }

        public DateTime DateChangement { get; set; }

        public string? Administrateur { get; set; }
    }
}
