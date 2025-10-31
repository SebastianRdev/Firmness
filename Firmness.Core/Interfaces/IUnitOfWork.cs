namespace Firmness.Core.Interfaces;

using Firmness.Core.Entities;
public interface IUnitOfWork : IDisposable
{
    // Propiedades de los repositorios
    IGenericRepository<Product> Products { get; }
    // Puedes agregar más repositorios específicos aquí, como Clientes, Ventas, etc.

    // Método para guardar los cambios de todas las entidades
    Task<int> CompleteAsync();
}