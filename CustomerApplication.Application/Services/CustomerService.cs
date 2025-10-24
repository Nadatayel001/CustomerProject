using CustomerApplication.CustomerApplication.Application.DTOs.Customer.Commands.CreateOrUpdate;
using CustomerApplication.CustomerApplication.Application.Interfaces;
using CustomerApplication.CustomerApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static CustomerApplication.CustomerApplication.Domain.Enums.LookupEnums;

namespace CustomerApplication.CustomerApplication.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly ILookupService _lookupService;

        public CustomerService(ICustomerRepository repository, ILookupService lookupService)
        {
            _repository = repository;
            _lookupService = lookupService;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Customer> GetByIdAsync(Guid id)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null || customer.IsDeleted)
                throw new Exception("Customer not found or deleted.");

            return customer;
        }

        public async Task<Guid> CreateOrUpdateAsync(Command dto)
        {
            var genderId = await MapEnumToLookupId(dto.Gender, CategoryCode.Gender);
            var governorateId = await MapEnumToLookupId(dto.Governorate, CategoryCode.Governorate);
            var districtId = await MapEnumToLookupId(dto.District, CategoryCode.District);
            var villageId = await MapEnumToLookupId(dto.Village, CategoryCode.Village);

            Customer customer;

            if (dto.Id.HasValue)
            {
                customer = await _repository.GetByIdAsync(dto.Id.Value);
                if (customer == null || customer.IsDeleted)
                    throw new Exception("Customer not found or deleted.");

                customer.FullName = dto.FullName;
                customer.NationalID = dto.NationalID;
                customer.GenderId = genderId;
                customer.GovernorateId = governorateId;
                customer.DistrictId = districtId;
                customer.VillageId = villageId;
                customer.Salary = dto.Salary;
                customer.BirthDate = dto.BirthDate;
                customer.Age = CalculateAge(dto.BirthDate);
                customer.IsActive = true;

                await _repository.UpdateAsync(customer);
            }
            else
            {
                // Create new customer
                customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    FullName = dto.FullName,
                    NationalID = dto.NationalID,
                    GenderId = genderId,
                    GovernorateId = governorateId,
                    DistrictId = districtId,
                    VillageId = villageId,
                    Salary = dto.Salary,
                    BirthDate = dto.BirthDate,
                    Age = CalculateAge(dto.BirthDate),
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                };

                await _repository.AddAsync(customer);
            }

            return customer.Id;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null || customer.IsDeleted)
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

        private async Task<Guid> MapEnumToLookupId<TEnum>(TEnum enumValue, CategoryCode category)
            where TEnum : Enum
        {
            var searchResult = await _lookupService.SearchAsync(new LookupSearchRequest
            {
                CategoryCode =category,
                Q = enumValue.ToString(),
                IsActive = true,
                Take = 1
            });

            var lookupItem = searchResult.Items.FirstOrDefault();
            if (lookupItem == null)
                throw new Exception($"Lookup not found for {category} value {enumValue}");

            return lookupItem.Id;
        }
    }
}
