using CustomerApplication.CustomerApplication.Domain.Entities;
using CustomerApplication.CustomerApplication.Application.DTOs.Customer.Commands.CreateOrUpdate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerApplication.CustomerApplication.Application.Interfaces
{
    public interface ICustomerService
    {
        //Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(Guid id);
        Task<Guid> CreateOrUpdateAsync(Command dto);
        Task<bool> DeleteAsync(Guid id);
        Task<byte[]> ExportCustomersToPdfAsync(string? search = null);

        Task<PagedResult<Customer>> GetPagedAsync(
            int skip = 0,
            int take = 20,
            string? search = null
        );
    }
}
