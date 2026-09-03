using InscriptionEtudiant.Data;
using InscriptionEtudiant.Infrastructure.Ui;
using InscriptionEtudiant.Models;
using InscriptionEtudiant.Services.Interfaces;
using InscriptionEtudiant.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InscriptionEtudiant.Services
{
    public class CandidatDashboardService : ICandidatDashboardService
    {
        private readonly ApplicationDbContext _db;

        public CandidatDashboardService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardEtudiantViewModel> GetDashboardAsync(int etudiantId)
        {
            var candidat = await _db.Candidats
                .AsNoTracking()
                .Include(c => c.DossierInscription!)
                    .ThenInclude(d => d.ChoixFilieres)
                        .ThenInclude(cf => cf.Filiere)
                .Include(c => c.DossierInscription!)
                    .ThenInclude(d => d.Documents)
                .Include(c => c.DossierInscription!)
                    .ThenInclude(d => d.ParcoursAcademique)
                .Include(c => c.DossierInscription!)
                    .ThenInclude(d => d.HistoriqueStatuts)
                        .ThenInclude(h => h.Administrateur)
                .Include(c => c.DossierInscription!)
                     .ThenInclude(d => d.FiliereAffectee)
                .SingleOrDefaultAsync(c => c.Id == etudiantId);

            if (candidat == null)
            {
                throw new InvalidOperationException("Candidat introuvable.");
            }

            var dossier = candidat.DossierInscription;
            var (statutLibelle, statutBadge) = dossier != null
                ? StatutDossierDisplay.GetDisplay(dossier.StatutActuel)
                : ("Aucun dossier", "bg-secondary");

            var choix = dossier?.ChoixFilieres
                .OrderBy(c => c.OrdreChoix)
                .Select(c => c.Filiere?.Nom)
                .ToList() ?? new List<string?>();

            var resume = new DossierResumeViewModel
            {
                Id = dossier?.Id ?? 0,
                DateDepot = dossier?.DateDepot,
                StatutActuel = dossier?.StatutActuel ?? StatutDossier.EnAttente,
                StatutLibelle = statutLibelle,
                StatutBadgeClass = statutBadge,
                NomCompletEtudiant = $"{candidat.Prenom} {candidat.Nom}".Trim(),
                CNE = candidat.CNE,
                Email = candidat.Email,
                Telephone = candidat.Telephone,
                Filiere1 = choix.ElementAtOrDefault(0),
                Filiere2 = choix.ElementAtOrDefault(1),
                Filiere3 = choix.ElementAtOrDefault(2),
                NombreDocuments = dossier?.Documents.Count ?? 0,
                Progression = CalculateProgression(candidat, dossier),
                PeutModifier = dossier?.StatutActuel == StatutDossier.Incomplet,
                PeutTelechargerRecu = dossier?.StatutActuel == StatutDossier.Accepte,
                PeutCompleter = dossier == null || dossier.StatutActuel == StatutDossier.Incomplet,
                AUnDossier = dossier != null,
                FiliereAffecteeId = dossier?.FiliereAffecteeId,
                FiliereAffecteeNom = dossier?.FiliereAffectee?.Nom
            };

            var historique = dossier?.HistoriqueStatuts
                .OrderByDescending(h => h.DateChangement)
                .Select(h =>
                {
                    var ancien = StatutDossierDisplay.GetDisplayFromString(h.AncienStatut);
                    var nouveau = StatutDossierDisplay.GetDisplayFromString(h.NouveauStatut);

                    return new HistoriqueItemViewModel
                    {
                        Date = h.DateChangement,
                        AncienStatut = h.AncienStatut,
                        AncienStatutLibelle = ancien.Libelle,
                        AncienStatutBadgeClass = ancien.BadgeClass,
                        NouveauStatut = h.NouveauStatut,
                        NouveauStatutLibelle = nouveau.Libelle,
                        NouveauStatutBadgeClass = nouveau.BadgeClass,
                        Commentaire = h.Commentaire,
                        Administrateur = h.Administrateur != null
                            ? $"{h.Administrateur.Prenom} {h.Administrateur.Nom}".Trim()
                            : null
                    };
                })
                .ToList() ?? new List<HistoriqueItemViewModel>();

            return new DashboardEtudiantViewModel
            {
                Dossier = resume,
                Historique = historique
            };
        }

        private static int CalculateProgression(Candidat candidat, DossierInscription? dossier)
        {
            var score = 0;

            if (!string.IsNullOrWhiteSpace(candidat.Nom)
                && !string.IsNullOrWhiteSpace(candidat.Prenom)
                && !string.IsNullOrWhiteSpace(candidat.CNE)
                && !string.IsNullOrWhiteSpace(candidat.Email)
                && !string.IsNullOrWhiteSpace(candidat.Telephone))
            {
                score += 25;
            }

            if (dossier == null)
            {
                return score;
            }

            var parcours = dossier.ParcoursAcademique;
            if (parcours != null
                && parcours.AnneeBac > 0
                && parcours.SerieBacId > 0
                && parcours.MentionId > 0)
            {
                score += 25;
            }

            if (dossier.Documents.Count > 0)
            {
                score += 25;
            }

            var choixCount = dossier.ChoixFilieres.Count;
            score += choixCount switch
            {
                >= 3 => 25,
                2 => 17,
                1 => 8,
                _ => 0
            };

            return Math.Min(score, 100);
        }
    }
}
