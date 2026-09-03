using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InscriptionEtudiant.Models;
using InscriptionEtudiant.Services.Interfaces;
using InscriptionEtudiant.ViewModels.Admin;
using InscriptionEtudiant.Services;

namespace InscriptionEtudiant.Controllers
{

    [Route("Admin/[controller]")]
    public class DossiersController : Controller
    {
        private readonly IDossierAdminService _service;
        private readonly IInscriptionService _inscriptionService;
        private readonly ILogger<DossiersController> _logger;

        public DossiersController(IDossierAdminService service, IInscriptionService inscriptionService, ILogger<DossiersController> logger)
        {
            _service = service;
            _inscriptionService = inscriptionService;
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(StatutDossier? statut, string? recherche)
        {
            var model = await _service.GetDossiersAsync(statut, recherche);
            return View("~/Views/Admin/Dossiers/Index.cshtml", model);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _service.GetDossierDetailsAsync(id);
            if (model == null)
                return NotFound();

            return View("~/Views/Admin/Dossiers/Details.cshtml", model);
        }

        [HttpPost("ChangerStatut")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangerStatut(ChangerStatutViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Données invalides.";
                return RedirectToAction("Details", new { id = vm.DossierId });
            }

            // get administrator id from claims (assumes NameIdentifier contains integer id)

            var administrateurId=0;
            Administrateur administrateur = await _inscriptionService.GetCurrentAdminAsync();
            _logger.LogInformation("Récupération de l'admin connecté controller  : " + administrateur?.Id + " - " + administrateur?.Nom);
            if (administrateur == null)
            {
                TempData["Error"] = "Administrateur non trouvé.";
                return RedirectToAction("Details", new { id = vm.DossierId });
            }
            else
            {
                administrateurId = administrateur.Id;
            }

                // If accepting, ensure a filiere is selected
                if (vm.NouveauStatut == StatutDossier.Accepte && !vm.FiliereAffecteeId.HasValue)
            {
                TempData["Error"] = "Veuillez sélectionner la filière d'affectation avant d'accepter le dossier.";
                return RedirectToAction("Details", new { id = vm.DossierId });
            }

            try
            {
                await _service.ChangerStatutAsync(vm.DossierId, vm.NouveauStatut, vm.Commentaire ?? string.Empty, administrateurId, vm.FiliereAffecteeId);
                TempData["Success"] = "Le statut du dossier a été mis à jour.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message ?? "Une erreur est survenue lors du changement de statut.";
            }

            return RedirectToAction("Details", new { id = vm.DossierId });
        }
    }
}
