using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Firmness.Domain.Entities;
using Firmness.Infrastructure.Services.Identity;
using Firmness.WebAdmin.Extensions;
using Firmness.Infrastructure.Data;
using Firmness.Infrastructure.Data.Seed;
using DotNetEnv;
using Firmness.Application.Interfaces;
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

Env.Load();

// ==========================================
// IDENTITY CONFIGURATION
// ==========================================
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthService, AuthService>();

// ==========================================
// AUTOMAPPER
// ==========================================
builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

// ==========================================
// VALIDATORS
// ==========================================
builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// ==========================================
// MVC
// ==========================================
builder.Services.AddControllersWithViews();

// ==========================================
// DATABASE
// ==========================================
builder.Services.AddDatabase(builder.Configuration);

// ==========================================
// API CLIENTS (en lugar de servicios directos)
// ==========================================
var apiBase = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5264/api/";


// API CLIENTS
builder.Services.AddHttpClient<IProductApiClient, ProductApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBase);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = 
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
    return handler;
});

builder.Services.AddHttpClient<ICategoryApiClient, CategoryApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBase);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = 
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
    return handler;
});

builder.Services.AddHttpClient<ICustomerApiClient, CustomerApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBase);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = 
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
    return handler;
});

// ==========================================
// AI SERVICES
// ==========================================
builder.Services.AddSingleton(sp =>
{
    var apiKey = builder.Configuration["GEMINI_API_KEY"]
                 ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");

    return string.IsNullOrWhiteSpace(apiKey)
        ? new Google.GenAI.Client()
        : new Google.GenAI.Client(apiKey: apiKey);
});

builder.Services.AddScoped<ISqlAgentService, Firmness.Infrastructure.Services.AI.SqlAgentService>();

var app = builder.Build();

// ==========================================
// MIDDLEWARE PIPELINE
// ==========================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

// ==========================================
// DATABASE SEEDING
// ==========================================
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

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

    await IdentitySeeder.SeedAsync(roleManager, userManager);
    await AdminSeed.SeedAdminsAsync(userManager, roleManager);
}

app.Run();