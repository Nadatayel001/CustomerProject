using CustomerApplication.CustomerApplication.Domain.Entities;
using CustomerApplication.CustomerApplication.Application.DTOs.Customer.Commands.CreateOrUpdate;

namespace CustomerApplication.CustomerApplication.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(Guid id);
        Task<Guid> CreateOrUpdateAsync(Command dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
