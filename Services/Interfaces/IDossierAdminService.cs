using InscriptionEtudiant.Models;
using InscriptionEtudiant.ViewModels.Admin;

namespace InscriptionEtudiant.Services.Interfaces
{
    public interface IDossierAdminService
    {
        Task<AdminDossierListViewModel> GetDossiersAsync(StatutDossier? statut = null, string? recherche = null);

        Task<AdminDossierDetailsViewModel> GetDossierDetailsAsync(int id);

        Task ChangerStatutAsync(
            int dossierId,
            StatutDossier nouveauStatut,
            string commentaire,
            int administrateurId,
            int? filiereAffecteeId = null);
    }
}
