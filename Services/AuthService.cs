using InscriptionEtudiant.Data;
using InscriptionEtudiant.Models;
using InscriptionEtudiant.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace InscriptionEtudiant.Services
{
    public class AuthService : AuthServiceInt
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterCandidat(RegisterViewModel model)
        {
            // prevent duplicate CNE or Email
            if (await _context.Candidats.AnyAsync(c => c.CNE == model.CNE || c.Email == model.Email))
            {
                return false;
            }

            var candidat = new Candidat
            {
                CNE = model.CNE,
                Nom = model.Nom,
                Prenom = model.Prenom,
                Email = model.Email,
                Telephone = model.Telephone,
                PasswordHash = HashPassword(model.Password)
            };

            _context.Candidats.Add(candidat);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Candidat?> LoginCandidat(LoginViewModel model)
        {
            var candidat = await _context.Candidats.SingleOrDefaultAsync(c => c.Email == model.Email);
            if (candidat == null)
                return null;

            if (VerifyPassword(model.Password, candidat.PasswordHash))
                return candidat;

            return null;
        }

        public async Task<Administrateur?> LoginAdmin(LoginViewModel model)
        {
            var admin = await _context.Administrateurs.SingleOrDefaultAsync(a => a.Email == model.Email);
            if (admin == null)
                return null;

            if (VerifyPassword(model.Password, admin.PasswordHash))
                return admin;

            return null;
        }

        // Simple salted SHA256 hashing. Suitable for demonstration; consider using a stronger algorithm (e.g., PBKDF2, bcrypt) in production.
        private static string HashPassword(string password)
        {
            // generate 16 bytes salt
            var saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var toHash = new byte[saltBytes.Length + passwordBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, toHash, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, toHash, saltBytes.Length, passwordBytes.Length);

            using (var sha = SHA256.Create())
            {
                var hashBytes = sha.ComputeHash(toHash);
                // store as "salt:hash" base64
                return Convert.ToBase64String(saltBytes) + ":" + Convert.ToBase64String(hashBytes);
            }
        }

        private static bool VerifyPassword(string password, string stored)
        {
            if (string.IsNullOrEmpty(stored))
                return false;

            var parts = stored.Split(':');
            if (parts.Length != 2)
                return false;

            var saltBytes = Convert.FromBase64String(parts[0]);
            var storedHash = Convert.FromBase64String(parts[1]);

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var toHash = new byte[saltBytes.Length + passwordBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, toHash, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, toHash, saltBytes.Length, passwordBytes.Length);

            using (var sha = SHA256.Create())
            {
                var hashBytes = sha.ComputeHash(toHash);
                return hashBytes.SequenceEqual(storedHash);
            }
        }
    }
}
