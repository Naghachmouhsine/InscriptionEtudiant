using System.Security.Claims;
using InscriptionEtudiant.Data;
using InscriptionEtudiant.Infrastructure;
using InscriptionEtudiant.Infrastructure.Ui;
using InscriptionEtudiant.Models;
using InscriptionEtudiant.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace InscriptionEtudiant.Services
{
    public class InscriptionService : IInscriptionService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _uploadsRoot;
        private readonly string _wizardTempRoot;
        private readonly string[] _allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
        private const long _maxFileBytes = 10 * 1024 * 1024;
        private readonly ILogger<InscriptionService> _logger;

        public InscriptionService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, ILogger<InscriptionService> logger)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _uploadsRoot = Path.Combine(env.WebRootPath ?? "wwwroot", "uploads");
            _wizardTempRoot = Path.Combine(_uploadsRoot, "_wizard-temp");
            _logger = logger;
        }

        public async Task<Candidat?> GetCurrentEtudiantAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                return null;
            }

            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(idClaim))
            {
                return null;
            }

            if (!int.TryParse(idClaim, out var id))
            {
                return null;
            }

            return await _db.Candidats.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Administrateur?> GetCurrentAdminAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                return null;
            }

            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(idClaim))
            {
                return null;
            }

            if (!int.TryParse(idClaim, out var id))
            {
                return null;
            }
            _logger.LogInformation("Récupération de l'admin connecté  : "+id);
            return await _db.Administrateurs.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Filiere>> GetFilieresDisponiblesAsync()
        {
            var now = DateTime.UtcNow;
            return await _db.Filieres
                .AsNoTracking()
                .Where(f => f.DateDebutInscription <= now && f.DateFinInscription >= now)
                .OrderBy(f => f.Nom)
                .ToListAsync();
        }

        public async Task<IEnumerable<SerieBac>> GetSeriesBacAsync()
        {
            return await _db.SerieBacs
                .AsNoTracking()
                .OrderBy(s => s.Nom)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mention>> GetMentionsAsync()
        {
            return await _db.Mentions
                .AsNoTracking()
                .OrderBy(m => m.Nom)
                .ToListAsync();
        }

        public async Task SaveParcoursAsync(int dossierId, ParcoursAcademique parcours)
        {
            ArgumentNullException.ThrowIfNull(parcours);

            await ValidateParcoursAsync(parcours);
        }

        public async Task SaveChoixFilieresAsync(int dossierId, IEnumerable<ChoixFiliere> choix)
        {
            await ValidateChoixFilieresAsync(choix);
        }

        public async Task<IEnumerable<Document>> UploadDocumentsAsync(int wizardKey, IEnumerable<(string FieldName, byte[] Content, string FileName)> files)
        {
            var list = files?.ToList() ?? new List<(string FieldName, byte[] Content, string FileName)>();
            if (list.Count == 0)
            {
                throw new InvalidOperationException(InscriptionMessages.WarningNoFilesSelected);
            }

            var targetDir = Path.Combine(_wizardTempRoot, wizardKey.ToString());
            Directory.CreateDirectory(targetDir);

            var prepared = new List<Document>();
            foreach (var file in list)
            {
                if (file.Content == null || file.Content.Length == 0)
                {
                    continue;
                }

                if (file.Content.Length > _maxFileBytes)
                {
                    throw new InvalidOperationException(InscriptionMessages.FileTooLarge(file.FileName));
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                {
                    throw new InvalidOperationException(InscriptionMessages.FileTypeNotAllowed(extension));
                }

                var safeName = $"{Path.GetRandomFileName()}{extension}";
                var tempPath = Path.Combine(targetDir, safeName);
                await File.WriteAllBytesAsync(tempPath, file.Content);

                prepared.Add(new Document
                {
                    TypeDocument = file.FieldName,
                    NomFichier = file.FileName,
                    Chemin = tempPath
                });
            }

            return prepared;
        }

        public async Task SubmitDossierAsync(Candidat candidat, InscriptionWizardState state)
        {
            ArgumentNullException.ThrowIfNull(candidat);
            ArgumentNullException.ThrowIfNull(state);

            if (state.CandidatId != candidat.Id)
            {
                throw new InvalidOperationException(InscriptionMessages.ErrorUnexpected);
            }

            if (await _db.DossierInscriptions.AsNoTracking().AnyAsync(d => d.CandidatId == candidat.Id))
            {
                throw new InvalidOperationException(InscriptionMessages.ErrorDossierAlreadySubmitted);
            }

            await ValidateParcoursAsync(state.Parcours.ToEntity());
            await ValidateChoixFilieresAsync(state.ChoixFilieres.Select(choice => choice.ToEntity()).ToList());
            await ValidateDocumentsAsync(state.Documents);

            var now = DateTime.UtcNow;
            var tempFilesToDelete = new List<string>();

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var dossier = new DossierInscription
                {
                    CandidatId = candidat.Id,
                    DateDepot = now,
                    StatutActuel = StatutDossier.Incomplet
                };

                _db.DossierInscriptions.Add(dossier);

                var parcours = state.Parcours.ToEntity();
                parcours.DossierInscription = dossier;
                _db.ParcoursAcademiques.Add(parcours);

                foreach (var choice in state.ChoixFilieres.OrderBy(choice => choice.OrdreChoix))
                {
                    _db.ChoixFilieres.Add(new ChoixFiliere
                    {
                        OrdreChoix = choice.OrdreChoix,
                        FiliereId = choice.FiliereId,
                        DossierInscription = dossier
                    });
                }

                var finalDir = Path.Combine(_uploadsRoot, "dossiers", candidat.Id.ToString());
                Directory.CreateDirectory(finalDir);

                foreach (var documentState in state.Documents)
                {
                    if (!File.Exists(documentState.TempFilePath))
                    {
                        throw new InvalidOperationException(InscriptionMessages.ErrorUnexpected);
                    }

                    var extension = Path.GetExtension(documentState.FileName).ToLowerInvariant();
                    var safeName = $"{Path.GetRandomFileName()}{extension}";
                    var finalPath = Path.Combine(finalDir, safeName);
                    File.Copy(documentState.TempFilePath, finalPath, true);
                    tempFilesToDelete.Add(finalPath);

                    _db.Documents.Add(new Document
                    {
                        DossierInscription = dossier,
                        TypeDocument = documentState.FieldName,
                        NomFichier = documentState.FileName,
                        Chemin = Path.Combine("uploads", "dossiers", candidat.Id.ToString(), safeName).Replace('\\', '/'),
                        DateDepot = now
                    });
                }

                _db.HistoriqueStatuts.Add(new HistoriqueStatut
                {
                    DossierInscription = dossier,
                    AncienStatut = null,
                    NouveauStatut = StatutDossier.Incomplet.ToString(),
                    Commentaire = "Préparation du dossier",
                    DateChangement = now
                });

                dossier.StatutActuel = StatutDossier.EnAttente;

                _db.HistoriqueStatuts.Add(new HistoriqueStatut
                {
                    DossierInscription = dossier,
                    AncienStatut = StatutDossier.Incomplet.ToString(),
                    NouveauStatut = StatutDossier.EnAttente.ToString(),
                    Commentaire = "Validation finale du dossier",
                    DateChangement = now
                });

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();

                foreach (var path in tempFilesToDelete.Distinct())
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }

                throw;
            }
        }

        private async Task ValidateParcoursAsync(ParcoursAcademique parcours)
        {
            if (parcours.AnneeBac <= 0)
            {
                throw new InvalidOperationException("Veuillez renseigner l'année du baccalauréat.");
            }

            var seriesExists = await _db.SerieBacs.AsNoTracking().AnyAsync(s => s.Id == parcours.SerieBacId);
            if (!seriesExists)
            {
                throw new InvalidOperationException("Veuillez sélectionner une série du baccalauréat valide.");
            }

            var mentionExists = await _db.Mentions.AsNoTracking().AnyAsync(m => m.Id == parcours.MentionId);
            if (!mentionExists)
            {
                throw new InvalidOperationException("Veuillez sélectionner une mention valide.");
            }

            if (parcours.NoteNationale.HasValue && (parcours.NoteNationale < 0 || parcours.NoteNationale > 20))
            {
                throw new InvalidOperationException("La note nationale doit être comprise entre 0 et 20.");
            }

            if (parcours.NoteRegionale.HasValue && (parcours.NoteRegionale < 0 || parcours.NoteRegionale > 20))
            {
                throw new InvalidOperationException("La note régionale doit être comprise entre 0 et 20.");
            }
        }

        private async Task ValidateChoixFilieresAsync(IEnumerable<ChoixFiliere> choix)
        {
            var list = choix?.ToList() ?? new List<ChoixFiliere>();
            if (list.Count < 1 || list.Count > 3)
            {
                throw new InvalidOperationException(InscriptionMessages.ErrorChoicesRange);
            }

            if (list.Select(choice => choice.FiliereId).Distinct().Count() != list.Count)
            {
                throw new InvalidOperationException("Chaque filière doit être choisie une seule fois.");
            }

            var filiereIds = list.Select(choice => choice.FiliereId).Distinct().ToList();
            var now = DateTime.UtcNow;
            var filieres = await _db.Filieres
                .AsNoTracking()
                .Where(filiere => filiereIds.Contains(filiere.Id))
                .ToListAsync();

            foreach (var filiereId in filiereIds)
            {
                var filiere = filieres.SingleOrDefault(item => item.Id == filiereId);
                if (filiere == null)
                {
                    throw new InvalidOperationException(InscriptionMessages.ErrorChoicesUnavailable);
                }

                if (filiere.DateDebutInscription > now || filiere.DateFinInscription < now)
                {
                    throw new InvalidOperationException(InscriptionMessages.ErrorChoicesUnavailable);
                }
            }
        }

        private Task ValidateDocumentsAsync(IEnumerable<InscriptionDocumentState> documents)
        {
            var list = documents?.ToList() ?? new List<InscriptionDocumentState>();
            foreach (var document in list)
            {
                if (string.IsNullOrWhiteSpace(document.FileName) || string.IsNullOrWhiteSpace(document.TempFilePath))
                {
                    throw new InvalidOperationException(InscriptionMessages.ErrorUnexpected);
                }
            }

            return Task.CompletedTask;
        }
    }

    internal static class InscriptionWizardMappingExtensions
    {
        public static ParcoursAcademique ToEntity(this InscriptionParcoursState state)
        {
            return new ParcoursAcademique
            {
                AnneeBac = state.AnneeBac,
                SerieBacId = state.SerieBacId,
                MentionId = state.MentionId,
                NoteNationale = state.NoteNationale,
                NoteRegionale = state.NoteRegionale
            };
        }

        public static ChoixFiliere ToEntity(this InscriptionChoiceState state)
        {
            return new ChoixFiliere
            {
                OrdreChoix = state.OrdreChoix,
                FiliereId = state.FiliereId
            };
        }
    }
}
