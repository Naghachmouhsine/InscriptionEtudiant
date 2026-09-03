using InscriptionEtudiant.ViewModels;

namespace InscriptionEtudiant.Services.Interfaces
{
    public interface ICandidatDashboardService
    {
        Task<DashboardEtudiantViewModel> GetDashboardAsync(int etudiantId);
    }
}
