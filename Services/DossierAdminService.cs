using InscriptionEtudiant.Data;
using InscriptionEtudiant.Infrastructure.Ui;
using InscriptionEtudiant.Models;
using InscriptionEtudiant.Services.Interfaces;
using InscriptionEtudiant.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace InscriptionEtudiant.Services
{
    public class DossierAdminService : IDossierAdminService
    {
        private readonly ApplicationDbContext _db;

        public DossierAdminService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<AdminDossierListViewModel> GetDossiersAsync(StatutDossier? statut = null, string? recherche = null)
        {
            var dossiers = await _db.DossierInscriptions
                .AsNoTracking()
                .OrderByDescending(d => d.DateDepot)
                .Select(d => new DossierListItemViewModel
                {
                    Id = d.Id,
                    CNE = d.Candidat!.CNE,
                    NomComplet = d.Candidat.Nom + " " + d.Candidat.Prenom,
                    Email = d.Candidat.Email,
                    DateDepot = d.DateDepot,
                    Statut = d.StatutActuel,
                    FilierePrincipale = d.ChoixFilieres
                        .Where(c => c.OrdreChoix == 1)
                        .Select(c => c.Filiere!.Nom)
                        .FirstOrDefault(),
                    NombreDocuments = d.Documents.Count
                })
                .ToListAsync();

            foreach (var dossier in dossiers)
            {
                var (libelle, badge) = StatutDossierDisplay.GetDisplay(dossier.Statut);
                dossier.StatutLibelle = libelle;
                dossier.StatutBadgeClass = badge;
            }

            var filtered = dossiers.AsQueryable();
            if (statut.HasValue)
            {
                filtered = filtered.Where(d => d.Statut == statut.Value);
            }

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                var q = recherche.Trim().ToLowerInvariant();
                filtered = filtered.Where(d => d.NomComplet.ToLower().Contains(q)
                                               || d.CNE.ToLower().Contains(q)
                                               || d.Email.ToLower().Contains(q));
            }

            return new AdminDossierListViewModel
            {
                Dossiers = filtered.ToList(),
                StatutFiltre = statut,
                Recherche = recherche
            };
        }

        public async Task<AdminDossierDetailsViewModel> GetDossierDetailsAsync(int id)
        {
            var dossier = await _db.DossierInscriptions
                .AsNoTracking()
                .Include(d => d.Candidat)
                .Include(d => d.ParcoursAcademique!)
                    .ThenInclude(p => p.SerieBac)
                .Include(d => d.ParcoursAcademique!)
                    .ThenInclude(p => p.Mention)
                .Include(d => d.ChoixFilieres)
                    .ThenInclude(c => c.Filiere)
                .Include(d => d.Documents)
                .Include(d => d.HistoriqueStatuts)
                    .ThenInclude(h => h.Administrateur)
                .SingleOrDefaultAsync(d => d.Id == id);

            if (dossier == null)
                throw new InvalidOperationException($"Dossier d'inscription #{id} introuvable.");

            if (dossier.Candidat == null)
                throw new InvalidOperationException($"Candidat associé au dossier #{id} introuvable.");

            var (statutLibelle, statutBadge) = StatutDossierDisplay.GetDisplay(dossier.StatutActuel);

            var details = new AdminDossierDetailsViewModel
            {
                Id = dossier.Id,
                DateDepot = dossier.DateDepot,
                Statut = dossier.StatutActuel,
                StatutLibelle = statutLibelle,
                StatutBadgeClass = statutBadge,
                Etudiant = new EtudiantViewModel
                {
                    Id = dossier.Candidat.Id,
                    CNE = dossier.Candidat.CNE,
                    Nom = dossier.Candidat.Nom,
                    Prenom = dossier.Candidat.Prenom,
                    NomComplet = string.IsNullOrWhiteSpace(dossier.Candidat.Nom) ? dossier.Candidat.Prenom : dossier.Candidat.Nom + " " + dossier.Candidat.Prenom,
                    Email = dossier.Candidat.Email,
                    Telephone = dossier.Candidat.Telephone ?? string.Empty
                },
                Parcours = dossier.ParcoursAcademique == null
                    ? null
                    : new ParcoursAcademiqueViewModel
                    {
                        AnneeBac = dossier.ParcoursAcademique.AnneeBac,
                        NoteNationale = dossier.ParcoursAcademique.NoteNationale,
                        NoteRegionale = dossier.ParcoursAcademique.NoteRegionale,
                        SerieBac = dossier.ParcoursAcademique.SerieBac?.Nom ?? string.Empty,
                        Mention = dossier.ParcoursAcademique.Mention?.Nom ?? string.Empty
                    },
                ChoixFilieres = dossier.ChoixFilieres
                    .OrderBy(c => c.OrdreChoix)
                    .Select(c => new ChoixFiliereViewModel
                    {
                        OrdreChoix = c.OrdreChoix,
                        FiliereId = c.FiliereId,
                        FiliereNom = c.Filiere?.Nom ?? string.Empty
                    })
                    .ToList(),
                Documents = dossier.Documents
                    .OrderBy(doc => doc.TypeDocument)
                    .Select(doc => new DocumentViewModel
                    {
                        TypeDocument = doc.TypeDocument,
                        NomFichier = doc.NomFichier,
                        Chemin = doc.Chemin
                    })
                    .ToList(),
                Historique = dossier.HistoriqueStatuts
                    .OrderByDescending(h => h.DateChangement)
                    .Select(h => new HistoriqueStatutViewModel
                    {
                        AncienStatut = h.AncienStatut,
                        AncienStatutLibelle = StatutDossierDisplay.GetDisplayFromString(h.AncienStatut).Libelle,
                        AncienStatutBadgeClass = StatutDossierDisplay.GetDisplayFromString(h.AncienStatut).BadgeClass,
                        NouveauStatut = h.NouveauStatut,
                        NouveauStatutLibelle = StatutDossierDisplay.GetDisplayFromString(h.NouveauStatut).Libelle,
                        NouveauStatutBadgeClass = StatutDossierDisplay.GetDisplayFromString(h.NouveauStatut).BadgeClass,
                        Commentaire = h.Commentaire,
                        DateChangement = h.DateChangement,
                        Administrateur = h.Administrateur != null ? $"{h.Administrateur.Prenom} {h.Administrateur.Nom}".Trim() : null
                    })
                    .ToList()
            };

            return details;
        }

        public async Task ChangerStatutAsync(
            int dossierId,
            StatutDossier nouveauStatut,
            string commentaire,
            int administrateurId,
            int? filiereAffecteeId = null)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var dossier = await _db.DossierInscriptions
                    .Include(d => d.ChoixFilieres)
                    .SingleOrDefaultAsync(d => d.Id == dossierId);

                if (dossier == null)
                    throw new InvalidOperationException($"Dossier d'inscription #{dossierId} introuvable.");

                var administrateurExists = await _db.Administrateurs
                    .AsNoTracking()
                    .AnyAsync(a => a.Id == administrateurId);

                if (!administrateurExists)
                    throw new InvalidOperationException($"Administrateur #{administrateurId} introuvable.");

                var ancienStatut = dossier.StatutActuel;
                var now = DateTime.UtcNow;

                // If accepting, verify the selected filiere belongs to the candidate's choices
                if (nouveauStatut == StatutDossier.Accepte)
                {
                    if (!filiereAffecteeId.HasValue)
                        throw new InvalidOperationException("Filière d'affectation non fournie.");

                    var valid = dossier.ChoixFilieres.Any(c => c.FiliereId == filiereAffecteeId.Value);
                    if (!valid)
                        throw new InvalidOperationException("La filière sélectionnée n'est pas parmi les choix du candidat.");

                    dossier.FiliereAffecteeId = filiereAffecteeId.Value;
                }

                dossier.StatutActuel = nouveauStatut;

                _db.HistoriqueStatuts.Add(new HistoriqueStatut
                {
                    DossierInscriptionId = dossier.Id,
                    AncienStatut = ancienStatut.ToString(),
                    NouveauStatut = nouveauStatut.ToString(),
                    Commentaire = string.IsNullOrWhiteSpace(commentaire) ? null : commentaire.Trim(),
                    DateChangement = now,
                    AdministrateurId = administrateurId
                });

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static HistoriqueStatutViewModel MapHistorique(HistoriqueStatut historique)
        {
            var ancien = StatutDossierDisplay.GetDisplayFromString(historique.AncienStatut);
            var nouveau = StatutDossierDisplay.GetDisplayFromString(historique.NouveauStatut);

            return new HistoriqueStatutViewModel
            {
                AncienStatut = historique.AncienStatut,
                AncienStatutLibelle = ancien.Libelle,
                AncienStatutBadgeClass = ancien.BadgeClass,
                NouveauStatut = historique.NouveauStatut,
                NouveauStatutLibelle = nouveau.Libelle,
                NouveauStatutBadgeClass = nouveau.BadgeClass,
                Commentaire = historique.Commentaire,
                DateChangement = historique.DateChangement,
                Administrateur = historique.Administrateur != null
                    ? $"{historique.Administrateur.Prenom} {historique.Administrateur.Nom}".Trim()
                    : null
            };
        }
    }
}
