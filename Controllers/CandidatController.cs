using InscriptionEtudiant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InscriptionEtudiant.Controllers
{
    [Authorize(Roles = "Candidat")]
    public class CandidatController : Controller
    {
        private readonly ICandidatDashboardService _dashboardService;
        private readonly IInscriptionService _inscriptionService;

        public CandidatController(
            ICandidatDashboardService dashboardService,
            IInscriptionService inscriptionService)
        {
            _dashboardService = dashboardService;
            _inscriptionService = inscriptionService;
        }

        public async Task<IActionResult> Index()
        {
            var candidat = await _inscriptionService.GetCurrentEtudiantAsync();
            if (candidat == null)
            {
                return Challenge();
            }

            var model = await _dashboardService.GetDashboardAsync(candidat.Id);
            return View(model);
        }
    }
}
