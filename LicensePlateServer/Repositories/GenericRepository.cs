using LicensePlateServer.Data;
using Microsoft.EntityFrameworkCore;

namespace LicensePlateServer.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly LicensePlateDbContext _context;

    public GenericRepository(LicensePlateDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    public async Task<T?> GetAsync(int? id)
    {
        if (id is null) return null;
        return await _context.Set<T>().FindAsync(id);
    }

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <returns>A list of all entities.</returns>
    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The added entity.</returns>
    public async Task<T> AddAsync(T entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateAsync(T entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if an entity exists by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <returns>True if the entity exists; otherwise, false.</returns>
    public async Task<bool> Exists(int id)
    {
        var entity = await GetAsync(id);
        return entity != null;
    }
}