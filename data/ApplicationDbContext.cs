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

        public DbSet<InscriptionEtudiant.Models.Filiere> Filieres { get; set; }
        public DbSet<DossierInscription> DossierInscriptions { get; set; }
        public DbSet<ParcoursAcademique> ParcoursAcademiques { get; set; }
        public DbSet<SerieBac> SerieBacs { get; set; }
        public DbSet<Mention> Mentions { get; set; }
        public DbSet<ChoixFiliere> ChoixFilieres { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<HistoriqueStatut> HistoriqueStatuts { get; set; }

        public DbSet<Admission> Admissions { get; set; }

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

            modelBuilder.Entity<InscriptionEtudiant.Models.Filiere>(entity =>
            {
                entity.Property(f => f.Nom).HasMaxLength(100).IsRequired();
                entity.Property(f => f.Description).HasMaxLength(500);
                entity.Property(f => f.Capacite).IsRequired();
            });

            // One-to-one Candidat <-> DossierInscription
            modelBuilder.Entity<Candidat>()
                .HasOne<DossierInscription>(c => c.DossierInscription)
                .WithOne(d => d.Candidat)
                .HasForeignKey<DossierInscription>(d => d.CandidatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DossierInscription>(d =>
            {
                d.HasMany(x => x.Documents).WithOne(x => x.DossierInscription).HasForeignKey(x => x.DossierInscriptionId).OnDelete(DeleteBehavior.Cascade);
                d.HasMany(x => x.ChoixFilieres).WithOne(x => x.DossierInscription).HasForeignKey(x => x.DossierInscriptionId).OnDelete(DeleteBehavior.Cascade);
                d.HasMany(x => x.HistoriqueStatuts).WithOne(x => x.DossierInscription).HasForeignKey(x => x.DossierInscriptionId).OnDelete(DeleteBehavior.Cascade);
                d.HasOne(x => x.ParcoursAcademique).WithOne(p => p.DossierInscription).HasForeignKey<ParcoursAcademique>(p => p.DossierInscriptionId).OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Admission>(entity =>
            {
                entity.HasKey(a => a.Id);

                // Relation Admission -> DossierInscription
                entity.HasOne(a => a.DossierInscription)
                    .WithMany()
                    .HasForeignKey(a => a.DossierInscriptionId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relation Admission -> ChoixFiliere
                entity.HasOne(a => a.ChoixFiliere)
                    .WithMany()
                    .HasForeignKey(a => a.ChoixFiliereId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relation Admission -> Administrateur
                entity.HasOne(a => a.Administrateur)
                    .WithMany()
                    .HasForeignKey(a => a.AdministrateurId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Enum stocké en entier
                entity.Property(a => a.Statut)
                    .HasConversion<int>()
                    .IsRequired();

                // Precision decimal pour éviter le warning NoteNationale
                entity.Property(a => a.Score)
                    .HasPrecision(10, 2);

                entity.Property(a => a.Commentaire)
                    .HasMaxLength(1000);

                entity.Property(a => a.DateCreation)
                    .HasDefaultValueSql("GETDATE()");
            });


            modelBuilder.Entity<ParcoursAcademique>(p =>
            {
                p.HasOne(s => s.SerieBac).WithMany().HasForeignKey(s => s.SerieBacId).OnDelete(DeleteBehavior.Restrict);
                p.HasOne(m => m.Mention).WithMany().HasForeignKey(m => m.MentionId).OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}