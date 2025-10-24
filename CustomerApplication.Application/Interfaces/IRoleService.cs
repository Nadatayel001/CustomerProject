using CustomerApplication.CustomerApplication.Application.DTOs.Role.Commands.CreateOrUpdate;
using CustomerApplication.CustomerApplication.Domain.Entities;

public interface IRoleService
{
    Task<Guid> CreateAsync(CreateOrUpdateRoleCommand command);
    Task<Role> GetByIdAsync(Guid id);
    Task<IEnumerable<Role>> GetListAsync();
    Task DeleteAsync(Guid id);
}
