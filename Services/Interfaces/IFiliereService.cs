using System.Collections.Generic;
using System.Threading.Tasks;
using InscriptionEtudiant.Models;

namespace InscriptionEtudiant.Services.Interfaces
{
    /// <summary>
    /// Service contract for managing Filieres.
    /// </summary>
    public interface IFiliereService
    {
        Task<IEnumerable<Filiere>> GetAllAsync(string? search = null);

        Task<Filiere?> GetByIdAsync(int id);

        Task<Filiere> CreateAsync(Filiere filiere);

        Task UpdateAsync(Filiere filiere);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}
