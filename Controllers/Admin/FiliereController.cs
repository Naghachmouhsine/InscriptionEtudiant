using System;
using System.Threading.Tasks;
using InscriptionEtudiant.Models;
using InscriptionEtudiant.Services.Interfaces;
using InscriptionEtudiant.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InscriptionEtudiant.Controllers.Admin
{
    [Route("Admin/[controller]/[action]")]
    public class FiliereController : Controller
    {
        private readonly IFiliereService _service;

        public FiliereController(IFiliereService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string? search)
        {
            ViewData["Title"] = "Liste des filières";
            var items = await _service.GetAllAsync(search);
            ViewData["Search"] = search;
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Ajouter une filière";
            var vm = new FiliereFormViewModel
            {
                DateDebutInscription = DateTime.Today,
                DateFinInscription = DateTime.Today.AddMonths(1)
            };
            return View("Form", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FiliereFormViewModel vm)
        {
            ViewData["Title"] = "Ajouter une filière";
            if (!ModelState.IsValid)
                return View("Form", vm);

            try
            {
                var entity = new Filiere
                {
                    Nom = vm.Nom,
                    Description = vm.Description,
                    Capacite = vm.Capacite,
                    DateDebutInscription = vm.DateDebutInscription.Date,
                    DateFinInscription = vm.DateFinInscription.Date
                };

                await _service.CreateAsync(entity);
                TempData["Success"] = "Filière créée avec succès.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Une erreur est survenue lors de la création.");
                // In production log the exception using ILogger (not added here to keep code concise)
                return View("Form", vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var vm = new FiliereFormViewModel
            {
                Id = entity.Id,
                Nom = entity.Nom,
                Description = entity.Description,
                Capacite = entity.Capacite,
                DateDebutInscription = entity.DateDebutInscription,
                DateFinInscription = entity.DateFinInscription
            };

            ViewData["Title"] = "Modifier une filière";
            return View("Form", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FiliereFormViewModel vm)
        {
            ViewData["Title"] = "Modifier une filière";
            if (!ModelState.IsValid)
                return View("Form", vm);

            try
            {
                if (!await _service.ExistsAsync(vm.Id))
                    return NotFound();

                var entity = new Filiere
                {
                    Id = vm.Id,
                    Nom = vm.Nom,
                    Description = vm.Description,
                    Capacite = vm.Capacite,
                    DateDebutInscription = vm.DateDebutInscription.Date,
                    DateFinInscription = vm.DateFinInscription.Date
                };

                await _service.UpdateAsync(entity);
                TempData["Success"] = "Filière mise à jour.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Une erreur est survenue lors de la mise à jour.");
                return View("Form", vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                TempData["Success"] = "Filière supprimée.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Impossible de supprimer la filière.";
            }

            return RedirectToAction("Index");
        }
    }
}
