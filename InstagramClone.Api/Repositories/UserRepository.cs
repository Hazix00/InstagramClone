using InstagramClone.Api.Data;
using InstagramClone.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InstagramClone.Api.Repositories;

public class UserRepository(ApplicationDbContext context) : Repository<User>(context), IUserRepository
{

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> ExistsByUsernameAsync(string username) =>
        await _dbSet.AnyAsync(u => u.Username == username);

    public async Task<bool> ExistsByEmailAsync(string email) =>
        await _dbSet.AnyAsync(u => u.Email == email);
}

