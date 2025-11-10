using System.Linq.Expressions;

namespace Firmness.Domain.Interfaces;


/// <summary>
/// Repositorio genérico con operaciones CRUD básicas.
/// Abstrae el acceso a datos sin exponer detalles de EF Core.
/// </summary>
/// <typeparam name="T">Tipo de entidad del dominio</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Obtiene todas las entidades de la tabla.
    /// </summary>
    /// <returns>Colección de entidades</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Obtiene una entidad por su ID.
    /// </summary>
    /// <param name="id">ID de la entidad</param>
    /// <returns>Entidad encontrada o null si no existe</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Agrega una nueva entidad al contexto.
    /// No persiste hasta llamar SaveChangesAsync().
    /// </summary>
    /// <param name="entity">Entidad a agregar</param>
    /// <returns>La entidad agregada</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Marca una entidad como modificada.
    /// No persiste hasta llamar SaveChangesAsync().
    /// </summary>
    /// <param name="entity">Entidad a actualizar</param>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Elimina una entidad por su ID.
    /// No persiste hasta llamar SaveChangesAsync().
    /// </summary>
    /// <param name="id">ID de la entidad a eliminar</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Verifica si existe una entidad con el ID especificado.
    /// </summary>
    /// <param name="id">ID a verificar</param>
    /// <returns>True si existe, false si no</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Persiste todos los cambios pendientes en la base de datos.
    /// </summary>
    /// <returns>Número de registros afectados</returns>
    Task<int> SaveChangesAsync();
    
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
}