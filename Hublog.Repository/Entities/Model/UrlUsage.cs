namespace Hublog.Repository.Entities.Model
{
    public class UrlUsage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string BrowserName { get; set; }
        public string Url { get; set; }
        public string Details { get; set; }
        public DateTime UsageDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
