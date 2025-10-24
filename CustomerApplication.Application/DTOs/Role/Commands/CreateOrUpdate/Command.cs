using AutoMapper;
using CustomerApplication.CustomerApplication.Domain.Entities;

namespace CustomerApplication.CustomerApplication.Application.DTOs.Role.Commands.CreateOrUpdate
{
    public class CreateOrUpdateRoleCommand
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }

    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<CreateOrUpdateRoleCommand, Domain.Entities.Role> ();
            
        }
    }
}
