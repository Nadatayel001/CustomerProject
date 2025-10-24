using CustomerApplication.CustomerApplication.Application.DTOs.Customer.Commands.CreateOrUpdate;
using CustomerApplication.CustomerApplication.Application.Interfaces;
using CustomerApplication.CustomerApplication.Domain.Entities;

namespace CustomerApplication.CustomerApplication.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Customer> GetByIdAsync(Guid id)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null || (bool)customer.IsDeleted)
                throw new Exception("Customer not found or deleted.");

            return customer;
        }

        public async Task<Guid> CreateOrUpdateAsync(Command dto)
        {
            Customer customer;

            if (dto.Id.HasValue)
            {
                customer = await _repository.GetByIdAsync(dto.Id.Value);
                if (customer == null ||(bool) customer.IsDeleted)
                    throw new Exception("Customer not found or deleted.");

                customer.FullName = dto.FullName;
                customer.NationalID = dto.NationalID;
                customer.GenderId = dto.GenderId;
                customer.GovernorateId = dto.GovernorateId;
                customer.DistrictId = dto.DistrictId;
                customer.VillageId = dto.VillageId;
                customer.Salary = dto.Salary;
                customer.IsActive = true;
                customer.BirthDate = dto.BirthDate;
                customer.Age = CalculateAge(dto.BirthDate);

                await _repository.UpdateAsync(customer);
            }
            else
            {
                customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    FullName = dto.FullName,
                    NationalID = dto.NationalID,
                    GenderId = dto.GenderId,
                    GovernorateId = dto.GovernorateId,
                    DistrictId = dto.DistrictId,
                    VillageId = dto.VillageId,
                    Salary = dto.Salary,
                    BirthDate = dto.BirthDate,
                    CreatedDate = DateTime.UtcNow,
                    Age = CalculateAge(dto.BirthDate),
                    IsActive=true,
                    IsDeleted = false
                };

                await _repository.AddAsync(customer);
            }

            return customer.Id;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null || (bool)customer.IsDeleted)
                return false;
            Customer.Delete(customer);
            
            await _repository.UpdateAsync(customer);
            return true;
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.UtcNow;
            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;
            return age;
        }
    }
}
