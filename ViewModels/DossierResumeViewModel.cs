using InscriptionEtudiant.Models;

namespace InscriptionEtudiant.ViewModels
{
    public class DossierResumeViewModel
    {
        public int Id { get; set; }

        public DateTime? DateDepot { get; set; }

        public StatutDossier StatutActuel { get; set; }

        public string StatutLibelle { get; set; } = string.Empty;

        public string StatutBadgeClass { get; set; } = "bg-secondary";

        public string NomCompletEtudiant { get; set; } = string.Empty;

        public string CNE { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telephone { get; set; } = string.Empty;

        public string? Filiere1 { get; set; }

        public string? Filiere2 { get; set; }

        public string? Filiere3 { get; set; }

        public int NombreDocuments { get; set; }

        public int Progression { get; set; }

        public bool PeutModifier { get; set; }

        public bool PeutTelechargerRecu { get; set; }

        public bool PeutCompleter { get; set; }

        public bool AUnDossier { get; set; }

        public int? FiliereAffecteeId { get; set; }
        public string? FiliereAffecteeNom { get; set; }

        // Utile pour la vue : est-ce qu'on a une filière affectée à afficher ?
        public bool AUneFiliereAffectee => FiliereAffecteeId.HasValue;
    }
}
