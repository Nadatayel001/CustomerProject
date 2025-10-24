using CustomerApplication.CustomerApplication.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.CustomerApplication.Domain.Entities;

public partial class Customer : ISoftDelete
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    [Required]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "NationalID must be exactly 14 digits.")]
    [StringLength(14, MinimumLength = 14)]
    public string NationalID { get; set; } = default!;

    public Guid? GenderId { get; set; }

    public Guid? GovernorateId { get; set; }

    public Guid? DistrictId { get; set; }

    public Guid? VillageId { get; set; }

    public decimal? Salary { get; set; }

    public DateTime BirthDate { get; set; }

    public int? Age { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? CreatedBy { get; set; }

    public bool IsActive { get; set; }

    public Guid? UserId { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Lookup? District { get; set; }

    public virtual Lookup? Gender { get; set; }

    public virtual Lookup? Governorate { get; set; }

    public virtual User? User { get; set; }

    public virtual Lookup? Village { get; set; }


    public static void Delete(Customer customer)
    {
        customer.IsDeleted = true;
        customer.DeletedAt = DateTime.UtcNow;
        customer.IsActive = false;
    }
}
