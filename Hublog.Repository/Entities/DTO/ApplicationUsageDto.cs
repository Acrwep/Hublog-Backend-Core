namespace Hublog.Repository.Entities.DTO
{
    public class ApplicationUsageDto
    {
        public int UserId { get; set; }
        public string ApplicationName { get; set; }
        public string TotalUsage { get; set; }
        public string Details { get; set; }
    }
}
