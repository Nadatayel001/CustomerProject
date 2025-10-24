using CustomerApplication.CustomerApplication.Domain.Entities;

namespace CustomerApplication.CustomerApplication.Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(Guid id);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Guid id);
        IQueryable<Customer> GetQueryable();

    }
}
