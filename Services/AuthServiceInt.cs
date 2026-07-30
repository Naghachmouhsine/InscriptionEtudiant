using InscriptionEtudiant.Models;
using InscriptionEtudiant.ViewModels;

namespace InscriptionEtudiant.Services
{
    public interface AuthServiceInt
    {
        Task<bool> RegisterCandidat(RegisterViewModel model);

        Task<Candidat?> LoginCandidat(LoginViewModel model);

        Task<Administrateur?> LoginAdmin(LoginViewModel model);
    }
}