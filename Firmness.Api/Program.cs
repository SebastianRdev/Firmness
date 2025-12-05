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
using Firmness.Infrastructure.Services;
using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Infrastructure.Services.Gemini;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Firmness.Application.Configuration;
using Firmness.Infrastructure.Services.Identity;
using Firmness.Infrastructure.Services.Email;
using Google.GenAI;

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
// 3.1 QUESTPDF LICENSE
// ==========================================
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

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
// 6.1 JWT AUTHENTICATION
// ==========================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "DefaultKeyMustBeLongEnough123456789"))
    };
});

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
// ==========================================
// 11.1 REGISTER GEMINI SERVICE
// ==========================================
builder.Services.AddScoped<IGeminiService, GeminiService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<ICustomerSaleService, CustomerSaleService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IReceiptPdfService, ReceiptPdfService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IImportService, ImportService>();


// ==========================================
// 11.1 EMAIL CONFIGURATION
// ==========================================
builder.Services.Configure<EmailSettings>(options =>
{
    options.SmtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com";
    options.SmtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
    options.SenderEmail = Environment.GetEnvironmentVariable("SMTP_SENDER_EMAIL") ?? "";
    options.SenderName = Environment.GetEnvironmentVariable("SMTP_SENDER_NAME") ?? "Firmness";
    options.Username = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "";
    options.Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";
    options.EnableSsl = bool.Parse(Environment.GetEnvironmentVariable("SMTP_ENABLE_SSL") ?? "true");
    options.AdminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "";
});

// ==========================================
// 11.2 GEMINI .NET OFFICIAL CLIENT
// ==========================================
builder.Services.AddSingleton(sp =>
{
    var apiKey = builder.Configuration["GEMINI_API_KEY"]
                 ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");
                 //?? Environment.GetEnvironmentVariable("GOOGLE_API_KEY");  fallback

    // Si apiKey viene null/empty, construimos el cliente sin parámetros
    // (el SDK leerá la variable de entorno interna).
    return string.IsNullOrWhiteSpace(apiKey)
        ? new Google.GenAI.Client()
        : new Google.GenAI.Client(apiKey: apiKey);
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

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
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
// CREATE RECEIPTS DIRECTORY
// ==========================================
var receiptsPath = Path.Combine(app.Environment.WebRootPath, "receipts");
if (!Directory.Exists(receiptsPath))
{
    Directory.CreateDirectory(receiptsPath);
    Console.WriteLine("✅ Receipts directory created");
}

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

// Disable HTTPS redirection in development to avoid CORS preflight issues
// app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
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
        db.Database.Migrate();
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

// Después de builder.Build() y antes de app.Run()
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

    // Ejecuta el seed (solo en entorno Development)
    if (app.Environment.IsDevelopment())
    {
        await Firmness.Infrastructure.Data.Seed.AdminSeed.SeedAdminsAsync(userManager, roleManager);
        Console.WriteLine("✅ Admin seed executed");
    }
}

// ==========================================
// RUN APPLICATION
// ==========================================
app.Run();

public partial class Program { }
