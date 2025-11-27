using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using OfficeOpenXml;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Firmness.Infrastructure.Data;
using Firmness.Infrastructure.Repositories;
using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Infrastructure.Services.Gemini;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. LOAD .env FILE FIRST
// ==========================================
Env.Load();
builder.Configuration.AddEnvironmentVariables();

// ==========================================
// 2. LOGGING
// ==========================================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// ==========================================
// 3. EXCEL LICENSE
// ==========================================
ExcelPackage.License.SetNonCommercialOrganization("Firmness.Api");

// ==========================================
// 4. VERIFY GEMINI API KEY (Development only)
// ==========================================
if (builder.Environment.IsDevelopment())
{
    var apiKey = builder.Configuration["GEMINI_API_KEY"] 
                 ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");
    
    if (string.IsNullOrEmpty(apiKey))
    {
        Console.WriteLine("⚠️ WARNING: GEMINI_API_KEY not found in .env file");
    }
    else
    {
        Console.WriteLine($"✅ GEMINI_API_KEY loaded: {apiKey[..10]}...{apiKey[^4..]}");
    }
}

// ==========================================
// 5. DATABASE CONNECTION STRING
// ==========================================
var host = Environment.GetEnvironmentVariable("DB_HOST");
var port = Environment.GetEnvironmentVariable("DB_PORT");
var user = Environment.GetEnvironmentVariable("DB_USER");
var pass = Environment.GetEnvironmentVariable("DB_PASS");
var name = Environment.GetEnvironmentVariable("DB_NAME");

var connectionString = $"Host={host};Port={port};Database={name};Username={user};Password={pass};";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// ==========================================
// 6. IDENTITY CONFIGURATION
// ==========================================
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredUniqueChars = 0;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ==========================================
// 7. CORS CONFIGURATION
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ==========================================
// 8. AUTOMAPPER
// ==========================================
builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

// ==========================================
// 9. REPOSITORIES
// ==========================================
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// ==========================================
// 10. APPLICATION SERVICES
// ==========================================
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IExcelService, ExcelService>();

// ==========================================
// 11. GEMINI AI SERVICE (HttpClient configurado)
// ⚠️ IMPORTANTE: Solo una vez, no duplicado
// ==========================================
builder.Services.AddHttpClient<IGeminiService, GeminiApiClient>((serviceProvider, client) =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "FirmnessApp/1.0");
});

// ==========================================
// 12. CONTROLLERS
// ==========================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// ==========================================
// 13. SWAGGER
// ==========================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Firmness API",
        Version = "v1",
        Description = "API for Firmness Inventory Management System"
    });
    
    // Optional: Include XML comments if you have them
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // c.IncludeXmlComments(xmlPath);
});

// ==========================================
// 14. DISABLE SSL VALIDATION (Development only)
// ==========================================
if (builder.Environment.IsDevelopment())
{
    ServicePointManager.ServerCertificateValidationCallback = 
        (sender, certificate, chain, sslPolicyErrors) => true;
}

// ==========================================
// BUILD APPLICATION
// ==========================================
var app = builder.Build();

// ==========================================
// 15. MIDDLEWARE PIPELINE
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Firmness API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication(); // ✅ Agregar esto si usas Identity
app.UseAuthorization();
app.MapControllers();

// ==========================================
// 16. DATABASE CONNECTION TEST
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    
    try
    {
        db.Database.OpenConnection();
        Console.WriteLine("✅ API connected to PostgreSQL successfully");
        db.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database connection error: {ex.Message}");
    }
    
    // Optional: Test Gemini Service
    try
    {
        var geminiService = services.GetRequiredService<IGeminiService>();
        Console.WriteLine("✅ Gemini Service registered successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Gemini Service registration issue: {ex.Message}");
    }
}

// ==========================================
// RUN APPLICATION
// ==========================================
app.Run();