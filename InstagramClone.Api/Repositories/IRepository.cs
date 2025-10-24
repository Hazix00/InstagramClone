namespace InstagramClone.Api.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync<TId>(TId id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync<TId>(TId id);
    Task<int> SaveChangesAsync();
}

