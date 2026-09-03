using Microsoft.AspNetCore.Mvc;

namespace InscriptionEtudiant.Controllers.Admin
{
    /// <summary>
    /// Minimal AdminController used as landing dashboard for administrators.
    /// Kept intentionally small: only Index action returning the Admin dashboard view.
    /// </summary>
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
