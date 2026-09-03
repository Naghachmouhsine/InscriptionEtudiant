using InscriptionEtudiant.Models;

namespace InscriptionEtudiant.Infrastructure.Ui
{
    public static class StatutDossierDisplay
    {
        public static (string Libelle, string BadgeClass) GetDisplay(StatutDossier statut)
        {
            return statut switch
            {
                StatutDossier.EnAttente => ("En attente", "bg-warning text-dark"),
                StatutDossier.Incomplet => ("Modification demandée", "bg-orange"),
                StatutDossier.Accepte => ("Accepté", "bg-success"),
                StatutDossier.Refuse => ("Refusé", "bg-danger"),
                _ => (statut.ToString(), "bg-secondary")
            };
        }

        public static (string Libelle, string BadgeClass) GetDisplayFromString(string? statut)
        {
            if (string.IsNullOrWhiteSpace(statut))
            {
                return ("—", "bg-secondary");
            }

            if (Enum.TryParse<StatutDossier>(statut, out var parsed))
            {
                return GetDisplay(parsed);
            }

            return statut switch
            {
                "En cours" => ("En cours", "bg-primary"),
                _ => (statut, "bg-secondary")
            };
        }
    }
}
