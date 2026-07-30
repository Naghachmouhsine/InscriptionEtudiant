using InscriptionEtudiant.Models;
using InscriptionEtudiant.Services;
using InscriptionEtudiant.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InscriptionEtudiant.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthServiceInt _authService;


        public AuthController(AuthServiceInt authService)
        {
            _authService = authService;
        }


        // ============================
        // REGISTER GET
        // ============================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }



        // ============================
        // REGISTER POST
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var result = await _authService.RegisterCandidat(model);


            if (!result)
            {
                ModelState.AddModelError(
                    "",
                    "Email ou CNE déjà utilisé."
                );

                return View(model);
            }


            TempData["Success"] =
                "Compte créé avec succès. Connectez-vous.";

            return RedirectToAction("Login");
        }




        // ============================
        // LOGIN GET
        // ============================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }




        // ============================
        // LOGIN POST
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }



            // Chercher candidat
            var candidat =
                await _authService.LoginCandidat(model);



            if (candidat != null)
            {
                await CreateAuthenticationCookie(
                    candidat.Id.ToString(),
                    candidat.Email,
                    "Candidat"
                );


                return RedirectToAction(
                    "Index",
                    "Candidat"
                );
            }




            // Chercher administrateur
            var admin =
                await _authService.LoginAdmin(model);



            if (admin != null)
            {
                await CreateAuthenticationCookie(
                    admin.Id.ToString(),
                    admin.Email,
                    "Administrateur"
                );


                return RedirectToAction(
                    "Index",
                    "Admin"
                );
            }



            ModelState.AddModelError(
                "",
                "Email ou mot de passe incorrect."
            );


            return View(model);
        }





        // ============================
        // LOGOUT
        // ============================
        [HttpPost]
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );


            return RedirectToAction("Login");
        }





        // ============================
        // CREATION COOKIE
        // ============================
        private async Task CreateAuthenticationCookie(
            string id,
            string email,
            string role)
        {

            var claims = new List<Claim>
            {

                new Claim(
                    ClaimTypes.NameIdentifier,
                    id
                ),


                new Claim(
                    ClaimTypes.Email,
                    email
                ),


                new Claim(
                    ClaimTypes.Role,
                    role
                )
            };



            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );



            var principal = new ClaimsPrincipal(identity);



            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );
        }

    }
}