using InscriptionEtudiant.Data;
using InscriptionEtudiant.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.AddConsole();   // Affiche dans la fenêtre "Output" ou la console
builder.Logging.AddDebug();     // Affiche dans la fenêtre "Output > Debug" de Visual Studio



// Configuration Entity Framework Core + SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);


builder.Services.AddAuthentication(
   CookieAuthenticationDefaults.AuthenticationScheme)
   .AddCookie(options =>
   {
       options.LoginPath = "/Auth/Login";
       options.AccessDeniedPath = "/Auth/AccessDenied";
   });

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddScoped<AuthServiceInt, AuthService>();
builder.Services.AddScoped<InscriptionEtudiant.Services.Interfaces.IFiliereService, InscriptionEtudiant.Services.FiliereService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<InscriptionEtudiant.Services.Interfaces.IInscriptionService, InscriptionEtudiant.Services.InscriptionService>();
builder.Services.AddScoped<InscriptionEtudiant.Services.Interfaces.ICandidatDashboardService, InscriptionEtudiant.Services.CandidatDashboardService>();
builder.Services.AddScoped<InscriptionEtudiant.Services.Interfaces.IDossierAdminService, InscriptionEtudiant.Services.DossierAdminService>();


var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}"
);


app.Run();
