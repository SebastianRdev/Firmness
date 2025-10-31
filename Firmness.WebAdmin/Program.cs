using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Firmness.Core.Entities;
using Firmness.Infrastructure.Services.Identity;
using Firmness.WebAdmin.Extensions;
using Firmness.Infrastructure.Data;
using Firmness.Infrastructure.Data.Seed;
using DotNetEnv;
using Firmness.Core.Interfaces;
using Firmness.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Le dice al framework que usara Identity con tu clase ApplicationUser y roles (IdentityRole)
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Opciones básicas de seguridad (puedes ajustarlas luego)
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>() // usa tu contexto actual - Le indica a Identity que guarde toda la información de usuarios en tu DB.
.AddDefaultTokenProviders(); // Añade mecanismos para generar tokens (por ejemplo, para restablecer contraseñas, confirmar emails, etc)

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Servicios de autenticación (ya lo registras en Program.cs, pero puedes centralizarlo aquí)
builder.Services.AddScoped<IAuthService, AuthService>();


// Add services to the container.
builder.Services.AddControllersWithViews();


// Add database
builder.Services.AddDatabase(builder.Configuration);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- Seeding y pruebas de conexión ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();

    try
    {
        db.Database.OpenConnection();
        Console.WriteLine("✅ Successful connection to the database");
        db.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error connecting to the database:");
        Console.WriteLine(ex.Message);
    }

    // Ejecutar Seeders
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

    await IdentitySeeder.SeedAsync(roleManager, userManager);
    await AdminSeed.SeedAdminsAsync(userManager, roleManager);
}


app.Run();