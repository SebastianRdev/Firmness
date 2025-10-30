namespace Firmness.Test;

using Xunit;
using Microsoft.EntityFrameworkCore;
using Firmness.Infrastructure.Data;
using DotNetEnv;

public class DatabaseConnectionTests
{
    [Fact]
    public void CanConnectToDatabase()
    {
        Env.Load("/home/Coder/Música/sebas/Firmness/Firmness.WebAdmin/.env");

        var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                               $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                               $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                               $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                               $"Password={Environment.GetEnvironmentVariable("DB_PASS")}";
        
        Console.WriteLine($"DB_HOST: {Environment.GetEnvironmentVariable("DB_HOST")}");
        Console.WriteLine($"DB_NAME: {Environment.GetEnvironmentVariable("DB_NAME")}");


        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        using var context = new ApplicationDbContext(options);

        Assert.True(context.Database.CanConnect(), "No se pudo conectar a la base de datos");
    }
}