namespace InscriptionEtudiant.ViewModels
{
    public class DashboardEtudiantViewModel
    {
        public DossierResumeViewModel Dossier { get; set; } = new();

        public List<HistoriqueItemViewModel> Historique { get; set; } = new();
    }
}
