using CustomerApplication.CustomerApplication.Application.DTOs;
using CustomerApplication.CustomerApplication.Application.Interfaces;
using CustomerApplication.CustomerApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using static CustomerApplication.CustomerApplication.Domain.Enums.LookupEnums;

public partial class Lookup : ISoftDelete
{
    public Guid Id { get; set; }

    public CategoryCode CategoryCode { get; set; }

    public Guid? ParentId { get; set; }

    public string? Code { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Customer> CustomerDistricts { get; set; } = new List<Customer>();
    public virtual ICollection<Customer> CustomerGenders { get; set; } = new List<Customer>();
    public virtual ICollection<Customer> CustomerGovernorates { get; set; } = new List<Customer>();
    public virtual ICollection<Customer> CustomerVillages { get; set; } = new List<Customer>();
    public virtual ICollection<Lookup> InverseParent { get; set; } = new List<Lookup>();


    public virtual Lookup? Parent { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    public static void Delete(Lookup lookup)
    {
        lookup.IsDeleted = true;
        lookup.DeletedAt = DateTime.UtcNow;
        lookup.IsActive = false;
    }
}
