namespace Firmness.Infrastructure.Repositories;
namespace Firmness.Infrastructure.Repositories;

using Firmness.Domain.Interfaces;
using Firmness.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

/// <summary>
/// Generic repository implementation for basic CRUD operations.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    /// <summary>
    /// Retrieves all entities with optional related data.
    /// </summary>
    /// <param name="includes">Navigation properties to include.</param>
    /// <returns>A collection of entities.</returns>
    public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.ToListAsync();
    }

    /// <summary>
    /// Retrieves an entity by its identifier with optional related data.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="includes">Navigation properties to include.</param>
    /// <returns>The entity if found, otherwise null.</returns>
    public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
    }

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The added entity.</returns>
    public async Task<T> AddAsync(T entity)
    {
       await _dbSet.AddAsync(entity);
       return entity;
    }

    /// <summary>
    /// Adds a range of entities to the repository.
    /// </summary>
    /// <param name="entities">The collection of entities to add.</param>
    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to delete.</param>
    public async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }
    
    /// <summary>
    /// Checks if an entity exists by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <returns>True if the entity exists, otherwise false.</returns>
    public async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet.AnyAsync(e => EF.Property<int>(e, "Id") == id);
    }
    
    /// <summary>
    /// Retrieves the first entity matching a predicate.
    /// </summary>
    /// <param name="predicate">The condition to filter entities.</param>
    /// <param name="includes">Navigation properties to include.</param>
    /// <returns>The matching entity if found, otherwise null.</returns>
    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.FirstOrDefaultAsync(predicate);
    }

    /// <summary>
    /// Retrieves entities matching a predicate.
    /// </summary>
    /// <param name="predicate">The condition to filter entities.</param>
    /// <param name="includes">Navigation properties to include.</param>
    /// <returns>A collection of matching entities.</returns>
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.Where(predicate).ToListAsync();
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}