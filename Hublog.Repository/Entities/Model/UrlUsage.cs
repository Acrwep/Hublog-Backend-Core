namespace Hublog.Repository.Entities.Model
{
    public class UrlUsage
    {
        public int UserId { get; set; }
        public string Url { get; set; } 
        public string TotalUsage { get; set; }
        public string Details { get; set; }
        public DateTime UsageDate { get; set; }
    }
}
