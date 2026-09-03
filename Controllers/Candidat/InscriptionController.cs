using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using InscriptionEtudiant.Infrastructure;
using InscriptionEtudiant.Infrastructure.Ui;
using InscriptionEtudiant.Models;
using InscriptionEtudiant.Services.Interfaces;
using InscriptionEtudiant.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InscriptionEtudiant.Controllers.Candidat
{
    [Authorize(Roles = "Candidat")]
    [Route("Candidat/[controller]/[action]")]
    public class InscriptionController : Controller
    {
        private readonly IInscriptionService _service;

        public InscriptionController(IInscriptionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int step = 1)
        {
            var candidat = await _service.GetCurrentEtudiantAsync();
            if (candidat == null)
            {
                return Challenge();
            }

            var state = InscriptionWizardSessionStore.GetOrCreate(HttpContext.Session, candidat);
            state.CurrentStep = step > 0 ? step : state.CurrentStep;
            InscriptionWizardSessionStore.Save(HttpContext.Session, state);

            var vm = new CreateInscriptionViewModel
            {
                CandidatId = candidat.Id,
                DossierId = candidat.Id,
                CNE = state.CNE,
                Nom = state.Nom,
                Prenom = state.Prenom,
                Email = state.Email,
                Telephone = state.Telephone,
                Parcours = new ParcoursAcademiqueViewModel
                {
                    AnneeBac = state.Parcours.AnneeBac,
                    SerieBacId = state.Parcours.SerieBacId,
                    MentionId = state.Parcours.MentionId,
                    NoteNationale = state.Parcours.NoteNationale,
                    NoteRegionale = state.Parcours.NoteRegionale
                },
                FilieresDisponibles = await _service.GetFilieresDisponiblesAsync(),
                ChoixFilieres = state.ChoixFilieres
                    .OrderBy(choice => choice.OrdreChoix)
                    .Select(choice => new ChoixFiliereViewModel
                    {
                        OrdreChoix = choice.OrdreChoix,
                        FiliereId = choice.FiliereId
                    })
                    .ToList()
            };

            ViewBag.SeriesBac = new SelectList(await _service.GetSeriesBacAsync(), "Id", "Nom", state.Parcours.SerieBacId);
            ViewBag.Mentions = new SelectList(await _service.GetMentionsAsync(), "Id", "Nom", state.Parcours.MentionId);
            ViewBag.WizardStateJson = JsonSerializer.Serialize(state, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            ViewBag.WizardState = state;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            var candidat = await _service.GetCurrentEtudiantAsync();
            if (candidat == null)
            {
                return Challenge();
            }

            InscriptionWizardSessionStore.GetOrCreate(HttpContext.Session, candidat);

            if (IsAjaxRequest())
            {
                return Json(new { success = true, nextStep = 1 });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveParcours(int dossierId, ParcoursAcademique parcours)
        {
            var candidat = await _service.GetCurrentEtudiantAsync();
            if (candidat == null)
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                return StepFailure(2, InscriptionMessages.BuildParcoursValidationMessages(ModelState), InscriptionMessages.GetFirstValidationField(ModelState));
            }

            try
            {
                await _service.SaveParcoursAsync(dossierId, parcours);
            }
            catch (System.InvalidOperationException ex)
            {
                return StepFailure(2, new[] { ex.Message });
            }

            var state = InscriptionWizardSessionStore.GetOrCreate(HttpContext.Session, candidat);
            state.Parcours = new InscriptionParcoursState
            {
                AnneeBac = parcours.AnneeBac,
                SerieBacId = parcours.SerieBacId,
                MentionId = parcours.MentionId,
                NoteNationale = parcours.NoteNationale,
                NoteRegionale = parcours.NoteRegionale
            };
            state.CurrentStep = 3;
            InscriptionWizardSessionStore.Save(HttpContext.Session, state);

            return StepSuccess(3, InscriptionMessages.SuccessParcoursSaved);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveChoix(int dossierId, CreateInscriptionViewModel model)
        {
            var candidat = await _service.GetCurrentEtudiantAsync();
            if (candidat == null)
            {
                return Challenge();
            }

            if (model?.ChoixFilieres == null || model.ChoixFilieres.Count < 1 || model.ChoixFilieres.Count > 3)
            {
                return StepFailure(3, new[] { InscriptionMessages.ErrorChoicesRange }, "choicesList");
            }

            var choix = model.ChoixFilieres
                .Select(choice => new ChoixFiliere
                {
                    OrdreChoix = choice.OrdreChoix,
                    FiliereId = choice.FiliereId
                })
                .ToList();

            try
            {
                await _service.SaveChoixFilieresAsync(dossierId, choix);
            }
            catch (System.InvalidOperationException ex)
            {
                return StepFailure(3, new[] { ex.Message }, "choicesList");
            }

            var state = InscriptionWizardSessionStore.GetOrCreate(HttpContext.Session, candidat);
            state.ChoixFilieres = choix
                .OrderBy(choice => choice.OrdreChoix)
                .Select(choice => new InscriptionChoiceState
                {
                    OrdreChoix = choice.OrdreChoix,
                    FiliereId = choice.FiliereId
                })
                .ToList();
            state.CurrentStep = 4;
            InscriptionWizardSessionStore.Save(HttpContext.Session, state);

            return StepSuccess(4, InscriptionMessages.SuccessChoicesSaved);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocuments(int dossierId, IFormFile[] files)
        {
            var candidat = await _service.GetCurrentEtudiantAsync();
            if (candidat == null)
            {
                return Challenge();
            }

            if (files == null || files.Length == 0)
            {
                return StepFailure(4, new[] { InscriptionMessages.WarningNoFilesSelected });
            }

            var filePayloads = new System.Collections.Generic.List<(string FieldName, byte[] Content, string FileName)>();
            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    continue;
                }

                using var memory = new MemoryStream();
                await file.CopyToAsync(memory);
                filePayloads.Add((file.Name, memory.ToArray(), file.FileName));
            }

            if (filePayloads.Count == 0)
            {
                return StepFailure(4, new[] { InscriptionMessages.WarningNoFilesSelected });
            }

            IEnumerable<Document> preparedDocuments;
            try
            {
                preparedDocuments = await _service.UploadDocumentsAsync(candidat.Id, filePayloads);
            }
            catch (System.InvalidOperationException ex)
            {
                return StepFailure(4, new[] { ex.Message });
            }

            var state = InscriptionWizardSessionStore.GetOrCreate(HttpContext.Session, candidat);
            state.Documents = preparedDocuments
                .Select(document => new InscriptionDocumentState
                {
                    FieldName = document.TypeDocument,
                    FileName = document.NomFichier,
                    ContentType = string.Empty,
                    TempFilePath = document.Chemin,
                    Size = 0
                })
                .ToList();
            var firstTempFile = state.Documents.FirstOrDefault()?.TempFilePath;
            state.TempFolderPath = string.IsNullOrWhiteSpace(firstTempFile)
                ? string.Empty
                : Path.GetDirectoryName(firstTempFile) ?? string.Empty;
            state.CurrentStep = 5;
            InscriptionWizardSessionStore.Save(HttpContext.Session, state);

            return StepSuccess(5, InscriptionMessages.SuccessDocumentsUploadedCount(state.Documents.Count), new { documents = state.Documents });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int dossierId)
        {
            var candidat = await _service.GetCurrentEtudiantAsync();
            if (candidat == null)
            {
                return Challenge();
            }

            var state = InscriptionWizardSessionStore.Load(HttpContext.Session, candidat.Id);
            if (state == null)
            {
                return FailFinal(InscriptionMessages.ErrorSubmitIncomplete);
            }

            try
            {
                await _service.SubmitDossierAsync(candidat, state);
            }
            catch (System.InvalidOperationException ex)
            {
                return FailFinal(ex.Message);
            }

            InscriptionWizardSessionStore.DeleteTempFiles(state);
            InscriptionWizardSessionStore.Clear(HttpContext.Session, candidat.Id);
            TempData["Success"] = InscriptionMessages.SuccessDossierSubmitted;

            if (IsAjaxRequest())
            {
                return Json(new { success = true, message = InscriptionMessages.SuccessDossierSubmitted, redirect = Url.Action(nameof(Index)) });
            }

            return RedirectToAction(nameof(Index));
        }

        private IActionResult StepSuccess(int nextStep, string message, object? data = null)
        {
            if (IsAjaxRequest())
            {
                return data == null
                    ? Json(new { success = true, nextStep, message })
                    : Json(new { success = true, nextStep, message, data });
            }

            TempData["Success"] = message;
            return RedirectToAction(nameof(Index), new { step = nextStep });
        }

        private IActionResult StepFailure(int step, IEnumerable<string> messages, string? focus = null)
        {
            var payload = new
            {
                success = false,
                step,
                focus,
                messages = messages.ToArray()
            };

            if (IsAjaxRequest())
            {
                return BadRequest(payload);
            }

            TempData["ValidationErrors"] = string.Join("\n", messages);
            TempData["ValidationFocus"] = focus ?? string.Empty;
            return RedirectToAction(nameof(Index), new { step });
        }

        private IActionResult FailFinal(string message)
        {
            if (IsAjaxRequest())
            {
                return BadRequest(new { success = false, message });
            }

            TempData["Error"] = message;
            return RedirectToAction(nameof(Index), new { step = 6 });
        }

        private bool IsAjaxRequest()
        {
            return string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
