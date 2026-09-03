using System.Text.Json;
using InscriptionEtudiant.Models;
using Microsoft.AspNetCore.Http;

namespace InscriptionEtudiant.Infrastructure
{
    public sealed class InscriptionWizardState
    {
        public int CandidatId { get; set; }
        public string CNE { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public int CurrentStep { get; set; } = 1;
        public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
        public InscriptionParcoursState Parcours { get; set; } = new();
        public List<InscriptionChoiceState> ChoixFilieres { get; set; } = new();
        public List<InscriptionDocumentState> Documents { get; set; } = new();
        public string TempFolderPath { get; set; } = string.Empty;
    }

    public sealed class InscriptionParcoursState
    {
        public int AnneeBac { get; set; }
        public int SerieBacId { get; set; }
        public string SerieBacNom { get; set; } = string.Empty;
        public int MentionId { get; set; }
        public string MentionNom { get; set; } = string.Empty;
        public decimal? NoteNationale { get; set; }
        public decimal? NoteRegionale { get; set; }
    }

    public sealed class InscriptionChoiceState
    {
        public int OrdreChoix { get; set; }
        public int FiliereId { get; set; }
        public string FiliereNom { get; set; } = string.Empty;
    }

    public sealed class InscriptionDocumentState
    {
        public string FieldName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string TempFilePath { get; set; } = string.Empty;
        public long Size { get; set; }
    }

    public static class InscriptionWizardSessionStore
    {
        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = false
        };

        public static string GetSessionKey(int candidatId) => $"inscription:wizard:{candidatId}";

        public static InscriptionWizardState GetOrCreate(ISession session, Candidat candidat)
        {
            var state = Load(session, candidat.Id) ?? Create(candidat);
            Save(session, state);
            return state;
        }

        public static InscriptionWizardState? Load(ISession session, int candidatId)
        {
            var payload = session.GetString(GetSessionKey(candidatId));
            return string.IsNullOrWhiteSpace(payload)
                ? null
                : JsonSerializer.Deserialize<InscriptionWizardState>(payload, SerializerOptions);
        }

        public static void Save(ISession session, InscriptionWizardState state)
        {
            state.LastUpdatedUtc = DateTime.UtcNow;
            session.SetString(GetSessionKey(state.CandidatId), JsonSerializer.Serialize(state, SerializerOptions));
        }

        public static void Clear(ISession session, int candidatId)
        {
            session.Remove(GetSessionKey(candidatId));
        }

        public static void DeleteTempFiles(InscriptionWizardState? state)
        {
            if (state == null)
            {
                return;
            }

            foreach (var document in state.Documents)
            {
                if (!string.IsNullOrWhiteSpace(document.TempFilePath) && File.Exists(document.TempFilePath))
                {
                    File.Delete(document.TempFilePath);
                }
            }

            if (!string.IsNullOrWhiteSpace(state.TempFolderPath) && Directory.Exists(state.TempFolderPath))
            {
                try
                {
                    Directory.Delete(state.TempFolderPath, true);
                }
                catch
                {
                }
            }
        }

        public static InscriptionWizardState Create(Candidat candidat)
        {
            return new InscriptionWizardState
            {
                CandidatId = candidat.Id,
                CNE = candidat.CNE,
                Nom = candidat.Nom,
                Prenom = candidat.Prenom,
                Email = candidat.Email,
                Telephone = candidat.Telephone,
                CurrentStep = 1,
                LastUpdatedUtc = DateTime.UtcNow
            };
        }
    }
}
