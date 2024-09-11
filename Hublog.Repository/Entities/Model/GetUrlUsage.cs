namespace Hublog.Repository.Entities.Model
{
    public class GetUrlUsage
    {
        public int UrlUsageId { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public int OrganizationId { get; set; }
        public string Url { get; set; } 
        public string TotalUsage { get; set; }
        public string Details { get; set; }
        public DateTime UsageDate { get; set; }
        public int TeamId { get; set; }
    }
}
