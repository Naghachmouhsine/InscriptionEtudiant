using InscriptionEtudiant.Data;
using InscriptionEtudiant.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);


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


builder.Services.AddScoped<AuthServiceInt, AuthService>();


var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);


app.Run();