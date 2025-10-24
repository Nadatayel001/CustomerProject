namespace CustomerApplication.CustomerApplication.Application.Interfaces
{
    public class ISoftDelete
    {
        public  bool IsDeleted { get; set; }=false;
        public DateTime DeletedAt { get; set; }

    }
}
