using CustomerApplication.CustomerApplication.Application.Interfaces;
using CustomerApplication.CustomerApplication.Domain.Entities;
using CustomerApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerApplication.CustomerApplication.Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .Where(c => c.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Save failed (Customer): {inner}", ex);
            }
        }
        public IQueryable<Customer> GetQueryable()
        {
            return _context.Customers.AsNoTracking();
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

        }

        public async Task DeleteAsync(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                Customer.Delete(customer);
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
            }
        }
    }
}
