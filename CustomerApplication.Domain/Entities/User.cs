using AutoMapper;
using CustomerApplication.CustomerApplication.Application.DTOs.User.Commands.CreateOrUpdate;
using CustomerApplication.CustomerApplication.Application.Interfaces;
using System;
using System.Collections.Generic;

namespace CustomerApplication.CustomerApplication.Domain.Entities;

public partial class User: ISoftDelete
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;


    public Guid? RoleId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Customer> CustomerCreatedByNavigations { get; set; } = new List<Customer>();

    public virtual ICollection<Customer> CustomerUsers { get; set; } = new List<Customer>();

    public static void Delete(User user)
    {
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
    }
    public virtual Role Role { get; set; } = null!;

    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateOrUpdateUserCommand, User>()
                .ForMember(d => d.PasswordHash, opt => opt.Ignore());
        }
    }
}
