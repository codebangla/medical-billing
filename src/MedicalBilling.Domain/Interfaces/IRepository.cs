using System.Linq.Expressions;

namespace MedicalBilling.Domain.Interfaces;

/// <summary>
/// Generic repository interface for data access
/// Provides common CRUD operations with async support
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Get entity by ID
    /// </summary>
    Task<T?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get all entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();
    
    /// <summary>
    /// Get paged results with optional filtering
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="filter">Optional filter expression</param>
    /// <param name="orderBy">Optional ordering function</param>
    /// <returns>Paged results</returns>
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
    
    /// <summary>
    /// Find entities matching a predicate
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// Add new entity
    /// </summary>
    Task<T> AddAsync(T entity);
    
    /// <summary>
    /// Update existing entity
    /// </summary>
    Task UpdateAsync(T entity);
    
    /// <summary>
    /// Delete entity by ID
    /// </summary>
    Task DeleteAsync(int id);
    
    /// <summary>
    /// Check if entity exists
    /// </summary>
    Task<bool> ExistsAsync(int id);
    
    /// <summary>
    /// Count entities matching predicate
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}
