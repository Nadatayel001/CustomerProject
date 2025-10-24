using AutoMapper;
using CustomerApplication.CustomerApplication.Application.DTOs.Role.Commands.CreateOrUpdate;
using CustomerApplication.CustomerApplication.Domain.Entities;
using CustomerApplication.CustomerApplication.Domain.Repositories;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public RoleService(IRoleRepository roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(CreateOrUpdateRoleCommand command)
    {
        Role role;

        if (command.Id == Guid.Empty || command.Id is null)
        {
            role = _mapper.Map<Role>(command);
            role.Id = Guid.NewGuid();
            await _roleRepository.AddAsync(role);
        }
        else
        {
            role = await _roleRepository.GetByIdAsync((Guid)command.Id);
            if (role == null)
                throw new Exception($"Role with ID {command.Id} not found.");

            _mapper.Map(command, role);
        }

        await _roleRepository.SaveChangesAsync();
        return role.Id;
    }

    public async Task<Role> GetByIdAsync(Guid id)
    {
        return await _roleRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Role>> GetListAsync()
    {
        return await _roleRepository.GetAllAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
         await _roleRepository.DeleteAsync(id);
        
    }
}
