using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DotNetEnv;

using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Firmness.Infrastructure.Data;
using Firmness.Infrastructure.Repositories;
using Firmness.Application.Interfaces;
using Firmness.Application.Services;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

var host = Environment.GetEnvironmentVariable("DB_HOST");
var port = Environment.GetEnvironmentVariable("DB_PORT");
var user = Environment.GetEnvironmentVariable("DB_USER");
var pass = Environment.GetEnvironmentVariable("DB_PASS");
var name = Environment.GetEnvironmentVariable("DB_NAME");

var connectionString = $"Host={host};Port={port};Database={name};Username={user};Password={pass};";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// ========================================
// 3. CONFIGURAR IDENTITY (por si acaso)
// ========================================
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ========================================
// 4. CONFIGURAR CORS (CRÍTICO)
// ========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Permite cualquier origen (solo para desarrollo)
              .AllowAnyMethod()   // GET, POST, PUT, DELETE
              .AllowAnyHeader();  // Authorization, Content-Type, etc.
    });
});

// ========================================
// 5. REGISTRAR SERVICIOS
// ========================================
builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IProductService, ProductService>();

// ========================================
// 6. CONFIGURAR CONTROLLERS
// ========================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// ========================================
// 7. SWAGGER
// ========================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ========================================
// 8. BUILD
// ========================================
var app = builder.Build();

// ========================================
// 9. MIDDLEWARE
// ========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll"); // ← IMPORTANTE: Antes de UseAuthorization
app.UseAuthorization();
app.MapControllers();

// ========================================
// 10. TEST DE CONEXIÓN
// ========================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        db.Database.OpenConnection();
        Console.WriteLine("✅ API conectada a PostgreSQL");
        db.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}");
    }
}

app.Run();
