using InscriptionEtudiant.Models;
using Microsoft.EntityFrameworkCore;

namespace InscriptionEtudiant.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Candidat> Candidats { get; set; }

        public DbSet<Administrateur> Administrateurs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Candidat>()
                .HasIndex(c => c.CNE)
                .IsUnique();

            modelBuilder.Entity<Candidat>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Administrateur>()
                .HasIndex(a => a.Email)
                .IsUnique();
        }

    }
}