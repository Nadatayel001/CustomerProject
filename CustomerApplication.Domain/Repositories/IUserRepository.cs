using CustomerApplication.CustomerApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerApplication.CustomerApplication.Domain.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByIdWithRoleAsync(Guid id);
        Task<User?> GetByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetAllWithRolesAsync();
        Task DeleteAsync(Guid id);

        Task SaveChangesAsync();
        Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null);
       

    }
}
