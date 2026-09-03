namespace InscriptionEtudiant.ViewModels
{
    public class HistoriqueItemViewModel
    {
        public DateTime Date { get; set; }

        public string? AncienStatut { get; set; }

        public string? AncienStatutLibelle { get; set; }

        public string AncienStatutBadgeClass { get; set; } = "bg-secondary";

        public string? NouveauStatut { get; set; }

        public string NouveauStatutLibelle { get; set; } = string.Empty;

        public string NouveauStatutBadgeClass { get; set; } = "bg-secondary";

        public string? Commentaire { get; set; }

        public string? Administrateur { get; set; }
    }
}
