using System.Collections.Generic;
using System.Threading.Tasks;
using InscriptionEtudiant.Infrastructure;
using InscriptionEtudiant.Models;

namespace InscriptionEtudiant.Services.Interfaces
{
    public interface IInscriptionService
    {
        Task<Candidat?> GetCurrentEtudiantAsync();

        Task<Administrateur?> GetCurrentAdminAsync();
       

        Task<IEnumerable<Filiere>> GetFilieresDisponiblesAsync();

        Task<IEnumerable<SerieBac>> GetSeriesBacAsync();

        Task<IEnumerable<Mention>> GetMentionsAsync();

        Task SaveParcoursAsync(int dossierId, ParcoursAcademique parcours);

        Task SaveChoixFilieresAsync(int dossierId, IEnumerable<ChoixFiliere> choix);

        Task<IEnumerable<Document>> UploadDocumentsAsync(int wizardKey, IEnumerable<(string FieldName, byte[] Content, string FileName)> files);

        Task SubmitDossierAsync(Candidat candidat, InscriptionWizardState state);

    }
}
