using CustomerApplication.CustomerApplication.Application.DTOs.Customer.Commands.CreateOrUpdate;
using CustomerApplication.CustomerApplication.Application.Interfaces;
using CustomerApplication.CustomerApplication.Domain.Entities;
using CustomerApplication.CustomerApplication.Domain.Repositories;
using CustomerApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Command = CustomerApplication.CustomerApplication.Application.DTOs.Customer.Commands.CreateOrUpdate.Command;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Document = QuestPDF.Fluent.Document;

namespace CustomerApplication.CustomerApplication.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public CustomerService(
            ICustomerRepository repository,
            IUserRepository userRepository,
            IConfiguration configuration,
            IRoleRepository roleRepository,
            AppDbContext context,
                    IPasswordHasher<User> passwordHasher)
        {
            _repository = repository;
            _configuration = configuration;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _context = context;
            _passwordHasher = passwordHasher;

        }

        #region GetById

        public async Task<Customer> GetByIdAsync(Guid id)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null || customer.IsDeleted)
                throw new Exception("Customer not found or deleted.");

            return customer;
        }

        #endregion

        #region Create or Update

        public async Task<Guid> CreateOrUpdateAsync(Command dto)
        {
            if (dto.Id.HasValue)
            {
                // Update existing customer
                var customer = await _repository.GetByIdAsync(dto.Id.Value);
                if (customer == null || customer.IsDeleted)
                    throw new Exception("Customer not found or deleted.");

                customer.FullName = dto.FullName;
                customer.NationalID = dto.NationalID;
                customer.GenderId = dto.GenderId;
                customer.GovernorateId = dto.GovernorateId;
                customer.DistrictId = dto.DistrictId;
                customer.VillageId = dto.VillageId;
                customer.Salary = dto.Salary;
                customer.BirthDate = dto.BirthDate;
                customer.Age = CalculateAge(dto.BirthDate);
                customer.IsActive = true;

                await _repository.UpdateAsync(customer);
                await _context.SaveChangesAsync(); // Save after update
                return customer.Id;
            }
            else
            {
                // Create new customer with user inside a transaction
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var role = await _roleRepository.GetByNameAsync("USER")
                                ?? throw new Exception("Default role 'USER' not found.");
                    var defaultPass = "123456";
                    
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        Username = dto.FullName,
                        PasswordHash= _passwordHasher.HashPassword(null, defaultPass),
                        RoleId = role.Id,
                        IsDeleted = false
                    };

                    await _userRepository.AddAsync(user);

                    var customer = new Customer
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
                        Age = CalculateAge(dto.BirthDate),
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false,
                        UserId = user.Id,
                        CreatedBy=user.Id
                       
                    };

                    await _repository.AddAsync(customer);

                    // Save all changes at once inside transaction
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return customer.Id;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        #endregion

        #region Delete

        public async Task<bool> DeleteAsync(Guid id)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null || customer.IsDeleted)
                return false;

            Customer.Delete(customer);
            await _repository.UpdateAsync(customer);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region GetPaged with optional search

        public async Task<PagedResult<Customer>> GetPagedAsync(int skip = 0, int take = 20, string? search = null)
        {
            take = Math.Clamp(take, 1, 200);
            skip = Math.Max(0, skip);

            var query = _repository.GetQueryable()
                                   .Where(c => !c.IsDeleted && c.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(c =>
                    (c.FullName != null && c.FullName.ToLower().Contains(search)) ||
                    (c.NationalID != null && c.NationalID.ToLower().Contains(search)));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.FullName)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return new PagedResult<Customer>
            {
                TotalCount = total,
                Items = items
            };
        }
public async Task<byte[]> ExportCustomersToPdfAsync(string? search = null)
    {
        var query = _context.Customers
            .Include(c => c.Gender)
            .Where(c => !c.IsDeleted && c.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            query = query.Where(c =>
                (c.FullName != null && c.FullName.ToLower().Contains(search)) ||
                (c.NationalID != null && c.NationalID.ToLower().Contains(search)));
        }

        var customers = await query
            .OrderBy(c => c.FullName)
            .Take(1000)
            .ToListAsync();

        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Text("Customer Listing").Bold().FontSize(18).AlignCenter();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30); 
                        columns.RelativeColumn(3);  
                        columns.RelativeColumn(2);  
                        columns.RelativeColumn(2); 
                        columns.RelativeColumn(2);  
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("#").Bold();
                        header.Cell().Text("Full Name").Bold();
                        header.Cell().Text("National ID").Bold();
                        header.Cell().Text("Gender").Bold();
                        header.Cell().Text("Salary").Bold();
                    });

                    int index = 1;
                    foreach (var c in customers)
                    {
                        table.Cell().Text(index++);
                        table.Cell().Text(c.FullName ?? "");
                        table.Cell().Text(c.NationalID ?? "");
                        table.Cell().Text(c.Gender?.Name ?? ""); 
                        table.Cell().Text(c.Salary?.ToString("C") ?? "0");
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf();

        return pdfBytes;
    }



    #endregion

    #region Helpers

    private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.UtcNow;
            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;
            return age;
        }

        #endregion
    }
}
