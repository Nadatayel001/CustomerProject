using CustomerApplication.CustomerApplication.Domain.Entities;

namespace CustomerApplication.CustomerApplication.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task AddAsync(Role role);
        Task<Role> GetByIdAsync(Guid id);
        Task<IEnumerable<Role>> GetAllAsync();
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
        Task<Role?> GetByNameAsync(string name);
    }
}
