using CustomerApplication.CustomerApplication.Application.Interfaces;
using System;
using System.Collections.Generic;

namespace CustomerApplication.CustomerApplication.Domain.Entities;

public partial class Role : ISoftDelete
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsDeleted { get; set; } = false;
    
   public DateTime? DeletedAt { get; set; }


    public static void Delete(Role role)
    {
        role.IsDeleted = true;
        role.DeletedAt = DateTime.UtcNow;
    }
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
