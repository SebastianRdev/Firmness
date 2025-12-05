using System.Linq.Expressions;

namespace Firmness.Domain.Interfaces;


/// <summary>
/// Generic repository with basic CRUD operations.
/// Abstract data access without exposing EF Core details.
/// </summary>
/// <typeparam name="T">Domain Entity Type</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// It retrieves all entities from the table.
    /// </summary>
    /// <returns>Collection of entities</returns>
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Retrieve an entity by its ID.
    /// </summary>
    /// <param name="id">Entity ID</param>
    /// <returns>Entity found or null if it does not exist</returns>
    Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Add a new entity to the context.
    /// It does not persist until SaveChangesAsync() is called.
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <returns>The aggregated entity</returns>
    Task<T> AddAsync(T entity);

    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Mark an entity as modified.
    /// It does not persist until SaveChangesAsync() is called.
    /// </summary>
    /// <param name="entity">Entity to be updated</param>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Remove an entity by its ID.
    /// It does not persist until SaveChangesAsync() is called.
    /// </summary>
    /// <param name="id">ID of the entity to be deleted</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Check if an entity with the specified ID exists.
    /// </summary>
    /// <param name="id">ID to verify</param>
    /// <returns>True if it exists, false if it doesn't</returns>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// It persists all pending changes in the database.
    /// </summary>
    /// <returns>Number of records affected</returns>
    Task<int> SaveChangesAsync();
    
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
}