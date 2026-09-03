using System;
using System.Collections.Generic;
using System.Linq;
using InscriptionEtudiant.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InscriptionEtudiant.Infrastructure.Ui
{
    public static class InscriptionMessages
    {
        public const string SuccessDossierCreated = "Votre dossier d'inscription a bien ete cree. Vous pouvez maintenant completer les informations demandees.";
        public const string SuccessParcoursSaved = "Vos informations de parcours ont bien ete enregistrees. Vous pouvez passer a l'etape suivante.";
        public const string SuccessChoicesSaved = "Vos choix de filieres ont bien ete enregistres.";
        public const string SuccessDocumentsUploaded = "Vos documents ont bien ete ajoutes a votre dossier.";
        public const string SuccessDossierSubmitted = "Votre dossier a bien ete transmis pour validation.";

        public const string ProgressCreatingDossier = "Creation de votre dossier...";
        public const string ProgressSaving = "Enregistrement en cours...";
        public const string ProgressUploadingDocuments = "Televersement des documents...";

        public const string InfoNoFilieresAvailable = "Aucune filiere n'est disponible pour le moment.";
        public const string InfoNoDocumentsYet = "Aucun document n'a encore ete ajoute.";
        public const string InfoNoChoicesYet = "Choisissez jusqu'a trois filieres afin de poursuivre.";

        public const string WarningNoFilesSelected = "Aucun document n'a ete selectionne.";
        public const string WarningMaxChoices = "Vous pouvez selectionner jusqu'a trois filieres.";

        public const string ErrorSessionExpired = "Votre session a expire. Merci de vous reconnecter pour continuer.";
        public const string ErrorDossierMissing = "Nous n'avons pas retrouve votre dossier. Veuillez actualiser la page et recommencer.";
        public const string ErrorUnexpected = "Une erreur inattendue est survenue. Veuillez reessayer dans quelques instants.";
        public const string ErrorParcoursInvalid = "Veuillez verifier les informations de votre parcours academique.";
        public const string ErrorChoicesRange = "Veuillez selectionner entre une et trois filieres.";
        public const string ErrorChoicesUnavailable = "L'une des filieres choisies n'est plus disponible. Veuillez mettre a jour votre selection.";
        public const string ErrorFileTooLarge = "Le fichier selectionne depasse la taille autorisee. Veuillez choisir un fichier plus leger.";
        public const string ErrorFileTypeNotAllowed = "Le format du fichier selectionne n'est pas accepte. Utilisez un fichier PDF, JPG, PNG, DOC ou DOCX.";
        public const string ErrorNoFiles = "Ajoutez au moins un document pour poursuivre.";
        public const string ErrorSubmitIncomplete = "Votre dossier est incomplet. Completez les informations demandees avant la validation finale.";
        public const string ErrorDossierAlreadySubmitted = "Un dossier a deja ete finalise pour ce compte. Vous ne pouvez pas creer une nouvelle inscription.";
        public const string ConfirmSubmitDossier = "Vous etes sur le point de transmettre votre dossier pour validation. Souhaitez-vous continuer ?";

        public static string SuccessDocumentsUploadedCount(int count)
            => count <= 1
                ? "Votre document a bien ete ajoute a votre dossier."
                : $"Vos {count} documents ont bien ete ajoutes a votre dossier.";

        public static string FileTooLarge(string fileName)
            => $"Le fichier {fileName} depasse la taille autorisee. Veuillez le choisir a nouveau.";

        public static string FileTypeNotAllowed(string? extension)
            => string.IsNullOrWhiteSpace(extension)
                ? ErrorFileTypeNotAllowed
                : $"Le format {extension} n'est pas accepte. Utilisez un fichier PDF, JPG, PNG, DOC ou DOCX.";

        public static IReadOnlyList<string> BuildParcoursValidationMessages(ModelStateDictionary modelState)
        {
            var messages = new List<string>();

            foreach (var entry in modelState)
            {
                if (entry.Value.Errors.Count == 0)
                {
                    continue;
                }

                var fieldName = NormalizeFieldName(entry.Key);

                foreach (var error in entry.Value.Errors)
                {
                    messages.Add(MapValidationMessage(fieldName, error.ErrorMessage));
                }
            }

            if (messages.Count == 0)
            {
                messages.Add(ErrorParcoursInvalid);
            }

            return messages;
        }

        public static string GetFirstValidationField(ModelStateDictionary modelState)
        {
            foreach (var entry in modelState)
            {
                if (entry.Value.Errors.Count > 0)
                {
                    var fieldName = NormalizeFieldName(entry.Key);
                    if (!string.IsNullOrWhiteSpace(fieldName))
                    {
                        return fieldName;
                    }
                }
            }

            return string.Empty;
        }

        public static string FormatFriendlyException(string fallbackMessage, Exception exception)
        {
            return exception is InvalidOperationException ? exception.Message : fallbackMessage;
        }

        private static string NormalizeFieldName(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            var trimmed = key.Trim();
            var dotIndex = trimmed.LastIndexOf('.');
            return dotIndex >= 0 ? trimmed[(dotIndex + 1)..] : trimmed;
        }

        private static string MapValidationMessage(string fieldName, string rawError)
        {
            return fieldName switch
            {
                nameof(ParcoursAcademique.AnneeBac) => "L'annee du baccalaureat est obligatoire.",
                nameof(ParcoursAcademique.NoteNationale) => "La note nationale doit etre comprise entre 0 et 20.",
                nameof(ParcoursAcademique.NoteRegionale) => "La note regionale doit etre comprise entre 0 et 20.",
                nameof(ParcoursAcademique.SerieBacId) => "Veuillez selectionner une serie du baccalaureat.",
                nameof(ParcoursAcademique.MentionId) => "Veuillez selectionner une mention.",
                _ when string.Equals(rawError, "The value '' is invalid.", StringComparison.OrdinalIgnoreCase)
                    => "Veuillez verifier les informations saisies.",
                _ => ErrorParcoursInvalid
            };
        }
    }
}
