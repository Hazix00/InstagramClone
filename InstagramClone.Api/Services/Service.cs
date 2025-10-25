using InstagramClone.Api.Repositories;

namespace InstagramClone.Api.Services;

public class Service<T>(IRepository<T> repository) : IService<T> where T : class
{
    protected readonly IRepository<T> _repository = repository;

    public virtual async Task<T?> GetByIdAsync<TId>(TId id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();
        return entity;
    }

    public virtual async Task DeleteAsync<TId>(TId id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity != null)
        {
            await _repository.DeleteAsync(entity);
            await _repository.SaveChangesAsync();
        }
    }

    public virtual async Task<bool> ExistsAsync<TId>(TId id)
    {
        return await _repository.ExistsAsync(id);
    }
}

