using AutoMapper;
using System;

namespace CustomerApplication.CustomerApplication.Application.DTOs.User.Commands.CreateOrUpdate
{

    public class CreateOrUpdateUserCommand
    {
        public Guid? Id { get; set; }
        public string Username { get; set; } = null!;
        public Guid? RoleId { get; set; }

        public string? Password { get; set; }

    }

    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateOrUpdateUserCommand, Domain.Entities.User>()
                .ForMember(d => d.PasswordHash, opt => opt.Ignore()) 
                .ForMember(d => d.Id, opt => opt.Ignore());         
        }
    }
}
