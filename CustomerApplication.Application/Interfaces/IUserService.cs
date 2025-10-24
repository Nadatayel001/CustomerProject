using CustomerApplication.CustomerApplication.Application.DTOs.User.Commands.CreateOrUpdate;
using CustomerApplication.CustomerApplication.Application.DTOs.User.Commands.Login;
using CustomerApplication.CustomerApplication.Application.DTOs.User.Queries;
using CustomerApplication.CustomerApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserService
{
    Task<Guid> CreateAsync(CreateOrUpdateUserCommand command);
    Task<User?> GetByIdAsync(Guid id, bool includeRole = false);
    Task<IEnumerable<User>> GetListAsync(bool includeRoles = false);
    Task<User?> GetByUsernameAsync(string username, bool includeRole = false);
    Task DeleteAsync(Guid id);
    Task<LoginResult> LoginAsync(LoginCommand command);
}
