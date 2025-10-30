namespace Firmness.WebAdmin.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Firmness.Infrastructure.Data;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var port = Environment.GetEnvironmentVariable("DB_PORT");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var pass = Environment.GetEnvironmentVariable("DB_PASS");
        var name = Environment.GetEnvironmentVariable("DB_NAME");

        // PostgreSQL connection string format
        var connectionString = $"Host={host};Port={port};Database={name};Username={user};Password={pass};";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString)
        );

        return services;
    }
}
