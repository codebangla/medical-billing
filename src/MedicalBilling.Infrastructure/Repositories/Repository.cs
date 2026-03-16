using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MedicalBilling.Domain.Interfaces;
using MedicalBilling.Infrastructure.Data;

namespace MedicalBilling.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation with EF Core
/// Optimized for performance with async operations and no-tracking queries
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public Repository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }
    
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }
    
    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        // Validate parameters
        if (pageNumber < 1) throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));
        if (pageSize < 1 || pageSize > 1000) throw new ArgumentException("Page size must be between 1 and 1000", nameof(pageSize));
        
        IQueryable<T> query = _dbSet.AsNoTracking();
        
        // Apply filter
        if (filter != null)
        {
            query = query.Where(filter);
        }
        
        // Get total count before paging
        var totalCount = await query.CountAsync();
        
        // Apply ordering
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        
        // Apply paging
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (items, totalCount);
    }
    
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }
    
    public virtual async Task<T> AddAsync(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public virtual async Task UpdateAsync(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
    
    public virtual async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Entity with ID {id} not found");
        }
        
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
    
    public virtual async Task<bool> ExistsAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity != null;
    }
    
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
        {
            return await _dbSet.CountAsync();
        }
        
        return await _dbSet.CountAsync(predicate);
    }
}
