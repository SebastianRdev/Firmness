using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Firmness.Domain.Entities;
using Firmness.Infrastructure.Services.Identity;
using Firmness.WebAdmin.Extensions;
using Firmness.Infrastructure.Data;
using Firmness.Infrastructure.Data.Seed;
using DotNetEnv;
using Firmness.Infrastructure.Repositories;
using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Domain.Interfaces;
using Firmness.Application.Mappings;
using Firmness.WebAdmin.ApiClients;
using FluentValidation;
using Firmness.WebAdmin.Validators.Customers;
using Firmness.WebAdmin.Validators.Categories;
using Firmness.WebAdmin.Validators.Products;
using FluentValidation.AspNetCore;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information); 

// Excel
ExcelPackage.License.SetNonCommercialOrganization("Firmness.WebAdmin");

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

Env.Load();

// Tells the framework to use Identity with your ApplicationUser class and roles (IdentityRole)
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Basic security options
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>() // Use your current context - Tells Identity to store all user information in your DB.
.AddDefaultTokenProviders(); // Add mechanisms to generate tokens (e.g., to reset passwords, confirm emails, etc.)

builder.Services.AddScoped<IAuthService, AuthService>();

// AutoMapper
builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

// Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Application Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IExcelService, ExcelService>();

// Authentication services (you already register this in Program.cs, but you can centralize it here)
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();


// Add services to the container.
builder.Services.AddControllersWithViews();

// Add database
builder.Services.AddDatabase(builder.Configuration);

// Consume the API
var apiBase = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5264/api/";

builder.Services.AddHttpClient<IProductApiClient, ProductApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBase);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    // Create a new HttpClientHandler to disable SSL validation in development
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // Disable SSL validation
    return handler;
});

builder.Services.AddHttpClient<ICategoryApiClient, CategoryApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBase);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    // Create a new HttpClientHandler to disable SSL validation in development
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // Disable SSL validation
    return handler;
});

builder.Services.AddHttpClient<ICustomerApiClient, CustomerApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBase);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    // Create a new HttpClientHandler to disable SSL validation in development
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // Disable SSL validation
    return handler;
});

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

// --- Seeding and connection testing ---
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