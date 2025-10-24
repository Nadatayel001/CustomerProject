using CustomerApplication.CustomerApplication.Domain.Entities;
using System;
using static CustomerApplication.CustomerApplication.Domain.Enums.LookupEnums;

namespace CustomerApplication.CustomerApplication.Application.DTOs.Customer.Commands.CreateOrUpdate
{
    public class Command
    {
        public Guid? Id { get; set; }
        public string FullName { get; set; } = default!;
        public string NationalID { get; set; } = default!;
        public Gender Gender { get; set; }           
        public Governorate Governorate { get; set; } 
        public District District { get; set; }       
        public Village Village { get; set; }        
        public DateTime BirthDate { get; set; }
        public decimal? Salary { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UserId { get; set; }
    }
}
