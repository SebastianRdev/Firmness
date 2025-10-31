namespace Firmness.Infrastructure.Repositories;

using Firmness.Core.Interfaces;
using Firmness.Infrastructure.Data;
using Firmness.Core.Entities;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _context;

    // Repositorios
    public IGenericRepository<Product> Products { get; }

    // Constructor
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Products = new GenericRepository<Product>(_context); // Inicializa el repositorio específico
    }

    // Guarda todos los cambios en la base de datos
    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    // Libera los recursos
    public void Dispose() => _context.Dispose();
}