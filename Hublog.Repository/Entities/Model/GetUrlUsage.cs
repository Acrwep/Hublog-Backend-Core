namespace Hublog.Repository.Entities.Model
{
    public class GetUrlUsage
    {
        public string Url { get; set; }
        public int UserCount { get; set; }
        public string TotalUsage { get; set; }
        public DateTime UsageDate { get; set; } 
    }
}
