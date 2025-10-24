namespace CustomerApplication.CustomerApplication.Application.DTOs.Customer.Commands.CreateOrUpdate
{
    public class Command
    {
        public Guid? Id { get; set; } 
        public string FullName { get; set; }
        public string NationalID { get; set; }
        public Guid GenderId { get; set; }
        public Guid GovernorateId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid VillageId { get; set; }
        public DateTime BirthDate { get; set; }
        public decimal? Salary { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UserId { get; set; }
    }
}
