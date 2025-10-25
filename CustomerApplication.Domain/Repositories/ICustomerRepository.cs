using CustomerApplication.CustomerApplication.Domain.Entities;
using System.Linq.Expressions;

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
        Task<bool> ExistsAsync(Expression<Func<Customer, bool>> predicate);


    }
}
