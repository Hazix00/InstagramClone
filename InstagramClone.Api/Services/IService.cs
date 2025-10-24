namespace InstagramClone.Api.Services;

public interface IService<T> where T : class
{
    Task<T?> GetByIdAsync<TId>(TId id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync<TId>(TId id);
    Task<bool> ExistsAsync<TId>(TId id);
}

