using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InscriptionEtudiant.Data;
using InscriptionEtudiant.Models;
using InscriptionEtudiant.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InscriptionEtudiant.Services
{
    /// <summary>
    /// Implementation of IFiliereService using EF Core.
    /// </summary>
    public class FiliereService : IFiliereService
    {
        private readonly ApplicationDbContext _db;

        public FiliereService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Filiere> CreateAsync(Filiere filiere)
        {
            _db.Filieres.Add(filiere);
            await _db.SaveChangesAsync();
            return filiere;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Filieres.FindAsync(id);
            if (entity == null) return;
            _db.Filieres.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Filiere>> GetAllAsync(string? search = null)
        {
            var query = _db.Filieres.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(f => f.Nom.Contains(search) || (f.Description ?? string.Empty).Contains(search));
            }
            return await query.OrderByDescending(f => f.DateDebutInscription).ToListAsync();
        }

        public async Task<Filiere?> GetByIdAsync(int id)
        {
            return await _db.Filieres.FindAsync(id);
        }

        public async Task UpdateAsync(Filiere filiere)
        {
            _db.Filieres.Update(filiere);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Filieres.AnyAsync(f => f.Id == id);
        }
    }
}
