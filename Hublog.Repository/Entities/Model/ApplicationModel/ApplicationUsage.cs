namespace Hublog.Repository.Entities.Model.ApplicationModel
{
    public class ApplicationUsage
    {
        public int UserId { get; set; }
        public string ApplicationName { get; set; }
        public string TotalUsage { get; set; }
        public string Details { get; set; }
        public DateTime UsageDate { get; set; }
    }
}
