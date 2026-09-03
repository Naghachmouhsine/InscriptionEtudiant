using InscriptionEtudiant.Models;

namespace InscriptionEtudiant.ViewModels.Admin
{
    public class AdminDossierListViewModel
    {
        public IEnumerable<DossierListItemViewModel> Dossiers { get; set; } = Enumerable.Empty<DossierListItemViewModel>();

        public StatutDossier? StatutFiltre { get; set; }

        public string? Recherche { get; set; }
    }
}
